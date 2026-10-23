---
name: pr-description
description: Use when drafting, proposing, creating, or editing prose for a dotnet/fsharp GitHub PR or issue — body, title, comment, review summary, edits — including bare asks like "open a PR", "ship this", "write up what I did", "summarise the change", "reply on the PR", "edit the issue body", "gh pr create", "gh pr comment", "gh pr edit --body", "gh issue comment", "gh pr review --body". Primary use case is PR descriptions; same rules apply to PR/issue comments and review summaries. Not for labels, reviewers, merging, or code-review findings (just the prose write-up of them).
---

# Authoring GitHub Prose for dotnet/fsharp

Reviewers can already see the Files tab, the commit log, and the issue thread. Say what behavior changed and why, in as few words as possible.

## Rules

Rules 1, 2, 4, 5, 6 are defaults; if the user insists, push back once then comply. Rule 3 is non-negotiable — `-b "..."` ships broken markdown (see PR #19866).

1. **No change inventory.** No file/module/method/test lists. No `## Changes`/`## Implementation` section. Mention an identifier only when it *is* the user-visible behavior. Whatever the reader already has (Files tab for PRs, commit log for follow-up comments, issue history for issue edits) — don't re-list it.
2. **No LLM slop, no justification scaffolding.** No emoji headers, no "TL;DR" above a 3-line body, no Motivation/Background/Approach/Testing sections, no re-stating the title or the comment you're replying to. No "matching the X norm", no "preventing the Y failure (PR #ZZZZ)", no stats, no links to past PRs as proof. The diff is the proof.
3. **Body via `--body-file`, built without shell expansion.** Write the file with your file-creation/edit tool (it writes bytes verbatim — no `$`/backtick evaluation, no delimiter collisions, OS-agnostic). Never `-b "..."` / `--body "..."` — backticks and `$` get shell-evaluated and the render breaks. If you build the file in a shell, use a pwsh verbatim here-string `@'...'@` (cross-platform; single-quoted is mandatory). Applies to `gh pr create/edit/comment/review`, `gh issue create/edit/comment`.
4. **`Fixes #N` to close issues.** Use only when the PR actually closes #N (auto-closes on merge). It is the highest-value line in most PR bodies — never omit it when valid. No "Related to" / speculative links. Preserve existing trailers (`Co-authored-by:`, `Signed-off-by:`, `Reverts #N`); don't invent them.
5. **Title:** imperative, ≤72 chars, no trailing period, no `fix:`/`feat:` prefix. Name the behavior, not the file. A specific title lets the body shrink to `Fixes #N` + one sentence.
6. **No hard-wrapped prose.** Write each paragraph as one unbroken line and let GitHub's renderer wrap it — blank lines separate paragraphs, and that's the only break you author. Manual mid-sentence line breaks (wrapping at a fixed column) are a machine tell and render raggedly across window widths.

## PR-body shapes (pick the smallest that carries the signal)

**One-liner** for bumps / mechanical fixes:
~~~
Update .NET SDK from 10.0.202 to 10.0.204.
~~~

**Title carries the construct + issue link** — preferred when the title is specific:
~~~
Fixes #18009

Wrong colorization when a qualified type name with generic parameters is used in a static member access expression.
~~~

**Issue link + 1-sentence why** — the most common non-trivial shape:
~~~
Fixes #19751

`--refout` MVIDs were unstable because hashing relied on per-process string randomization. Switched to a deterministic hash.
~~~

**Before/After code block** — when prose loses information; ≤15 lines, language tag:
~~~markdown
Fixes #15803

```fsharp
let f (x: int) = x <- 1
// before: FS0027 suggested `let mutable x = expression` (illegal for parameters)
// after:  FS0027 suggests `let mutable x = x` shadow or `byref<_>`
```
~~~

**Behavior-changes bullet list** — for genuinely multi-behavior PRs. 3–5 bullets naming *behaviors*, never files/modules; if bullets map 1:1 to files, collapse to prose or split the PR:
~~~
Fixes #19710
Fixes #19720

- `match x with null` now preserves type aliases.
- FS0027 on parameters suggests `let mutable x = x` shadow / `byref<_>`.
- `let _ = &expr` compiles like `let x = &expr`.
~~~

Prefer inline backticks for short identifiers over a fenced block. Fenced blocks are the exception — use only when prose loses information.

**Comments / review summaries:** same rules, no title. Usually 1–3 sentences. Quote-reply only the line you're responding to; don't restate it.

## Workflow

Show the title + body (or comment text) in chat first. **Do not run `gh` until the user approves.** Then:

1. **Write the body to a file** with your file-creation tool (bytes verbatim, OS-agnostic, avoids every shell-quoting trap). Or in pwsh:

   ```powershell
   @'
   Fix false-positive FS3261 when nullness narrowing leaks across iterations of seq/list/array comprehensions.

   Fixes #19644
   '@ | Set-Content -NoNewline pr-body.md
   ```

2. **Post** with exactly one:

   ```
   gh pr create  --title "<imperative title>" --body-file pr-body.md
   gh pr edit    <N> --body-file pr-body.md     # REPLACES body — read first if extending
   gh pr comment <N> --body-file pr-body.md
   gh pr review  <N> --body-file pr-body.md --comment   # or --approve / --request-changes
   gh issue create  --title "<t>" --body-file pr-body.md
   gh issue edit    <N> --body-file pr-body.md
   gh issue comment <N> --body-file pr-body.md
   ```

   To extend without losing the existing body:
   `gh pr view <N> --json body -q .body > pr-body.md` → edit → `gh pr edit <N> --body-file pr-body.md`.

3. **Verify** the live render — fetch the body and compare to the file with your view/diff tool. Fail loudly if `gh` fetch errors (a piped failure prints an "all-deleted" diff that misreads as "GitHub mangled it").
