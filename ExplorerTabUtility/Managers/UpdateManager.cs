using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using AutoUpdaterDotNET;
using AutoUpdaterDotNET.Markdown;
using ExplorerTabUtility.Helpers;

namespace ExplorerTabUtility.Managers;

internal static class UpdateManager
{
    private static readonly string[] KnownRuntimeTokens =
    {
        "net9_0",
        "net9",
        "net481",
        "net48",
        "netfw4_8_1",
        "netfw"
    };

    private static readonly string[] KnownArchTokens = { "x86", "x64", "arm64" };

    static UpdateManager()
    {
        AutoUpdater.FlattenRootFolder = true;
        AutoUpdater.Icon = Helper.GetIcon()!.ToBitmap();
        AutoUpdater.ChangelogViewerProvider = new MarkdownViewerProvider();

        AutoUpdater.ParseUpdateInfoEvent += ParseUpdateInfo;
    }

    public static void CheckForUpdates() => AutoUpdater.Start(Constants.UpdateUrl);

    private static void ParseUpdateInfo(ParseUpdateInfoEventArgs p)
    {
        try
        {
            var jsonNode = JsonSerializer.Deserialize<JsonNode>(p.RemoteData);
            if (jsonNode == null) return;

            p.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = jsonNode["tag_name"]!.GetValue<string>().TrimStart('v'),
                ChangelogText = jsonNode["body"]!.GetValue<string>(),
                ChangelogURL = jsonNode["html_url"]!.GetValue<string>(),
                DownloadURL = FindMatchingUpdateAssetUrl(jsonNode)
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Update check failed: {ex.Message}");
        }
    }

    private static string? FindMatchingUpdateAssetUrl(JsonNode jsonNode)
    {
        var currentArch = GetCurrentArchitectureToken();
        var currentRuntimeTokens = GetCurrentRuntimeTokens();
        var assets = jsonNode["assets"]?.AsArray();
        if (assets == null)
        {
            Debug.WriteLine("自动更新资产匹配失败：Release 响应中没有 assets。");
            return null;
        }

        var candidates = new List<(int Score, string Name, string Url)>();
        foreach (var asset in assets)
        {
            string assetName;
            string downloadUrl;
            try
            {
                assetName = asset?["name"]?.GetValue<string>() ?? string.Empty;
                downloadUrl = asset?["browser_download_url"]?.GetValue<string>() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"自动更新资产匹配跳过异常资产：{ex.Message}");
                continue;
            }

            if (string.IsNullOrWhiteSpace(assetName) || string.IsNullOrWhiteSpace(downloadUrl))
                continue;

            var score = ScoreUpdateAsset(assetName, currentArch, currentRuntimeTokens);
            if (score > 0)
                candidates.Add((score, assetName, downloadUrl));
        }

        var selected = candidates
            .OrderByDescending(candidate => candidate.Score)
            .ThenBy(candidate => candidate.Name, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(selected.Url))
        {
            Debug.WriteLine($"自动更新资产匹配成功：{selected.Name}");
            return selected.Url;
        }

        Debug.WriteLine(
            $"自动更新资产匹配失败：未找到适用于 {currentArch}/{string.Join(",", currentRuntimeTokens)} 的 ZIP 资产。");
        return null;
    }

    private static string GetCurrentArchitectureToken() =>
        RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            _ => "x64"
        };

    private static string[] GetCurrentRuntimeTokens()
    {
#if NET481
        return new[] { "netfw4_8_1", "net481", "net48", "netfw" };
#else
        var major = Environment.Version.Major;
        return new[] { $"net{major}_0", $"net{major}" };
#endif
    }

    private static int ScoreUpdateAsset(string assetName, string currentArch, IReadOnlyCollection<string> currentRuntimeTokens)
    {
        var fileName = Path.GetFileName(assetName);
        if (!fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            return 0;

        var normalized = NormalizeAssetName(fileName);
        if (ContainsAny(normalized, "setup", "installer", "unsigned", "bundle", "symbols", "pdb"))
            return 0;

        var tokens = normalized.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        var archScore = ScoreArchitecture(tokens, currentArch);
        if (archScore == 0)
            return 0;

        var runtimeScore = ScoreRuntime(normalized, currentRuntimeTokens);
        if (runtimeScore == 0)
            return 0;

        var score = archScore + runtimeScore;

        if (ContainsToken(normalized, "explorertabutility"))
            score += 20;

        if (ContainsToken(normalized, "selfcontained"))
            score += 30;
        else if (ContainsToken(normalized, "portable"))
            score += 25;
        else if (ContainsToken(normalized, "frameworkdependent"))
            score += 15;
        else
            score += 5;

        if (ContainsToken(normalized, "zh") && ContainsToken(normalized, "cn"))
            score += 5;

        return score;
    }

    private static int ScoreArchitecture(IReadOnlyCollection<string> tokens, string currentArch)
    {
        if (tokens.Any(token => KnownArchTokens.Contains(token) && token != currentArch))
            return 0;

        return tokens.Contains(currentArch) ? 100 : 10;
    }

    private static int ScoreRuntime(string normalizedAssetName, IReadOnlyCollection<string> currentRuntimeTokens)
    {
        if (currentRuntimeTokens.Any(token => ContainsToken(normalizedAssetName, token)))
            return 80;

        return KnownRuntimeTokens.Any(token => ContainsToken(normalizedAssetName, token)) ? 0 : 5;
    }

    private static bool ContainsAny(string normalizedAssetName, params string[] tokens) =>
        tokens.Any(token => ContainsToken(normalizedAssetName, token));

    private static bool ContainsToken(string normalizedAssetName, string token)
    {
        var normalizedToken = NormalizeAssetName(token);
        return $"_{normalizedAssetName}_".IndexOf($"_{normalizedToken}_", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static string NormalizeAssetName(string value) =>
        Regex.Replace(value.ToLowerInvariant(), "[^a-z0-9]+", "_").Trim('_');
}
