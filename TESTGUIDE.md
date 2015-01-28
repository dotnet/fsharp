# F# Compiler, Core Library and Visual F# Tools Tests

## Test Suites

The F# tests are split into a few different suites.  Understanding the suites' structure, similarities, and differences is important when running, debugging, or authoring the tests.

### FSharp Suite (aka "Cambridge Suite")
The test cases for this suite reside under `tests\fsharp`. This suite dates back to F#'s origins at Microsoft Research, Cambridge, and utilizes a simple batch script framework to execute.  In general, this suite has broad coverage of mainline compiler and runtime scenarios.

### FSharpQA Suite (aka "Redmond Suite")
The test cases for this suite reside under `tests\fsharpqa\source`.
This suite was first created when F# 2.0 was being added to Visual Studio 2010.  Tests for this suite are driven by the "RunAll" framework, implemented in Perl.  This suite is rather large and has broad and deep coverage of a variety of compiler, runtime, and syntax scenarios.

### Compiler and Library Core Unit Test Suites
The test cases for these suites reside next to the F# core library code, at `src\fsharp\FSharp.Core.Unittests` and `src\fsharp\FSharp.Compiler.Unittests`. These suites are standard NUnit test cases, implemented in F#.  The FSharp.Core.Unittests suite focuses on validation of the core F# types and the public surface area of `FSharp.Core.dll`, and the FSharp.Compiler.Unittests suite focuses on validation of compiler internals.

### Visual F# Tools IDE Unit Test Suite
The test cases for this suite reside next to the Visual F# Tools code, at `vsintegration\src\unittests`.  This suite is a set of standard NUnit test cases, implemented in F#.  This suite exercises a wide range of behaviors in the F# Visual Studio project system and language service.


## Prerequisites
In order to run all of the tests, you will need to install

