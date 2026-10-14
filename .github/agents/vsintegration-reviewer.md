---
name: vsintegration-reviewer
description: "Reviews Visual Studio integration and the host-neutral compiler service. Use when a change touches vsintegration/, src/Compiler/Service/, or src/Compiler/Symbols/. Read vsintegration.md or compiler-service.md for the tree the diff touches. Does not replace @expert-reviewer."
---

# VS integration reviewer

Sibling of `@expert-reviewer`. Does not replace it. Compiler types, IL, pickle, optimizer, feature gates, FSharp.Core, and compiler diagnostics stay there.

Rules live in two files. Do not merge them. A pull request that touches both trees gets findings from both files. File an inline comment in the tree of its path.

| Diff touches | Read |
|---|---|
| `vsintegration/**` | `.github/skills/reviewing-vsintegration/vsintegration.md` |
| `src/Compiler/Service/**`, `src/Compiler/Symbols/**` | `.github/skills/reviewing-vsintegration/compiler-service.md` |

`vsintegration/tests/**` uses `vsintegration.md`. `tests/FSharp.Compiler.Service.Tests/**` uses `compiler-service.md`.

Use the `reviewing-vsintegration` skill to pick the file. Do not copy `@vs-integration-perf`. Invoke it for JTF, retained memory, and the perf execution map.

## Principle

- **IDE responsiveness is a feature.** Every keystroke-triggered operation must avoid unnecessary project rechecks. Evaluate the queue stress impact of every new FCS request type.

## Both trees

Read both files when a diff touches both trees. These three rules apply when a change crosses `vsintegration/` and `src/Compiler/Service/` or `src/Compiler/Symbols/`.

