# Find All References & Rename Symbol Architecture

This document describes the architecture of **Find All References** and **Rename Symbol** features in the F# compiler service. These features are closely related—Rename essentially performs Find All References followed by text replacement.

## Overview

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           Visual Studio / IDE Layer                         │
├─────────────────────────────────────────────────────────────────────────────┤
│  vsintegration/src/FSharp.Editor/                                           │
│  ├── Navigation/FindUsagesService.fs    ← Roslyn FindUsages adapter         │
│  ├── InlineRename/InlineRenameService.fs ← Roslyn InlineRename adapter      │
│  └── LanguageService/SymbolHelpers.fs    ← Core symbol lookup helpers       │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         FSharp.Compiler.Service Layer                        │
├─────────────────────────────────────────────────────────────────────────────┤
│  src/Compiler/Service/                                                       │
│  ├── service.fs / service.fsi          ← FSharpChecker API                  │
│  │   └── FindBackgroundReferencesInFile                                      │
│  ├── BackgroundCompiler.fs             ← Traditional incremental build       │
│  │   └── FindReferencesInFile                                                │
│  ├── TransparentCompiler.fs            ← New transparent compiler            │
│  │   └── FindReferencesInFile                                                │
│  │   └── ComputeItemKeyStore                                                 │
│  ├── IncrementalBuild.fs               ← BoundModel with ItemKeyStore        │
│  │   └── GetOrComputeItemKeyStoreIfEnabled                                   │
│  └── ItemKey.fs / ItemKey.fsi          ← Binary key store for symbols       │
│       └── ItemKeyStore + ItemKeyStoreBuilder                                 │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                              Compiler Core                                   │
├─────────────────────────────────────────────────────────────────────────────┤
│  src/Compiler/                                                               │
│  ├── Checking/NameResolution.fs        ← CapturedNameResolutions             │
│  ├── Service/ServiceNavigation.fs      ← Symbol navigation                   │
│  └── Symbols/Symbols.fs                ← FSharpSymbol types                  │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Key Components

### 1. ItemKeyStore (src/Compiler/Service/ItemKey.fs)

The **ItemKeyStore** is a binary format for storing and searching symbol references. It uses a memory-mapped file for efficient lookup.

**Key Types:**
- `ItemKeyStore` - Stores range+key pairs, provides `FindAll(Item) -> seq<range>`
- `ItemKeyStoreBuilder` - Writes Item+range pairs into the store
- `ItemKeyTags` - String tags to differentiate different item types (e.g., `#E#` for EntityRef, `u$` for UnionCase)

**Write Process:**
```fsharp
// Items are written with their range and a unique key string
member _.Write(m: range, item: Item) =
    writeRange m
    // ... write item-specific key based on item type
```

**Item Types Supported:**
- `Item.Value` - Values, properties, members
- `Item.UnionCase` - Union cases
- `Item.ActivePatternCase` - Active pattern cases
- `Item.ActivePatternResult` - Active pattern results
- `Item.RecdField` - Record fields
- `Item.ExnCase` - Exception cases
- `Item.Event` - Events
- `Item.Property` - Properties
- `Item.Trait` - Traits
- `Item.TypeVar` - Type variables
- `Item.Types` - Types
- `Item.MethodGroup` / `Item.CtorGroup` - Methods/Constructors
- `Item.ModuleOrNamespaces` - Modules/Namespaces
- `Item.DelegateCtor` - Delegate constructors
- `Item.OtherName` - Named arguments

**NOT fully supported (may cause missing references):**
- `Item.CustomOperation`
- `Item.CustomBuilder`
- `Item.ImplicitOp`
- `Item.SetterArg`
- Empty lists / multiple items (flattened elsewhere)

### 2. IncrementalBuild.fs - Traditional Path

The `BoundModel` class stores the `ItemKeyStore` after type-checking:

```fsharp
type TcInfoExtras = {
    itemKeyStore: ItemKeyStore option
    semanticClassificationKeyStore: SemanticClassificationKeyStore option
}
```

The store is built from `CapturedNameResolutions`:

