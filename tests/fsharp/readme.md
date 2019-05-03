# F# Compiler & FSharp.Core Test Suite

## Layout

TODO

## Workflow when adding or fixing tests

When a test is run, .err/.vserr output files are created and compared to their matching .bsl files.

When many tests fail due to a change being worked on, the [update.base.line.with.actuals.fsx](update.base.line.with.actuals.fsx) script helps updating the .bsl against the actuals.

After editing the folder list, evaluating the script should replace the .bsl files with actual .err/.vserr, after which the same test is supposed to pass.

Tests are organized under modules as functions bearing NUnit `[<Test>]` attribute and can be run from an IDE.