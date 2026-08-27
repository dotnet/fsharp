---
applyTo:
  - "src/Compiler/SyntaxTree/SyntaxTree.{fs,fsi}"
  - "src/Compiler/SyntaxTree/SyntaxTreeOps.{fs,fsi}"
  - "src/Compiler/SyntaxTree/ParseHelpers.{fs,fsi}"
  - "src/Compiler/pars.fsy"
---

# The Untyped Syntax Tree

Read `docs/changing-the-ast.md`.

## The parse tree describes the source, not the semantics

`SynBinding`, `SynExpr`, `SynPat` and friends answer one question: **what did the user write, and where?** They are not a private staging area for the type checker — they are the public output of `FSharpParseFileResults`, and for formatters, analyzers, source generators and refactoring tooling they are the *only* view of the file.

The parser is the last stage that sees the source. Anything it discards or rewrites is gone: no downstream consumer can recover it.

So do not move, merge, synthesize or drop nodes in the parser to suit a downstream consumer, even when the relocation is semantically correct. Lower it in `BindingNormalization`, in the checker, or wherever the consumer actually reads — those stages can rewrite freely because the parse tree survives them intact.

Watch for the lossy variants specifically. Narrowing a range (an attribute's own span instead of the `[< >]` that encloses it) and flattening a grouping (splicing several `SynAttributeList`s into one) both destroy information that no later stage can reconstruct.

If a checker-side fix tempts you to edit `mkSynBinding` or a `pars.fsy` action, ask what the untyped tree now claims about source it can no longer describe.

## Changing tree shape requires baseline coverage

`tests/service/data/SyntaxTree` pretty-prints parse trees to `.bsl` files, so a shape change surfaces as a baseline diff a reviewer has to accept. That safety net only works for syntax the corpus actually contains — an uncovered case produces no diff, which reads exactly like a change that broke nothing.

When you change what the parser produces, add a `.fs`/`.bsl` pair for the syntax you touched before relying on a green run. Typed-tree tests (`AttributeCheckingTests.fs`, `Symbols.fs`, component tests) cannot substitute: they observe the tree *after* lowering, and will pass while the parse tree is wrong.

See `docs/postmortems/regression-parse-tree-fidelity-return-attributes.md` for what this cost when it was ignored.
