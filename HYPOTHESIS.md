# XML Doc Warning FS3879 - Investigation Hypotheses

## Test Failure Summary
- **Initial State**: Total tests: 60, Passed: 40, Failed: 15, Skipped: 5, Time: 7.1 seconds
- **After Fix**: Total tests: 60, Passed: 55, Failed: 0, Skipped: 5, Time: 6.8 seconds

## Failed Tests (Before Fix)
1. `let bindings 03 - 'let in' with attributes after 'let'`
2. `types 03 - xml doc after 'and'`
3. `exception 02 - attribute after 'exception'`
4. `types 05 - attributes after 'type'`
5. `let bindings 09 - xml doc after 'and'`
6. `type members 04 - property accessors`
7. `let bindings 10 - xml doc before/after 'and'`
8. `let bindings 01 - allowed positions`
9. `types 04 - xml doc before/after 'and'`
10. `Discriminated Union - triple slash after case definition should warn`
11. `exception 01 - allowed positions`
12. `let bindings 07 - attribute after 'let'`
13. `type members 06 - implicit ctor`
14. `module 02 - attributes after 'module'`
15. `type members 02`

## Hypothesis 1: The XmlDocCollector members are not being set correctly
**Problem**: The `SetLastNonCommentTokenLine` and `GetLastNonCommentTokenLine` methods in LexerStore.fs reference XmlDocCollector members that don't exist or aren't accessible.

**Evidence**: Compilation errors showed `XmlDocCollector` does not define these members even though they were supposedly added in commit 1006511.

**Test**: Check if XmlDoc.fs actually has the required members and if they're in the correct scope.

**Status**: ✅ RESOLVED - The members existed and were accessible.

## Hypothesis 2: The warning check in LexFilter is not correctly identifying /// comments
**Problem**: The code in rulesForBothSoftWhiteAndHardWhite may not be correctly checking for `///` comments or may not have access to the lexeme text.

**Evidence**: Tests are expecting FS3879 warnings but they're not being emitted. The attempt to access `lexbuf.LexBuffer.Lexeme` is incorrect - LexBuffer doesn't have a LexBuffer property.

**Test**: Verify how to correctly access comment text in LexFilter and check if the LINE_COMMENT token actually contains the text.

**Status**: ✅ CONFIRMED - This was a contributing factor but not the root cause.

## Hypothesis 3: The warning check is in the wrong location (LexFilter instead of lex.fsl)
**Problem**: The warning is being checked in LexFilter.fs at line 2640, but at that point:
1. We don't have reliable access to the lexeme text to check if it's `///` vs `//`
2. The LINE_COMMENT token doesn't distinguish between comment types
3. `lexbuf.LexemeView` may not be available or reliable in LexFilter

**Evidence**: 
- The lexer (lex.fsl) at lines 740-746 specifically matches `///` and knows it's an XML doc comment
- The lexer already calls XmlDocStore functions (AddGrabPointDelayed)
- The lexer has access to the lexeme range and can easily check lastNonCommentTokenLine

**Solution**: Move the warning check from LexFilter.fs to lex.fsl, specifically in the `"///"` pattern handler at line 740-746.

**Status**: ✅ **CONFIRMED - THIS WAS THE ROOT CAUSE**

## Final Solution
1. ✅ Removed the incorrect warning check from LexFilter.fs (lines 2640-2660)
2. ✅ Added warning check in lex.fsl at line 740-746 (the `///` handler):
   ```fsharp
   let lastNonCommentLine = XmlDocStore.GetLastNonCommentTokenLine lexbuf
   if lastNonCommentLine = m.StartLine then
       warning(Error(FSComp.SR.xmlDocNotFirstOnLine(), m))
   ```
3. ✅ Build command: `./build.sh -c Release` - Time: 3m 43.9s - Errors: 0
4. ✅ Test command: `dotnet test artifacts/bin/FSharp.Compiler.Service.Tests/Release/net10.0/FSharp.Compiler.Service.Tests.dll --filter "FullyQualifiedName~XmlDocTests"`
5. ✅ Final Results: **60 total tests, 55 passed, 0 failed, 5 skipped, 6.8 seconds**
