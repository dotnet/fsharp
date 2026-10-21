# Frozen corpus inventory and scope

These are inventory and scope gates for the existing performance-improvement reviewer, not
a new reviewer or a claim that the corpus has been inspected. The historical
[corpus](../corpus.md), [evidence](../evidence.md), [validation](../validation.md),
reviewer, integration skill and release-note entry are preserved. The corpus has
only an additive current-status link; its historical text is unchanged.

`discovery.json` seals the original 485 PR identities: Q01-Q29 contributed 1,205
occurrences and 475 unique PRs; saved linked expansion contributed ten more.
Queries, their complete memberships, actual authors (including Copilot results
from author:T-Gro searches), repository and exact historical bounds are frozen.
No current search is used. The original acquisition head and the continuation's
starting branch/head are recorded separately. SHA-256 identities cover the
source manifests, query receipts, historical holdout ledgers and preserved
repository files. Source `url` is null for machine-local manifests that have no
published URL; `cache_path` locates them relative to a runtime archive root.
Repository-file hashes normalize CRLF to LF and match the starting Git blobs;
archive hashes cover the original bytes without normalization.
The release-note hash preserves the original PR20619 entry under `### Added`,
not later notes for unrelated changes. Frozen-file checks reject a missing,
modified or duplicate entry. The entry must remain in the original section.
Query URLs describe historical requests, not immutable responses: the saved
response hashes are authoritative.

The validator retains the original canonical SHA-256 of discovery (sorted JSON
keys, ASCII escapes, compact separators, UTF-8). The sole additive field,
`scope_freeze`, is sealed separately and pins the scope projection, source
receipts, exposure and protocol. Original identities, memberships, historical
partition receipts and bounds remain byte-for-byte equivalent JSON values.
Editing discovery and the index together cannot silently substitute a
same-sized universe. The production CLI has no alternate-seal option and
requires the real 485-member manifest.
Small injected seals are available only to Python unit tests.

## Index contract (schema version 2)

`work-index.jsonl` has one object per PR, sorted by PR number. Required fields:

| Fields | Meaning |
| --- | --- |
| `schema_version`, `repository`, `pr`, `node_id`, `url`, `author` | Stable identity and actual captured author; must match discovery exactly. |
| `discovery_ids` | Exact original query memberships, or `linked`. |
| `ordinal`, `shard` | Sorted numeric position over **all** 485 PRs, zero-based `0..484`; `shard = ordinal % 6`. Six pairwise-disjoint shards have sizes `81,81,81,81,81,80` and union exactly to discovery. A connected family may span mining shards. |
| `scope` | Explicit `in`/`out`, PR-specific content-based reason and exact excerpt, immutable description/path/event sources, historical identity, policy version and reconciliation against the preserved old decision. No workload or author-identity exclusions. |
| `family`, `split` | Transitive connected-family identity and prospective development/control assignment. Historical aliases remain in the exposure ledger; all members share a split. None is certified as a fresh holdout. This does not grant training or inspection credit. |
| `source_status` | `pending`, `failed`, `stale`, or `acquired`. All start pending: receipt availability alone is not a new acquisition audit. |
| `inspection_status` | `pending`, `failed`, `stale`, or `inspected`. All 485 start pending. `acquired` is never an inspection status. |
| `receipt`, `rule_links` | Null and empty initially. A future inspection receipt is a cache-relative identifier, not arbitrary prose. Schema checks alone cannot substantiate it. |
| `historical` | Original decision, available development counts, reserve metadata, holdout counts/statuses and source references. None grants inspection credit. |

Source references use manifest IDs and `#pr=N` selectors for PR scope receipts (select
`number` or `pr` in the referenced array, not an array offset). All original
decisions are preserved, including exclusions and controls. Earlier focused
inspection/triage labels and channel counts remain historical. Raw discussions,
patches and large archives are not committed.

The collector still reconstructs the original schema-1 pending index and
one-based/groups-of-25 partition receipts; it never overwrites this schema-2
freeze. Both schemas are validated against their respective frozen provenance.

`inventory` validates the frozen set, uniqueness, repository, bounds, schema,
memberships and index structure. `scope` additionally validates all adjudications,
the modulo-six partition, immutable policies and connected-family exposure.
Both explicitly report `final_corpus_complete: false`. The stages `red`,
`acquisition`, `batch --batch N`, `training`, `rules`, `audit`, `sealed` and
`complete` fail explicitly as not implemented, after checking inventory. They
never fall back to an easier stage. Even fabricated status/receipt fields cannot
make `complete` succeed. Neither implemented gate is an acquisition,
inspection-completion or reviewer-quality certificate.

## Scope and exposure freeze

