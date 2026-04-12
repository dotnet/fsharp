🤖 *Repo Assist here — I'm an automated AI assistant for this repository.*

## Activity for April 2026

## Suggested Actions for Maintainer

**Comprehensive list** of all pending actions requiring maintainer attention (excludes items already actioned):

* [ ] **Close issue** #13512: completion filtering for obsolete fields/events — fixed by merged PR #19506 (2026-04-12), but issue was not auto-closed — [View](https://github.com/dotnet/fsharp/issues/13512)
* [ ] **Verify and close** #11127: expression tree with anonymous records — confirmed fixed with FSharp.Core 11.0.100 (PR #19243), existing test coverage at FSharpQuotations.fs#L265-L303. T-Gro confirmed on April 1 — [View](https://github.com/dotnet/fsharp/issues/11127)
* [ ] **Merge PR** #19468: regression test for #13114 (SynPat.Record / QuoteExpr traversal) — all CI passes, all review threads resolved — [Review](https://github.com/dotnet/fsharp/pull/19468)
* [ ] **Review/Merge PR** #19471: Fix #12386 — SRTP trait call correct overload resolution in multi-submit FSI — CI passes, no reviews yet — [Review](https://github.com/dotnet/fsharp/pull/19471)
* [ ] **Close PR** #19472: regression test for #12796 — bug confirmed NOT fixed on Desktop .NET Framework; T-Gro flagged the test as wrong — [Review](https://github.com/dotnet/fsharp/pull/19472)
* [ ] **Check comment** on #16982: `AI-thinks-windows-only` removed — semantic highlighting with `delegate` keyword still broken, reproduced via FCS — [View](https://github.com/dotnet/fsharp/issues/16982)
* [ ] **Check comment** on #16034: confirmed still present — indexed property setter with named arg gives FS0193 — [View](https://github.com/dotnet/fsharp/issues/16034)
* [ ] **Check comment** on #15339: confirmed still present — `GenerateSignature()` drops attribute annotations — [View](https://github.com/dotnet/fsharp/issues/15339)
* [ ] **Check comment** on #16464: confirmed still present — FS0670 when chaining multiple `+` on `INumber<>` — [View](https://github.com/dotnet/fsharp/issues/16464)
* [ ] **Check comment** on #8353: root cause identified — optional bool attribute arg without `DefaultParameterValue` causes internal error (now assigned to T-Gro/Copilot) — [View](https://github.com/dotnet/fsharp/issues/8353)
* [ ] **Issue NOT fixed** #18841: `let _ = &s` still raises FS0421 on current `main` — [View](https://github.com/dotnet/fsharp/issues/18841)

## Additional observations for maintainer's attention

- Issue #16154: Regression PR Shepherd found the task CE / IQueryable `VerificationException` still occurs on Desktop .NET Framework 4.8 — the CoreCLR-only test was a false positive. PR #19530 was merged as a CoreCLR-only regression test.
- Issue #16982: Delegate semantic highlighting bug confirmed via FCS `GetSemanticClassification`. The entire delegate signature is classified as `Method` (from the synthetic `Invoke` member), and bare `int` keywords don't get `ValueType` classification.
- Issue #11127: Fix is in FSharp.Core (PR #19243), not the compiler. Will ship with next SDK. Existing test coverage at `FSharpQuotations.fs#L265-L303`.
- Issue #19445 (duplicate `.cctor` in method table for generic DU with `static member val`) — confirmed still present in F# 11.0 dev.
- Issues #8353 and #13512 are now assigned to T-Gro/Copilot — #13512 fix (PR #19506) has been merged.
- Issue #7114: Compiler allows boxing a byref-like type (`Span<int>.Empty.GetType()`), which compiles without error but crashes at runtime with `InvalidProgramException`. Confirmed still present.
- Issue #6110: Error message for mixing struct/reference anonymous records still says "One tuple type is a struct tuple, the other is a reference tuple" — should mention anonymous records instead.
- Issue #16292: Debug build with SRTP and mutable struct enumerator produces incorrect codegen — defensive copy causes infinite loop. Release build works correctly. Confirmed still present on F# 15.2.101.0 (bd2823e2d).

## Future Work for Repo Assist

- Task 1: Continue scanning pre-2024 bugs (c=16464, next batch starts at #16465+)
- Task 3: woc=18119 — all windows-only issues reassessed; monitoring for newly labeled issues
- Task 2: rtc=19456 — monitoring for newly labeled `AI-thinks-issue-fixed` issues

## Run History

### 2026-04-12 12:49 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24307099989)
- 📋 Task 1: Scanned issues #16083–#16464 (31 pre-2024 bugs above c=15994). Investigated #16292, #16254, #16362, #16432, #16455, #16099 — all confirmed still present. c updated to 16464
- 📋 Task 3: woc=18119 — no windows-only issues above cursor
- 📋 Task 2: rtc=19456 — no new `AI-thinks-issue-fixed` issues
- 📝 Updated monthly summary: #13512 fixed by merged PR #19506; removed merged PRs #19476/#19530 from suggested actions; added #16292 Debug codegen observation

### 2026-04-10 12:50 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24243684888)
- 📋 Task 1: c=0, investigated 7 pre-2024 bugs — all confirmed still present (#7451, #6110, #7114, #3998, #8137, #4601, #10282)
- 📋 Task 3: woc=18119 — no windows-only issues above cursor
- 📋 Task 2: rtc=19456 — no new `AI-thinks-issue-fixed` issues
- 📝 Updated monthly summary: PRs #19476 and #19530 merged; added observations on #7114 and #6110

### 2026-04-09 12:53 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24191087607)
- 📋 No pre-2024 bugs with new activity; no windows-only or fixed issues above cursors
- 📝 Updated monthly summary: PR #19476 CI now fully passing

### 2026-04-09 00:54 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24166616679)
- 📋 No candidates for Tasks 1/2/3
- 📝 Updated monthly summary: noted #19476 push failures, #8353/#13512 assigned to Copilot

### 2026-04-08 12:51 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24136181519)
- 📋 No candidates for Tasks 1/2/3
- 📝 Updated monthly summary: refreshed PR statuses

### 2026-04-01 12:49 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/23849335121)
- 🏷️ Labelled #11127 with `AI-thinks-issue-fixed`; commented with test coverage link
- 🏷️ Removed `AI-thinks-issue-fixed` from #12386; commented acknowledging false positive
- 🏷️ Removed `AI-thinks-windows-only` from #16982; commented with FCS repro
- 📋 Task 3: Confirmed #17280, #17307, #18064, #18119 correctly labeled windows-only; woc=18119
- ✅ Merged by maintainers: PRs #19467, #19528, #19529, #19531, #19533, #19534, #19535 + #19532

### 2026-04-01 07:02 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/23836210093)
- 📋 Task 3: Confirmed #16083, #16130, #16275, #16300, #16886 correctly labeled windows-only; woc=16886

### 2026-04-01 01:07 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/23826888959)
- 📝 Created April 2026 monthly activity summary (this issue)
