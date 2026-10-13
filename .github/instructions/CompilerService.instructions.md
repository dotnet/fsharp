---
applyTo:
  - "src/Compiler/Service/**/*.{fs,fsi}"
  - "src/Compiler/Symbols/**/*.{fs,fsi}"
---

# IDE service

These folders are Pack B of `@vsintegration-reviewer`. Invoke that agent for a review. Do not restate its CHECK list here.

- No VS, MEF, JTF, or Roslyn types in these folders. Service must not reference `vsintegration`.
- Public signature, cache identity, and stale results are Pack B checks. Do not restate them here.
- `@expert-reviewer` still owns type checking, IL, pickle, and FSharp.Core. Do not add those rules here.
