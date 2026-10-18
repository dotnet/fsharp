# Compiler service

Paths: `src/Compiler/Service/**`, `src/Compiler/Symbols/**`. Tests: `tests/FSharp.Compiler.Service.Tests/**`.

Host-neutral API for IDEs: checker, incremental builder, snapshots, workspace, navigation, completion, symbols, and the diagnostics surface. This is not the rest of `FSharp.Compiler.Service.fsproj`. No Visual Studio, MEF, JTF, or Roslyn types.

- Do not fork `DelayedILModuleReader`. Cross-project IL reads use `FSharpReferencedProject.ILModuleReference`, or `PEReference` with `DelayedILModuleReader`. The Visual Studio host uses `PEReference`.
- Use `ProjectSnapshotBase.ParsingVersion`, `SignatureKey`, or `FullKey` according to the cached computation’s inputs rather than assigning every cache the whole project closure.
- Process-wide ambient state is a leak. Readers, file-system shims, diagnostic loggers, console encoding, cancellation tokens, and generated-name state travel with the request or are restored when that owner finishes.
- One syntactic form, one flag value, or the happy path is not the feature. Sibling constructs, the other checking flag, and the error path get the same rule and an assertion of the observable result.
- A green suite is not coverage. The test calls the public service API, asserts names, ranges, or text, fails when the fix is absent, and does not share process-wide state with other tests.
- Do not do work the caller did not ask for, and do not return before publishing a result the caller will observe. An unchanged stamp stays cold, an optional capture stays off, and a memoized get stores before it returns.
- Unfinished and ill-typed code stays resolvable. The checker does not throw out to the host, and it does not drop later names, uses, highlighting, or sequence-point ranges because one construct failed.
- Treat changes to existing exported `SynExpr`, `SynPat`, or service-member signatures as compatibility breaks, verified by `SurfaceAreaTest.VerifySurfaceAreaFSharpCompilerService`, even if source patterns compile.

### FCS API surface

A public FCS signature change must update `FSharp.Compiler.Service.SurfaceArea.netstandard20.bsl`, including when the API is marked experimental.

- Internal compiler types must not appear on the public FCS API.
- The FCS Symbol API must be safe for concurrent callers.
- An async implementation change must not change the public FCS signature.
- A new public argument and a changed exception need an XML doc in the `.fsi`.
- `CheckFileInProject` wins over `CheckFileInProjectAllowingStaleCachedResults` unless the caller uses the stale API.
- Project setup must not drop references for a clean solution or an unrestored package.
- Error loggers must be the same object across checking phases.

All FCS requests must support cancellation.

