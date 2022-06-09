#addin nuget:?package=Cake.FileHelpers&version=5.0.0

var target = Argument("target", "Build");
var artifactsDir = "artifacts";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("BuildWindows")
    .WithCriteria(() => IsRunningOnWindows())
    .Does(() =>
{

});

Task("BuildMacOS")
    .WithCriteria(() => IsRunningOnMacOs())
    .Does(() =>
{
    // Set new minimum target to 10.15
    var filePaths = new[] { "sdl/build-scripts/clang-fat.sh" };

    foreach (var filePath in filePaths)
        ReplaceRegexInFiles(filePath, @"10\.6", "10.15", System.Text.RegularExpressions.RegexOptions.Singleline);

    // Build
    var buildDir = "sdl/build";
    CreateDirectory(buildDir);
    StartProcess("sdl/configure", new ProcessSettings {
        WorkingDirectory = buildDir,
        EnvironmentVariables = new Dictionary<string, string>{
            { "CC", "../build-scripts/clang-fat.sh" }
        }
    });
    StartProcess("make", new ProcessSettings { WorkingDirectory = buildDir });

    // Copy artifact
    CreateDirectory(artifactsDir);
    CopyFile("sdl/build/build/.libs/libSDL2-2.0.0.dylib", $"{artifactsDir}/libSDL2.dylib");
});

Task("BuildLinux")
    .WithCriteria(() => IsRunningOnLinux())
    .Does(() =>
{
    
});

// IsRunningOnMacOs


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("BuildWindows")
    .IsDependentOn("BuildMacOS")
    .IsDependentOn("BuildLinux");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);