﻿using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;

#pragma warning disable CA1819
namespace Rocket.Surgery.Nuke.GithubActions;

/// <summary>
///     A detailed trigger for version control
/// </summary>
public class RocketSurgeonGitHubActionsVcsTrigger : GitHubActionsDetailedTrigger
{
    /// <summary>
    ///     The kind of the trigger
    /// </summary>
    public RocketSurgeonGitHubActionsTrigger Kind { get; set; }

    /// <summary>
    ///     The branches
    /// </summary>
    public string[] Branches { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     The Tags
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     The included paths
    /// </summary>
    public string[] IncludePaths { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     The excluded paths
    /// </summary>
    public string[] ExcludePaths { get; set; } = Array.Empty<string>();

    /// <inheritdoc />
    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine(Kind.GetValue() + ":");

        if (Kind is RocketSurgeonGitHubActionsTrigger.WorkflowDispatch or RocketSurgeonGitHubActionsTrigger.WorkflowCall) return;
        using (writer.Indent())
        {
            if (Branches.Length > 0)
            {
                writer.WriteLine("branches:");
                using (writer.Indent())
                {
                    Branches.ForEach(x => writer.WriteLine($"- '{x}'"));
                }
            }

            if (Tags.Length > 0)
            {
                writer.WriteLine("tags:");
                using (writer.Indent())
                {
                    Tags.ForEach(x => writer.WriteLine($"- '{x}'"));
                }
            }

            if (IncludePaths.Length == 0 && ExcludePaths.Length > 0)
            {
                writer.WriteLine("paths-ignore:");
                using (writer.Indent())
                {
                    ExcludePaths.ForEach(x => writer.WriteLine($"- '{x}'"));
                }
            }
            else if (IncludePaths.Length > 0 && ExcludePaths.Length == 0)
            {
                writer.WriteLine("paths:");
                using (writer.Indent())
                {
                    IncludePaths.ForEach(x => writer.WriteLine($"- '{x}'"));
                }
            }
            else if (IncludePaths.Length > 0 || ExcludePaths.Length > 0)
            {
                writer.WriteLine("paths:");
                using (writer.Indent())
                {
                    IncludePaths.ForEach(x => writer.WriteLine($"- '{x}'"));
                    ExcludePaths.ForEach(x => writer.WriteLine($"- '!{x}'"));
                }
            }
        }
    }
}
