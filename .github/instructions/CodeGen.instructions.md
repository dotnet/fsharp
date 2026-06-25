---
applyTo:
  - "src/Compiler/CodeGen/**/*.{fs,fsi}"
---

Read `docs/representations.md`.

For `--realsig+` visibility / closure-placement bugs (MethodAccessException under realsig+, IL `private` vs `assembly`, where synthesized closures/state-machines/TLR-lifts nest via `eenv.cloc`), see the `realsig-codegen` skill.
