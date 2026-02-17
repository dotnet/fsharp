# `#elif` Preprocessor Directive

- Language suggestion: [fslang-suggestions#1370](https://github.com/fsharp/fslang-suggestions/issues/1370) (approved-in-principle)

F# adds the `#elif` preprocessor directive for conditional compilation, aligning with C# and reducing nesting depth when checking multiple conditions.

## Motivation

Currently F# only has `#if`, `#else`, and `#endif` for conditional compilation. When you need to check multiple conditions, you must nest `#if` inside `#else` blocks, leading to deeply indented code:

```fsharp
let myPath =
#if WIN64
    "/library/x64/runtime.dll"
#else
    #if WIN86
        "/library/x86/runtime.dll"
    #else
        #if MAC
            "/library/iOS/runtime-osx.dll"
        #else
            "/library/unix/runtime.dll"
        #endif
    #endif
#endif
```

An alternative workaround uses repeated `#if` blocks with complex negated conditions, which is error-prone and verbose:

```fsharp
let myPath =
#if WIN64
    "/library/x64/runtime.dll"
#endif
#if WIN86
    "/library/x86/runtime.dll"
#endif
#if MAC
    "/library/iOS/runtime-osx.dll"
#endif
#if !WIN64 && !WIN86 && !MAC
    "/library/unix/runtime.dll"
#endif
```

Both approaches are harder to read, maintain, and extend than they need to be.

## Feature Description

With `#elif`, the same logic is flat and clear:

```fsharp
let myPath =
#if WIN64
    "/library/x64/runtime.dll"
#elif WIN86
    "/library/x86/runtime.dll"
#elif MAC
    "/library/iOS/runtime-osx.dll"
#else
    "/library/unix/runtime.dll"
#endif
```

### Semantics

- `#elif` is short for "else if" in the preprocessor.
- It evaluates its condition only if no previous `#if` or `#elif` branch in the same chain was active.
- Only one branch in a `#if`/`#elif`/`#else`/`#endif` chain is ever active.
- `#elif` supports the same boolean expressions as `#if`: identifiers, `&&`, `||`, `!`, and parentheses.
- `#elif` can appear zero or more times between `#if` and `#else`/`#endif`.
- `#elif` after `#else` is an error.
- `#elif` without a matching `#if` is an error.
- `#elif` blocks can be nested inside other `#if`/`#elif`/`#else` blocks.
- Each `#elif` must appear at the start of a line (same rule as `#if`/`#else`/`#endif`).

## Detailed Semantics

The following table shows which branch is active for a `#if A` / `#elif B` / `#else` chain under all combinations of A and B:

| Source | A=true, B=true | A=true, B=false | A=false, B=true | A=false, B=false |
|---|---|---|---|---|
| `#if A` block | **active** | **active** | skip | skip |
| `#elif B` block | skip | skip | **active** | skip |
| `#else` block | skip | skip | skip | **active** |

Only the **first** matching branch is active. When both A and B are true, the `#if A` block is active and the `#elif B` block is skipped — the `#elif` condition is never evaluated.

## Language Version

- This feature requires F# 11.0 (language version `11.0` or `preview`).
- Using `#elif` with an older language version produces a compiler error directing the user to upgrade.

## F# Language Specification Changes

This feature requires changes to [section 3.3 Conditional Compilation](https://fsharp.github.io/fslang-spec/lexical-analysis/#33-conditional-compilation) of the F# Language Specification.

### Grammar

**Current spec grammar:**

```
token if-directive = "#if" whitespace if-expression-text
token else-directive = "#else"
token endif-directive = "#endif"
```

**Proposed spec grammar (add `elif-directive`):**

```
token if-directive = "#if" whitespace if-expression-text
token elif-directive = "#elif" whitespace if-expression-text
token else-directive = "#else"
token endif-directive = "#endif"
```

### Description Updates

The current spec says:

> If an `if-directive` token is matched during tokenization, text is recursively tokenized until a corresponding `else-directive` or `endif-directive`.

This should be updated to:

> If an `if-directive` token is matched during tokenization, text is recursively tokenized until a corresponding `elif-directive`, `else-directive`, or `endif-directive`. An `elif-directive` evaluates its condition only if no preceding `if-directive` or `elif-directive` in the same chain evaluated to true. At most one branch in an `if`/`elif`/`else` chain is active.

Additionally:

> An `elif-directive` must appear after an `if-directive` or another `elif-directive`, and before any `else-directive`. An `elif-directive` after an `else-directive` is an error.

## Tooling Impact

This change affects:

- **Fantomas** (F# code formatter) — will need to recognize `#elif` for formatting.
- **FSharp.Compiler.Service consumers** — new `ConditionalDirectiveTrivia.Elif` case, new `FSharpTokenKind.HashElif`.
- **Any tool** that processes F# source code with preprocessor directives.

These are non-breaking changes since `#elif` was previously a syntax error, so no existing valid F# code uses it.

## Compatibility

- **Backward compatible**: Old code without `#elif` continues to work unchanged.
- **Forward compatible**: Code using `#elif` will produce a clear error on older compilers when `#elif` appears in active code. However, if `#elif` appears inside an inactive `#if` branch (e.g., `#if FALSE` / `#elif X` / `#endif`), older compilers silently skip the `#elif` line as inactive text without error, potentially producing wrong branch selection. The language version gate (`LanguageFeature.PreprocessorElif` at F# 11.0) prevents this scenario in practice by requiring a compiler that understands `#elif`.
