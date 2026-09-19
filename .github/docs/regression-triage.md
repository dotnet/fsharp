# Reported regression triage

`regression-triage.md` is an independent agentic workflow. It adds the existing
`Regression` label to open `Needs-Triage` reports supporting previously working
behavior becoming broken. This means **reported regression awaiting reproduction**,
not independently verified behavior or a bisected cause. It neither requires
`Bug` nor removes `Needs-Triage` or any human label.

## Operation and safety

Issue opened/edited/reopened/labeled/transferred and human comment
created/edited/deleted events supply issue-number hints, never authoritative
state. Unknown contributors are readable (`roles: all`, `min-integrity: none`).
PR payloads, bot event senders/comments and labels other than `Needs-Triage` are
filtered before model activation. Scheduled collection still reads real reports
opened by bots and their human discussion.

Hourly reconciliation covers automation-generated labels that do not trigger
another workflow, missed events and replacement of pending concurrency runs.
For example, an opened event can precede `add_to_project.yml` applying
`Needs-Triage`; the next sweeps discover it without a label event. There is no
creation-date cutoff, search-cap enumeration or issue-number cursor.
All triggers share `regression-triage` concurrency with cancellation disabled.

The collector uses read-only credentials and immutable `github.workflow_sha`
helper code. Only the default-branch workflow/ref is accepted, including manual
dispatch. The two dispatch inputs are an optional positive safe issue number and
a boolean `staged` (default true), not arbitrary instructions. The compiler also
injects internal `aw_context`; this adapter accepts it only when empty.

The model reads a separate input artifact containing exact current human and
directly linked issue/PR text, source identities, fingerprints and a small prior
record excerpt. It cannot execute samples, access attachments, edit files, use
shell/CLI/web-fetch tools, or write GitHub data. Its GitHub MCP surface is limited
to issue/PR reads. Untrusted text cannot authorize extra tools or operations.
GH AW v0.76.1 still emits an edit grant for `edit: false`; a trusted
`pre-agent-steps` launcher prepends Copilot's overriding write/shell/URL
denials, excludes delegation and disables custom instructions. The generated harness invokes that
launcher after the checksum-verified CLI installation; the shared model stays
unchanged. The detector has the same restrictions: its verdict goes to stdout,
which the trusted framework records, so it needs no model-side file writes.
One custom safe output accepts a strict bounded proposal batch; receipt is not
publication. The batch is at most 64000 UTF-8 bytes, not 64 KiB: this stays
within v0.76.1 HTTP's 64000-character threshold for replacing large strings with
file-reference text. It must cover every selected report and cite each report itself,
not just a linked comparison. Missing-data/tool, no-op and failure-as-issue routes
are disabled.

Pinned ingestion adds an `errors` array; any nonempty or malformed array rejects
the whole publication, even if it also contains a valid proposal. The trusted
`output-validation.json` config uses the runtime's `GH_AW_VALIDATION_CONFIG_PATH`
to preserve the single JSON string field verbatim (`sanitize: false`).
Markdown sanitization would change generic types, mentions, XML, URLs and even
JSON syntax. These are evidence bytes, not public comment text: the publisher
still enforces all schema/size/source bounds and emits only fixed labels/questions.
Threat detection receives the unchanged proposals and remains mandatory.

The publisher runs only after successful agent/threat detection. Its read/write
token is confined to that trusted job. It resolves the immutable collector artifact
by this repository/run/attempt/policy/code-SHA prefix, rejects missing or ambiguous
artifacts, checks service run metadata, downloads by ID, and verifies the content
hash in its name. The model cannot upload/delete/replace this artifact. No
agent-workspace file supplies publication authority or memory deltas.

## Bounds, memory and recovery

