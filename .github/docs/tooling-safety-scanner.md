# Tooling safety scanner

The hourly workflow runs ordinary code before it starts the classifier.
Only new or changed, eligible fork PRs enter the classifier context.
PRs created before May 12, 2026, and draft PRs remain excluded.
Same-repository PRs use the deterministic bypass path.

The input identity includes the full head SHA, title, body, repository, base target, and merge base.
Comments, labels, and `updated_at` do not affect this identity.
A base-tip change with the same merge base does not start another classification.
Policy edits apply to subsequent changed inputs, not an automatic backlog audit.

The classifier receives snapshots, not GitHub access or the scan database.
The publisher reads the original selector artifact by its immutable artifact ID.
It validates result identities and categories after threat detection succeeds.

## State and recovery

`state.json` on `safety/scanned-PRs` belongs to the deterministic scripts.
Records remain stored when PRs close, become drafts, or disappear from a listing.
Missing or invalid state stops the workflow instead of creating an empty scan history.

The first run migrates legacy records without a backlog scan.
Matching legacy heads and existing PRs without history receive a marked baseline.
A baseline does not certify a scan under the current policy or apply a clean label.
Known legacy heads that changed remain eligible for classification.

Each selected input records its run before the agent starts.
Completed classifications are saved before labels or comments are changed.
Publication retries use those saved classifications.

If publication or a state write fails, rerun the failed jobs in the original run.
If classification results are missing or invalid, rerun all jobs in that run.
Subsequent schedules do not automatically repeat unfinished classifications.

A comment POST can succeed even when its response is lost.
The next run looks for its exact workflow marker, including minimized comments.
If the outcome remains uncertain, inspect the recorded run and reconcile the posting record.
Do not clear scan history or blindly repeat the POST.

## Development

Run `node --test .github/scripts/pr-tooling-safety.test.cjs`.
After editing the workflow, run `gh aw compile labelops-pr-security-scan`.
Commit the generated lock file with its Markdown source.
