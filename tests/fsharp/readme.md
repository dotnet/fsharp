# Folder organization

This folder contains a set of test suites related to fsc and fsi.

## EndToEndBuildTests

To be described.

## fsharp

An heterogenous set of tests.

### about .bsl based tests (baseline)

Some of the tests in this suite are checking the compiler output messages against an expected baseline; the baseline is stored in .bslpp or .bsl files and after the test is run, an associated .err or .vserr file is expected to be matching the baseline.

.bslpp indicates a preprocess baseline, which is transformed on the test environment into a .bsl file before the diff is performed.

You can look at the `diff` function in single-test.fs.

`Test.fs` file can be used in interactive, for example you can send the top of it and run a single test

```fsharp
singleNegTest (testConfig "typecheck/sigs") "neg1"
```

If you want to update all the baseline files, you can also use the convenience script `update.base.line.with.actuals.fsx`, note that the .bslpp will need to have their preprocess peculiarities edited back in (or reverted) when using it.

## FSharp.Build.UnitTests

To be described.

## FSharp.Compiler.UnitTests

To be described.

## FSharp.Core.UnitTests

To be described.

## fsharpqa

To be described.

## projects

To be described.

## scripts

To be described.

## service

To be described.

## walkthroughs