```fsharp
let sResolutions = sink.GetResolutions()
let builder = ItemKeyStoreBuilder(tcGlobals)
sResolutions.CapturedNameResolutions
|> Seq.iter (fun cnr ->
    builder.Write(cnr.Range, cnr.Item))
```

### 3. TransparentCompiler.fs - New Path

The TransparentCompiler uses a similar approach but with different caching:

```fsharp
let ComputeItemKeyStore (fileName: string, projectSnapshot: ProjectSnapshot) =
    caches.ItemKeyStore.Get(
        projectSnapshot.FileKey fileName,
        async {
            let! sinkOpt = tryGetSink fileName projectSnapshot
            return sinkOpt |> Option.bind (fun sink ->
                let builder = ItemKeyStoreBuilder(tcGlobals)
                // ... build and return
            )
        })

member _.FindReferencesInFile(fileName, projectSnapshot, symbol, _) =
    async {
        match! ComputeItemKeyStore(fileName, projectSnapshot) with
        | None -> return Seq.empty
        | Some itemKeyStore -> return itemKeyStore.FindAll symbol.Item
    }
```

### 4. FSharp.Editor Layer (vsintegration)

#### FindUsagesService.fs

Implements `IFSharpFindUsagesService` for Roslyn integration:

```fsharp
let findReferencedSymbolsAsync (document, position, context, allReferences, userOp) =
    // 1. Get symbol at position
    // 2. Get check results
    // 3. Find declaration
    // 4. Call SymbolHelpers.findSymbolUses
```

#### InlineRenameService.fs

Implements `FSharpInlineRenameServiceImplementation`:

```fsharp
type InlineRenameInfo(...) =
    // Uses SymbolHelpers.getSymbolUsesInSolution
    let symbolUses = SymbolHelpers.getSymbolUsesInSolution(...)
    
    override _.FindRenameLocationsAsync(...) =
        // Convert symbol uses to rename locations
```

#### SymbolHelpers.fs

Core helpers for symbol operations:

```fsharp
// Find symbol uses within a single document
let getSymbolUsesOfSymbolAtLocationInDocument (document, position) = ...

// Find symbol uses across projects  
let getSymbolUsesInProjects (symbol, projects, onFound) = ...

// Main entry point for finding all uses
let findSymbolUses symbolUse currentDocument checkFileResults onFound = ...

// Get uses as dictionary by document
let getSymbolUsesInSolution (symbolUse, checkFileResults, document) = ...
```

## Data Flow

### Find All References Flow

```
1. User triggers "Find All References" on a symbol
         │
         ▼
2. FindUsagesService.findReferencedSymbolsAsync
   - TryFindFSharpLexerSymbolAsync (get lexer symbol at position)
   - GetFSharpParseAndCheckResultsAsync
   - GetSymbolUseAtLocation
         │
         ▼
3. SymbolHelpers.findSymbolUses
   - Determines scope (CurrentDocument, SignatureAndImplementation, Projects)
   - For project scope: getSymbolUsesInProjects
         │
         ▼
4. Project.FindFSharpReferencesAsync
   - Gets FSharpProjectSnapshot
   - Calls FSharpChecker.FindBackgroundReferencesInFile for each file
         │
         ▼
5. FSharpChecker.FindBackgroundReferencesInFile
   - Delegates to BackgroundCompiler or TransparentCompiler
         │
         ▼
6. BackgroundCompiler/TransparentCompiler.FindReferencesInFile
   - Gets/builds ItemKeyStore for the file
   - Calls itemKeyStore.FindAll(symbol.Item)
         │
         ▼
7. ItemKeyStore.FindAll
   - Builds key string for target symbol
   - Scans memory-mapped file for matching key strings
   - Returns matching ranges
```

### Rename Flow

```
1. User triggers rename on a symbol
         │
         ▼
2. InlineRenameService.GetRenameInfoAsync
   - Get symbol at position
   - Create InlineRenameInfo with symbol
         │
         ▼
3. InlineRenameInfo.FindRenameLocationsAsync
   - Uses SymbolHelpers.getSymbolUsesInSolution
   - Same flow as Find All References
         │
         ▼
4. InlineRenameLocationSet.GetReplacementsAsync
   - Validates new name (Tokenizer.isValidNameForSymbol)
   - Applies text changes to each location
   - Returns new solution
```

