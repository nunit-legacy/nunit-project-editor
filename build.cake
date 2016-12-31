//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// SET PACKAGE VERSION
//////////////////////////////////////////////////////////////////////

var version = "1.0";
var modifier = "";

var isAppveyor = BuildSystem.IsRunningOnAppVeyor;
var packageVersion = version + modifier;
var dbgSuffix = configuration == "Debug" ? "-dbg" : "";

//////////////////////////////////////////////////////////////////////
// DEFINE RUN CONSTANTS
//////////////////////////////////////////////////////////////////////

var PROJECT_DIR = Context.Environment.WorkingDirectory.FullPath + "/";
var PACKAGE_DIR = PROJECT_DIR + "package/";
var TOOLS_DIR = PROJECT_DIR + "tools/";
var BIN_DIR = PROJECT_DIR + "bin/" + configuration + "/";

var SOLUTION = PROJECT_DIR + "nunit-editor.sln";
var TESTS_NAME = "nunit-editor.tests.dll";
var TESTS = BIN_DIR + TESTS_NAME;

var NUNIT3_CONSOLE = TOOLS_DIR + "NUnit.ConsoleRunner/tools/nunit3-console.exe";

//////////////////////////////////////////////////////////////////////
// CLEAN
//////////////////////////////////////////////////////////////////////

Task("Clean")
.Does(() =>
{
    CleanDirectory(BIN_DIR);
});

//////////////////////////////////////////////////////////////////////
// INITIALIZE FOR BUILD
//////////////////////////////////////////////////////////////////////

Task("InitializeBuild")
.Does(() =>
{
    NuGetRestore(SOLUTION);

    if (isAppveyor)
    {
        var tag = AppVeyor.Environment.Repository.Tag;

        if (tag.IsTag)
        {
            packageVersion = tag.Name;
        }
        else
        {
            var buildNumber = AppVeyor.Environment.Build.Number.ToString("00000");
            var branch = AppVeyor.Environment.Repository.Branch;
            var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;

            if (branch == "master" && !isPullRequest)
            {
                packageVersion = version + "-dev-" + buildNumber + dbgSuffix;
            }
            else
            {
                var suffix = "-ci" + buildNumber + dbgSuffix;

                if (isPullRequest)
                suffix += "-pr-" + AppVeyor.Environment.PullRequest.Number;
                else if (branch.StartsWith("release"))
                    suffix += "-pre-" + buildNumber;
                else
                    suffix += "-" + branch;

                // Nuget limits "special version part" to 20 chars. Add one for the hyphen.
                if (suffix.Length > 21)
                    suffix = suffix.Substring(0, 21);

                packageVersion = version + suffix;
            }
        }

        AppVeyor.UpdateBuildVersion(packageVersion);
    }
});

//////////////////////////////////////////////////////////////////////
// BUILD
//////////////////////////////////////////////////////////////////////

Task("Build")
.IsDependentOn("InitializeBuild")
.Does(() =>
{
    if(IsRunningOnWindows())
    {
        MSBuild(SOLUTION, new MSBuildSettings()
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
			.SetPlatformTarget(PlatformTarget.MSIL)
            .SetNodeReuse(false));
    }
    else
    {
        XBuild(SOLUTION, new XBuildSettings()
            .SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .WithTarget("Build"));
    }
});

//////////////////////////////////////////////////////////////////////
// TEST
//////////////////////////////////////////////////////////////////////

Task("Test")
.IsDependentOn("Build")
.Does(() =>
{
    int rc = StartProcess(NUNIT3_CONSOLE, new ProcessSettings()
    {
        Arguments = TESTS
    });

    if (rc != 0)
        throw new Exception(string.Format("NUnit test run for {0} returned code {1}.", TESTS_NAME, rc)); 
});

//////////////////////////////////////////////////////////////////////
// PACKAGE
//////////////////////////////////////////////////////////////////////

Task("PackageZip")
.Does(() =>
{
    var path = PACKAGE_DIR + "nunit-project-editor-" + packageVersion + dbgSuffix + ".zip";

    EnsureDirectoryExists(PACKAGE_DIR);

    CopyFileToDirectory("LICENSE.txt", BIN_DIR);
    CopyFileToDirectory("CHANGES.txt", BIN_DIR);

    var zipFiles = new FilePath[]
    {
        BIN_DIR + "LICENSE.txt",
        BIN_DIR + "CHANGES.txt",
        BIN_DIR + "nunit-editor.exe",
        BIN_DIR + "nunit.ico"
    };

    Zip(BIN_DIR, File(path), zipFiles);
});

Task("PackageChocolatey")
.Does(() =>
{
	EnsureDirectoryExists("PACKAGE_DIR");

	ChocolateyPack("nunit-project-editor.nuspec", 
		new ChocolateyPackSettings()
		{
			Version = packageVersion,
			OutputDirectory = PACKAGE_DIR,
			Files = new []
			{
				new ChocolateyNuSpecContent { Source = "LICENSE.txt" },
				new ChocolateyNuSpecContent { Source = "CHANGES.txt" },
				new ChocolateyNuSpecContent { Source = BIN_DIR + "nunit-editor.exe", Target = "tools" },
				new ChocolateyNuSpecContent { Source = BIN_DIR + "nunit.ico", Target = "tools" },
			}
		});
}); 

//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////

void RunGitCommand(string arguments)
{
    StartProcess("git", new ProcessSettings()
    {
        Arguments = arguments
    });
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Rebuild")
.IsDependentOn("Clean")
.IsDependentOn("Build");

Task("Package")
.IsDependentOn("PackageZip")
.IsDependentOn("PackageChocolatey");

Task("Appveyor")
.IsDependentOn("Build")
.IsDependentOn("Test")
.IsDependentOn("Package");

Task("Travis")
.IsDependentOn("Build")
.IsDependentOn("Test");

Task("Default")
.IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);