# F# Compiler, Core Library and Visual F# Tools Tests

## Quick start: Running Tests

Tests are grouped as noted below. Some test groups can only be run in `CI` configuration, for that, you need to pass the `-ci -bl` or `-ci -nobl` arguments. Some test groups can only be run in Release mode, this is indicated below. Some tests can only be run on Windows.

To run tests, from a command prompt, use variations such as the following, depending on which test suite and build configuration you want.

### Tests runnable in any configuration

The following tests sets can be run in Release or Debug mode, with or without the `-ci` argument. 

    .\build -testCompiler -c Release
    .\build -testDesktop -c Release
    .\build -testCoreClr -c Release
    .\build -testFSharpCore -c Release    
    .\build -testScripting -c Release
    .\build -testVs -c Release

### Tests that are Windows-only

The following testsets are known to fail on Linux and Mac, but can be run on Windows:

* testFSharpQA
* testCambridge (some tests create windows)
* testVs (interact with VS)
* testDesktop (requires .NET Framework, use testCoreClr instead)

### Tests that can only be run in Release mode

The following tests **must** be run in Release mode with `-c Release`:

    .\build -testAll -c Release
    .\build -test -c Release
    
TODO: verify (and do these test run Cambridge/FSharpQA?)
    
### Tests that can only run with `-ci`

The following tests **must** be run in Release mode and with the CI argument like `-ci -bl` or `-ci -nobl`:

    .\build -testCambridge  -c Release -ci -nobl
    .\build -testFSharpQA -c Release -ci -nobl

    
### Tests that open other windows

The following testsets open other windows and may interfere with you using your workstation, or change focus while you're doing something else:

* FSharpQA
* Cambridge    

### Running tests online in CI

You can also submit pull requests to https://github.com/dotnet/fsharp and run the tests via continuous integration. Most people do wholesale testing that way. A few notes:

* Online, sometimes unrelated tests or builds may fail, a rerun may solve this
* A CI build can be restarted by closing/reopening the PR
* A new CI build will be started on each pushed commit
* CI builds that are not finished will be canceled on new commits. If you need to complete such runs, you can do so in the <kbd>Checks</kbd> tab of the PR by selecting the actual commit from the dropdown.

#### Analyzing CI results

Finding the logs in the online CI results can be tricky, a small video can be found below under "Test gotchas".

## Prerequisites

The prerequisites are the same as for building the `FSharp.sln`, plus, at a minimum:

* An installation of Perl, required for running FSharpQA tests

## Test Suites

The F# tests are split as follows:

* [FSharp Suite](tests/fsharp) - Older suite with broad coverage of mainline compiler and runtime scenarios.

* [FSharpQA Suite](tests/fsharpqa/Source) - Broad and deep coverage of a variety of compiler, runtime, and syntax scenarios.

* [FSharp.Core.UnitTests](tests/FSharp.Core.UnitTests) - Validation of the core F# types and the public surface area of `FSharp.Core.dll`.

* [FSharp.Compiler.UnitTests](tests/FSharp.Compiler.UnitTests) - Validation of compiler internals.

* [VisualFSharp.UnitTests](vsintegration/tests/unittests) - Visual F# Tools IDE Unit Test Suite
  This suite exercises a wide range of behaviors in the F# Visual Studio project system and language service.

## More Details

### FSharp Suite

This is compiled using [tests\fsharp\FSharp.Tests.FSharpSuite.fsproj](tests/fsharp/FSharp.Tests.FSharpSuite.fsproj) to a unit test DLL which acts as a driver script. Each individual test is an NUnit test case, and so you can run it like any other NUnit test.

    .\build.cmd net40 test-net40-fsharp

Tests are grouped in folders per area. Each test compiles and executes a `test.fsx|fs` file in its folder using some combination of compiler or FSI flags specified in the FSharpSuite test project.  
If the compilation and execution encounter no errors, the test is considered to have passed. 

There are also negative tests checking code expected to fail compilation. See note about baseline under "Other Tips" bellow for tests checking expectations against "baseline" files.

### FSharpQA Suite

