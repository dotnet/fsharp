---
applyTo:
  - "src/Compiler/Service/**/*.{fs,fsi}"
  - "src/Compiler/Symbols/**/*.{fs,fsi}"
---

# Compiler service

Read `.github/skills/reviewing-vsintegration/compiler-service.md`. Invoke `@ide-integration`. Do not restate that file here.

- No VS, MEF, JTF, or Roslyn types in these folders. These folders must not reference `vsintegration`.
- `@expert-reviewer` still owns type checking, IL, pickle, and FSharp.Core. Do not add those rules here.
