# Vision: Splitting FSharp.Compiler.Service into Multiple Projects

## High-Level Goal

Split the monolithic `FSharp.Compiler.Service.fsproj` (~460+ source files) into multiple smaller projects with proper `ProjectReference` dependencies. This enables:

1. **Faster incremental builds** - MSBuild only rebuilds changed projects
2. **Parallel compilation** - Independent projects can build simultaneously
3. **Better code organization** - Clearer module boundaries

## Key Design Decisions

### 1. Project Split Architecture

Based on analysis of the compilation phases (see `docs/overview.md`) and file ordering in the .fsproj, the following project structure is proposed:

```
┌─────────────────────────────────────────────────────────────────────┐
│                     FSharp.Compiler.Service                         │
│  (Final assembly - references all below, adds Service/Interactive)  │
└─────────────────────────┬───────────────────────────────────────────┘
                          │
        ┌─────────────────┼─────────────────┐
        │                 │                 │
        ▼                 ▼                 ▼
┌───────────────┐ ┌───────────────┐ ┌───────────────┐
│  Symbols      │ │   Driver      │ │ Interactive   │
│ (Symbol API)  │ │(fsc/compiler) │ │   (fsi)       │
└───────┬───────┘ └───────┬───────┘ └───────┬───────┘
        │                 │                 │
        └─────────────────┼─────────────────┘
                          │
                          ▼
              ┌───────────────────────┐
              │       CodeGen         │
              │   (IlxGen, Erase*)    │
              └───────────┬───────────┘
                          │
                          ▼
              ┌───────────────────────┐
              │     Optimizations     │
              │ (Optimizer, Lower*)   │
              └───────────┬───────────┘
                          │
                          ▼
              ┌───────────────────────┐
              │      Checking         │
              │(TypeCheck, Patterns)  │
              └───────────┬───────────┘
                          │
        ┌─────────────────┴─────────────────┐
        │                                   │
        ▼                                   ▼
┌───────────────────┐             ┌───────────────────┐
│    TypedTree      │             │     Parsing       │
│(TAST, TcGlobals)  │             │  (Lex/Parse/AST)  │
└────────┬──────────┘             └─────────┬─────────┘
         │                                  │
         └──────────────┬───────────────────┘
                        │
                        ▼
              ┌───────────────────────┐
              │      AbstractIL       │
              │ (IL types, read/write)│
              └───────────┬───────────┘
                          │
                          ▼
              ┌───────────────────────┐
              │       Utilities       │
              │(Facilities + Utils)   │
              └───────────────────────┘
```

### 2. Parallelization Opportunities

The key insight from `docs/overview.md` is that **Parsing** and **IL Import** (AbstractIL reading) are independent:
- Parsing produces AST from source files
- IL Import reads referenced assemblies  
- Neither depends on the other; both feed into TypeChecking

Therefore:
- `AbstractIL` (il.fs, ilread.fs, ilwrite.fs etc.) can be built in parallel with `Parsing` (SyntaxTree files, lexer/parser)
- Both converge at `Checking` which needs TypedTree (depends on both)

### 3. Project Boundaries (Proposed 8 Projects)