* [Perl](http://www.perl.org/get.html) (ActiveState Perl 5.16.3 is known to work fine)
* [NUnit](http://nunit.org/?p=download) (2.6.3 is known to work fine)

Perl and NUnit must be included in the `%PATH%` for the below steps to work.  It is also recommended that you run tests from an elevated command prompt, as there are a couple of test cases which modify the GAC, and this requires administrative privileges.

Before running tests, make sure you have successfully built all required projects as specified in the 'Prepare For Tests' section of the [DEVGUIDE](DEVGUIDE.md).

Don't forget to build and install the Visual Studio components, as well, if you want to run the IDE tests below.

## Running Tests

Note: **Don't** run tests from a Visual Studio developer command prompt (see below).

The script `tests\RunTests.cmd` has been provided to make execution of the above suites simple.  You can kick off a full test run of any of the above suites like this:

```
RunTests.cmd <debug|release> fsharp [tags to run] [tags not to run]
RunTests.cmd <debug|release> fsharpqa [tags to run] [tags not to run]
RunTests.cmd <debug|release> compilerunit
RunTests.cmd <debug|release> coreunit
RunTests.cmd <debug|release> coreunitportable47
RunTests.cmd <debug|release> coreunitportable7
RunTests.cmd <debug|release> coreunitportable78
RunTests.cmd <debug|release> coreunitportable259
RunTests.cmd <debug|release> ideunit
```

`RunTests.cmd` sets a handful of environment variables which allow for the tests to work, then puts together and executes the appropriate command line to start the specified test suite.

All test execution logs and result files will be dropped into the `tests\TestResults` folder, and have file names matching `FSharp_*.*`, `FSharpQA_*.*`, `CompilerUnit_*.*`, `CoreUnit_*.*`, `IDEUnit_*.*`, e.g. `FSharpQA_Results.log` or `FSharp_Failures.log`.

For the FSharp and FSharpQA suites, the list of test areas and their associated "tags" is stored at

```
tests\test.lst                   // FSharp suite
tests\fsharpqa\source\test.lst   // FSharpQA suite
```

Tags are in the left column, paths to to corresponding test folders are in the right column.  If no tags are specified to `RunTests.cmd`, all tests will be run.

If you want to re-run a particular test area, the easiest way to do so is to set a temporary tag for that area in test.lst (e.g. "RERUN"), then call `RunTests.cmd <debug|release> <fsharp|fsharpqa> RERUN`.

If you want to specify multiple tags to run or not run, pass them comma-delimited and enclosed in double quotes, e.g. `RunTests.cmd debug fsharp "Core01,Core02"`. 
From a Powershell environment, make sure the double quotes are passed literally, e.g. `.\RunTests.cmd debug fsharp '"Core01,Core02"'`
 or `.\RunTests.cmd --% debug fsharp "Core01,Core02"`.

`RunTests.cmd` is mostly just a simple wrapper over `tests\fsharpqa\testenv\bin\RunAll.pl`, which has capabilities not discussed here. More advanced test execution scenarios can be achieved by invoking `RunAll.pl` directly.  
Run `perl tests\fsharpqa\testenv\bin\RunAll.pl -?` to see a full list of flags and options.

## More Details

### FSharp Suite

These tests are fairly easy to execute directly when needed, without help from `RunTests.cmd` or `RunAll.pl`. 

Test area directories in this suite will have either a `Build.bat` script, a `Run.bat` script, or both. 

To run the test area, you can simply call `Build.bat` (if it exists), then `Run.bat` (if it exists).  In this way it is simple to re-run a specific test area by itself. 

**NOTE:** If you are re-running tests manually like this, make sure you set environment variables similarly to how `RunTests.cmd` does (e.g. set `%FSCBINPATH%` to `%root%\release\net40\bin` for a release test run), to ensure tests are running against your open-built bits, and not against another deployment of F#.

`Build.bat` and `Run.bat` scripts typically invoke `tests\Config.bat` to pick up a variety of environment variables and configuration options, then invoke `tests\fsharp\single-test-build.bat` and `tests\fsharp\single-test-run.bat`.

This will compile and execute the local `test.fsx` file using some combination of compiler or fsi flags.  If the compilation and execution encounter no errors, the test is considered to have passed.

### FSharpQA Suite

These tests require use of the `RunAll.pl` framework to execute. 

Test area directories in this suite will contain a number of source code files and a single `env.lst` file.  The `env.lst` file defines a series of test cases, one per line.  

Test cases will run an optional "pre command," compile some set of source files using some set of flags, optionally run the resulting binary, then optionally run a final "post command." 
If all of these steps complete without issue, the test is considered to have passed.

### FSharp.Compiler and FSharp.Core Unit Test Suites

To build these unit test binaries, from the `src` directory call 

- `msbuild fsharp-compiler-unittests-build.proj`
  - Output binary is `FSharp.Compiler.Unittests.dll`
- `msbuild fsharp-library-unittests-build.proj`
  - Output binary is `FSharp.Core.Unittests.dll`
  - Output binary is `FSharp.Core.Unittests.SurfaceAreadll`

You can execute and re-run these tests using any standard NUnit approach - via graphical `nunit.exe` or on the command line via `nunit-console.exe`.

### Visual F# Tools IDE Unit Test Suite

To build the unit test binary, call 

```
msbuild fsharp-vsintegration-unittests-build.proj
```

from the `src` directory.  Tests are contained in the binary `Unittests.dll`. 

The IDE unit tests rely on the "Salsa" library, which is a set of Visual Studio mocks. The code for Salsa resides at `vsintegration\src\Salsa`.

Note that for compatibility reasons, the IDE unit tests should be run in a 32-bit process, either `nunit-console-x86.exe` or `nunit-x86.exe`.

You can execute and re-run these tests using any standard NUnit approach - via graphical `nunit-x86.exe` or on the command line via `nunit-console-x86.exe`.


### Other Tips

* Run as admin, or a handful of tests will fail
* **Don't** run tests from a Visual Studio developer command prompt.  Running from a developer command prompt will put the Visual Studio F# tools on the `%PATH%`, which is not desirable when testing open F# tools.
* Making the tests run faster
  * NGen-ing the F# bits (fsc, fsi, FSharp.Core, etc) will result in tests executing much faster.  Make sure you run `src\update.cmd` with the `-ngen` flag before running tests.
  * The Fsharp and FsharpQA suites will run test cases in parallel by default. You can comment out the relevant line in `RunTests.cmd` (look for `PARALLEL_ARG`) to disable this.
  * By default, tests from the FSharpQA suite are run using a persistent, hosted version of the compiler.  This speeds up test execution, as there is no need for the `fsc.exe` process to spin up repeatedly.  To disable this, uncomment the relevant line in `RunTests.cmd` (look for `HOSTED_COMPILER`).