All **485** PRs have evidence-backed decisions: **141 in, 344 out**. The
freeze reconciles 35 changed decisions against the historical 122-in snapshot;
27 enter scope and eight become explicit out-of-scope/context rows. Complete
preserved description hashes, exact excerpts, labels, changed-path receipts and
older-created activity events were checked against the original caches.
The exposure boundary contains 277 connected original families and five
unverified supplemental benchmark-path leads, outside the unchanged inventory.
The additive correction receipt joins #20083 and #20084 as variants of the same
central-package-management change, retaining both historical aliases and all
access events. Prior freeze hashes remain recorded; scope decisions, partitions
and the predeclared protocol are unchanged.

[Protocol](evaluation/protocol.json) freezes the original eligibility rule,
positive source-specific human-composition provenance, two-PR/two-independent-
family recurrence, contrary-feedback accounting, required real-source strata,
claim-scoped scoring fields and acceptance before any new rule changes or runs.
All description-level authorship remains **uncertain**, not certified human;
identity, original/quoted text, author rationale/independent review and
missing/no-feedback are separate concepts.

[Exposure](evaluation/exposure.json) records all original PRs and connected
families, immutable prior report/run references, historical/final variants and
the 36 C6-S **replays**. It distinguishes acquisition, metadata screening,
semantic inspection, shared oracles, execution, scores, derivation and unknown
access. Earlier reserve triage counts as exposure even when inspection was
incomplete. Dates explicitly distinguish original run timestamps from audit
timestamps where the original access time is unknown. Renaming families cannot
erase their history.

No original family has a positive untouched-access certificate. Supplemental
benchmark-path leads remain separately labeled **validation-only, unverified,
not fresh**; the 804-PR metadata screen neither qualifies them nor changes the
485 denominator. Required real-source availability gaps are recorded by stratum
and **block evaluation acceptance**. In particular, the old search found no
real benchmark-only PR; synthetic BENCH-01 cannot fill that gap. No new review,
oracle or output bodies were opened for potentially untouched candidates, and
no reviewer rule or execution is part of this freeze.

## Commands

From the repository root, using only the Python standard library:

```powershell
python -m unittest discover -s docs\performance-improvement-review\mining\tools -p "test_*.py"
python -O -m unittest discover -s docs\performance-improvement-review\mining\tools -p "test_*.py"
python docs\performance-improvement-review\mining\tools\validate.py --stage inventory
python docs\performance-improvement-review\mining\tools\validate.py --stage scope
python docs\performance-improvement-review\mining\tools\validate.py --stage complete
```

The last command **must exit nonzero** while the corpus is unfinished.
`--discovery`, `--index`, `--protocol` and `--exposure` accept alternate file
locations, not alternate production seals. Validation does not need the original
machine's cache.

To reproduce the import, set `$archiveRoot` to the directory containing the
original session directories identified by `sources[*].cache_path`:

```powershell
python docs\performance-improvement-review\mining\tools\collect.py --source-root $archiveRoot --output <empty-directory>
```

The collector is offline and read-only toward source caches. It requires all
named manifests, reconciles saved search pages and decisions against the
original corpus table, and compares the result to the production seal. A
missing, changed or malformed cache fails explicitly; it never redownloads or
regenerates discovery. Identical output is reused. Different existing output is
never overwritten; use `--output <empty-directory>` to reconstruct an initial
index without discarding later work. LF/CRLF-only output differences are reused
without rewriting the existing file.

## Recovery checks

Tests were written and run before the implementation (missing-module failures).
The regression suite exercises exact identity substitutions as well as 484/486
rows, duplicate numbers/nodes, changed repository/cutoff, missing memberships,
fabricated completion, malformed JSON, optimized execution, historical-status
non-promotion and fail-closed stages. The original corpus table parser also
distinguishes delimiters from their later reproduction-code quotation.
Temporary archives exercise `recover()` and the collector entry point together:
paginated and overlapping queries, empty queries, linked PRs, historical
development/holdout receipts, source hashes and unchanged-cache/output reuse.
Parameterized failures cover missing, malformed, ambiguous and inconsistent
receipts. A byte-only source change must fail the frozen seal before creating
outputs or modifying existing ones. These tests need no workstation archive.

Initial recovery results: 16 tests passed normally and under `python -O`; inventory passed
with exactly 485 rows; `complete` exits 1 with 485 pending inspections. Offline
reproduction reuses identical outputs with zero network requests. Frozen-file
hashes match the starting Git blobs after removing only the exact current-status
addition in `corpus.md`. Scope mutation tests additionally reject undecided or
source-less rows, identity-only exclusion, mutable temporal evidence, shifted
ordinals, coordinated family renaming, unknown or semantic exposure presented as fresh,
copied human provenance and changes to the predeclared protocol.
Freshness tests first validate an isolated metadata-only holdout, then add
each of the six disqualifying exposure categories or invalidate one audit at a
time. A connected-variant regression also rejects separating #20083 and #20084
even with consistent aliases. Family summaries and test-only seals stay
consistent, so unrelated validation failures cannot satisfy these assertions.
Authorship cases share one parameterized setup and assert distinct diagnostics
for copied provenance and account identity, with consistent test-only seals.