Production limits are five classifications, ten issue-list page requests and ten
snapshot reads per run. Each snapshot bounds comment/timeline reads to ten requests
each, five direct links and five requests per PR review endpoint. Discussion
enumerations must agree across two passes; moving pages or incomplete dependencies
are not complete evidence. Input is limited to 48 KiB per selected entry and
192 KiB per batch; oversized entries remain pending, never silently truncated.
Content eligibility is checked before fair selection, so oversized reports cannot
consume classification slots. Batch-capacity rejections are refilled from the
remaining already-read snapshots without expanding the read budget.
Direct dependencies include local `#N`, qualified `owner/repo#N`, and HTTP(S)
GitHub issue/PR URLs (including `www.github.com`), deduplicated case-insensitively.
They are read through the API; linked-only corrections change the fingerprint.
Repository renames and linked transfers use the canonical API identity for source
IDs, discussion reads and local references. Aliases deduplicate only when their
human evidence agrees; a self-alias still requires the final target recheck.
Publication remains restricted to the original issue number in `dotnet/fsharp`.
An out-of-repository root is rejected before discussion reads and selection, not
admitted to a batch that would block other reports. It remains visibly incomplete
and pending, preserving history while valid reports and discovery progress are saved.
The manifest is bounded to 4 MiB. The collection step has a ten-minute deadline;
the agent and publication have fifteen-minute deadlines. Trusted Node watchdogs
enforce collection/publication deadlines because v0.76.1 discards custom step
timeouts. An interrupted publisher leaves its persisted intent for recovery.

An update-time scan with fifteen-minute overlap and an independent labeled-backlog
sweep retain page boundaries and continuations. Snapshot reads reserve
least-recently attempted work; analysis separately reserves the longest-waiting
pending work, using its last selection time or initial discovery time.
Only admission to the model batch resets that wait, not a snapshot read or a
content-bound rejection. Unresolved publication and omitted proposals rotate
without dropping their pending work. Remaining slots favor event hints and recent
input. The staged suite drains eleven stable reports in three
runs at production limits and drains a backlog despite continuously changing
high-priority reports or unresolved historical records.

Authoritative memory is schema 1 `state.json` on **`memory/regression-triage`**:
scan continuations, pending queue, fingerprints, policy, cited evidence, missing
facts, actual results, human decisions, clarification receipts and pending intents.
It is not agent-writable repo-memory or an Actions cache.
`createCommitOnBranch(expectedHeadOid)` persists whole discovery deltas together
with work/intents using CAS. It is **not** a transaction across GitHub API calls.
See the [helper contract](../scripts/regression-triage/README.md) for state details.

Even empty selections submit `results: []` so the publisher can preserve discovery.
Missing output or failed agent/detection leaves old memory for retry. An independent
trusted completion job fails active runs if any required stage, including
publication/staging, did not succeed; absent output cannot silently skip the
publisher and report success. Incomplete
scans/reads are summarized; the publisher retains pending work and fails the job
visibly after saving legitimate progress. A CAS conflict requires recollection,
not replay over a newer head. Failed memory reads never become empty memory.

Immediately before each mutation the publisher rechecks open/Needs-Triage state,
complete human/linked fingerprints and human label decisions. Corrections invalidate
stale proposals; human rejection/removal is not silently reversed. Policy-version
changes reanalyze without resetting human decisions or clarification receipts.
When changing classification semantics, bump `POLICY_VERSION` and the workflow's
example/artifact-name version together, freeze expectations and rerun evaluation.
At most one fixed-template question is asked; unresolved/missing historical
receipts suppress potentially duplicate questions. Never delete memory to retry.
Stable API issue identity preserves clarification history when an issue transfers
out and back under a new number, even if its receipt was deleted. Older records
without that identity migrate only through an authenticated matching live receipt.
Durable rejecting corrections require human author provenance, not merely a
matching quote from a bot-authored report.

`staged: true` **or** `GH_AW_SAFE_OUTPUTS_STAGED=true` suppresses all issue writes,
branch creation and remote memory commits; model fields cannot disable either.
The custom job exposes the latter through the repository variable of the same name.
The job summary shows would-label/comment/save receipts. Staging does not advance
remote memory; local integration explicitly reloads the returned simulated state.

No supported bisector workflow exists in this inventory. A separate bisector
can discover `Regression` through reconciliation, not solely bot-generated label
events. This workflow does not dispatch anything, especially not the regression
PR shepherd. Repo Assist, its schedules and `memory/repo-assist` remain unchanged.

## Local validation (no live issues or dispatch)

From the feature worktree in PowerShell:

```powershell
node --test .github\scripts\regression-triage\core.test.cjs
node --test (Get-ChildItem .github\scripts\regression-triage -Recurse -Filter *.test.cjs).FullName
node --check .github\scripts\regression-triage\core.cjs
node --check .github\scripts\regression-triage\github.cjs
node --check .github\scripts\regression-triage\publish.cjs
node --check .github\scripts\regression-triage\workflow.cjs
node --check .github\scripts\regression-triage\evaluate.cjs
git --no-pager diff --check
```

