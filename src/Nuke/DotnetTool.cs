using System.Collections.Immutable;
using System.Text.Json;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

namespace Rocket.Surgery.Nuke;

#pragma warning disable CA1724
/// <summary>
///     DotNetTool for Nuke builds
/// </summary>
public static class DotNetTool
{
    /// <summary>
    ///     Gets the tool definition for a given local dotnet tool
    /// </summary>
    /// <param name="nugetPackageName"></param>
    /// <returns></returns>
    public static Tool GetProperTool(string nugetPackageName) => ResolveToolsManifest().GetProperTool(nugetPackageName);

    /// <summary>
    ///     Gets the tool definition for a given local dotnet tool
    /// </summary>
    /// <param name="nugetPackageName"></param>
    /// <returns></returns>
    public static Tool GetTool(string nugetPackageName) => ResolveToolsManifest().GetTool(nugetPackageName);

    /// <summary>
    ///     Gets the tool definition for a given local dotnet tool
    /// </summary>
    /// <param name="nugetPackageName"></param>
    /// <returns></returns>
    public static ToolDefinition GetToolDefinition(string nugetPackageName) => ResolveToolsManifest().GetToolDefinition(nugetPackageName);

    /// <summary>
    ///     Determine if a dotnet tool is installed in the dotnet-tools.json
    /// </summary>
    /// <param name="nugetPackageName"></param>
    /// <returns></returns>
    public static bool IsInstalled(string nugetPackageName) => ResolveToolsManifest().IsInstalled(nugetPackageName);

    private static ResolvedToolsManifest ResolveToolsManifest()
    {
        if (toolsManifest is { }) return toolsManifest;

        if (ToolsManifestLocation.Value.FileExists())
        {
            #pragma warning disable CA1869
            var manifest =
                // ReSharper disable once NullableWarningSuppressionIsUsed
                JsonSerializer.Deserialize<ToolsManifset>(
                    File.ReadAllText(ToolsManifestLocation.Value),
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )!;
            #pragma warning restore CA1869
            toolsManifest = ResolvedToolsManifest.Create(manifest);
        }
        else
        {
            toolsManifest = new(ImmutableDictionary<string, ToolDefinition>.Empty, ImmutableDictionary<string, FullToolCommandDefinition>.Empty);
        }

        return toolsManifest;
    }

    private static ResolvedToolsManifest? toolsManifest;
    private static Lazy<AbsolutePath> ToolsManifestLocation { get; } = new(() => NukeBuild.RootDirectory / ".config" / "dotnet-tools.json");
}