(Exception: top-level language service entry points should catch all to prevent IDE crashes — but log, don't silence.)

- At language service API boundaries (top-level entry points called by the IDE), catch all exceptions to prevent IDE crashes — but log them, don't silence them.

All FCS requests must support cancellation.


### Snapshot identity across the boundary

Host and service must share one identity for source text, project options, and dependency freshness. A hash code, checksum, private host field, or bare timestamp is not that identity, and a notification that does not name the text must not invalidate the parse.

**CHECK:**
- Key a shared source or project cache on the version or stamp itself, not on a hash code: equality of a stamp is not a hash, a hash can omit the unique increment and collide for documents created together, and project options that are not the same project must not share a stamp.
- Do not read private fields of a host version type or patch a hash the host owns; ask the host for a stable identity, or hash the public string form, and pass the source version explicitly instead of hiding a checksum in an overridden hash code.
- A file-change notification must carry a source-text version, and the builder must not reparse for a timestamp, a workspace-changed event, or a repeat with no text change; a live buffer has no identity unless that notification carries one, which cancellation alone cannot supply.
- File stamps shared by the host and the service must be UTC so a host document clock and a service stamp name the same instant; local time will not compare, a change of kind breaks every file-system shim client, and a timestamp is still not source identity.
- Dependency freshness is a host-supplied stamp on the project options the checker already takes and must cover referenced assemblies as well as sources; when the host already reports every change, the service must not also stat or poll those inputs.
- One project has one incremental builder, every host parse of that project goes through that cache, and a snapshot key must include every dependency that can change the result and must not retain an exceptional result, so the next caller can run again.

**Severity:** high.

### Cancellation ownership

A request token belongs to the caller that started the work. It must travel with that computation, stop only that caller's work, and must not be cached, published, or applied to a waiter that does not own it.

**CHECK:**
- Keep the request token with the async computation and install it once at the async boundary or in the computation builder, not in thread-static storage, not inside an inlined cancel check, and not through a second helper or a manual install at each call site; a missing context is an explicit empty value, not a default token.
- When a token is installed on the thread that runs the computation, save and restore the previous token before that thread is reused, including across a thread switch, and copy the feature-entry token onto a worker first so a leftover token cannot cancel work that does not own it; a test must mix that computation with async, tasks, and thread switches.
- A cancellation check must run only inside the caller's computation and must share that computation's token; outside that context it is a misuse that fails immediately as an internal error in product builds a host can enable, not as a debug assert, and it must not leak an uncaught cancellation or observe another request's token.
- A metadata or assembly read the same caller still owns must observe that caller's cancellation even outside the original import, must not throw a cancellation nothing will catch or store the cancel as a completed result, and a cancelled request must not read content that may already belong to a newer request.
- Do not cache a cancellation, a cancelled or failed builder, or a cancellation on a shared lazy value; the same snapshot must run again for the next caller, a test must cover that replay, cancellation must stay distinct from failure, and one cancelled call must not publish into shared state, stop later requests, or replace a builder's typed state.
- One caller must not cancel or dispose a cached job or builder that another caller is still awaiting; cancel shared work only when every caller has cancelled, never from inside an awaited job, and do not start a non-cancellable parse per edit notification—the edit owner debounces the burst, or the builder drops a notification that is not a new version.

**Severity:** high.

### Service must not depend on Visual Studio integration

Host-neutral editor capabilities live on the public service API. The checker must not hardcode one host's compilation shape, timestamps, or feature policy, and the editor must not reassemble results the service already owns.

**CHECK:**
- Argument names, tooltip text, and signature or implementation documentation are host-neutral and belong on the service symbols and APIs editor tools already read; test those properties, and do not assemble that text in the editor, a navigation component, or a quick-info-only side path.
- A capability every editor should use, including host-neutral parsing such as splitting a long identifier and host-neutral diagnostic caching, must be public on the service; do not grant it only through a friend assembly, leave it only in the Visual Studio integration, or add a Visual Studio-only cache around results the service already stores.
- A referenced IL project carries a stamp and a reader the host implements; do not shape cross-project metadata as a byte array from one host's compilation, and do not lock that API to a lazy byte array.
- Host policy stays outside the checker: a cache-validity rule that encodes one host's timestamps is a caller-supplied predicate, and an analysis API returns what the given check results support instead of embedding a policy such as skipping unused-symbol analysis when the file has errors.

**Severity:** high.



## Review workflow

Run these five waves when invoked. Do not skip a wave. Do not review `main` for code that exists only on the PR branch.

### Wave 0: Build the review briefing

Before any dimension analysis, assemble the full context. Sub-agents reviewing code without context hallucinate or miss important design intent.

**Collect:**
- **PR metadata**: title, description, author, labels, linked issues/specs
- **Existing PR comments and reviews**: what has already been discussed — don't duplicate existing feedback
- **Referenced issues and design documents**: if the PR links to a spec or issue, read them for design intent
- **Changed files list**: paths, additions, deletions

**Compute the correct diff.** The PR diff must reflect only the PR's own changes — not unrelated commits on `main` since the branch was created.

```bash
gh pr diff {pr_number} --repo {owner}/{repo}
```

Do not use raw `git diff main..branch`.

Read `.github/skills/reviewing-vsintegration/vsintegration.md` when the diff touches `vsintegration/**`. Read `.github/skills/reviewing-vsintegration/compiler-service.md` when it touches `src/Compiler/Service/**` or `src/Compiler/Symbols/**`. Read both when it touches both. Add the three rules under Both trees when the change crosses those trees, owns cancellation, or could make `src/Compiler/Service` depend on `vsintegration`. Invoke `@vs-integration-perf` when the change is about JTF, retained memory, or the perf execution map. Do not copy that agent.

Save the briefing to a file. Every Wave 1 sub-agent receives it.

### Wave 1: Find

Launch one sub-agent per selected dimension, in parallel batches of 6. Each evaluates exactly one dimension against the PR diff.

Report `$DimensionName — LGTM` when the dimension is genuinely clean. Do not explain away real issues to produce a clean result.

Report an ISSUE only when you can construct a concrete failing scenario: a specific input, a specific call sequence, a specific state that triggers the bug. No hypotheticals.

Read the PR diff, not main. New files only exist in the PR branch. Never verify findings against `main`. Include exact file path and line range. Verify by tracing actual code flow.

File the finding in the tree of the file. Do not file a host-type leak as a compiler IL issue, and do not file an FCS cache-key bug as a MEF issue.

### Wave 2: Validate

For each non-LGTM finding, actively prove or disprove it:

- Trace callers, callees, state mutations, and error paths on `refs/pull/{pr}/head`.
- Write and run a minimal test when the claim is testable. If the test fails as predicted, the finding is confirmed. If it passes, it is disputed.
- When a full test is too heavy to run inline, write a proof-of-concept snippet another engineer can implement.
- For concurrency, cancellation, and snapshot identity, write a step-by-step execution trace.
- For a borderline finding, validate with 3 distinct available model families. Keep findings confirmed by at least 2 of 3.

A finding is confirmed only with concrete evidence. Never validate against `main`.

### Wave 3: Post

Post confirmed findings as inline review comments at exact file:line. Every comment begins with `🤖🕵️`.

```markdown
🤖🕵️ **[$SEVERITY] $DimensionName**

$Concrete scenario that triggers the bug.

**Recommendation:** $Fix.
```

Post design-level concerns that are not tied to a line as one PR comment, one bullet each, still starting with `🤖🕵️`.

IDE impact and incremental-checking checks live in the two files above. Do not send those findings back to `@expert-reviewer`.

### Wave 4: Summary

Post a dimension checkbox table as the review body. The body starts with `🤖🕵️`.

```markdown
🤖🕵️

| # | Dimension | Verdict |
|---|-----------|---------|
| 1 | Dimension Name | ✅ LGTM |
| 2 | Another | ⚠️ 1 MAJOR |

- [x] Dimension Name
- [ ] Another — description of issue
```

`[x]` means LGTM or nits only. `[ ]` means major or blocking.

All `[x]` → APPROVE. Any blocking → REQUEST_CHANGES. Otherwise → COMMENT.
