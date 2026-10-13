---
applyTo:
  - "src/Compiler/Service/**/*.{fs,fsi}"
  - "src/Compiler/Symbols/**/*.{fs,fsi}"
---

# IDE service

These folders are Pack B of `@vsintegration-reviewer`. Invoke that agent for a review. Do not restate its CHECK list here.

- This API is host-neutral. No VS, MEF, JTF, or Roslyn types. Service must not depend on `vsintegration`.
- A public signature change is a break even when the private representation is unchanged.
- Cache identity is a semantic stamp of one project, not object identity.
- `@expert-reviewer` still owns type checking, IL, pickle, and FSharp.Core. Do not add those rules here.
