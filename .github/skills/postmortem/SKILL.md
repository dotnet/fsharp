---
name: postmortem
description: Write a postmortem for a regression that escaped to production, broke real users, and traces back to a design flaw worth documenting for future implementors. Only invoke after confirming no existing postmortem or doc covers the same root cause.
---

# Postmortem Writing

## When to Invoke This Skill

All of the following must be true:

1. **Production escape.** The bug shipped in a released SDK or NuGet package. Internal-only or caught-in-CI issues do not qualify.
2. **User-visible breakage.** Real users hit the bug — build failures, runtime crashes, silent wrong behavior. Not a cosmetic or tooling-only issue.
3. **Non-obvious root cause.** The bug traces back to a design assumption, invariant violation, or interaction between independently-correct changes that is worth explaining to future contributors.
4. **Not already documented.** Check `docs/postmortems/` for existing write-ups covering the same root cause. Check `.github/instructions/` for rules that already encode the lesson. If covered, stop.

Do **not** write a postmortem for:
- Typos, simple off-by-one errors, or straightforward logic bugs.
- Bugs caught by CI before merge.
- Issues where the fix is obvious from the diff alone.

## What to Learn Before Writing

Before writing a single line, answer these questions:

1. **How did the bug reach users?** Trace the path: which PR introduced it, which release shipped it, why CI didn't catch it. Understanding the gap in coverage is often more valuable than the fix.
2. **What made it hard to diagnose?** Was the error message misleading? Did the symptom appear far from the cause? Did it only reproduce under specific configurations?
3. **What design assumption was violated?** Every qualifying postmortem has one. A format invariant, a compatibility contract, a threading assumption. Name it precisely.
4. **What would have prevented it?** A test? A code review checklist item? A compiler warning? An agentic instruction? This becomes the actionable outcome.

## Postmortem Structure

Write the file in `docs/postmortems/` with a descriptive filename (e.g., `regression-fs0229-bstream-misalignment.md`).

Use this outline:

### Summary
Two to three sentences. What broke, who was affected, what the root cause was.

### Error Manifestation
What did users actually see? Include the exact error message or observable behavior. Someone searching for this error should find this doc.

### Root Cause
Explain the design assumption that was violated. Keep it high-level enough that someone unfamiliar with the specific code can follow. Use short code snippets only if they clarify the mechanism — not to show the full diff.

### Why It Escaped
How did this get past code review, CI, and testing? Be specific: "The test suite only exercised single-TFM builds" is useful. "Testing was insufficient" is not.

### Fix
Brief description of what changed and why it restores the invariant. Link to the PR.

### Timeline
Table of relevant PRs/dates showing how the bug was introduced, exposed, and fixed. Include latent periods where the bug existed but was masked.

### Prevention
What has been or should be added to prevent recurrence: tests, agentic instructions, CI changes, code review checklists. Link to the specific artifacts (e.g., the `.github/instructions/` file that encodes the lesson).

## After Writing

1. **Identify the trigger paths.** Determine which source files, when changed, would risk repeating this class of bug. Be specific — e.g., `src/Compiler/TypedTree/TypedTreePickle.{fs,fsi}`, not "the compiler". These are the files where a future contributor needs to see the lesson.

2. **Create or update an instruction file.** Check `.github/instructions/` for an existing instruction file whose `applyTo` covers those paths. If one exists, add a reference to your postmortem. If none exists, create one with an `applyTo` scoped to exactly those paths:

   ```yaml
   ---
   applyTo:
     - "src/Compiler/Path/To/File.{fs,fsi}"
   ---
   ```

   The instruction file should encode the **generalized rule** (not the incident details). Link the postmortem as a "see also" for deeper context. The postmortem explains *why the rule exists*; the instruction file tells agents *what to do* when editing those files.

3. **Do not create instructions without path scoping.** A postmortem lesson that applies "everywhere" is too vague to be actionable. If you can't name the files where the lesson matters, the postmortem may not meet the threshold for this skill.

4. **Update `docs/postmortems/README.md`** if it maintains an index.
