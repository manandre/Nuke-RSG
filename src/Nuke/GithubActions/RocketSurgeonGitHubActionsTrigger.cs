using Nuke.Common.Tooling;

namespace Rocket.Surgery.Nuke.GithubActions;

/// <summary>
///     The kind of trigger for the github actions
/// </summary>
[PublicAPI]
public enum RocketSurgeonGitHubActionsTrigger
{
    /// <summary>
    ///     Push
    /// </summary>
    [EnumValue("push")]
    Push,

    /// <summary>
    ///     Pull request
    /// </summary>
    [EnumValue("pull_request")]
    PullRequest,

    /// <summary>
    ///     Release
    /// </summary>
    [EnumValue("release")]
    Release,

    /// <summary>
    ///     Workflow dispatch
    /// </summary>
    [EnumValue("workflow_dispatch")]
    WorkflowDispatch,

    /// <summary>
    ///     Workflow call
    /// </summary>
    [EnumValue("workflow_call")]
    WorkflowCall,

    /// <summary>
    ///     Workflow run
    /// </summary>
    [EnumValue("workflow_run")]
    WorkflowRun,

    /// <summary>
    ///     Pull request target
    /// </summary>
    [EnumValue("pull_request_target")]
    PullRequestTarget,
}
