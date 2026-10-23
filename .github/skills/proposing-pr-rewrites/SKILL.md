---
name: proposing-pr-rewrites
description: "Use when reviewing a PR where duplicated mechanisms, misplaced responsibilities, or competing fixes suggest a substantial design alternative rather than local corrections. Not for isolated bugs, style preferences, or size alone."
---

# Proposing PR rewrites

## Why and when

A redesign can protect SDK/IDE users or reduce maintenance risk beyond a local fix. Rework costs contributors time. Establish the benefit first.

Duplication, one-off helpers, passing tests, and diff size invite investigation, not a verdict. Prefer the least disruptive change meeting product requirements. Without sufficient evidence, ask a question rather than demand a rewrite.

## Establish the alternative

1. Read the goal, discussion, and agreed constraints. Compare repository helpers and prior attempts by contracts and callers, not names.
2. Prototype privately in isolation within the agreed scope and budget. Use `code-compaction` for investigation and comparison. Test failures and counterexamples against the changed implementation. Measure performance claims and state validation limits.
3. Preserve required behavior, compatibility, diagnostics, and execution semantics while making the intended correction. Reject alternatives with unintended regressions. A smaller diff does not compensate for broken behavior.
4. Confirm cited maintainer decisions. Retain a larger implementation when a requirement needs it. A planned follow-up does not resolve a proven current defect.

Do not change the author's branch or publish the prototype unless requested.

## Offer the proposal

Give a respectful preface naming the benefit, then concrete edits: delete X, reuse Y, change Z's contract. Acknowledge improvements without comparing contributors. Invite missing constraints.

Separate required fixes for proven defects from optional maintainability suggestions. State failures plainly, not guesses as facts. Contributors retain implementation agency. Maintainers decide acceptance.

Distill the diff, do not paste it. Aim for 80-150 words with essential snippets and evidence. Never omit correctness constraints for brevity.

## Preview and publish

Use `pr-description` for safe posting. Show exact wording in chat and obtain approval. Refresh the diff and anchors. Reconfirm changed wording or relevance. Publish approved text unchanged, verify the live body, and return its URL. A staged draft is not posted.
