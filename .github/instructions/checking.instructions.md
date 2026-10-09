---
applyTo: 'src/Compiler/Checking/**'
---

Type inference and declaration checking: CheckExpressions, ConstraintSolver, NameResolution, CheckDeclarations.

- `NameResolver` must be constructed with the right `TcGlobals`, `ImportMap`, `InfoReader`, and instantiation generator because lookup caches and generic instantiation depend on them.
- Run `escapeDotnetFormatString` before parsing format specifiers; the lexer strips brace escapes, so the checker must re-double them to keep .NET formatting semantics.
- Keep `BindSubExprOfInput` and `GetSubExprOfInput` threading the generalized typars and instantiation together; pattern compilation must not lose the dummy type substitution needed for polymorphic matches.
- Keep `MutRecShapes` and the `MutRecDefnsPhase*` data flow intact; nested modules, tycons, and `Lets` must stay grouped in the declared recursive shape.
- Always remap signature attributes with `sigToImplRemap` before comparing or propagating them; signature/implementation identity is not stable without that rename layer.
- Only enable `TryFindRelevantImplicitConversion` when required and actual types are known precisely and no feasible subtype match exists; otherwise implicit conversions steal normal overloads.
- Keep `ConstraintSolver`'s resolution order deterministic; overload and SRTP solving rely on fixed sequencing and only limited backtracking.
- Preserve `CombineTwoLimits` scope/flag normalization, especially the stack-referring span-like case forcing scope 1; otherwise byref escaping checks miss method-level violations.
