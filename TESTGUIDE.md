# F# Compiler, Core Library and Visual F# Tools Tests

Where this guide mentions the command `build` it means either `build.cmd` in the root folder for Windows, or `build.sh` for Linux/MacOS.

## In this guide

* [Quick start: Running Tests](#quick-start-running-tests)
* [Prerequisites](#prerequisites)
* [Test Suites](#test-suites)
* [More details](#more-details)
* [Other Tips and gotchas](#other-tips-and-gotchas)
* [Solving common errors](#solving-common-errors)
* [Approximate running times](#approximate-running-times)

## Quick start: Running Tests

To run the tests in Release mode:

```shell
build -testCompiler -c Release
build -testCompilerService -c Release
build -testCompilerComponentTests -c Release
build -testCambridge -c Release -ci -nobl
build -testFSharpQA -c Release -ci -nobl
build -testFSharpCore -c Release
build -testScripting -c Release
build -testVs -c Release
build -testAll -c Release
```

### Tests grouping summary

| Group name | OS | Description |
|------------|----|-------------|
| testDesktop  | Windows | Runs all net472 tests in 32 bit processes, this includes tests from other groups |
| testCoreClr  | Linux/Mac/Windows | Runs all .NetStandard and .NETCore tests in 64 bit processes, this includes tests from other groups |
| testFSharpCore | Windows | Runs all test for FSharp.Core.dll |
| testCambridge | Windows | Runs the Cambridge suite tests |
| testFSharpQA  | Windows | Runs the FSharpQA tests, requires Perl |
| testVS        | Windows + VS | Runs all VS integration tests |
| testCompiler  | Windows | Runs a few quick compiler tests |
| testScripting | Windows | Runs scripting fsx and fsi commandline tests |
| test          | Windows | Same as testDesktop |
| testAll       | Windows | Runs all above tests |

Some test groups can only be run in `CI` configuration, for that, you need to pass the `-ci -bl` or `-ci -nobl` arguments. Some test groups can only be run in Release mode, this is indicated below. Some tests can only be run on Windows.

To run tests, from a command prompt, use variations such as the following, depending on which test suite and build configuration you want.

### Tests that can be run on Linux and MacOS

If you're using Linux or MacOS to develop, the group of tests that are known to succeed are all in `-testCoreClr`. Any other `-testXXX` argument will currently fail. An effort is underway to make testing and running tests easier on all systems.

### Tests that can only be run in Release mode

The following tests **must** be run in Release mode with `-c Release`:

```shell
build -testAll -c Release
build -test -c Release
build -testDesktop -c Release
build -testCoreClr -c Release
```

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
* Run `git clean -xdf -e .vs` before running tests when:
  * Making changes to the lexer or parser
  * Between switching git branches
  * When merging with latest `main` upstream branch.

## More Details

The F# tests are split as follows:

* [FSharp Suite](tests/fsharp) - Older suite with broad coverage of mainline compiler and runtime scenarios.

* [FSharpQA Suite](tests/fsharpqa/Source) - Broad and deep coverage of a variety of compiler, runtime, and syntax scenarios.

* [FSharp.Core.UnitTests](tests/FSharp.Core.UnitTests) - Validation of the core F# types and the public surface area of `FSharp.Core.dll`.

* [FSharp.Compiler.UnitTests](tests/FSharp.Compiler.UnitTests) - Validation of compiler internals.

* [FSharp.Compiler.ComponentTests](tests/FSharp.Compiler.ComponentTests) - Validation of compiler APIs.

* [VisualFSharp.UnitTests](vsintegration/tests/unittests) - Visual F# Tools IDE Unit Test Suite
  This suite exercises a wide range of behaviors in the F# Visual Studio project system and language service.

### FSharp Suite

This is compiled using [tests\fsharp\FSharp.Tests.FSharpSuite.fsproj](tests/fsharp/FSharp.Tests.FSharpSuite.fsproj) to a unit test DLL which acts as a driver script. Each individual test is an NUnit test case, and so you can run it like any other NUnit test.

```shell
.\build.cmd net40 test-net40-fsharp
```

Tests are grouped in folders per area. Each test compiles and executes a `test.fsx|fs` file in its folder using some combination of compiler or FSI flags specified in the FSharpSuite test project.  
If the compilation and execution encounter no errors, the test is considered to have passed. 

There are also negative tests checking code expected to fail compilation. See note about baseline under "Other Tips" bellow for tests checking expectations against "baseline" files.

### FSharpQA Suite

The FSharpQA suite relies on [Perl](http://www.perl.org/get.html), StrawberryPerl package from https://strawberryperl.com.

These tests use the `RunAll.pl` framework to execute, however the easiest way to run them is via the `.\build` script, see [usage examples](#quick-start-running-tests).

Tests are grouped in folders per area. Each folder contains a number of source code files and a single `env.lst` file. The `env.lst` file defines a series of test cases, one per line.

Each test case runs an optional "pre command," compiles a given set of source files using given flags, optionally runs the resulting binary, then optionally runs a final "post command".

If all of these steps complete without issue, the test is considered to have passed.

Read more at [tests/fsharpqa/readme.md](tests/fsharpqa/readme.md).

#### Test lists

For the FSharpQA suite, the list of test areas and their associated "tags" is stored at

```shell
tests\fsharpqa\source\test.lst   // FSharpQA suite
```

Tags are in the left column, paths to to corresponding test folders are in the right column.  If no tags are specified, all tests will be run.

If you want to re-run a particular test area, the easiest way to do so is to set a temporary tag for that area in test.lst (e.g. "RERUN") and adjust `ttags` [run.fsharpqa.test.fsx script](tests/fsharpqa/run.fsharpqa.test.fsx) and run it.

### FSharp.Compiler.UnitTests, FSharp.Core.UnitTests, VisualFSharp.UnitTests

These are all NUnit tests. You can execute these tests individually via the Visual Studio NUnit3 runner
extension or the command line via `nunit3-console.exe`.

Note that for compatibility reasons, the IDE unit tests should be run in a 32-bit process,
using the `--x86` flag to `nunit3-console.exe`

### Logs and output

All test execution logs and result files will be dropped into the `tests\TestResults` folder, and have file names matching

```shell
    net40-fsharp-suite-*.*
    net40-fsharpqa-suite-*.*
    net40-compilerunit-suite-*.*
    net40-coreunit-suite-*.*
    vs-ideunit-suite-*.*
```

### Working with baseline tests

FSharp Test Suite works with a couple of `.bsl` (or `.bslpp`) files describing "expected test results" and are called the _Baseline Tests_. Those are matched against the actual output that resides under `.err` or `.vserr` files of the same name during test execution.
When doing so keep in mind to carefully review the diff before comitting updated baseline files.

The `.bslpp` (for: baseline pre-process) files are specially designed to enable substitution of certain tokens to generate the `.bsl` file. You can look further about the pre-processing logic under [tests/fsharp/TypeProviderTests.fs](tests/fsharp/TypeProviderTests.fs), this is used only for type provider tests for now.

To update baselines use this:

```shell
fsi tests\scripts\update-baselines.fsx
```

Use `-n` to dry-run:

```shell
fsi tests\scripts\update-baselines.fsx -n
```

## Other Tips and gotchas

This section contains general tips, for solving errors see [next section](#solving-common-errors).

### Close any open VisualFSharp.sln

If you have the `VisualFSharp.sln` open, or if you recently debugged it through `VisualFSharpFull` as start-up project, certain tests may fail because files will be in use. It's best to close Visual Studio and any debugging sessions during a test run. It is fine to have VS open on a different solution, or to have it open from a different F# repo folder.

### Finding the logs on CI

Finding the proper logs in the CI system can be daunting, this video shows you where to look once you have an open PR. It shows you how to get the `FsharpQA` logs, but the same method applies to getting any other test logs.

![b51e0ea3-e12b-4ee8-b26a-3b98c11dae33](https://user-images.githubusercontent.com/16015770/91355183-1a6ff900-e7ee-11ea-8fb4-e3627cc9b811.gif)

The console output of the CI runs do not contain output of the FSharpQA tests, but for most other tests the console output contains enough info and can be found by clicking <kbd>Raw output</kbd> in the CI window, or clicking <kbd>download logs</kbd>:

![download logs](https://user-images.githubusercontent.com/6309070/89307267-b9596900-d625-11ea-86e9-a1657ce2a368.png)

### Increase command screen line buffer on Windows

You can increase the window buffer so that more lines of the console output can be scrolled back to, as opposed to them disappearing off the top. The default size on Windows is very small:

* Click top-left icon of the command window
* Go to <kbd>Properties</kbd> then <kbd>Layout</layout>
* Select a higher _Screen buffer size_ than the window size (this will add a scroll bar)
* You may want to increase the width and height as well
* Click <kbd>OK</kbd>.

### Run as Administrator

Running tests should now be possible without admin privileges. If you find tests that don't run unless you are an admin, please create an issue.

### Running tests on other (feature) branches

When you switch branches, certain temporary files, as well as the .NET version (downloaded to `.dotnet` folder) are likely to not be in sync anymore and can lead to curious build errors. Fix this by running `git clean` like this (this will leave your VS settings intact):

```shell
git clean -xdf -e .vs
```

If you get "file in use" errors during cleaning, make sure to close Visual Studio and any running `dotnet.exe` and `VBCSCompiler.exe`, esp those that show up at the bottom of [Process Explorer](https://docs.microsoft.com/en-us/sysinternals/downloads/process-explorer) without parent process.

#### Running tests on release/dev16.6 etc branches

Some tests are known to fail on these older branches when run using one of the `testXXXX` commandline arguments. However, `-test`, `-testAll`, `-testCoreClr` and `-testDesktop` are known to work on at least the `dev16.6` and `dev16.7` branches.

### Making the tests run faster

* Adding the `-norestore` flag to the commandline speeds up the build part a little bit.
* When using the `-ci` flag (mandatory for some testsets), adding the `-nobl` flag prevents creating the binary log files.

Some tests run in parallel by default, or use a hosted compiler to speed things up:

* The FSharp and FSharpQA suites will run test cases in parallel by default. You can comment out the relevant line (look for `PARALLEL_ARG`) to disable this.
* By default, tests from the FSharpQA suite are run using a persistent, hosted version of the compiler. This speeds up test execution, as there is no need for the `fsc.exe` process to spin up repeatedly. To disable this, uncomment the relevant line (look for `HOSTED_COMPILER`).

## Solving common errors

The following are common errors that users have encountered while running tests on their system.

### Error that a file cannot be accessed

The build often leaves dangling processes like `HostedCompilerServer.exe`, `VBCSCompiler.exe` or `MSBuild.exe`. In [Process Explorer](https://docs.microsoft.com/en-us/sysinternals/downloads/process-explorer) you can see these processes having no parent process anymore. You can also use this to kill such processes. A typical error looks like and contains the process IDs (here 23152, 25252 and 24704):

> C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\Microsoft.Common.CurrentVersion.targets(4364,5): error MSB3026: Could not copy "D:\Projects\FSharp\artifacts\bin\FSharp.Core\Debug\net45\FSharp.Core.dll" to "D:\Projects\FSharp\tests\fsharpqa\testenv\bin\FSharp.Core.dll". Beginning retry 1 in 1000ms. The process cannot access the file 'D:\Projects\FSharp\tests\fsharpqa\testenv\bin\FSharp.Core.dll' because it is being used by another process. The file is locked by: "HostedCompilerServer (23152), HostedCompilerServer (25252), HostedCompilerServer (24704)" [D:\Projects\OpenSource\FSharp\tests\fsharpqa\testenv\src\ILComparer\ILComparer.fsproj]

### StackOverflow exception

This usually happens when you try to run tests without specifying `-c Release`, or as `-c Debug` (which is the default). Run the same set with `-c Release` instead and the SOE should disappear.

## Approximate running times

Some tests can run for several minutes, this doesn't mean that your system froze:

![image](https://user-images.githubusercontent.com/16015770/91359250-7dfd2500-e7f4-11ea-86bf-518c07ad61ab.png)

To get an idea of how long it may take, or how much coffee you'll need while waiting, here are some rough indications from an older workstation run, using arguments `-c Release -nobl -norestore`:

| Testset | Approx running time | Ngen'ed running time |
|-------|-------|-----|
| sln build time | 1 min* | n/a |
| `-testDesktop` | 5 min | ? |
| `-testCoreClr` | 36 min | ? |
| `-testCambridge` | 72 min | 35 min |
| `-testFSharpQA`  | 13 min | ? |
| `-testCompiler` | 30 seconds | n/a |
| `-testFSharpCore` | 2 min | ? |
| `-testScripting` | 2 min | 1.5 min |
| `-testVS` | 13 min | ? |

* This is the build time when a previous build with the same configuration succeeded, and without `-ci` present, which always rebuilds the solution. With `-norestore` the build part can go down to about 10-20 seconds, before tests are being run
