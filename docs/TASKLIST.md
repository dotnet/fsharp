# Find All References & Rename Symbol - Issue Tracking

**Reference:** [SEARCH_AND_RENAME_ARCHITECTURE.md](./SEARCH_AND_RENAME_ARCHITECTURE.md)

---

## Summary

| Category | Count | Status |
|----------|-------|--------|
| FindAllReferences Bugs | 6 | Pending |
| FindAllReferences Feature Improvements | 2 | Pending |
| FindAllReferences Feature Requests | 3 | Pending |
| RenameSymbol Bugs | 5 | Pending |
| RenameSymbol Feature Improvements | 1 | Pending |
| RenameSymbol Feature Requests | 1 | Pending |
| **Total** | **18** | **Pending** |

---

## Issue Clusters

### Cluster 1: Active Pattern Issues
Both rename and find all references share problems with active patterns due to how they're stored in ItemKeyStore.

| Issue | Title | Type | Labels | Status |
|-------|-------|------|--------|--------|
| [#19173](https://github.com/dotnet/fsharp/issues/19173) | FindBackgroundReferencesInFile for TransparentCompiler not returning Partial/Active Pattern Values | Bug | Area-LangService-FindAllReferences | [ ] |
| [#14969](https://github.com/dotnet/fsharp/issues/14969) | Finding references / renaming doesn't work for active patterns in signature files | Bug | Impact-Medium, Area-LangService-FindAllReferences | [ ] |

**Root Cause:** Active patterns are written to `ItemKeyStore` as `Item.Value` with no case information. In signature files, they don't get proper `SymbolUse` entries.

**Likely Fix Location:** `src/Compiler/Service/ItemKey.fs` - `writeActivePatternCase` and how active patterns are captured in name resolution.

**Test Exists:** `FindReferences.fs` - `ActivePatterns` module has test showing the issue (line 579-593)

---

### Cluster 2: Operator Rename Issues
Operators have special naming and parsing requirements that cause rename problems.

| Issue | Title | Type | Labels | Status |
|-------|-------|------|--------|--------|
| [#17221](https://github.com/dotnet/fsharp/issues/17221) | Support / fix replacing reference (Refactor -> Rename) of F# operator | Feature Improvement | Area-LangService-RenameSymbol | [ ] |
| [#14057](https://github.com/dotnet/fsharp/issues/14057) | In Visual Studio: Renaming operator with `.` only renames right of `.` | Bug | Impact-Medium, Area-VS-Editor, Area-LangService-RenameSymbol | [ ] |

**Root Cause:** Operator symbol handling in `Tokenizer.fixupSpan` and `Tokenizer.isValidNameForSymbol` doesn't handle all operator cases correctly.

**Likely Fix Location:** 
- `vsintegration/src/FSharp.Editor/LanguageService/Tokenizer.fs`
- `vsintegration/src/FSharp.Editor/InlineRename/InlineRenameService.fs`

**Test Exists:** `FindReferences.fs` - `We find operators` test (line 173-185)

---

### Cluster 3: Property/Member Rename Issues
Properties with get/set accessors have incorrect rename behavior.

| Issue | Title | Type | Labels | Status |
|-------|-------|------|--------|--------|
| [#18270](https://github.com/dotnet/fsharp/issues/18270) | Renaming property renames get and set keywords use braking the code | Bug | Impact-Medium, Area-LangService-RenameSymbol | [ ] |
| [#15399](https://github.com/dotnet/fsharp/issues/15399) | Interface renaming works weirdly in some edge cases | Bug | Impact-Medium, Area-LangService-RenameSymbol, Tracking-External | [ ] |

**Root Cause:** The range returned for property symbols includes the accessor keywords (`get`/`set`), not just the property name.

**Likely Fix Location:**
- `src/Compiler/Service/ItemKey.fs` - `writeValRef` / `writeValue` for property handling
- `vsintegration/src/FSharp.Editor/LanguageService/Tokenizer.fs` - `fixupSpan`

**Test Needed:** Add test for property rename with get/set accessors

---

### Cluster 4: Symbol Resolution Edge Cases

| Issue | Title | Type | Labels | Status |
|-------|-------|------|--------|--------|
| [#5546](https://github.com/dotnet/fsharp/issues/5546) | Get all symbols: all symbols in SynPat.Or patterns considered bindings | Bug | Impact-Low, Area-LangService-FindAllReferences | [ ] |
| [#5545](https://github.com/dotnet/fsharp/issues/5545) | Symbols are not always found in SAFE bookstore project | Bug | Impact-Low, Area-LangService-FindAllReferences | [ ] |
| [#4136](https://github.com/dotnet/fsharp/issues/4136) | Symbols API: GetAllUsesOfAllSymbolsInFile contains generated handler value for events | Bug | Impact-Low, Area-LangService-FindAllReferences | [ ] |

**Root Cause:** Name resolution captures incorrect or synthetic symbols in certain patterns.

**Likely Fix Location:** 
- `src/Compiler/Checking/NameResolution.fs` - Symbol capture logic
- Filter synthetic symbols when building ItemKeyStore

**Test Needed:** Tests for `SynPat.Or` patterns, event handlers

---

### Cluster 5: Directive/Generated Code Issues

| Issue | Title | Type | Labels | Status |
|-------|-------|------|--------|--------|
| [#9928](https://github.com/dotnet/fsharp/issues/9928) | Find References doesn't work if #line directives are used | Bug | Impact-Medium, Area-LangService-FindAllReferences | [ ] |
| [#16394](https://github.com/dotnet/fsharp/issues/16394) | Roslyn crashes F# rename when F# project contains `cshtml` file | Bug | Impact-Low, Area-LangService-RenameSymbol | [ ] |

**Root Cause:** Range remapping for `#line` directives not handled; Roslyn interop issues with generated files.

**Likely Fix Location:**
- Range handling in ItemKeyStore and service layer
- Roslyn integration in FSharp.Editor

**Test Needed:** Test with `#line` directives

---

### Cluster 6: Constructor/Type Reference Improvements

| Issue | Title | Type | Labels | Status |
|-------|-------|------|--------|--------|
| [#14902](https://github.com/dotnet/fsharp/issues/14902) | Finding references of additional constructors in VS | Feature Request | Area-LangService-FindAllReferences | [ ] |
| [#15290](https://github.com/dotnet/fsharp/issues/15290) | Find all references of records should include copy-and-update and construction | Feature Improvement | Area-LangService-FindAllReferences | [ ] |
| [#16621](https://github.com/dotnet/fsharp/issues/16621) | Find all references of a DU case should include case testers | Feature Request | Area-LangService-FindAllReferences, help wanted | [ ] |

**Root Cause:** Implicit constructions and testers are not captured as symbol uses.

**Likely Fix Location:**
- Name resolution to capture implicit constructor calls
- `ItemKeyStore` to include tester patterns

**Test Exists:** `FindReferences.fs` has constructor tests (lines 33-51)

---

### Cluster 7: Performance/Optimization

| Issue | Title | Type | Labels | Status |
|-------|-------|------|--------|--------|
| [#10227](https://github.com/dotnet/fsharp/issues/10227) | [VS] Find-all references on symbol from referenced DLL optimization | Feature Request | Area-LangService-FindAllReferences | [ ] |

**Root Cause:** When searching for external DLL symbols, all projects are checked. Should only check projects referencing that DLL.

**Likely Fix Location:** `vsintegration/src/FSharp.Editor/LanguageService/SymbolHelpers.fs` - `findSymbolUses` scope determination

---

### Cluster 8: Miscellaneous

| Issue | Title | Type | Labels | Status |
|-------|-------|------|--------|--------|
| [#15721](https://github.com/dotnet/fsharp/issues/15721) | Renaming works weirdly for disposable types | Bug | Impact-Medium, Area-LangService-RenameSymbol | [ ] |
| [#16993](https://github.com/dotnet/fsharp/issues/16993) | Go to definition and Find References not working for C# extension method `AsMemory()` in this repo | Feature Improvement | Area-LangService-FindAllReferences, Area-LangService-Navigation | [ ] |
| [#4760](https://github.com/dotnet/fsharp/issues/4760) | Rename does not work in strings | Feature Request | Area-LangService-RenameSymbol | [ ] |

---

## Detailed Issue Checklist

### BUGS (Priority: Fix First)

#### FindAllReferences Bugs

- [ ] **#19173** - FindBackgroundReferencesInFile for TransparentCompiler not returning Partial/Active Pattern Values
  - **Type:** Bug
  - **Impact:** TransparentCompiler path broken for active patterns
  - **Likely Cause:** Active pattern cases not properly written to ItemKeyStore in TransparentCompiler
  - **Likely Fix:** Fix `ComputeItemKeyStore` in TransparentCompiler.fs or active pattern capture
  - **Test:** Add test comparing BackgroundCompiler vs TransparentCompiler for active patterns

- [ ] **#14969** - Finding references / renaming doesn't work for active patterns in signature files
  - **Type:** Bug
  - **Impact:** Medium - active patterns in .fsi files not found
  - **Likely Cause:** Active patterns stored as single `Item.Value` without case info
  - **Likely Fix:** Modify `ItemKeyStoreBuilder.writeActivePatternCase` to handle signature files
  - **Test:** Existing test at FindReferences.fs:579-593 shows issue

- [ ] **#9928** - Find References doesn't work if #line directives are used
  - **Type:** Bug
  - **Impact:** Medium - generated code scenarios broken
  - **Likely Cause:** Range not remapped for #line directives
  - **Likely Fix:** Handle range remapping in ItemKeyStore or service layer
  - **Test:** Add test with #line directive

- [ ] **#5546** - Get all symbols: all symbols in SynPat.Or patterns considered bindings
  - **Type:** Bug
  - **Impact:** Low - incorrect IsFromDefinition classification
  - **Likely Cause:** Both sides of Or pattern marked as bindings
  - **Likely Fix:** Fix in NameResolution.fs symbol capture
  - **Test:** Add test for SynPat.Or patterns

- [ ] **#5545** - Symbols are not always found in SAFE bookstore project
  - **Type:** Bug
  - **Impact:** Low - intermittent missing references
  - **Likely Cause:** Race condition or caching issue
  - **Likely Fix:** Investigate and fix caching/ordering
  - **Test:** Need repro project

- [ ] **#4136** - Symbols API: GetAllUsesOfAllSymbolsInFile contains generated handler value for events
  - **Type:** Bug
  - **Impact:** Low - synthetic symbols appearing
  - **Likely Cause:** Generated `handler` value not filtered
  - **Likely Fix:** Filter synthetic symbols in ItemKeyStore builder
  - **Test:** Add test for event handler filtering

#### RenameSymbol Bugs

- [ ] **#18270** - Renaming property renames get and set keywords use braking the code
  - **Type:** Bug
  - **Impact:** Medium - property rename breaks code
  - **Likely Cause:** Range includes get/set keywords
  - **Likely Fix:** Fix range calculation in Tokenizer.fixupSpan
  - **Test:** Add property with get/set rename test

- [ ] **#16394** - Roslyn crashes F# rename when F# project contains `cshtml` file
  - **Type:** Bug
  - **Impact:** Low - Roslyn interop crash
  - **Likely Cause:** Generated .cshtml files not handled
  - **Likely Fix:** Filter or handle non-F# files in rename locations
  - **Test:** Add project with cshtml file

- [ ] **#15721** - Renaming works weirdly for disposable types
  - **Type:** Bug
  - **Impact:** Medium - rename timing issues
  - **Likely Cause:** Warning preventing rename, or race condition
  - **Likely Fix:** Investigate async rename flow
  - **Test:** Add disposable type rename test

- [ ] **#15399** - Interface renaming works weirdly in some edge cases
  - **Type:** Bug
  - **Impact:** Medium - interface rename broken
  - **Likely Cause:** Interface implementation not tracked correctly
  - **Likely Fix:** Fix interface member symbol resolution
  - **Test:** Add interface rename edge case tests

- [ ] **#14057** - In Visual Studio: Renaming operator with `.` only renames right of `.`
  - **Type:** Bug
  - **Impact:** Medium - operator rename broken
  - **Likely Cause:** Tokenizer splits on `.` incorrectly
  - **Likely Fix:** Fix Tokenizer.getSymbolAtPosition for operators
  - **Test:** Add operator with `.` rename test

---

### FEATURE IMPROVEMENTS (Priority: Second)

- [ ] **#17221** - Support / fix replacing reference (Refactor -> Rename) of F# operator
  - **Type:** Feature Improvement
  - **Current:** Operators cannot be renamed to other operators
  - **Needed:** Allow renaming operators with proper validation
  - **Likely Fix:** Update `Tokenizer.isValidNameForSymbol` for operators

- [ ] **#16993** - Go to definition and Find References not working for C# extension method `AsMemory()` in this repo
  - **Type:** Feature Improvement
  - **Current:** C# extension methods not found
  - **Needed:** Cross-language extension method support
  - **Likely Fix:** Enhance symbol resolution for IL extension methods

- [ ] **#15290** - Find all references of records should include copy-and-update and construction
  - **Type:** Feature Improvement
  - **Current:** `{ x with Field = value }` not found
  - **Needed:** Capture implicit record constructor usage
  - **Likely Fix:** Extend name resolution to capture these patterns

---

### FEATURE REQUESTS (Priority: Third)

- [ ] **#16621** - Find all references of a DU case should include case testers
  - **Type:** Feature Request
  - **Current:** `A.IsB` not found as reference to B
  - **Needed:** Include case testers in references
  - **Likely Fix:** Capture tester usage in name resolution

- [ ] **#14902** - Finding references of additional constructors in VS
  - **Type:** Feature Request
  - **Current:** `new()` constructor uses not found from `new` keyword
  - **Needed:** Associate additional constructor uses with constructor definition
  - **Likely Fix:** Enhance constructor symbol resolution

- [ ] **#10227** - [VS] Find-all references on symbol from referenced DLL optimization
  - **Type:** Feature Request
  - **Current:** All projects searched for external symbols
  - **Needed:** Only search projects that reference the DLL
  - **Likely Fix:** Filter projects by DLL references in SymbolHelpers.fs

- [ ] **#4760** - Rename does not work in strings
  - **Type:** Feature Request
  - **Current:** String literals not included in rename
  - **Needed:** Option to rename in strings/comments
  - **Likely Fix:** Add text search alongside symbol search

---

## Test Commands

```bash
# Run all FindReferences tests
dotnet test tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  -c Release --filter "FullyQualifiedName~FindReferences" -v normal

# Run specific test
dotnet test tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  -c Release --filter "Name~active patterns" -v normal

# Run with transparent compiler
USE_TRANSPARENT_COMPILER=1 dotnet test tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  -c Release --filter "FullyQualifiedName~FindReferences" -v normal
```

---

## Priority Order for Fixing

1. **High Priority Bugs** (breaks core functionality):
   - #19173 - TransparentCompiler active patterns (affects new compiler)
   - #18270 - Property rename breaking code
   - #14969 - Active patterns in signature files

2. **Medium Priority Bugs** (edge cases with workarounds):
   - #14057 - Operator rename with `.`
   - #15399 - Interface rename edge cases
   - #15721 - Disposable type rename
   - #9928 - #line directive references

3. **Low Priority Bugs** (minor issues):
   - #16394 - cshtml crash (Roslyn issue)
   - #5546 - SynPat.Or binding classification
   - #5545 - Intermittent missing symbols
   - #4136 - Event handler synthetic symbols

4. **Feature Improvements**:
   - #17221 - Operator rename support
   - #15290 - Record copy-update references
   - #16993 - C# extension methods

5. **Feature Requests**:
   - #16621 - DU case tester references
   - #14902 - Additional constructor references
   - #10227 - DLL reference optimization
   - #4760 - Rename in strings
