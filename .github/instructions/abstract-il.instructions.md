---
applyTo: 'src/Compiler/AbstractIL/**'
---

In-memory IL model and binary/PDB I/O: il.fs (model), ilread.fs (lazy metadata reader), ilwrite.fs (PE writer), ilwritepdb.fs (portable PDB emit).

- Enforce the `maximumMethodsPerDotNetType` cap before writing type defs; overflowing the method count must fail early instead of producing an invalid table layout.
- Any instruction-expanding morphism must remap `ILCode` labels and exception clauses together; changing instructions alone leaves branch targets and handler clauses stale.
- Treat `LazyOrderedMultiMap` as order-preserving infrastructure; add or filter by creating a new map rather than mutating the cached list.
- Keep `ReadResFile` two-pass resource parse aligned with the actual resource tree shape, including the `RT_DLGINCLUDE` skip, so node counts and payload reads stay in sync.
- Keep `getBinaryFile` `safeHolder` ownership intact so the mapped bytes stay alive until the finalizer disposes the underlying stream.
- Keep `SequencePoint.orderBySource`, `SequencePoint.orderByOffset`, and `scopeSorter` consistent with PDB traversal, because debug info is serialized in sorted source/offset order.
- Keep `splitILTypeNameWithPossibleStaticArguments` and `splitTypeNameRight` behavior stable, because IL type-name parsing feeds metadata lookup and provided-type mangling.
- When rewriting shadowed locals, sort child scopes with `scopeSorter` before splitting scopes so nested ranges stay valid and deterministic.
- Keep `PEFile.GetView` and the weak `BinaryFile` cache lifetime pattern, because `PEReader.Dispose` is owned by finalization and the cache avoids holding duplicate buffers.
