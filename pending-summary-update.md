🤖 *Repo Assist here — I'm an automated AI assistant for this repository.*

## Activity for April 2026

## Suggested Actions for Maintainer

**Comprehensive list** of all pending actions requiring maintainer attention (excludes items already actioned — merged PRs and closed issues removed):

* [ ] **Close PR** [#19472](https://github.com/dotnet/fsharp/pull/19472): regression test for [#12796](https://github.com/dotnet/fsharp/issues/12796) — bug confirmed NOT fixed on Desktop .NET Framework; T-Gro flagged the test as wrong — [Review](https://github.com/dotnet/fsharp/pull/19472)
* [ ] **Close issue** [#5418](https://github.com/dotnet/fsharp/issues/5418): wrong FS0020 warning range — confirmed fixed by PR #19504, existing test at [WarnExpressionTests.fs#L179-L193](https://github.com/dotnet/fsharp/blob/c3c01c991d17643700d343cee5c5a1e20c06ce03/tests/FSharp.Compiler.ComponentTests/ErrorMessages/WarnExpressionTests.fs#L179-L193) — [View](https://github.com/dotnet/fsharp/issues/5418)
* [ ] **Close issue** [#13512](https://github.com/dotnet/fsharp/issues/13512): obsolete completion filtering — confirmed fixed by PR #19506, comprehensive tests at [CompletionTests.fs#L727+](https://github.com/dotnet/fsharp/blob/c3c01c991d17643700d343cee5c5a1e20c06ce03/tests/FSharp.Compiler.Service.Tests/CompletionTests.fs#L727-L736) — [View](https://github.com/dotnet/fsharp/issues/13512)
* [ ] **Check comment** on [#16982](https://github.com/dotnet/fsharp/issues/16982): delegate keyword breaks semantic highlighting — reproduced via FCS, AI-thinks-windows-only removed — [View](https://github.com/dotnet/fsharp/issues/16982)
* [ ] **Check comment** on [#16034](https://github.com/dotnet/fsharp/issues/16034): indexed property setter with named arg gives FS0193 — confirmed still present — [View](https://github.com/dotnet/fsharp/issues/16034)
* [ ] **Check comment** on [#15339](https://github.com/dotnet/fsharp/issues/15339): GenerateSignature() drops CustomComparison and CustomEquality — confirmed still present — [View](https://github.com/dotnet/fsharp/issues/15339)
* [ ] **Check comment** on [#16464](https://github.com/dotnet/fsharp/issues/16464): FS0670 when chaining multiple + on INumber — confirmed still present — [View](https://github.com/dotnet/fsharp/issues/16464)
* [ ] **Check comment** on [#8353](https://github.com/dotnet/fsharp/issues/8353): optional bool attribute arg without DefaultParameterValue causes internal error — [View](https://github.com/dotnet/fsharp/issues/8353)
* [ ] **Check comment** on [#18841](https://github.com/dotnet/fsharp/issues/18841): let _ = &s still raises FS0421 — AI-thinks-issue-fixed removed — [View](https://github.com/dotnet/fsharp/issues/18841)

## Additional observations for maintainer's attention

* Issue [#16154](https://github.com/dotnet/fsharp/issues/16154): PR [#19530](https://github.com/dotnet/fsharp/pull/19530) was merged (Apr 10), but issue remains open — may still affect Desktop .NET Framework 4.8.
* Issue [#16982](https://github.com/dotnet/fsharp/issues/16982): Delegate semantic highlighting bug confirmed via FCS GetSemanticClassification — not windows-only.

## Future Work for Repo Assist

* Task 1: Cursor at 0 — re-scanning pre-2024 bugs using lr filter (only issues with new activity since last run)
* Task 3: woc=18119 — all windows-only issues reassessed; monitoring for newly labeled issues
* Task 2: rtc=14566 — #5418 and #13512 have existing test coverage (Outcome 1); monitoring for newly labeled issues

## Run History

### 2026-04-16 01:09 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24486521708)
- 📋 Task 1: No pre-2024 bugs with new activity since 2026-04-15; #7741 has human discussion, no re-engagement needed
- 📋 Task 3: woc=18119 — no windows-only issues above cursor
- 📋 Task 2: #5418 and #13512 already have Outcome 1 comments (existing test coverage identified Apr 15); no new AI-thinks-issue-fixed issues above cursor
- ✅ Verified #5418 fix: FS0020 warning range now correctly points to `123` only (line 4, col 5-8) on F# 11.0
- 📊 Updated monthly summary: added close suggestions for #5418 and #13512, removed 4 merged PRs (#19468, #19471, #19579-#19581) and 3 closed issues (#1255, #4473, #14566)

### 2026-04-15 01:08 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24430988167)
- 🔍 Scanned remaining pre-2024 bugs (#16454, #16455, #16464): all still reproduce, no actionable insight
- 📊 Cleaned up monthly summary: removed 7 merged PRs and 5 closed issues from suggested actions

### 2026-04-14 12:53 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24399903012)
- 💬 Commented on #4473: Confirmed fix — extern function parameters no longer flagged as unused with --warnon:1182
- 🏷️ Labelled #4473 with `AI-thinks-issue-fixed`
- 🔧 Created PR #19580: regression test for #4473 (extern function params unused warning)
- 💬 Commented on #14566: Confirmed fix — query CE variables no longer produce false positive FS1182 warnings
- 🏷️ Labelled #14566 with `AI-thinks-issue-fixed`
- 🔧 Created PR #19581: regression test for #14566 (query CE unused value warnings)
- 🔍 Scanned ~80 pre-2024 bugs (#1287-#16432): most confirmed still present

### 2026-04-14 01:08 UTC — [Run](https://github.com/dotnet/fsharp/actions/runs/24375315766)
- 💬 Commented on #1255: Confirmed code generation fix — identifiers with @ now produce correct values
- 🏷️ Labelled #1255 with `AI-thinks-issue-fixed`
- 🔧 Created PR #19579: regression test for #1255 (identifiers with @ codegen)
- 🔍 Investigated 11 pre-2024 bugs: #13849, #14508, #14492, #7741, #3448, #995, #527, #774, #1564, #1241, #462 — all confirmed still present

### 2026-04-13 12:53 UTC
- Updated monthly summary: removed completed items (PR #19476 merged, PR #19530 merged, #11127 closed, #10043 closed)

### 2026-04-09 12:53 UTC
- PR #19476 CI now fully passing after workflow approval — ready for merge

### 2026-04-09 00:54 UTC
- Updated with #19476 merge conflicts and #8353/#13512 assigned to Copilot

### 2026-04-08 12:51 UTC
- Refreshed PR statuses (#19530 confirmed not-fixed, #13114 all review threads resolved)

### 2026-04-01 12:56 UTC
- Investigated 6 pre-2024 bugs, reassessed windows-only issues, verified fixes

### 2026-03-31 00:52 UTC
- Created PR #19530 (regression test for #16154)

### 2026-03-30 12:55 UTC
- Investigated 4 pre-2024 bugs, reassessed 5 windows-only issues