## Testing

### Test Files

- `tests/FSharp.Compiler.ComponentTests/FSharpChecker/FindReferences.fs` - Main test file
- `tests/FSharp.Test.Utilities/ProjectGeneration.fs` - Test workflow helpers

### Key Test Patterns

```fsharp
// Place cursor and find all references
project.Workflow {
    placeCursor "FileName" "symbolName"
    findAllReferences (expectToFind [
        "File.fs", line, startCol, endCol
        // ...
    ])
}

// Find references in a specific file
project.Workflow {
    placeCursor "First" line col fullLine ["symbolName"]
    findAllReferencesInFile "First" (fun ranges -> ...)
}

// Using singleFileChecker for simple cases
let fileName, options, checker = singleFileChecker source
let symbolUse = getSymbolUse fileName source "symbol" options checker |> Async.RunSynchronously
checker.FindBackgroundReferencesInFile(fileName, options, symbolUse.Symbol)
|> Async.RunSynchronously
|> expectToFind [...]
```

### Running Tests

```bash
# Run all FindReferences tests
dotnet test tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  -c Release --filter "FullyQualifiedName~FindReferences"

# Run tests with transparent compiler (if supported)
USE_TRANSPARENT_COMPILER=1 dotnet test ...
```

## Known Issues & Architecture Notes

### ItemKeyStore Limitations

1. **Active Patterns in Signature Files** (#14969)
   - Active patterns are written as single `Item.Value` with no case information
   - Signature files don't capture individual case information

2. **Key Collisions**
   - Different items might generate the same key string
   - Tags (e.g., `#E#`, `u$`) help but don't eliminate all collisions

3. **Memory-Mapped File**
   - Linear scan through file for matching keys
   - No indexing - O(n) lookup per file

### Symbol Scope Determination

`FSharpSymbolUse.GetSymbolScope` determines search scope:
- `CurrentDocument` - Only search current file
- `SignatureAndImplementation` - Search .fs and .fsi pair
- `Projects(projects, isFromDefinitionOnly)` - Search specific projects

### Transparent vs Background Compiler

- **TransparentCompiler**: Uses caches, async computation
- **BackgroundCompiler**: Uses GraphNode-based incremental build
- Both end up calling `ItemKeyStore.FindAll` for the actual search

## Adding New Symbol Types

To support Find All References for a new symbol type:

1. **ItemKey.fs** - Add a new tag in `ItemKeyTags` module
2. **ItemKeyStoreBuilder.Write** - Add case for the new `Item` variant
3. **Test** - Add test in `FindReferences.fs`

Example for a hypothetical new item:
```fsharp
// ItemKeyTags
[<Literal>]
let itemNewSymbol = "x$"

// ItemKeyStoreBuilder.Write
| Item.NewSymbol info ->
    writeString ItemKeyTags.itemNewSymbol
    writeEntityRef info.SomeRef
    writeString info.Name
```

## Related Files

| File | Description |
|------|-------------|
| `src/Compiler/Service/ItemKey.fs` | ItemKeyStore implementation |
| `src/Compiler/Service/ItemKey.fsi` | ItemKeyStore public API |
| `src/Compiler/Service/service.fs` | FSharpChecker API |
| `src/Compiler/Service/BackgroundCompiler.fs` | Traditional compiler backend |
| `src/Compiler/Service/TransparentCompiler.fs` | New compiler backend |
| `src/Compiler/Service/IncrementalBuild.fs` | Incremental build model |
| `vsintegration/src/FSharp.Editor/Navigation/FindUsagesService.fs` | VS integration |
| `vsintegration/src/FSharp.Editor/InlineRename/InlineRenameService.fs` | VS rename |
| `vsintegration/src/FSharp.Editor/LanguageService/SymbolHelpers.fs` | Symbol helpers |
| `tests/FSharp.Compiler.ComponentTests/FSharpChecker/FindReferences.fs` | Tests |
