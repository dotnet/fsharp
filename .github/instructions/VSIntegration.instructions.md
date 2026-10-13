---
applyTo: "vsintegration/**/*.{fs,fsi}"
---

# VS integration

This folder is Pack A of `@vsintegration-reviewer`. Invoke that agent for a review. Do not restate its CHECK list here.

- Keep VS, MEF, shell, and UI-thread types in this tree. Do not push them into `src/Compiler/Service` or `src/Compiler/Symbols`.
- Do not block the UI thread, and do not call a UI-thread service from a background callback.
- An editor feature must not let an exception reach the Visual Studio UI.
- For JTF, retained memory, and the perf execution map, invoke `@vs-integration-perf`. Do not copy it.
