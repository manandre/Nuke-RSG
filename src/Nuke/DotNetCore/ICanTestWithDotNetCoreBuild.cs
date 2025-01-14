using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;

namespace Rocket.Surgery.Nuke.DotNetCore;

/// <summary>
///     Defines a `dotnet test` test run with code coverage via coverlet
/// </summary>
public interface ICanTestWithDotNetCoreBuild : IHaveCollectCoverage,
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
                                       () => MSBuildTasks.MSBuild(
                                           settings =>
                                               settings
                                                  .SetSolutionFile(Solution)
                                                  .SetConfiguration(Configuration)
                                                  .SetDefaultLoggers(LogsDirectory / "test.build.log")
                                                  .SetGitVersionEnvironment(GitVersion)
                                                  .SetAssemblyVersion(GitVersion?.AssemblySemVer)
                                                  .SetPackageVersion(GitVersion?.NuGetVersionV2)
                                       )
                                   )
                                  .Executes(
                                       () =>
                                       {
                                           EnsureCleanDirectory(TestResultsDirectory);
                                           CoverageDirectory.GlobFiles("*.cobertura.xml", "*.opencover.xml", "*.json", "*.info")
                                                            .Where(x => Guid.TryParse(Path.GetFileName(x).Split('.')[0], out var _))
                                                            .ForEach(DeleteFile);
                                       }
                                   )
                                  .Executes(
                                       () =>
                                       {
                                           var runSettings = TestsDirectory / "coverlet.runsettings";
                                           if (!runSettings.FileExists())
                                           {
                                               runSettings = NukeBuild.TemporaryDirectory / "default.runsettings";
                                               if (!runSettings.FileExists())
                                               {
                                                   using var tempFile = File.Open(runSettings, FileMode.CreateNew);
                                                   typeof(ICanTestWithDotNetCore)
                                                      .Assembly
                                                      .GetManifestResourceStream("Rocket.Surgery.Nuke.default.runsettings")!.CopyTo(tempFile);
                                               }
                                           }

                                           DotNetTasks.DotNetTest(
                                               s => s.SetProjectFile(Solution)
                                                     .SetDefaultLoggers(LogsDirectory / "test.log")
                                                     .SetGitVersionEnvironment(GitVersion)
                                                     .EnableNoRestore()
                                                     .SetLoggers("trx")
                                                     .SetConfiguration(Configuration)
                                                     .EnableNoBuild()
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
                                                              .SetSettingsFile(runSettings)
                                                      )
                                           );

                                           // Ensure anything that has been dropped in the test results from a collector is
                                           // into the coverage directory
                                           foreach (var file in TestResultsDirectory
                                                               .GlobFiles("**/*.cobertura.xml")
                                                               .Where(x => Guid.TryParse(Path.GetFileName(x.Parent), out var _))
                                                               .SelectMany(coverage => coverage.Parent.GlobFiles("*.*")))
                                           {
                                               var folderName = Path.GetFileName(file.Parent);
                                               var extensionPart = string.Join(".", Path.GetFileName(file).Split('.').Skip(1));
                                               CopyFile(
                                                   file,
                                                   CoverageDirectory / $"{folderName}.{extensionPart}",
                                                   FileExistsPolicy.OverwriteIfNewer
                                               );
                                           }
                                       }
                                   );
}