The FSharpQA suite relies on [Perl](http://www.perl.org/get.html), StrawberryPerl package from nuget is used automatically by the test suite.

These tests use the `RunAll.pl` framework to execute, however the easiest way to run them is via the `.\build` script, see [usage examples](#quick-start-running-tests).

Tests are grouped in folders per area. Each folder contains a number of source code files and a single `env.lst` file. The `env.lst` file defines a series of test cases, one per line.

Each test case runs an optional "pre command," compiles a given set of source files using given flags, optionally runs the resulting binary, then optionally runs a final "post command".

If all of these steps complete without issue, the test is considered to have passed.

Read more at [tests/fsharpqa/readme.md](tests/fsharpqa/readme.md).

#### Test lists

For the FSharpQA suite, the list of test areas and their associated "tags" is stored at

    tests\fsharpqa\source\test.lst   // FSharpQA suite

Tags are in the left column, paths to to corresponding test folders are in the right column.  If no tags are specified, all tests will be run.

If you want to re-run a particular test area, the easiest way to do so is to set a temporary tag for that area in test.lst (e.g. "RERUN") and adjust `ttags` [run.fsharpqa.test.fsx script](tests/fsharpqa/run.fsharpqa.test.fsx) and run it.

### FSharp.Compiler.UnitTests, FSharp.Core.UnitTests, VisualFSharp.UnitTests

These are all NUnit tests. You can execute these tests individually via the Visual Studio NUnit3 runner 
extension or the command line via `nunit3-console.exe`.

Note that for compatibility reasons, the IDE unit tests should be run in a 32-bit process, 
using the `--x86` flag to `nunit3-console.exe`


### Logs and output

All test execution logs and result files will be dropped into the `tests\TestResults` folder, and have file names matching

    net40-fsharp-suite-*.*
    net40-fsharpqa-suite-*.*
    net40-compilerunit-suite-*.*
    net40-coreunit-suite-*.*
    vs-ideunit-suite-*.*

### Baselines

FSharp Test Suite works with couples of .bsl (or .bslpp) files considered "expected" and called baseline, those are matched against the actual output which resides under .err or .vserr files of same name at the during test execution.
When doing so keep in mind to carefully review the diff before comitting updated baseline files.
.bslpp (baseline pre-process) files are specially designed to enable substitution of certain tokens to generate the .bsl file. You can look further about the pre-processing logic under [tests/fsharp/TypeProviderTests.fs](tests/fsharp/TypeProviderTests.fs), this is used only for type provider tests for now.

To update baselines use this:

    fsi tests\scripts\update-baselines.fsx

Use `-n` to dry-run:

    fsi tests\scripts\update-baselines.fsx -n

## Other Tips and gotchas

This section contains general tips, for solving errors see next section.

### Close an open VisualFSharp.sln

If you have the `VisualFSharp.sln` open, or if you recently debugged it through `VisualFSharpFull` as start-up project, certain tests may fail because files will be in use. It's best to close Visual Studio and any debugging sessions during a test run. It is fine to have VS open on a different solution, or to have it open from a different F# repo folder.

### Finding the logs on CI

It's not trivial to find the logs in the CI system, but this video is only half a minute long and explains you where to look. It shows you how to get the `FsharpQA` logs, but the same method applies to getting any other test logs.

![b51e0ea3-e12b-4ee8-b26a-3b98c11dae33](https://user-images.githubusercontent.com/16015770/91355183-1a6ff900-e7ee-11ea-8fb4-e3627cc9b811.gif)

The console output of the CI runs do not contain output of the FSharpQA tests, but for many tests this is enough and can be found in the top-right, or by clicking <kbd>Raw output</kbd>:

![download logs](https://user-images.githubusercontent.com/6309070/89307267-b9596900-d625-11ea-86e9-a1657ce2a368.png)

### Increase command screen line buffer on Windows

You can increase the window buffer so that more lines of the console output can be scrolled back to, as opposed to them disappearing off the top. The default size on Windows is very small:

* Click top-left icon of the command window
* Go to <kbd>Properties</kbd> then <kbd>Layout</layout>
* Select a higher _Screen buffer size_ than the window size (this will add a scroll bar)
* You may want to increase the width and height as well
* Click <kbd>OK</kbd>.

### Run as Administrator

Running tests should be possible without admin privileges, but sometimes it helps to run as admin. If you find tests that don't run unless you are an admin, please create  an issue.

### Making the tests run faster

* NGen-ing the F# bits (fsc, fsi, FSharp.Core, etc) will result in tests executing much faster. Make sure you run `src\update.cmd` with the `-ngen` flag before running tests.
* The FSharp and FSharpQA suites will run test cases in parallel by default. You can comment out the relevant line (look for `PARALLEL_ARG`) to disable this.
* By default, tests from the FSharpQA suite are run using a persistent, hosted version of the compiler. This speeds up test execution, as there is no need for the `fsc.exe` process to spin up repeatedly. To disable this, uncomment the relevant line (look for `HOSTED_COMPILER`).



## Solving common errors

The following are common errors that users have encountered while running tests on their system.

### Error that a file cannot be accessed

The build often leaves dangling processes like `HostedCompilerServer.exe`, `VBSCompiler.exe` or `MSBuild.exe`. In `Process Explorer` you can see these processes having no parent process anymore. You can also use this to kill such processes. A typical error looks like and contains the process IDs (here 23152, 25252 and 24704):

> C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\Microsoft.Common.CurrentVersion.targets(4364,5): error MSB3026: Could not copy "D:\Projects\FSharp\artifacts\bin\FSharp.Core\Debug\net45\FSharp.Core.dll" to "D:\Projects\FSharp\tests\fsharpqa\testenv\bin\FSharp.Core.dll". Beginning retry 1 in 1000ms. The process cannot access the file 'D:\Projects\FSharp\tests\fsharpqa\testenv\bin\FSharp.Core.dll' because it is being used by another process. The file is locked by: "HostedCompilerServer (23152), HostedCompilerServer (25252), HostedCompilerServer (24704)" [D:\Projects\OpenSource\FSharp\tests\fsharpqa\testenv\src\ILComparer\ILComparer.fsproj]

### StackOverflow exception

This usually happens when you try to run tests without specifying `-c Release`, or as `-c Debug` (which is the default). Run the same set with `-c Release` instead and the SOE should disappear.

## Approximate running times

Some tests can run for several minutes, this doesn't mean that your system froze:

![image](https://user-images.githubusercontent.com/16015770/91359250-7dfd2500-e7f4-11ea-86bf-518c07ad61ab.png)

The following are rough indications from an older workstation run, actual durations will vary per system:

| Testset | Approx running time | Ngen'ed running time |
|-------|-------|-----|
| sln build time | 1 min | n/a |
| `-testCambridge` | 72 min | 35 min |
| `-testFSharpQA`  | 13 min | ? |
| `-testCompiler` | 30 seconds | n/a |

