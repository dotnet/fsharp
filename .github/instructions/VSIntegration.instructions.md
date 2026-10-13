---
applyTo: "vsintegration/**/*.{fs,fsi}"
---

# VS integration

This folder is Pack A of `@vsintegration-reviewer`. Invoke that agent for a review. Do not restate its CHECK list here.

- Host types stay in this tree. Service and Symbols stay host-neutral.
- UI-thread calls and editor exceptions are Pack A checks. Do not restate them here.
- For JTF, retained memory, and the perf execution map, invoke `@vs-integration-perf`. Do not copy it.
