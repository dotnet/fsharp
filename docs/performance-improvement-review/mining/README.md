# Frozen corpus inventory

This is an inventory gate for the existing performance-improvement reviewer, not
a new reviewer or a claim that the corpus has been inspected. The historical
[corpus](../corpus.md), [evidence](../evidence.md), [validation](../validation.md),
reviewer, integration skill and release-note entry are preserved unchanged.

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
Query URLs describe historical requests, not immutable responses: the saved
response hashes are authoritative.

The validator pins a canonical SHA-256 of the entire discovery object (sorted
JSON keys, ASCII escapes, compact separators, UTF-8). Editing discovery and the
index together cannot silently substitute a same-sized universe. The production
CLI has no alternate-seal option and requires the real 485-member manifest.
Small injected seals are available only to Python unit tests.

## Index contract (schema version 1)

`work-index.jsonl` has one object per PR, sorted by PR number. Required fields:

| Fields | Meaning |
| --- | --- |
| `schema_version`, `repository`, `pr`, `node_id`, `url`, `author` | Stable identity and actual captured author; must match discovery exactly. |
| `discovery_ids` | Exact original query memberships, or `linked`. |
| `ordinal`, `shard` | Frozen one-based position and groups of 25 in numeric PR order; these are work partitions, not independent evidence units. |
| `scope` | `decision` (`pending`, `included`, `excluded`), nonempty `reason`, and `source_ids` into discovery sources. All imported decisions remain pending; reasons are historical, not newly adjudicated. |
| `family`, `split` | Provisional historical family and development/control/holdout assignment, not a new scope decision or inspection result. |
| `source_status` | `pending`, `failed`, `stale`, or `acquired`. All start pending: receipt availability alone is not a new acquisition audit. |
| `inspection_status` | `pending`, `failed`, `stale`, or `inspected`. All 485 start pending. `acquired` is never an inspection status. |
| `receipt`, `rule_links` | Null and empty initially. A future inspection receipt is a cache-relative identifier, not arbitrary prose. Schema checks alone cannot substantiate it. |
| `historical` | Original decision, available development counts, reserve metadata, holdout counts/statuses and source references. None grants inspection credit. |

Source references use manifest IDs and optional `#pr=N` selectors (select
`number` or `pr` in the referenced array, not an array offset). All original
decisions are preserved, including exclusions and controls. Earlier focused
inspection/triage labels and channel counts remain historical. Raw discussions,
patches and large archives are not committed.

Only `inventory` is implemented. It validates the frozen set, uniqueness,
repository, bounds, schema, memberships and index structure. Success explicitly
reports `final_corpus_complete: false`. The recognized stages `scope`, `red`,
`acquisition`, `batch --batch N`, `training`, `rules`, `audit`, `sealed` and
`complete` fail explicitly as not implemented, after checking inventory. They
never fall back to an easier stage. Even fabricated status/receipt fields cannot
make `complete` succeed. Do not use an inventory pass as a scope, acquisition,
inspection or reviewer-quality certificate.

## Commands

From the repository root, using only the Python standard library:

```powershell
python -m unittest discover -s docs\performance-improvement-review\mining\tools -p "test_*.py"
python -O -m unittest discover -s docs\performance-improvement-review\mining\tools -p "test_*.py"
python docs\performance-improvement-review\mining\tools\validate.py --stage inventory
python docs\performance-improvement-review\mining\tools\validate.py --stage complete
```

The last command **must exit nonzero** while the corpus is unfinished.
`--discovery` and `--index` accept alternate file locations, not alternate
production identities. Validation does not need the original machine's cache.

To reproduce the import, set `$archiveRoot` to the directory containing the
original session directories identified by `sources[*].cache_path`:

```powershell
python docs\performance-improvement-review\mining\tools\collect.py --source-root $archiveRoot
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

Recovery results: 13 tests pass normally and under `python -O`; inventory passes
with exactly 485 rows; `complete` exits 1 with 485 pending inspections. Offline
reproduction reuses identical outputs with zero network requests. Frozen-file
hashes match the starting Git blobs, and `git diff --check` passes.
Compiler builds, benchmarks and
whole-repository formatting do not apply to this Python/data/documentation
change.
