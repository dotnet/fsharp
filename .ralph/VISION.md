# Issue 10273: CLIEvent Properties Recognized as Events

## High-Level Goal

Fix the F# Symbol API and XmlDoc generation so that members decorated with `[<CLIEvent>]` attribute are properly recognized as **events**, not **properties**:

1. **`FSharpMemberOrFunctionOrValue.IsEvent`** should return `true` for CLIEvent properties
2. **`XmlDocSig`** should use `"E:"` prefix instead of `"P:"` for CLIEvent properties
3. Display names should show `"event EventName"` instead of `"property EventName"`

## Problem Context

F# properties tagged with `[<CLIEvent>]` attribute are compiled to CLI events (not properties), but:
- The XmlDoc ID mentions them as properties (`P:...` prefix)
- `FSharpMemberOrFunctionOrValue.XmlDocSig` returns `P:` prefix
- `FSharpMemberOrFunctionOrValue.IsEvent` returns `false`

Example from issue:
```fsharp
type T() =
    /// An event.
    [<CLIEvent>]
    member x.E = Event<_>().Publish
```
Currently generates `P:Namespace.T.E` but should generate `E:Namespace.T.E`.

## Key Design Decisions

### 1. Fix at the Source, Not Post-Processing

**Reviewer feedback (auduchinok, T-Gro):** Don't patch the XmlDocSig string after generation. Fix it where the signature originates - in `XmlDocSigOfVal` in `TypedTreeOps.fs`.

### 2. Use Existing `IsFSharpEventProperty` Pattern (CRITICAL)

**Reviewer feedback (auduchinok):** The code in `TypedTreeOps.fs` should check `IsFSharpEventProperty` instead of manually checking for the CLIEvent attribute with `HasFSharpAttribute`.

**Problem with current PR:** Uses `HasFSharpAttribute g g.attrib_CLIEventAttribute v.Attribs` directly instead of using the canonical helper.

**Solution:** The `IsFSharpEventProperty` on `ValRef` in `infos.fs` checks:
```fsharp
member x.IsFSharpEventProperty g =
    x.IsMember && CompileAsEvent g x.Attribs && not x.IsExtensionMember
```

However, in `TypedTreeOps.fs` we work with `Val` not `ValRef`. We need to either:
- Create a helper that does the same check on `Val` 
- Or use `CompileAsEvent` directly (which is what `IsFSharpEventProperty` uses internally)

The key is NOT to use `HasFSharpAttribute` directly but to use the same logic as `IsFSharpEventProperty`.

### 3. Clean Code Quality

- No redundant comments that don't add value
- Use `{caret}` marker approach in tests (like other tests in the file)
- No cleanup comments that explain obvious code

## Architecture Notes

### Key Files and Their Roles

| File | Role |
|------|------|
| `src/Compiler/TypedTree/TypedTreeOps.fs` | `XmlDocSigOfVal` - generates XmlDoc signature for values |
| `src/Compiler/Checking/infos.fs` | `ValRef.IsFSharpEventProperty` and `PropInfo.IsFSharpEventProperty` |
| `src/Compiler/Symbols/Symbols.fs` | `FSharpMemberOrFunctionOrValue.IsEvent` and `XmlDocSig` property |
| `tests/FSharp.Compiler.Service.Tests/Symbols.fs` | Unit tests for CLIEvent recognition |
| `tests/FSharp.Compiler.Service.Tests/ProjectAnalysisTests.fs` | Integration tests (project3, project28) |
| `tests/FSharp.Compiler.Service.Tests/Common.fs` | `attribsOfSymbol` helper |

### Flow of XmlDocSig Generation

1. `XmlDocSigOfVal` in `TypedTreeOps.fs` generates the signature
2. For members with `PropertyGet`/`PropertySet`/`PropertyGetSet`:
   - Check if it's a CLIEvent property using same logic as `IsFSharpEventProperty`
   - Use `"E:"` prefix if yes, `"P:"` otherwise
3. `GetXmlDocSigOfValRef` in `InfoReader.fs` caches the result
4. `FSharpMemberOrFunctionOrValue.XmlDocSig` returns the cached value

## Unresolved Reviewer Feedback (PR 18584) - MUST FIX

| Reviewer | Comment | Location | Action Required |
|----------|---------|----------|-----------------|
| auduchinok | Use `IsFSharpEventProperty` instead of manual attribute check | TypedTreeOps.fs:9079 | Use `CompileAsEvent` (same as IsFSharpEventProperty uses internally) |
| auduchinok | Remove useless comments | Symbols.fs:1913 | Remove the "// CLIEvent properties should be considered events" comments |
| auduchinok | Use `{caret}` approach instead of manual coordinates | Symbols.fs:1301 | Rewrite test to use `Checker.getSymbolUse` with `{caret}` marker |
| auduchinok | Please cleanup | Common.fs | Remove the "// Only add prop tag..." comments |

## Test Style (IMPORTANT)

Use the `{caret}` marker approach like other tests in the file:
```fsharp
let symbolUse = Checker.getSymbolUse """
type T() =
    [<CLIEvent>]
    member this.Ev{caret}ent = Event<int>().Publish
"""
```

NOT the manual coordinate approach:
```fsharp
// DON'T DO THIS
checkResults.GetSymbolUsesAtLocation(4, 21, "...", [ "Event" ])
```

## Lessons from Previous Attempt

1. ❌ Used `HasFSharpAttribute` directly instead of using same logic as `IsFSharpEventProperty`
2. ❌ Added noisy comments explaining obvious code
3. ❌ Test used manual coordinates instead of `{caret}` marker approach
4. ❌ Did not address ALL reviewer feedback

## Definition of Done

- [ ] `IsEvent` returns `true` for CLIEvent properties
- [ ] `XmlDocSig` uses `E:` prefix for CLIEvent properties  
- [ ] Uses `CompileAsEvent` check (same as `IsFSharpEventProperty`) in TypedTreeOps.fs
- [ ] All existing tests pass (including project3, project28)
- [ ] New test uses `{caret}` marker approach
- [ ] No redundant comments in production code
- [ ] No redundant comments in test helpers
- [ ] Release notes added to `docs/release-notes/.FSharp.Compiler.Service/11.0.0.md`
- [ ] Formatting passes
