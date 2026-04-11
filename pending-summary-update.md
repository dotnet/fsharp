🤖 *Repo Assist here — I'm an automated AI assistant for this repository.*

## Activity for April 2026

## Suggested Actions for Maintainer

**Comprehensive list** of all pending actions requiring maintainer attention:

* [ ] **Merge PR** #19468: regression test for #13114 (SynPat.Record / QuoteExpr traversal) — all CI passes, all review threads resolved — [Review](https://github.com/dotnet/fsharp/pull/19468)
* [ ] **Review/Merge PR** #19471: Fix #12386 — SRTP trait call correct overload resolution in multi-submit FSI — CI passes, no reviews yet — [Review](https://github.com/dotnet/fsharp/pull/19471)
* [ ] **Close PR** #19472: regression test for #12796 — bug confirmed NOT fixed on Desktop .NET Framework; T-Gro flagged the test as wrong — [Review](https://github.com/dotnet/fsharp/pull/19472)
* [ ] **Verify and close** #11127: expression tree with anonymous records — confirmed fixed with FSharp.Core 11.0.100 (PR #19243), existing test coverage at FSharpQuotations.fs#L265-L303. T-Gro confirmed on April 1 — [View](https://github.com/dotnet/fsharp/issues/11127)
* [ ] **Check comment** on #16982: AI-thinks-windows-only removed — semantic highlighting with delegate keyword still broken, reproduced via FCS — [View](https://github.com/dotnet/fsharp/issues/16982)
* [ ] **Check comment** on #16034: confirmed still present — indexed property setter with named arg gives FS0193 — [View](https://github.com/dotnet/fsharp/issues/16034)
* [ ] **Check comment** on #15339: confirmed still present — GenerateSignature() drops certain attributes — [View](https://github.com/dotnet/fsharp/issues/15339)
* [ ] **Check comment** on #16464: confirmed still present — FS0670 when chaining multiple + on INumber<> — [View](https://github.com/dotnet/fsharp/issues/16464)
* [ ] **Check comment** on #8353: root cause identified — optional bool attribute arg without DefaultParameterValue causes internal error (assigned to T-Gro/Copilot) — [View](https://github.com/dotnet/fsharp/issues/8353)
* [ ] **Check comment** on #13512: events/fields ignore allowObsolete in ResolveCompletionsInType (assigned to T-Gro/Copilot) — [View](https://github.com/dotnet/fsharp/issues/13512)
* [ ] **Issue NOT fixed** #18841: let _ = &s still raises FS0421 on current main — [View](https://github.com/dotnet/fsharp/issues/18841)

## Additional observations for maintainer's attention

- Issue #16154: Regression test PR #19530 was merged but the issue was reopened — task CE / IQueryable VerificationException still occurs on Desktop .NET Framework 4.8. The CoreCLR fix works but a separate Desktop .NET Framework fix may be needed.
- Issue #16982: Delegate semantic highlighting bug confirmed via FCS GetSemanticClassification. The entire delegate signature is classified as Method (from the synthetic Invoke member), and bare int keywords don't get ValueType classification.
- Issue #11127: Fix is in FSharp.Core (PR #19243), not the compiler. Will ship with next SDK. Existing test coverage at FSharpQuotations.fs#L265-L303.
- Issues #8353 and #13512 are assigned to T-Gro/Copilot — Copilot Coding Agent may be working on fixes.

## Future Work for Repo Assist

- Task 1: Continue scanning pre-2024 bugs with new activity (c=0, uses lr filter — currently no candidates)
- Task 3: woc=18119 — all windows-only issues reassessed; monitoring for newly labeled issues
- Task 2: rtc=19456 — monitoring for newly labeled AI-thinks-issue-fixed issues

## Run History

### 2026-04-11 01:00 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24270928387)
- ✅ PR #19476 merged on 2026-04-10 (closing #10043); PR #19530 merged on 2026-04-10 (#16154 reopened)
- 📋 Task 1: No pre-2024 bugs with new activity since 2026-04-10 (lr filter); no candidates
- 📋 Task 3: woc=18119 — no windows-only issues above cursor
- 📋 Task 2: rtc=19456 — #11127 skipped (below cursor, human confirmed test coverage exists)
- 📝 Updated monthly summary: removed merged PRs #19476/#19530 from suggested actions

### 2026-04-09 12:53 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24191087607)
- 📋 Task 1: No pre-2024 bugs with new activity since 2026-04-09 (lr filter); no candidates
- 📋 Task 3: woc=18119 — no windows-only issues above cursor; all previously assessed
- 📋 Task 2: rtc=19456 — #11127 already handled (Outcome 1); no new AI-thinks-issue-fixed issues
- 📝 Updated monthly summary: PR #19476 CI now fully passing after workflow approval — ready for merge

### 2026-04-09 00:54 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24166616679)
- 📋 Task 1: No pre-2024 bugs with new activity since 2026-04-08 (lr filter); no candidates
- 📋 Task 3: woc=18119 — no windows-only issues above cursor; all previously assessed
- 📋 Task 2: rtc=19456 — #11127 already handled (Outcome 1); no new AI-thinks-issue-fixed issues
- 📝 Updated monthly summary: noted #19476 persistent merge conflict/push failures, #8353/#13512 assigned to Copilot

### 2026-04-08 12:51 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24136181519)
- 📋 Task 1: No pre-2024 bugs with new activity since 2026-04-01 (lr filter); no candidates
- 📋 Task 3: woc=18119 — no windows-only issues above cursor; all previously assessed
- 📋 Task 2: rtc=19456 — #11127 already handled (Outcome 1); no new AI-thinks-issue-fixed issues
- 📝 Updated monthly summary: refreshed PR statuses (#19530 confirmed not-fixed by PR Shepherd, #19468 ready to merge, #19472 CI failing)

### 2026-04-01 12:49 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/23849335121)
- 🏷️ Labelled #11127 with AI-thinks-issue-fixed (expression tree with anonymous records — confirmed fixed with local FSharp.Core 11.0.100)
- 💬 Commented on #11127: confirmed fix, linked existing test coverage at FSharpQuotations.fs#L265-L303
- ��️ Removed AI-thinks-issue-fixed from #12386 (false positive — humans confirmed still repros in multi-submit FSI)
- 💬 Commented on #12386: acknowledged false positive, noted fix PR #19471 in progress
- 🏷️ Removed AI-thinks-windows-only from #16982 (semantic highlighting testable via FCS)
- 💬 Commented on #16982: reproduced delegate classification bug via FCS, full evidence provided
- 📋 Task 3: Confirmed #17280, #17307, #18064, #18119 correctly labeled windows-only; woc updated to 18119
- ✅ Merged today by maintainers: PRs #19467, #19528, #19529, #19531, #19533, #19534, #19535 (7 regression test PRs + #19532 FS0452 fix)

### 2026-04-01 07:02 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/23836210093)
- 📋 Task 1: No pre-2024 bugs with new activity since last run
- 📋 Task 3: Confirmed #16083, #16130, #16275, #16300, #16886 correctly labeled AI-thinks-windows-only; woc updated to 16886
- 📋 Task 2: No new AI-thinks-issue-fixed issues above rtc=19456; no action

### 2026-04-01 01:07 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/23825361389)
- 📝 Created monthly activity issue #19537 for April 2026
- 📋 Scanned pre-2024 bugs, investigated 7 issues (#7451, #6110, #7114, #3998, #8137, #4601, #10282) — all still present
- 📋 Task 3: Assessed #14192-#15961 windows-only labels (all confirmed correct)
- 📋 Task 2: Processed AI-thinks-issue-fixed issues up to rtc=19456