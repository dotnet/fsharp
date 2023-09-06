# F# Compiler Cross-Platform Test Suite

## Layout

The tests are NUNIT test cases. They test a very wide range of compiler, interactive and FSharp.Core scenarios.

The bulk of the test cases are enumerated in tests.fs, these are the old cambridge test suite.  They build on a test-suite ported from windows batch files.  They run the compiler and fsi as seperate processes, when built for the coreclr it runs the coreclr versions using dotnet.exe 

The framework and utilities can be found in test-framework.fs, single-test.fs.

test cases look similar to:
````
    [<Test>]
    let ``array-FSI`` () = singleTestBuildAndRun "core/array" FSI
````
This test case builds and runs the test case in the folder core/array

this #define is used to exclude from the build tests that run will not run correctly on the coreclr
__#if !NETCOREAPP__

There are some older tests in this section that looks similar to:
````
    [<Test>]
    let events () = 
        let cfg = testConfig "core/events"
        fsc cfg "%s -a -o:test.dll -g" cfg.fsc_flags ["test.fs"]
        peverify cfg "test.dll"
        csc cfg """/r:"%s" /reference:test.dll /debug+""" cfg.FSCOREDLLPATH ["testcs.cs"]
        peverify cfg "testcs.exe"
        use testOkFile = fileguard cfg "test.ok"
        fsi cfg "" ["test.fs"]
        testOkFile.CheckExists()
        exec cfg ("." ++ "testcs.exe") ""
````
These tests build, compile, peverify and run fsi.

Below the Compiler directory there is a set of tests built on the compiler service.  They are nunit and instead of executing the compiler and fsi using files on disk the tests are built from memory.  These tests use the CompilerAssert framework and look similar to:

This test verifies that a warning is produces when a value is implicitly discarded.  The line ````x = 20``` looks like an assignment but in F# is a test for equality it yields and discards the value false.
````
    [<Test>]
    let ``Unused compare with immutable when assignment might be intended``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = 10
let y = "hello"

let changeX() =
    x = 20
    y = "test"
            """
            FSharpDiagnosticSeverity.Warning
            20
            (6, 5, 6, 11)
            "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to mutate a value, then mark the value 'mutable' and use the '<-' operator e.g. 'x <- expression'."
````


## Workflow when adding or fixing tests

When a test is run, .err/.vserr output files are created and compared to their matching .bsl files.

Refer to [Test Guide](../../TESTGUIDE.md#baselines) to know more about how to update them.

Tests are organized under modules as functions bearing NUnit `[<Test>]` attribute and can be run from an IDE or the command line (see the [Test Guide](../../TESTGUIDE.md)).
