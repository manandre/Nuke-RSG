﻿using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;

namespace Rocket.Surgery.Nuke.DotNetCore;

/// <summary>
///     Defines a `dotnet test` test run with code coverage via coverlet
/// </summary>
public interface ICanTestWithDotNetCore : IHaveCollectCoverage,
                                          IHaveBuildTarget,
                                          ITriggerCodeCoverageReports,
                                          IComprehendTests,
                                          IHaveTestArtifacts,
                                          IHaveGitVersion,
                                          IHaveSolution,
                                          IHaveConfiguration,
                                          IHaveOutputLogs,
                                          ICan
{
    /// <summary>
    ///     dotnet test
    /// </summary>
    public Target CoreTest => _ => _
                                  .Description("Executes all the unit tests.")
                                  .After(Build)
                                  .OnlyWhenDynamic(() => TestsDirectory.GlobFiles("**/*.csproj").Count > 0)
                                  .WhenSkipped(DependencyBehavior.Execute)
                                  .Executes(
                                       () => DotNetTasks.DotNetBuild(
                                           s => s
                                               .SetProjectFile(Solution)
                                               .SetDefaultLoggers(LogsDirectory / "test.build.log")
                                               .SetGitVersionEnvironment(GitVersion)
                                               .SetConfiguration("Debug")
                                               .EnableNoRestore()
                                       )
                                   )
                                  .CreateOrCleanDirectory(TestResultsDirectory)
                                  .CleanCoverageDirectory(CoverageDirectory)
                                  .EnsureRunSettingsExists(RunSettings)
                                  .Executes(
                                       () => DotNetTasks.DotNetTest(
                                           s => s
                                               .SetProjectFile(Solution)
                                               .SetDefaultLoggers(LogsDirectory / "test.log")
                                               .SetGitVersionEnvironment(GitVersion)
                                               .SetConfiguration("Debug")
                                               .EnableNoRestore()
                                               .EnableNoBuild()
                                               .SetLoggers("trx")
                                                // DeterministicSourcePaths being true breaks coverlet!
                                               .SetProperty("DeterministicSourcePaths", "false")
                                               .SetResultsDirectory(TestResultsDirectory)
                                               .When(
                                                    !CollectCoverage,
                                                    x => x.SetProperty((string)"CollectCoverage", "true")
                                                          .SetProperty("CoverageDirectory", CoverageDirectory)
                                                )
                                               .When(
                                                    CollectCoverage,
                                                    x => x
                                                        .SetProperty("CollectCoverage", "false")
                                                        .SetDataCollector("XPlat Code Coverage")
                                                        .SetSettingsFile(RunSettings)
                                                )
                                       )
                                   )
                                  .CollectCoverage(TestResultsDirectory, CoverageDirectory);
}
