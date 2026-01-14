# Test Framework Additions Needed

Track functionality gaps in FSharp. Test.Utilities that block migration.  Once implemented, move to Completed section.

## Already Available (Reference)

These are NOT missing - they exist in Compiler.fs:

| Need | Available Function |
|------|-------------------|
| Runtime output verification | `verifyOutput "expected"` |
| Output contains check | `verifyOutputContains [|"text1"; "text2"|]` |
| Output baseline comparison | `verifyOutputWithBaseline "path/to/baseline. txt"` |
| Negative output assertion | `verifyNotInOutput "should not appear"` |
| Get raw output | `getOutput` returns `string option` |