Also exercise the actual pinned MCP and ingestion code, not a sanitizer mock.
Set `$private` to an existing directory outside the repository, then:

```powershell
curl.exe --fail --silent --show-error --location https://api.github.com/repos/github/gh-aw/tarball/v0.76.1 --output "$private\gh-aw-source.tar.gz"
New-Item -ItemType Directory -Force "$private\gh-aw-source" | Out-Null
tar -xf "$private\gh-aw-source.tar.gz" -C "$private\gh-aw-source" --strip-components=1
$env:GH_AW_RUNTIME = "$private\gh-aw-source\actions\setup\js"
node --test .github\scripts\regression-triage\framework.test.cjs
node --test (Get-ChildItem .github\scripts\regression-triage -Recurse -Filter *.test.cjs).FullName
```

Without `GH_AW_RUNTIME` only this external-runtime test is skipped; that is not
a passing framework handoff gate. It uses the compiled tool configuration,
official tool generation, HTTP server/transport and ingestion, then the real
staged publisher. Five-result batches exercise the exact byte boundary with
ASCII, Unicode and escaped text, rejecting oversized output without writes.

Run the deterministic suite before semantic evaluation. Set `$private` to an
existing directory outside the repository and `$copilot` to a supported Copilot
CLI executable or JS entry point:

```powershell
node .github\scripts\regression-triage\evaluate.cjs run $private $copilot
node .github\scripts\regression-triage\evaluate.cjs check $private
```

This uses the marked policy in the actual workflow, shared configured model
(`gpt-5.6-sol` by default), hidden frozen expectations and zero model tools.
Private artifacts record raw proposals, exact model and policy/corpus hashes.
All cases must match classifications, citation provenance, missing facts and
allowed effects. Validated actual proposals traverse collector, strict validator,
staged publisher and restart/deduplication; stale closure/correction and policy
retry are exercised too. Expected-output replay in unit tests is **not** semantic
evaluation. Model unavailability is a blocked gate, not a passing evaluation.

Use only the repository-pinned GH AW **v0.76.1**, not a newer installed extension.
Download an isolated official executable and verify the published checksum:

```powershell
$tools = Join-Path $private 'gh-aw-v0.76.1'
New-Item -ItemType Directory -Force $tools | Out-Null
gh release download v0.76.1 --repo github/gh-aw --pattern windows-amd64.exe --pattern checksums.txt --dir $tools
$expected = (Get-Content "$tools\checksums.txt" | Where-Object { $_ -match '\swindows-amd64.exe$' }) -split '\s+'
if ((Get-FileHash "$tools\windows-amd64.exe" -Algorithm SHA256).Hash.ToLower() -ne $expected[0]) { throw 'Checksum mismatch' }
& "$tools\windows-amd64.exe" --version
& "$tools\windows-amd64.exe" compile --help
& "$tools\windows-amd64.exe" compile regression-triage --validate --no-check-update
$hash = (Get-FileHash .github\workflows\regression-triage.lock.yml).Hash
& "$tools\windows-amd64.exe" compile regression-triage --validate --no-check-update
if ((Get-FileHash .github\workflows\regression-triage.lock.yml).Hash -ne $hash) { throw 'Non-reproducible lock' }
node --test .github\scripts\regression-triage\workflow.test.cjs
git rev-parse HEAD
```

Never compile unrelated workflows or hand-edit the lock. Automation-only paths
have no product release-note sink and require no F# compiler build. Record local
exit codes and the resulting feature SHA separately; unrun CI is not CI success.
The new lock's Git attributes preserve LF and permit the pinned generator's
trailing whitespace rather than hand-editing generated bytes.

If the Windows executable stalls in an inherited terminal, use an isolated
PowerShell job (the same verified binary, not the installed extension):

```powershell
$job = Start-Job -ArgumentList "$tools\windows-amd64.exe",(Get-Location).Path -ScriptBlock {
    param($exe,$cwd)
    Set-Location $cwd
    & $exe compile regression-triage --no-check-update
    if ($LASTEXITCODE) { throw "Compiler exit $LASTEXITCODE" }
}
$job | Wait-Job | Receive-Job
if ($job.State -ne 'Completed') { throw 'Pinned compilation failed' }
Remove-Job $job
```
