# F# Query Expression Bug Fixes - Vision & Architecture

## High-Level Goal

Fix 11 open bugs in the F# query expression system that affect LINQ provider compatibility, particularly with Entity Framework Core. These bugs prevent F# from being a first-class citizen for database access scenarios.

## Approach

**Incremental fixes grouped by root cause**, not by GitHub issue number. Many issues share underlying causes:

### Root Cause Clusters

1. **Expression Tree Generation Issues** (Issues #11131, #15648, #16918, #3782)
   - Anonymous record field ordering affects generated expression trees
   - Array indexing uses `GetArray` instead of `get_Item`
   - Tuple creation wraps in non-translatable delegates
   - **Common fix area:** `Linq.fs` - `LeafExpressionConverter` quotation-to-LINQ translation

2. **Query Builder Translation Issues** (Issues #7885, #47, #15133)
   - Tuple join keys don't match correctly
   - GroupBy with tuples fails member access
   - Multi-value selections break composability
   - **Common fix area:** `Query.fs` - `QueryBuilder` translation methods

3. **Edge Case Handling** (Issues #19099, #3845, #3445)
   - `EvaluateQuotation` missing patterns (Sequential, VarSet, void returns)
   - `headOrDefault` returns null for non-nullable types
   - Conditional without else branch fails
   - **Common fix area:** Various - targeted fixes

4. **Compiler Diagnostics** (Issue #422)
   - FS1182 false positive in query expressions
   - **Fix area:** `CheckComputationExpressions.fs`

## Key Design Decisions

### 1. Preserve Backward Compatibility
All fixes must maintain backward compatibility with existing code. Expression trees may change internal structure but must produce equivalent results.

### 2. Testing Strategy
- Add tests to `tests/FSharp.Core.UnitTests/` for runtime behavior
- Use `AsQueryable()` for most tests - no external database needed
- Verify both expression tree structure AND execution results
- Use baseline tests where IL/expression trees are verified

### 3. Minimal Changes
Each fix should be surgical - change only what's necessary to fix the bug. Don't refactor unrelated code.

### 4. Issue #11131 and #15648 are Duplicates
Both describe anonymous record field ordering affecting expression translation. Fix once, close both.

### 5. Breaking Change Consideration for #3845
`headOrDefault` returning null for non-nullable types is fundamentally unsound. Options:
- **Option A:** Add compiler warning when T doesn't admit null (preferred - non-breaking)
- **Option B:** Change return type to `ValueOption<'T>` (breaking)
- Decision: Start with Option A (warning), evaluate breaking change for future version

## Important Context for Sprints

### Build Commands
```bash
# Full build and test on Linux/Mac
./build.sh -c Release --testcoreclr

# Update baselines
TEST_UPDATE_BSL=1 ./build.sh -c Release --testcoreclr

# Surface area tests only
TEST_UPDATE_BSL=1 dotnet test tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --filter "SurfaceAreaTest" -c Release /p:BUILDING_USING_DOTNET=true
```

### File Locations
| Purpose | Location |
|---------|----------|
| Query builder | `src/FSharp.Core/Query.fs` |
| LINQ expression conversion | `src/FSharp.Core/Linq.fs` |
| Query extensions | `src/FSharp.Core/QueryExtensions.fs` |
| CE checking | `src/Compiler/Checking/Expressions/CheckComputationExpressions.fs` |
| Query tests | `tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Linq/` |
| Integration tests | `tests/fsharp/core/queriesOverIQueryable/` |

### Dependencies Between Issues
- #15648 depends on or duplicates #11131 (same root cause)
- #3782 may improve with #11131 fix (tuple handling)
- #47 shares tuple translation concerns with #7885

## Constraints

1. **No external NuGet packages** - codebase is self-contained
2. **Target .NET Standard 2.0** for FSharp.Core
3. **Surface area baselines** will change if public API is modified
4. **ILVerify** may flag new IL patterns - update baselines if legitimate

## Lessons Learned

### Issue #3845: headOrDefault with non-nullable types
- **Problem**: `headOrDefault` returns `null` for empty sequences when T is a reference type (including F# tuples). Accessing tuple fields on null causes NRE.
- **Root cause**: LINQ's `FirstOrDefault()` returns `default(T)` which is `null` for reference types.
- **Attempted fix**: Cannot be fixed in FSharp.Core alone because the return type is `'T`, and for reference types `Unchecked.defaultof<'T>` is `null`.
- **Proper solution**: Compiler warning when T doesn't admit null (Option A from design decisions). This requires changes to CheckComputationExpressions.fs, not FSharp.Core.
- **Current status**: Documented as known limitation with test demonstrating the behavior.

### Issue #422: FS1182 false positive in query expressions
- **Problem**: When using `--warnon:1182`, query expressions like `for x in source do where (x > 2) select 1` incorrectly report that `x` is unused, even though it's used in the `where` clause.
- **Root cause**: Query expression translation creates two separate sets of Vals for the same pattern:
  1. **varSpace Vals**: Created in `addVarsToVarSpace` via `TcMatchPattern` for tracking the query variable space
  2. **Lambda Vals**: Created when the generated `SynMatchClause` is typechecked
  
  These are different Val objects even though they have the same name. The varSpace Vals are used to build synthetic expressions (e.g., `yield x`), but the FS1182 warning is about the Lambda Vals which may not be directly referenced.
- **Attempted fixes**:
  1. Marking varSpace Vals as referenced in `addVarsToVarSpace` - doesn't work because Lambda Vals are different objects
  2. Marking varSpace Vals as referenced in `mkExprForVarSpace`/`mkSimplePatForVarSpace`/`mkPatForVarSpace` - same issue
- **Proper solution**: Requires deeper changes to query expression typechecking:
  - Option A: Share Vals between varSpace and the generated lambdas (avoid typechecking the pattern twice)
  - Option B: Mark Lambda Vals as referenced based on matching names with varSpace Vals
  - Option C: Add query-specific context to suppress FS1182 for query pattern bindings
- **Workaround**: Users can prefix query variables with underscore (e.g., `for _x in source do select 1`)
- **Current status**: Documented as known limitation. The warning is off by default and only appears with `--warnon:1182`.

### LINQ Expression Pattern Handlers
- When adding new handlers to `ConvExprToLinqInContext`, ensure the LINQ Expression equivalent exists:
  - Sequential → Expression.Block
  - VarSet → Expression.Assign 
  - FieldSet → Expression.Assign(Expression.Field(...))
  - PropertySet → Expression.Assign(Expression.Property(...))
- For void-returning expressions, use `Action<_>` delegates instead of `Func<_, _>`.

---

## Sprint Execution Notes

Each sprint produces a **tested, verified increment**. The agent will:
1. Write/update tests first (TDD where applicable)
2. Implement the fix
3. Run core build and tests
4. Update baselines if needed
5. Verify the fix addresses the specific issue behavior