| Project | Contents | Dependencies |
|---------|----------|--------------|
| **FSharp.Compiler.Utilities** | Utilities/*.fs + Facilities/*.fs (prim-lexing, prim-parsing, ranges, diagnostics) | FSharp.Core |
| **FSharp.Compiler.AbstractIL** | AbstractIL/*.fs (IL types, binary read/write, illex/ilpars) | Utilities |
| **FSharp.Compiler.SyntaxTree** | SyntaxTree/*.fs + lex.fsl/pars.fsy outputs (AST, parsing, lexing) | AbstractIL (for inline IL parsing) |
| **FSharp.Compiler.TypedTree** | TypedTree/*.fs (TAST, TcGlobals, TypeProviders, Pickle) | SyntaxTree |
| **FSharp.Compiler.Checking** | Checking/*.fs (import, type checking, constraint solving) | TypedTree |
| **FSharp.Compiler.Optimize** | Optimize/*.fs (Optimizer, Lower*, Detuple) | Checking |
| **FSharp.Compiler.CodeGen** | CodeGen/*.fs (IlxGen, EraseClosures/Unions) | Optimize |
| **FSharp.Compiler.Service** | Driver/*.fs, Symbols/*.fs, Service/*.fs, Interactive/*.fs | CodeGen |

### 4. Key Technical Constraints

1. **FsLex/FsYacc handling**: 
   - IL lexer/parser (illex.fsl, ilpars.fsy) → AbstractIL project
   - F# lexer/parser (lex.fsl, pars.fsy, pplex.fsl, pppars.fsy) → SyntaxTree project
   - Each project needs its own FsLex/FsYacc targets

2. **NoWarn directives**: Keep central in each project that needs them (44, 57, 75, 1204, etc.)

3. **Embedded resources**: 
   - FSComp.txt → Utilities (or wherever DiagnosticsLogger lives)
   - FSIstrings.txt → Service (for Interactive)
   - FSStrings.resx → Utilities

4. **InternalsVisibleTo**: Must be preserved on final FSharp.Compiler.Service.dll

5. **Link files, don't move**: Use `<Compile Include="..\..\path\file.fs"><Link>folder\file.fs</Link></Compile>` to avoid changing file locations

6. **Type circularity**: TypedTree references AbstractIL.IL types extensively; SyntaxTree is independent of AbstractIL

### 5. Dependencies Between Files (Key Observations)

From open statements analysis:

- **il.fs** opens: `FSharp.Compiler.IO`, `Internal.Utilities.Library`, `FSharp.Compiler.AbstractIL.Diagnostics` (internal)
- **SyntaxTree.fs** opens: `FSharp.Compiler.Text`, `FSharp.Compiler.Xml`, `FSharp.Compiler.SyntaxTrivia` (no AbstractIL!)
- **TypedTree.fs** opens: `FSharp.Compiler.AbstractIL.IL`, `FSharp.Compiler.Syntax`, `FSharp.Compiler.QuotationPickler`, etc.
- **import.fs** opens: `FSharp.Compiler.AbstractIL.IL`, `FSharp.Compiler.TypedTree`, `FSharp.Compiler.TcGlobals`
- **IlxGen.fs** opens: `FSharp.Compiler.AbstractIL.IL`, `FSharp.Compiler.TypedTree`, `FSharp.Compiler.Optimizer`, `FSharp.Compiler.Import`

This confirms:
- ~~SyntaxTree is independent of AbstractIL~~ **CORRECTED**: SyntaxTree DOES depend on AbstractIL - see Subtask 3 notes
- TypedTree depends on both AbstractIL and SyntaxTree ✓
- Checking depends on TypedTree (and transitively AbstractIL, SyntaxTree) ✓

### 6. Build Graph for Parallelization (REVISED)

```
                    FSharp.Core
                         │
                         ▼
              FSharp.Compiler.Utilities
                         │
                         ▼
              FSharp.Compiler.AbstractIL
                         │
                         ▼
              FSharp.Compiler.SyntaxTree
                         │
                         ▼
              FSharp.Compiler.TypedTree
                         │
                         ▼
              FSharp.Compiler.Checking
                         │
                         ▼
              FSharp.Compiler.Optimize
                         │
                         ▼
              FSharp.Compiler.CodeGen
                         │
                         ▼
              FSharp.Compiler.Service
```

**Note**: Original vision assumed AbstractIL and SyntaxTree could build in parallel. 
However, SyntaxTree depends on AbstractIL due to inline IL parsing (ParseHelpers.fs uses 
`AsciiParser`/`AsciiLexer` from AbstractIL to parse `(# ... #)` inline IL syntax).

## Important Gotchas

1. **Module recursion**: Some modules use `module rec` - ensure all files for a recursive module stay in same project

2. **Conditional compilation**: `#if NO_TYPEPROVIDERS` and similar must be handled consistently

3. **Proto configuration**: Building proto compiler has special rules (`Configuration=Proto`)

4. **Generated files**: FsYacc/FsLex outputs go to `$(IntermediateOutputPath)` - each project needs its own IntermediateOutputPath

5. **NuGet packaging**: Final FSharp.Compiler.Service.nupkg must aggregate all split assemblies or remain as single assembly (prefer single assembly output via ILMerge or similar if needed for compatibility)

## Incremental Build Benefit Estimate

Current monolithic build: Any change rebuilds entire compiler (~460 files)

With split:
- Change in SyntaxTree: Rebuilds SyntaxTree → TypedTree → Checking → Optimize → CodeGen → Service
- Change in AbstractIL: Rebuilds AbstractIL → TypedTree → ... (but SyntaxTree skipped!)
- Change in Service only: Rebuilds only Service

For a typical development iteration on Service layer, this could save 70%+ of compilation time.

## Implementation Notes

### Subtask 1: FSharp.Compiler.Utilities ✅ COMPLETED

**Completed**: Created `src/Compiler/split/FSharp.Compiler.Utilities.fsproj`

**Key finding**: `Utilities/TypeHashing.fs` cannot be in the Utilities project because it depends on higher layers:
- `FSharp.Compiler.AbstractIL.IL`
- `FSharp.Compiler.Syntax`  
- `FSharp.Compiler.TcGlobals`
- `FSharp.Compiler.TypedTree`
- `FSharp.Compiler.TypedTreeBasics`

This file should be placed in TypedTree or Checking layer instead.

### Subtask 2: FSharp.Compiler.AbstractIL ✅ COMPLETED

**Completed**: Created `src/Compiler/split/FSharp.Compiler.AbstractIL.fsproj`

**Key findings for cross-project compilation**:

1. **InternalsVisibleTo Required**: FSharp.Compiler.Utilities must have `InternalsVisibleTo` for FSharp.Compiler.AbstractIL because:
   - The FsYacc-generated `ilpars.fs` uses `Internal.Utilities.Text.Parsing.IParseState` and related internal types
   - The FsLex-generated `illex.fs` uses `Internal.Utilities.Text.Lexing` internal types

2. **Inline Functions in Internal Modules**: The F# compiler's monolithic design uses `module internal` extensively for utility modules with inline functions. When splitting into separate assemblies:
   - Inline expansion fails across assembly boundaries even with `InternalsVisibleTo` if the containing module is internal
   - **Solution**: Changed all `module internal` to `module` (public) in:
     - `NullnessShims.fs` - utility operators for nullness
     - `illib.fs/illib.fsi` - 18 internal modules including `PervasiveAutoOpens`, `Dictionary`, `List`, `Array`, etc.
   - This is a necessary architectural change for multi-assembly builds

3. **FsLex/FsYacc Integration**: The AbstractIL project correctly generates `illex.fs` and `ilpars.fs` using the buildtools targets.

### Subtask 3: FSharp.Compiler.SyntaxTree - BLOCKED

**Status**: Project file created but encountering multiple build system issues

**Key findings**:

1. **SyntaxTree DOES depend on AbstractIL** - ParseHelpers.fs uses `AsciiParser`/`AsciiLexer` from AbstractIL to parse inline IL syntax `(# ... #)`. This contradicts the original VISION assumption that SyntaxTree and AbstractIL are independent. The dependency graph is now LINEAR (no parallelism at SyntaxTree level):
   ```
   Utilities → AbstractIL → SyntaxTree → TypedTree → ...
   ```

2. **Dependency chain requires**: SyntaxTree → AbstractIL + SyntaxTree → Utilities (direct refs to both)

3. **FSComp.SR access issue**: The `FSComp.SR` module is auto-generated from FSComp.txt EmbeddedText with `type internal SR`. Even with `InternalsVisibleTo`, accessing from another assembly requires:
   - Direct project reference to Utilities
   - The `FSComp` namespace to be resolvable
   
4. **Type extension issue**: `String.StartsWithOrdinal` and similar extension methods defined in `PervasiveAutoOpens` (illib.fs) are not accessible from SyntaxTree even though:
   - `open Internal.Utilities.Library` is present
   - `InternalsVisibleTo` is configured
   - The extensions use `[<AutoOpen>]`

5. **MSBuild multi-TFM rebuild issue**: When SyntaxTree is built with both AbstractIL and Utilities as project references, AbstractIL fails to rebuild with `System.Reflection.Emit` type resolution errors. This happens ONLY during transitive builds, not when AbstractIL is built directly.

6. **Root cause hypothesis**: The diamond dependency pattern (SyntaxTree → AbstractIL → Utilities AND SyntaxTree → Utilities) combined with multi-TFM (netstandard2.0 + net10.0) creates package restoration race conditions or incorrect assembly resolution.

**Created files**:
- `src/Compiler/split/FSharp.Compiler.SyntaxTree.fsproj` - Project file with:
  - All SyntaxTree/*.fs files linked
  - FsLex/FsYacc targets for lex.fsl, pars.fsy, pplex.fsl, pppars.fsy
  - References to AbstractIL and Utilities
  - FsLex/FsYacc buildtools references

**Potential solutions to investigate**:
1. Build only net10.0 TFM initially to avoid netstandard2.0 System.Reflection.Emit complexity
2. Use `PrivateAssets="All"` on transitive dependencies
3. Investigate MSBuild project evaluation order for multi-TFM
4. Consider making FSComp module public or providing a public facade

### Progress Summary

| Subtask | Project | Status |
|---------|---------|--------|
| 1 | FSharp.Compiler.Utilities | ✅ Complete (commit 4d761bb) |
| 2 | FSharp.Compiler.AbstractIL | ✅ Complete |
| 3 | FSharp.Compiler.SyntaxTree | 🚫 Blocked (build system issues) |
| 4 | FSharp.Compiler.TypedTree | Pending |
| 5 | FSharp.Compiler.Checking | Pending |
| 6 | FSharp.Compiler.Optimize | Pending |
| 7 | FSharp.Compiler.CodeGen | Pending |
| 8 | FSharp.Compiler.Service (update) | Pending |
