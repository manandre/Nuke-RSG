using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines.Configuration;

namespace Rocket.Surgery.Nuke.GithubActions;

/// <summary>
///     A parameter that will be provided to the nuke build
/// </summary>
public class GithubActionsNukeParameter : ConfigurationEntity
{
    /// <summary>
    ///     The name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The default value of the parameter
    /// </summary>
    public string Default { get; set; } = null!;

    /// <inheritdoc />
    public override void Write(CustomFileWriter writer)
    {
        using var a = writer.WriteBlock($"{Name}: '{Default}'");
    }
}
