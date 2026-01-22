You're right on both counts.  Let me fix that. 

---

## Final Migration Plan:  Nullness IFDEF & Shim Cleanup (Revised)

### Summary

With F#9 shipping NRT, the following cleanup can be performed: 
1. **Remove** the specific `#if BUILDING_WITH_LKG` **code blocks** that are now obsolete (but keep the define itself for future features)
2. **Remove** the `MaybeNull` type alias shim (use `'T | null` directly)
3. **Keep** useful helper functions (`!! `, `isNotNull`, `nullSafeEquality`, `objEqualsArg`)
4. **Rename** `NullnessShims. fs` → `NullHelpers.fs` (no longer "shims")
5. **Per-project decision** on whether to keep including `NullHelpers.fs` based on actual usage

---

### Phase 1: Remove Obsolete `#if BUILDING_WITH_LKG` Code Blocks

**Keep the define itself** in `FSharp. Profiles.props` and `Directory.Build.props` - it's useful for future feature development. 

**Remove these specific obsolete code blocks:**

| File | Lines | What to Remove |
|------|-------|----------------|
| `src/FSharp.Core/prim-types. fsi` | ~1221-1226 | `#if BUILDING_WITH_LKG \|\| BUILD_FROM_SOURCE` block for `byref<'T,'Kind>` - keep only `IsError=true` branch |
| `src/FSharp. Core/prim-types.fsi` | ~1237-1241 | Same for `ByRefKinds` module |
| `src/FSharp. Core/prim-types.fsi` | ~1243-1248 | Same for `ByRefKinds. Out` |
| `src/FSharp.Core/prim-types.fsi` | ~1251-1256 | Same for `ByRefKinds.In` |
| `src/FSharp.Core/prim-types.fsi` | ~1259-1264 | Same for `ByRefKinds. InOut` |
| `src/FSharp.Build/FSharpEmbedResourceText.fs` | Boilerplate | Remove `#if BUILDING_WITH_LKG` / `#nowarn "3261"` from generated code template |

---

### Phase 2:  Eliminate `MaybeNull` Type Alias

| Step | Details |
|------|---------|
| **2a. ** Find all `MaybeNull` usages | ~100+ usages across compiler, FSharp.Build, VS integration |
| **2b.** Replace with canonical syntax | `'T MaybeNull` → `'T | null`, `string MaybeNull` → `string | null` |
| **2c.** Remove from NullnessShims.fs | Delete:  `type 'T MaybeNull when 'T:  not null and 'T: not struct = 'T | null` |

---

### Phase 3: Remove `(^)` Null-Propagation Operator

| Step | Details |
|------|---------|
| **3a. ** Find all `(^)` usages | Limited usage - null propagation combinator |
| **3b.** Replace with explicit patterns | Use `match x with Null -> null | NonNull v -> ... ` or other idiomatic F# |
| **3c.** Remove from NullnessShims.fs | Delete the `(^)` operator definition |

---

### Phase 4: Evaluate `(|NonEmptyString|_|)` Active Pattern

| Step | Details |
|------|---------|
| **4a.** Check usage count | If widely used, keep; if minimal, inline |
| **4b.** Decision | If keeping, it stays in the renamed file |

---

### Phase 5: Rename the File

| Current | New |
|---------|-----|
| `src/Compiler/Utilities/NullnessShims.fs` | `src/Compiler/Utilities/NullHelpers.fs` |
| `module internal NullnessShims` | `module internal NullHelpers` |

**After cleanup, the file contains:**
```fsharp
namespace Internal. Utilities. Library

open System

[<AutoOpen>]
module internal NullHelpers =

    let inline isNotNull (x:  'T) = not (isNull x)

    let inline (! !) (x: 'T | null) = Unchecked.nonNull x

    let inline nullSafeEquality (x: 'T | null) (y: 'T | null) 
        ([<InlineIfLambda>] nonNullEqualityFunc: 'T -> 'T -> bool) =
        match x, y with
        | null, null -> true
        | null, _ | _, null -> false
        | x, y -> nonNullEqualityFunc (Unchecked.nonNull x) (Unchecked.nonNull y)

#if NET5_0_OR_GREATER
    type objEqualsArg = objnull
#else
    type objEqualsArg = obj
#endif

    // If kept after evaluation: 
    [<return: Struct>]
    let inline (|NonEmptyString|_|) (x: string | null) =
        match x with
        | null -> ValueNone
        | "" -> ValueNone
        | v -> ValueSome v
```

---

### Phase 6: Per-Project Include Decisions

After `MaybeNull` and `(^)` are removed, evaluate each project's actual usage of the remaining helpers:

| Project | Current Include | Evaluate |
|---------|-----------------|----------|
| `src/Compiler/FSharp.Compiler.Service. fsproj` | Yes | Uses `!!`, `isNotNull`, etc. → **Keep** |
| `src/FSharp.Build/FSharp.Build. fsproj` | Yes | Check if still uses helpers after `MaybeNull` removal |
| `src/FSharp. DependencyManager. Nuget/FSharp.DependencyManager.Nuget.fsproj` | Yes | Check if still uses helpers after `MaybeNull` removal |
| `vsintegration/src/FSharp. VS.FSI/FSharp.VS.FSI.fsproj` | Yes | Check if still uses helpers after `MaybeNull` removal |
| `vsintegration/tests/Salsa/VisualFSharp. Salsa. fsproj` | Yes | Check if still uses helpers after `MaybeNull` removal |
| `vsintegration/tests/UnitTests/VisualFSharp.UnitTests. fsproj` | Yes | Check if still uses helpers after `MaybeNull` removal |

**Action per project:** After Phase 2-3 replacements, if a project no longer uses any helpers from `NullHelpers. fs`, remove the include from that project's `.fsproj`.

---

### Phase 7: Keep User-Space Templates Unchanged

| File | Element | Action |
|------|---------|--------|
| `src/FSharp.Build/FSharpEmbedResourceText.fs` | `#if NULLABLE` / `#if !NULLABLE` inside string templates | **KEEP** - goes into user code |
| `src/FSharp.Build/FSharpEmbedResourceText. fs` | `#nowarn "3262"` in template | **KEEP** - user code compatibility |

---

### Estimated Effort

| Task | Count | Effort |
|------|-------|--------|
| Remove obsolete `#if` code blocks | 6 locations | Low |
| Replace `MaybeNull` type alias | ~100+ usages | Medium (mechanical) |
| Replace `(^)` operator | ~5-10 usages | Low |
| Rename file/module | 1 file + update module name | Low |
| Per-project include evaluation | 6 projects | Low |
| **Total** | | **Medium** |

---

### What Stays

| Element | Reason |
|---------|--------|
| `BUILDING_WITH_LKG` define | Useful for future features |
| `!!` operator | Concise, widely used |
| `isNotNull` | Readable, avoids double negation |
| `nullSafeEquality` | Useful pattern for null-safe comparisons |
| `objEqualsArg` | Legitimate .NET framework compatibility |
| User-space `#if NULLABLE` templates | User projects may have different Nullable settings |