# F# Compiler, Core Library and Visual F# Tools Tests

## Prerequisites

In order to run the FSharpQA suite, you will need to install [Perl](http://www.perl.org/get.html) (ActiveState Perl 5.16.3 is known to work fine).
Perl must be included in the `%PATH%` for the below steps to work. It is also recommended that you run tests from an elevated command prompt, as there are a couple of test cases which require administrative privileges.

The Perl requirement is gradually being removed.

## Quick start: Running Tests

To run tests, use variations such as the following, depending on which test suite and build configuration you want:

    build.cmd compiler,smoke
    build.cmd compiler
    build.cmd ci
    build.cmd all
    build.cmd debug,compiler
    build.cmd debug,ci
    build.cmd debug,all

Default is `ci`

* ``ci`` = the build and tests done by continuous integration
* ``compiler`` = build the compiler 
* ``compiler,smoke`` = build the compiler and run some smoke tests
* ``debug`` = use Debug configuration instead of Release
* ``pcls`` = build and test the Portable PCL libraries for FSharp.Core
* ``build_only`` = build, don't test
* ``all`` = build and test everything

## Test Suites

The F# tests are split as follows:

* [FSharp Suite](tests/fsharp) - Older suite with broad coverage of mainline compiler and runtime scenarios.

* [FSharpQA Suite](tests/fsharpqa/Source) - Broad and deep coverage of a variety of compiler, runtime, and syntax scenarios.

* [FSharp.Core.Unittests](src/fsharp/FSharp.Core.Unittests) - Validation of the core F# types and the public surface area of `FSharp.Core.dll`.

* [FSharp.Compiler.Unittests](src/fsharp/FSharp.Compiler.Unittests) - Validation of compiler internals.

* [VisualFSharp.UnitTests](vsintegration/tests/unittests) - Visual F# Tools IDE Unit Test Suite
  This suite exercises a wide range of behaviors in the F# Visual Studio project system and language service.

## More Details

### FSharp Suite

This is now compiled using [tests\fsharp\FSharp.Tests.fsproj] to a unit test DLL which acts as a driver script.

This compiles and executes the `test.fsx` file using some combination of compiler or FSI flags.  
If the compilation and execution encounter no errors, the test is considered to have passed.

### FSharpQA Suite

These tests require use of the `RunAll.pl` framework to execute. 
Test area directories in this suite will contain a number of source code files and a single `env.lst` file. The `env.lst` file defines a series of test cases, one per line.  
Test cases will run an optional "pre command," compile some set of source files using some set of flags, optionally run the resulting binary, then optionally run a final "post command." 
If all of these steps complete without issue, the test is considered to have passed.

### FSharp.Compiler.Unittests, FSharp.Core.Unittests, VisualFSharp.Unittests

These are all NUnit tests. You can execute these tests individually via the Visual Studio NUnit3 runner 
extension or the command line via `nunit3-console.exe`.

Note that for compatibility reasons, the IDE unit tests should be run in a 32-bit process, 
using the '--x86' flag to `nunit3-console.exe`

### RunTests.cmd

The script `tests\RunTests.cmd` is used to execute the suites. It's used like this:

    RunTests.cmd <debug|release> fsharp [tags to run] [tags not to run]
    RunTests.cmd <debug|release> fsharpqa [tags to run] [tags not to run]
    RunTests.cmd <debug|release> compilerunit
    RunTests.cmd <debug|release> coreunit
    RunTests.cmd <debug|release> coreunitportable47
    RunTests.cmd <debug|release> coreunitportable7
    RunTests.cmd <debug|release> coreunitportable78
    RunTests.cmd <debug|release> coreunitportable259
    RunTests.cmd <debug|release> ideunit

`RunTests.cmd` sets a handful of environment variables which allow for the tests to work, then puts together and executes the appropriate command line to start the specified test suite.

All test execution logs and result files will be dropped into the `tests\TestResults` folder, and have file names matching `FSharp_*.*`, `FSharpQA_*.*`, `CompilerUnit_*.*`, `CoreUnit_*.*`, `IDEUnit_*.*`, e.g. `FSharpQA_Results.log` or `FSharp_Failures.log`.

For the FSharp and FSharpQA suites, the list of test areas and their associated "tags" is stored at

    tests\test.lst                   // FSharp suite
    tests\fsharpqa\source\test.lst   // FSharpQA suite

Tags are in the left column, paths to to corresponding test folders are in the right column.  If no tags are specified to `RunTests.cmd`, all tests will be run.

If you want to re-run a particular test area, the easiest way to do so is to set a temporary tag for that area in test.lst (e.g. "RERUN"), then call `RunTests.cmd <debug|release> <fsharp|fsharpqa> RERUN`.

If you want to specify multiple tags to run or not run, pass them comma-delimited and enclosed in double quotes, e.g. `RunTests.cmd debug fsharp "Core01,Core02"`. 
From a Powershell environment, make sure the double quotes are passed literally, e.g. `.\RunTests.cmd debug fsharp '"Core01,Core02"'`
 or `.\RunTests.cmd --% debug fsharp "Core01,Core02"`.

`RunTests.cmd` is mostly just a simple wrapper over `tests\fsharpqa\testenv\bin\RunAll.pl`, which has capabilities not discussed here. More advanced test execution scenarios can be achieved by invoking `RunAll.pl` directly.  
Run `perl tests\fsharpqa\testenv\bin\RunAll.pl -?` to see a full list of flags and options.

### Other Tips

* Run as Administrator, or a handful of tests will fail

* Making the tests run faster
  * NGen-ing the F# bits (fsc, fsi, FSharp.Core, etc) will result in tests executing much faster. Make sure you run `src\update.cmd` with the `-ngen` flag before running tests.
  * The FSharp and FSharpQA suites will run test cases in parallel by default. You can comment out the relevant line in `RunTests.cmd` (look for `PARALLEL_ARG`) to disable this.
  * By default, tests from the FSharpQA suite are run using a persistent, hosted version of the compiler. This speeds up test execution, as there is no need for the `fsc.exe` process to spin up repeatedly. To disable this, uncomment the relevant line in `RunTests.cmd` (look for `HOSTED_COMPILER`).
