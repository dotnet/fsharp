# Investigation: FS3879 Warning - Opening Brace Issue

## Initial Observations  
- Compiler fails building FSharp.Core with FS3879 errors
- 14 errors total (7 per target framework)
- FSharp.Core has legitimate `///` after `{` in record type definitions
- Example: `type R = { /// Documentation for field`  
- These are valid XML documentation comments per F# spec

## Hypotheses

### Hypothesis 1: LBRACE token not excluded from tracking
**Theory**: The lexer doesn't know that `{` should be allowed before `///`
**Evidence**: Build fails on TypedTree.fs line 5528 with `{ /// Syntax after...`
**Test**: Exclude LBRACE token from updating lastNonCommentTokenLine in LexFilter.fs  
**Expected if true**: Partial fix - some errors remain
**Expected if false**: Same build failures
**Result**: ✅ PARTIALLY CORRECT - Pattern `LBRACE _` added but not sufficient

### Hypothesis 2: EQUALS token also needs exclusion  
**Theory**: The pattern `type R = { ///` means EQUALS comes before LBRACE
**Evidence**: All error cases follow pattern: `type Name = { /// field`
**Test**: Also exclude EQUALS token from updating lastNonCommentTokenLine
**Expected if true**: Build succeeds completely
**Expected if false**: Same build failures
**Result**: ✅ CONFIRMED - THIS FIXED THE ISSUE

### Hypothesis 3: Bootstrap compiler issue
**Theory**: The proto/bootstrap compiler doesn't have the LexFilter.fs changes yet
**Evidence**: Changes made to LexFilter.fs but errors persist after clean rebuild
**Test**: Full clean rebuild (rm -rf artifacts)
**Expected if true**: Need two-stage build
**Expected if false**: Single rebuild should work
**Result**: ✗ NOT THE ISSUE - Full clean rebuild with both LBRACE and EQUALS exclusions works

## Final Solution
1. ✅ Modified LexFilter.fs line 676-684 to exclude both LBRACE and EQUALS tokens:
   ```fsharp
   match token with
   | LINE_COMMENT _ -> ()
   | COMMENT _
   | WHITESPACE _
   | LBRACE _ -> () // XML doc comments after opening brace are legitimate
   | EQUALS -> () // XML doc comments after = (before {) are also legitimate  
   | _ -> XmlDocStore.SetLastNonCommentTokenLine lexbuf tokenLexbufState.EndPos.Line
   ```
2. ✅ Added test case "XML doc after opening brace should not warn"
3. ✅ Build command: `./build.sh -c Release` - Time: 4m 44.9s - Errors: 0
4. ✅ Test command: `dotnet test ... --filter "FullyQualifiedName~XmlDocTests"`
5. ✅ Final Results: **61 total tests, 56 passed, 0 failed, 5 skipped, 2 seconds**

## Root Cause
Record type definitions in F# follow the pattern `type Name = { field1; field2 }`.
When XML docs are placed after `=` and before `{`, they document the type itself.
When placed after `{` on a new line, they document individual fields.
Both patterns are legitimate and widely used in FSharp.Core.

The fix ensures that both `=` and `{` tokens don't update the "last non-comment token" 
position, allowing `///` to appear after them without triggering the warning.