Service entry points: (Exception: top-level language service entry points should catch all to prevent IDE crashes — but log, don't silence.)

- Do not require `FSharpChecker` to catch all exceptions; host catch-and-log stays at the Visual Studio entry point.

### Key caches on a semantic stamp

A lookup returns another project's result, an older script closure, or a result whose types were dropped.

**CHECK:**
- Build `ProjectSnapshotBase.FullKey` from frozen project options, reference versions, and source-file versions rather than retaining mutable options as the cache identity.
- Keep `ProjectId` distinct across target frameworks and retain the project/output-file pair in `ProjectConfig.Identifier` when selecting or clearing per-project cache entries.
- For `ProjectSnapshotBase.SignatureKey`, include signature-file versions but omit paired implementation versions while retaining script-closure and referenced-project dependencies.
- an imported assembly is shared only when the key covers the file, the whole reference closure, and the configuration that changes import, and a shared project signature is the reader's view, not the producer's live compilation.
- Verify that `BackgroundCompiler.ParseFile` uses the supplied `ISourceText` unchanged, and apply type-retention checks to successful `FSharpCheckFileResults`, not untyped `FSharpParseFileResults`.
- a caller that must not read or write the parse cache has an entry point that does neither, and a parse-file cache still distinguishes files, at least by file name.
- a full invalidation, including solution close, clears the module-reader cache, a host-supplied reader updates the write stamp when observable bytes change, and compiler-generated name state is per project.

**Severity:** critical.

**Hotspots:** `src/Compiler/Service/FSharpProjectSnapshot.fs`, `src/Compiler/Service/FSharpWorkspace*.fs`, `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`

### Refresh only what the stamp changed

An edit reparses unchanged files, or a diagnostics request misses files the builder skipped and returns errors from a different parse.

**CHECK:**
- an edit reparses only the file the stamp says changed, and a stamp update names those files so they can be reparsed without waiting for the whole graph.
- a request for project diagnostics type-checks implementation files the incremental builder skipped, and diagnostics are collected when the check is finalized, not only from files visited so far.
- a full type-check of an implementation that has a signature does not block the partial path; start that check so it can run alongside the graph.
- `FSharpChecker.ParseAndCheckProject` must return parse errors from the same parse the check used.
- `useTransparentCompiler` selects `BackgroundCompiler` or `TransparentCompiler`; do not add a third builder for the same setting.
- optional capture stays off unless the caller sets the flag, the compact type-check summary is computed when the full check finishes, a timestamp read does not take the builder lock, and a cache hit is a distinct event from building a new cache.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`, `src/Compiler/Service/FSharpProjectSnapshot.fs`, `tests/FSharp.Compiler.Service.Tests/*`

### Lock one key and publish before return

A wide lock, a queued caller-thread sequence, or an unsynchronized cache deadlocks, races, or lets the caller observe a miss for work that already finished.

**CHECK:**
- a builder lock covers one cache key, not the whole builder, and a cache count taken while another thread mutates the cache uses that same lock.
- a lock that only starts a parse stays synchronous and is not held for the parse; do not replace a caller-thread build-graph sequence with a thread-pool queue when that deadlocks the import lock.
- Evaluate `SyntaxTree.ParseNode` through `GraphNode.GetOrComputeValue` and share `BackgroundCompiler.parseFileInFlight` entries under `parseCacheLock` instead of requiring eager parsing at construction.
- a memoizing cache publishes its result before get returns so a following lookup observes that completion; a recent-results lookup may still miss a write that is queued, so a test waits for the completion event rather than assuming the write is visible.
- a cache of shared framework imports and a script-resolution cache tolerate concurrent updates, and temporary builder state uses the same guard as the published state.
- a finally that restores thread-static context does not assume it runs on the thread that saved that context.
- a mutually recursive check that never suspends stays a synchronous loop with an explicit cancel check; do not convert it to async only to share a cancellation type, do not start a thread per recursion chunk, and a deep concatenate is a tail call or covered by the stack guard.

**Severity:** critical.

**Hotspots:** `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`

### Keep later names after an error

A type error or exception drops later resolution, or escapes the transparent compiler into the host.

**CHECK:**
- a check of ordinary source does not throw out of the transparent compiler, and an exception while checking one file is contained in the incremental builder so the host process stays up.
- Type-error recovery in `CheckDeclarations` and `CheckPatterns` stays with `@expert-reviewer`.
- a type error in one pattern does not stop checking the rest of the match, and later clauses must not look unmatched.
- a type error in a base-constructor argument does not skip let bindings and do expressions in the implicit constructor, and a type error on an assignment target does not skip name resolution on the right-hand side.
- Use `RecoverableException` at recovery points that ignore the exception so `OperationCanceledException` is not swallowed.

**Severity:** critical.

**Hotspots:** `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`, `tests/FSharp.Compiler.Service.Tests/TypeChecker/**`

### Keep diagnostics in the request that emitted them

A logger, phase, or severity leaks into the next caller, or a missing range is reported as a real project file.

**CHECK:**
- awaiting work inside the build graph keeps the compilation diagnostics scope, including restoring the logger and phase, on both the cached path and any operation that never enters that cache.
- `CompilationGlobalsScope` must restore the previous logger and phase, and callers must not rely on `AssertFalseDiagnosticsLogger` throwing because its sink currently discards diagnostics.
- ambient diagnostic state travels with the request; a thread-local copy is not equivalent until the same requests agree under both stores.
- any service operation that applies command-line options, including find-usages, runs inside a diagnostics scope, and diagnostics emitted while creating a project builder honor the compiler's severity options.
- a diagnostic with no range uses a sentinel that cannot name a project file, including whatever file is at index zero, and callers can tell that sentinel from a real range.
- Clamp parser EOF diagnostics to the current source bounds in `FSharpDocumentDiagnosticAnalyzer.GetDiagnostics` instead of treating every overrun as a snapshot mismatch.
- a diagnostic for a combined construct covers the keyword that introduces the error, a documentation comment that no declaration consumes is reported, and a diagnostic type name spells the concept.

**Severity:** high.

Use `src/Compiler/Service/{ServiceCompilerDiagnostics,service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants,BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey,QuickParse,ServiceParseTreeWalk,ServiceParsedInputOps,ServiceXmlDocParser,SynExpr,SynPat}.*`, `src/Compiler/Symbols/{FSharpDiagnostic,XmlDocInheritance,XmlDocSigParser}.*`, and `src/Compiler/Facilities/DiagnosticResolutionHints.*` as diagnostic hotspots.

### Pass readers and restore process state

A process-wide reader, shim, or console encoding leaks into the next solution or drops writers the host installed.

**CHECK:**
- project reads and script reads are not distinguished by mutating the process-wide assembly reader; pass the reader with the referenced project, and accept a custom module reader rather than only a metadata stream.
- Host-supplied `ILModuleReader` values in `FSharpReferencedProject` must support concurrent reads and remain logically immutable until their `getStamp` value changes.
- a file-system shim installed for a solution is restored when that solution closes, and a default shim keeps its members overridable so a client replaces only the operations it needs.
- a file-system source read used by the lexer hands back characters directly, and a metadata reader that holds a binary view weakly does not capture that view in a closure.
- a module reader keeps nested namespaces and type definitions lazy until a lookup needs them.
- a compilation flag that changes console encoding restores the encoding object taken from the console, without rebuilding it from a code page and without replacing the host's output streams.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`, `src/Compiler/Service/FSharpWorkspace*.fs`, `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/ServiceAssemblyContent.*`, `src/Compiler/Service/ExternalSymbol.*`, `src/Compiler/Service/ServiceLexing.*`

### Treat a public signature change as a break

A public symbol, diagnostic, or tooltip signature changes and is treated as cleanup, or a test-only member is published.

**CHECK:**
- a public signature change is the break, even when the private representation is unchanged; restoring compatibility means restoring the signature callers construct.
- A public break is a diff in `FSharp.Compiler.Service.SurfaceArea.netstandard20.bsl`; do not accept or reject it for being easier.
- A new public type must ship in `FSharp.Compiler.CodeAnalysis` or `FSharp.Compiler.Symbols`, and an experimental member must be marked experimental.
- a helper that exists only so tests can observe a cache is not a public member, and reader implementations are not published on the public IL signature; a host that already has assembly symbols supplies them through the same extension point as cross-project references.
- a diagnostic range uses the same range type as other service APIs, a symbol query that needs only a count or a yes-or-no answer does not force callers to build the full child list, and a symbol predicate tools already depend on is a member named like the existing predicates.
- a new symbol-description API returns the same text as declaration lists and tooltips, a display choice such as unqualified type names is an explicit option on the display context, and a new completion option defaults to the items callers already see.
- For public `SynExpr` or `SynPat` shape changes, verify source-pattern compatibility and `SurfaceAreaTest`’s exported constructor signatures separately.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`, `src/Compiler/Symbols/{Symbols,SymbolHelpers,SymbolPatterns,Exprs}.*`, `src/Compiler/Symbols/FSharpDiagnostic.*`, `src/Compiler/Service/{ServiceDeclarationLists,ServiceParamInfoLocations,ServiceInterfaceStubGenerator}.*`, `src/Compiler/Service/ServiceNavigation.*`

### Report the symbol the user wrote

Tooling sees a compiler-generated symbol as user-written, misses the property or method at that range, or looks up a symbol whose identity was dropped.

**CHECK:**
- a compiler-generated symbol or attribute is marked synthetic and is not reported over the range of the symbol the user wrote; a type that analysis only saw, and that is not the type of a written expression, stays retrievable without being treated as completable. A generated name shown to tooling or the debugger is a legal identifier, does not collide with a name already in scope, and identifies a repeated construct by where it appears rather than by an ordinal alone.
- a symbol listing does not add another symbol on a range that already names a different member, unless the added symbol is an intended accessor of that member, and a range replace does not drop a constructor when both the type and the constructor belong there.
- Check `FSharpMemberOrFunctionOrValue.IsProperty` and `FSharpField.DeclaringUnionCase` for accessor and named-union-field results without requiring the unpublished `FSharpParameter.Owner` API.
- a symbol stored for later lookup keeps the case kind that lookup uses, including exception identity, and a symbol-use record used for rename carries the same value information already stored for other values, including signature parameters.
- symbol uses include a top-level namespace declaration, a unification failure still reports the method symbol, and a file signature exposed to tools includes members the signature file hides.
- a fake value inserted only so a symbol can be built does not enter the name environment used for completion, a namespace that comes from both an assembly and a type provider has one stable symbol tag, and a derived public path compares only the mangled path and the name.
- an accessor query does not throw when the member name is only the accessor prefix, and a base-constructor call does not prefer a nested environment that hides the primary constructor's parameters.

**Severity:** high.

**Hotspots:** `src/Compiler/Symbols/{Symbols,SymbolHelpers,SymbolPatterns,Exprs}.*`, `src/Compiler/Service/ExternalSymbol.*`, `src/Compiler/Service/ServiceNavigation.*`, `src/Compiler/Service/{ServiceDeclarationLists,ServiceParamInfoLocations,ServiceInterfaceStubGenerator}.*`, `tests/FSharp.Compiler.Service.Tests/GotoDefinition/**`

### Cover the construct the user wrote

A navigation, collapse, or breakpoint range is empty or stops at the name, so the host lands off the declaration or disagrees with the compiler's sequence points.

**CHECK:**
- Check the documented extents of `NavigationItem.Range`, `NavigationItem.BodyRange`, and `FSharpSymbol.DeclarationLocation` separately instead of applying one keyword-inclusive range rule to all three.
- a collapse range for a constructor call is non-empty, including a call split across lines, and a folding range may be one range for a large union rather than a range for each small case.
- a pattern range published to tools includes its delimiters and trivia, constructor parameter ranges include the parentheses, and an attribute-list range includes the closing delimiter.
- Preserve operator spelling and parentheses in `IdentTrivia.OriginalNotationWithParen` while keeping the underlying identifier range separate from the enclosing expression range.
- an optional or synthetic measure carries a range that distinguishes it from a measure that was parsed, and an optional token range is absent when recovery did not parse that token.
- Keep `SyntaxTraversal.pick`’s existing whitespace/end-of-input fallback instead of imposing strict range containment on every traversal.
- Sequence-point emission in `IlxGen` stays with `@expert-reviewer`.

**Severity:** high.

Use `src/Compiler/Service/{ServiceNavigation,ServiceStructure,FSharpParseFileResults,ServiceParsedInputOps,QuickParse,ServiceParseTreeWalk,ServiceXmlDocParser,SynExpr,SynPat}.*` and `src/Compiler/Symbols/{Symbols,SymbolHelpers,SymbolPatterns,Exprs}.*` as range-check hotspots.

### Complete from the tree, and keep legal items

Completion hides a legal item, returns the wrong scope, or follows a source scan after the tree already preserves the node.

**CHECK:**
- when the syntax tree preserves a completion context, the tree is the owner; a source scan may ship only while the tree lacks the node, and those tests stay as the regression net.
- a completion filter does not remove an item that is legal to write at that position; rank it lower, and a host that wants it hidden opts in through a completion option rather than changing the shared list.
- a pattern filter that drops values still offers a literal, a class-only filter does not drop static members of value types and simple types, and static members drawn from every referenced assembly are optional.
- completion in an empty record offers that record's fields and, at the first field, names from the enclosing scope; a position just after a type declaration returns the enclosing scope, and an empty prefix inside a record field type still returns items.
- an unfinished expression that ends in a dot is not checked as the prefix alone, and an unfinished type name still produces a symbol and a diagnostic that describes the real problem rather than reporting the type as undefined when it resolved.
- completion filters its items once, and classifying a same-named group uses the display name already on the symbol instead of building a compiled name for every item.
- a feature that already resolves the symbol at the range may use a completion-oriented environment, and tests of that position mark the caret in the source; coordinates that only matched a previous environment are not a regression.

**Severity:** high.

Use `src/Compiler/Service/{ServiceDeclarationLists,ServiceParamInfoLocations,ServiceInterfaceStubGenerator,FSharpParseFileResults,ServiceParsedInputOps,QuickParse,ServiceParseTreeWalk,ServiceXmlDocParser,SynExpr,SynPat,ServiceAssemblyContent}.*` and `tests/FSharp.Compiler.Service.Tests/{Completion,ParameterInfo}/**` as completion hotspots.

### Apply the walk to every sibling form

A walk or classification fixes one syntactic form and leaves the sibling wrong, or reads text and allocates before the irrelevant symbols are dropped.

**CHECK:**
- a walk applies the same visitor rule to syntactic siblings, including anonymous records beside records, computed bindings beside ordinary bindings, and an active pattern used as an expression beside an operator used as an expression.
- a walk that visits types also visits the type in a type test, a record resolved as an expression is reported as a symbol use, and a record-update walk still exposes the keyword that separates the copied expression from the fields.
- a parsed node kept only for inspection stays on its own field, and an existing member walk does not treat that field as members unless the walk is explicitly extended.
- a lookup of captured items at one position skips every group whose range does not contain that position, and a query does not classify every captured type in the file or build a full list before a search that could stop.
- Apply the early local-value and generic-parameter exclusions in `UnusedOpens.filterSymbolUses` before reading source lines, not to rename or general symbol-use queries.
- Deduplicate primary classifications in `TcResolutions.GetSemanticClassification` while preserving extra classifications explicitly requested by `relatedSymbolKinds`.
- a closed set of metadata or attribute values is decoded once and shared, a one-element name-resolution item is stored directly, an empty symbol result reuses one empty array, and a fresh collection callers only read by length is an array rather than a list wrapped as read-only.

**Severity:** high.

Use `src/Compiler/Service/{FSharpParseFileResults,ServiceParsedInputOps,QuickParse,ServiceParseTreeWalk,ServiceXmlDocParser,SynExpr,SynPat,ServiceAnalysis,SemanticClassification*,ServiceAssemblyContent}.*` and `src/Compiler/Symbols/{Symbols,SymbolHelpers,SymbolPatterns,Exprs}.*` as walk/classification hotspots.

### Keep the parsed form for tools

The tree tooling walks has been desugared, has dropped a legal form, or records a separator that was not written.

**CHECK:**
- a syntax tree published to tools keeps the form that was parsed; desugaring for later compilation is not the only stored form, and parentheses and error nodes are not dropped for each tool to reconstruct differently.
- an identifier in the tree tooling walks keeps the original source text; replace it with the compiled representation only in a later phase.
- a legal syntactic form stays marked in the tree even when it is non-idiomatic, because tooling cannot analyze a form the tree drops.
- `SynExprSequentialTrivia.SeparatorRange` must match `sequentialExpr`’s recorded separator, including THEN/OTHEN ranges and `None` when the grammar records no separator.
- Use `FSharpToken.Range` for token extents and keep every `SyntaxTreeTests.ParseFile` baseline case addressable by its fixture filename.

**Severity:** medium.

Use `src/Compiler/Service/{FSharpParseFileResults,ServiceParsedInputOps,QuickParse,ServiceParseTreeWalk,ServiceXmlDocParser,SynExpr,SynPat,ServiceLexing}.*` as parsed-form hotspots.

### Assert the observable service result

A green suite never calls the public API, never sets the other flag, or asserts a count that stays green when the wrong names are reported.

**CHECK:**
- a service behavior change has a test that calls the public checker, tooltip, or symbol API and asserts the observable result; a host or editor test alone does not cover other hosts.
- the assertion fails when the fix is absent, and it asserts names, ranges, or text, not a count that stays green when the wrong items are reported.
- both values of a checking flag other hosts set are tested, a new cache or mode is on for the scenarios the existing suite already runs, and a partial-checking change fails when implementations are fully checked or skipped wrongly.
- a new syntactic form, directive, attribute order, or diagnostic-range fix is asserted beside the existing sibling form, including a documentation comment preceded by attributes and a multi-line comment rather than only a single line, and the assertion that locks the original syntax is not replaced.
- completion and caret tests drive the caret through the existing fixture and mark it in the source; do not hand-supply a position the context already computes, and do not add a branch the range check already excludes.
- a cache test counts hits or waits until the cache reports no in-flight update; a green result, a fixed sleep, or an immediate read of a still-queued update does not show the cache behavior.
- an FSI evaluation test reads the streams it claims to capture, a parser test passes source in memory, a baseline update deletes generated files the new run no longer produces, and a public wrapper needs a new harness only when it changes the observable text.

**Severity:** high.

**Hotspots:** `tests/FSharp.Compiler.Service.Tests/**`, `tests/FSharp.Compiler.Service.Tests/Completion/**`, `tests/FSharp.Compiler.Service.Tests/ParameterInfo/**`, `tests/FSharp.Compiler.Service.Tests/Tooltip/**`, `tests/FSharp.Compiler.Service.Tests/GotoDefinition/**`, `tests/FSharp.Compiler.Service.Tests/ErrorList/**`, `tests/FSharp.Compiler.Service.Tests/TypeChecker/**`

### Isolate process-wide test state

One test cancels, redirects, or mutates process-wide state, and the failure is reported as a product bug.

**CHECK:**
- a test must not cancel the process-wide default cancellation token, because that token is shared with other tests.
- a test that mutates process-wide service state, the console, the process environment, or a shared project is serialized or kept out of the parallel run, and the exclusion mark sits on the test class, not on a helper.
- a test must not fire-and-forget work, synchronize with a fixed delay, or initialize a shared checker from a static constructor that parallel workers can race.
- a test that captures console output does not redirect the process console, and a test that listens to a process-wide cache by name does not run beside other listeners of that name.
- a session that writes artifacts uses its own temporary directory; a test that does not write files can stay in its source directory, and a tokenizer fixture does not share a mutable buffer across cases.
- a test-collection definition lives in the assembly that uses it, and a test-runner change keeps the trait or filter that selects a batch; a green run does not mean a skipped batch passed.
- Distinguish test-owned state from the service state protected by `IncrementalBuilder.updateCurrentState` before classifying a together-only failure as isolation.

**Severity:** high.

**Hotspots:** `tests/FSharp.Compiler.Service.Tests/**`

