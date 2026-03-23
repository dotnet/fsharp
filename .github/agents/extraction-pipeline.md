---
name: extraction-pipeline
description: "Extracts review expertise from a GitHub user's history and generates Copilot instructions, skills, and a review agent. Invoke when setting up expert review capabilities for a repository based on a specific reviewer's historical feedback patterns."
---

# Expert Reviewer Extraction Pipeline

Generate folder-scoped instructions, topic-scoped skills, and a multi-dimensional review agent from a GitHub user's public review history. Produces anonymized, deduplicated, Copilot-compatible `.github/` artifacts.

## Pipeline Overview

```
Phase 1: Collect        Phase 2: Enrich        Phase 3: Generate      Phase 5: Verify
─────────────────       ───────────────        ────────────────       ───────────────
1.1 Index activity      2.1 Study repo         3.1 Raw creation       5.1-5.5 Quality checks
    → gh_activity           → feature_areas        → agent.md (raw)   5.6 Codebase verify
1.2 Collect comments        → ci_summary.txt       → SKILL.md (raw)       → agent.md (verified)
    → user_comments     2.2 Classify               → instructions     5.7 Overfitting check
1.3 Collect PR context      → comment_analysis         (raw)              → final artifacts
    → pr_contexts       2.2b Deduplicate       3.2 Anonymize
1.4 Reconcile paths         → pr_rule_votes        → *.md (anon)      Phase 4 is NOT a pipeline
    → user_comments     2.3 Synthesize         3.3 Anthropic guide    step — it defines the
       (paths updated)      → dimensions.json      → *.md (polished)  review workflow EMBEDDED
1.5 Backup                  → principles.json  3.4 Deduplicate        in the generated agent.
    → JSON files            → dim_evidence         → *.md (deduped)
```

Each phase checks its output tables exist before running — skip completed phases on resume.

## Scale

This pipeline processes **thousands** of GitHub items (typically 3,000–10,000+ issues, PRs, discussions, and review comments spanning a decade). It will not fit in a single context window.

**Use sub-agents for everything.** The orchestrator manages SQLite state and dispatches work. Sub-agents do the heavy lifting — see each phase for batch sizes and parallelism guidance.

**Context management:** Store all intermediate results in **SQLite** (queryable) and **JSON backup files** (recoverable). Sub-agents write results to files; the orchestrator imports into SQLite and dispatches the next phase. Never pass large datasets through agent context — use the filesystem.

**Model selection:** Use the best available reasoning model (e.g., `claude-opus-4.6`) for classification and synthesis sub-agents. Fast/cheap models produce shallow rules. Collection sub-agents can use standard models. Use background mode so agents run in parallel.

**Reliability:** After each batch of sub-agents completes, validate output files: >500 bytes, parseable JSON, contains entries for all assigned items. Re-dispatch incomplete outputs up to 3 times. Keep batch assignments to ≤5 batches per agent — agents given too much work produce placeholders or give up.

## Inputs

Collect these before starting. If any are missing, ask the user via the `ask_user` tool.

| Parameter | Required | Example |
|-----------|----------|---------|
| `landing_repo` | yes | `owner/repo` — the repo receiving the artifacts |
| `username` | yes | GitHub username whose review history to extract |
| `earliest_year` | no (default: 10 years back) | `2015` |
| `reference_repos` | no | Additional repos to search (e.g., `dotnet/sdk`, `dotnet/runtime`) |
| `agent_name` | no (default: `expert-reviewer`) | Name for the review agent and skill |
| `skill_trigger` | no (default: auto-derived) | Keywords that trigger the review skill |

### Prerequisites

The landing repo must be checked out locally — the pipeline searches its directory structure, verifies file paths, reads existing docs, and validates generated artifacts against the actual codebase. If not checked out, clone it first:

```bash
gh repo clone {landing_repo}
```

If `reference_repos` are specified and the pipeline needs to search their code (e.g., for cross-repo integration patterns), check those out as sibling directories.

---

## Phase 1: Data Collection

**Completeness is critical.** Do not sample — collect ALL activity. A reviewer who leaves one precise comment on a well-written PR teaches as much as 50 comments on a messy one. Sampling biases toward noisy PRs and misses the signal in clean approvals.

### 1.1 Index all activity

> **Sub-agents:** 1 per repo × date-range chunk (parallelize 6+)
> **Input:** GitHub API search results
> **Output:** SQLite `gh_activity` table, JSON backup per chunk
> **Resume:** Skip if `gh_activity` has rows for this repo+date range

Search each repo for issues, PRs, and discussions where `username` participated. **Include ALL states** — open, closed, merged, and rejected PRs all carry learning potential. Rejected PRs often contain the strongest review opinions.

GitHub search returns max 1000 results per query — split by 1-year date ranges to capture everything. For high-volume users, split by 6 months.

For each repo, run FOUR searches (not two — capture both commenter and author roles):
```
search_pull_requests: commenter:{username} created:{year_start}..{year_end}
search_pull_requests: author:{username} created:{year_start}..{year_end}
search_issues:        commenter:{username} created:{year_start}..{year_end}
search_issues:        author:{username} created:{year_start}..{year_end}
```

**Own PRs are first-class data.** When the user is the PR author, their PR description reveals design intent, priorities, and rationale that never appears in review comments. Tag each item with the user's role: `reviewer`, `author`, or `both`.

For discussions (if the repo uses them), use the GitHub GraphQL API:
```graphql
query {
  search(query: "repo:{owner}/{repo} commenter:{username} type:discussion", type: DISCUSSION, first: 100) {
    nodes { ... on Discussion { number title body createdAt url category { name } } }
  }
}
```

Store in SQLite (`gh_activity` table): repo, type (issue/pr/discussion), number, title, state, created_at, updated_at, labels, url, author, user_role.

Parallelize across repos and date ranges. Use sub-agents for large volumes. Paginate ALL results — do not stop at page 1.

### 1.2 Collect actual comments

> **Sub-agents:** 1 per ~15 PRs (parallelize aggressively)
> **Input:** `gh_activity` table (PR/issue numbers to fetch)
> **Output:** SQLite `user_comments` table, JSON backup per batch
> **Resume:** Skip PRs already in `user_comments`
> **Validation:** Each output file >500 bytes, contains entries for all 15 assigned PRs

For EVERY indexed item (not a sample), fetch the user's actual words. **All comment types matter:**

- **PR descriptions** (when user is author): `pull_request_read` → `get` → save body. These reveal design intent and priorities — often the most valuable content.
- **PRs — general comments**: `pull_request_read` → `get_comments` → filter to username. This is the primary comment channel for many reviewers.
- **PRs — review comments** (code-level, with file path + diff hunk): `pull_request_read` → `get_review_comments` → filter to username
- **PRs — reviews** (approval/request-changes with summary body): `pull_request_read` → `get_reviews` → filter to username. These carry the reviewer's top-level verdict and summary — often the most opinionated content. Skip reviews with empty bodies.
- **Issues — body** (when user is author): save the issue body as a comment.
- **Issues — comments**: `issue_read` → `get_comments` → filter to username
- **Discussions**: Use GraphQL to fetch comment nodes filtered to username.

Store in SQLite (`user_comments` table): comment_id, activity_id, repo, comment_type (pr_description, issue_description, issue_comment, review_comment, pr_comment, review, discussion_comment), body, created_at, file_path, diff_hunk, url.

This is the most API-intensive phase. Batch into sub-agents of ~15 PRs each (not 15 comments — each agent handles 15 PRs and fetches all comment types for each). When fetching comments for a single PR, paginate through all pages (`get_review_comments` returns max 100 per page). Parallelize aggressively. Handle rate limits with retry and exponential backoff.

### 1.3 Collect PR context

> **Sub-agents:** 1 per ~15 PRs (can share with 1.2 agents)
> **Input:** `gh_activity` table (PRs with review comments)
> **Output:** SQLite `pr_contexts` table (files_changed, labels, description per PR)

For PRs with review comments, also collect:
- Files changed (`get_files`): path, additions, deletions, status
- PR labels and description

This maps comments to code areas.

### 1.4 Cross-validate against current codebase

> **Sub-agents:** 1 (or orchestrator directly)
> **Input:** `user_comments` table, local repo checkout
> **Output:** `user_comments` table (paths updated in-place), `path_mapping` table, `obsolete_terms` list

Collected data references files, folders, and terminology as they existed at the time of the comment — migrations and refactorings happen. Reconcile before enrichment:

**File paths:**
1. Extract all unique file paths from collected comments (review comments have `file_path`, PR files have `path`).
2. For each path, check if it exists in the current repo (`Test-Path` or `glob`).
3. If missing, search for the filename in its current location (files get moved between folders). Update the path if found.
4. If the file was deleted entirely, keep the comment's essence (the rule it teaches) but drop the file pointer. The rule may still apply to successor code.

**Technical terms:** `grep` every technical term used in comments (function names, type names, internal concepts) against the current codebase. Terms with zero matches are obsolete — do not use them in generated artifacts.

### 1.5 Backup

Write all collected data as JSON to a backup directory (e.g., `{landing_repo}-analysis/`). The SQLite database is the working copy; JSON is the safety net.

---

## Phase 2: Data Enrichment and Catalogization

### 2.1 Study the landing repo

> **Sub-agents:** 1 (explore agent)
> **Input:** Local repo checkout (`src/`, `tests/`, `eng/`, `.github/`, CI configs)
> **Output:** SQLite `feature_areas` table, `ci_summary.txt`, `existing_artifacts.txt`

Before analyzing comments, understand the codebase:
- Directory structure → feature area mapping
- Existing documentation (specs, wiki, guides)
- Existing `.github/` artifacts (instructions, skills, agents, copilot-instructions.md, AGENTS.md)
- Technology stack, conventions, key files
- **CI configuration** — analyze CI config files (GitHub Actions, Azure Pipelines, Jenkins, etc.) and produce a CI coverage summary: what CI already enforces (platform coverage, test suites, formatting, linting, etc.). Provide this summary to every classification sub-agent in §2.2.

Store feature areas in SQLite: `CREATE TABLE feature_areas (area_name TEXT, folder_glob TEXT, description TEXT)`. Store CI summary as a text file.

### 2.2 Semantic analysis

> **Sub-agents:** 1 for bootstrap, then ~N/15 for classification (where N = number of PRs with comments)
> **Input:** `user_comments` table, `feature_areas` table, `ci_summary.txt`
> **Output:** SQLite `comment_analysis` table, `taxonomy.json`
> **Context per sub-agent:** taxonomy + CI summary + 15 PR packets (all comments on each PR)

For each collected comment, classify using a sub-agent (Opus). **Do not use a hardcoded category list** — derive categories from the data:

1. **Bootstrap pass**: Take a stratified sample of ~300 comments: proportional by year, at least 5 per major feature area from §2.1, and at least 20 each of review_comments, pr_descriptions, and issue_comments. Ask a sub-agent to read them and propose a category taxonomy. The agent should identify recurring themes, name them, and define each in one sentence. Expect 15–40 categories to emerge. After deriving the taxonomy, cross-check it against the feature area table — if any area representing >10% of the codebase has zero categories, re-sample with enforced coverage.

2. **Classification pass**: Using the derived taxonomy, classify all comments in batches (~15 PR packets per sub-agent, where each packet includes all comments on that PR). For each comment extract:
   - **Categories** (one or more, from the derived taxonomy)
   - **Feature area**: map to the landing repo's code structure (from 2.1)
   - **File/folder**: which code path does this apply to
   - **Severity**: trivial, minor, moderate, major, critical
   - **Derived rule**: actionable rule extracted from the comment, phrased as a generalizable principle — not tied to the specific PR

3. **Taxonomy refinement**: After the first full pass, review category distribution. Merge categories with <5 occurrences into broader ones. Split categories with >500 occurrences if they contain distinct sub-themes. Re-classify affected comments.

**Anti-overfitting rules for classification:**
- **Normalize by PR, not by comment.** A PR with 50 comments gets weight=1, same as a PR with 1 comment. Count how many PRs a rule appears in, not how many comments mention it. The reviewer saying something once on a clean PR means the same as repeating it 10 times on a messy one.
- **Generalize, don't transcribe.** The derived rule must be applicable to a future PR the classifier has never seen. "Always normalize type representations before pattern matching" is good. "Call helper X on line 47 of file Y" is overfitted.
- **Distinguish reviewer opinion from CI enforcement.** If the reviewer says "please add tests", that's a review rule. If the reviewer says "run tests on Linux and Windows", that might just mean "CI should cover this" — not a rule for human reviewers. Check the CI summary from §2.1: if CI already enforces it, don't encode it as a review rule.
- **Distinguish design guidance from implementation instruction.** "Gate features behind feature flags" is design guidance (always applicable). "Use helper X after calling Y" is an implementation instruction for a specific code path (only applicable when touching that code).

Store in SQLite (`comment_analysis` table).

Process in batches. Use sub-agents — each handles ~15 PR packets with full context. Run in parallel.

### 2.2b Deduplication (enforce PR-normalization)

> **Sub-agents:** None (orchestrator runs SQL directly)
> **Input:** `comment_analysis` table
> **Output:** `pr_rule_votes` table (1 vote per rule per PR)

Before synthesis, collapse per-comment rows into per-PR votes:

```sql
CREATE TABLE pr_rule_votes AS
SELECT DISTINCT activity_id, derived_rule, category, feature_area
FROM comment_analysis;
```

This ensures a PR with 50 comments gets weight=1, same as a PR with 1 comment. The synthesis agent in §2.3 reads `pr_rule_votes`, never raw `comment_analysis`.

### 2.3 Clustering

> **Sub-agents:** 1 (Opus, synthesis)
> **Input:** `pr_rule_votes` table, `taxonomy.json`, `feature_areas` table, `ci_summary.txt`
> **NOT available:** raw `user_comments`, JSON backups — synthesis works only with classified, deduplicated data
> **Output:** `dimensions.json`, `principles.json`, `folder_hotspots.json`, SQLite `dimension_evidence` table

Aggregate the deduplicated `pr_rule_votes` to identify:

1. **Review dimensions**: Recurring themes across many PRs. Each dimension should be specific enough to act on, broad enough to apply across many PRs. Target 8–24 dimensions. If any single dimension accounts for >40% of total PR-votes, flag it for splitting.

2. **Folder hotspots**: Which directories receive the most review feedback, and which dimensions apply there.

3. **Overarching principles**: Cross-cutting rules that apply everywhere.

4. **Repo-specific knowledge**: Rules that are unique to this codebase, not generic programming advice.

The synthesis sub-agent receives:
- The taxonomy from §2.2 step 1
- The `pr_rule_votes` table (deduplicated: one vote per rule per PR)
- The `feature_areas` table from §2.1
- The CI coverage summary from §2.1

The synthesis agent MUST NOT access raw comment data (`user_comments` table or JSON backups). It works only with classified, deduplicated data.

It produces:
- Dimension list with rules, severity, and PR-count evidence
- Folder → dimension mapping
- Principle list
- A `dimension_evidence` table: `(dimension, pr_count, example_prs)` for verification in Phase 5

---

## Phase 3: Artifact Generation

### 3.1 Raw creation

> **Sub-agents:** 1 per artifact type (3 total)
> **Input:** `dimensions.json`, `principles.json`, `folder_hotspots.json`, existing `.github/` artifacts
> **Output:** `agent.md` (raw), `SKILL.md` (raw), `*.instructions.md` (raw)
> **Anti-overfitting:** generation sub-agents must follow the same rules from §2.2

Generate three artifact types:

#### Instructions (`.github/instructions/*.instructions.md`)
- One per major code folder/area
- YAML frontmatter with `applyTo` glob matching existing folders
- Content: folder-specific rules derived from review feedback for that area
- Concise (under 60 lines) — these load on every edit in scope
- Reference docs, don't reproduce them
- Do NOT duplicate AGENTS.md or copilot-instructions.md

#### Skills (`.github/skills/*/SKILL.md`)
- One per overarching topic that doesn't map to a single folder
- YAML frontmatter: `name` (gerund form, lowercase+hyphens, ≤64 chars), `description` (third person, ≤1024 chars, describes WHAT and WHEN — this is the discovery trigger)
- Content: decision frameworks, checklists, rules, examples
- Under 500 lines — use progressive disclosure for longer content
- Reference docs, don't reproduce them

#### Review Agent (`.github/agents/{agent_name}.md`)
- Single source of truth for dimension definitions and review workflow — all CHECK rules must be inline
- Contains: overarching principles, all dimensions inline (with rules + CHECK flags), review workflow
- The folder→dimension routing table belongs in the skill (operational configuration, not methodology) — the agent references the skill during Wave 1 to select dimensions
- The review workflow is 5 waves (see below)
- The artifact-generation sub-agent must follow the same anti-overfitting rules from §2.2: every CHECK item must be a generalizable principle applicable to future PRs about features that don't exist yet

**Commit** after raw creation.

### 3.2 Anonymize

> **Input:** `*.md` (raw)
> **Output:** `*.md` (anonymized)

Remove all personal names, comment counts, PR number references, evidence statistics, "distilled from" language. The artifacts should read as authoritative engineering guidance, not data analysis output.

**Commit** after anonymization.

### 3.3 Improve per Anthropic guide

> **Input:** `*.md` (anonymized)
> **Output:** `*.md` (polished)

Apply https://platform.claude.com/docs/en/agents-and-tools/agent-skills/best-practices:
- `name`: gerund form, lowercase+hyphens
- `description`: third person, specific triggers, ≤1024 chars
- Concise — only add what the model doesn't already know
- No time-sensitive information
- Consistent terminology
- Progressive disclosure (SKILL.md as overview, reference files for detail)
- One level deep references only

**Commit** after improvements.

### 3.4 Deduplicate and cross-reference

> **Input:** `*.md` (polished), existing `.github/` content
> **Output:** `*.md` (deduplicated) — committed to repo

Compare new artifacts against existing `.github/` content:
- Check trigger overlap between new and existing skills
- Check body overlap (same content in two places) — if the same concept appears in both agent and skill, keep it in the agent (source of truth) and have the skill point to it
- Instructions must not repeat AGENTS.md, copilot-instructions.md, or the agent's CHECK items verbatim — instructions are for concise auto-loaded reminders only
- All doc links verified to exist on disk
- The YAML `description` field is how the model picks from 100+ skills — invest in keyword-rich, third-person, specific trigger descriptions

**Commit** after deduplication.

---

## Phase 4: Review Workflow Specification (embedded in the generated agent)

This section defines the workflow that the generated review agent will follow at runtime. The pipeline does not execute this workflow — it embeds it as instructions in the agent artifact.

The review agent runs a 5-wave process when invoked:

### Wave 0: Build Review Briefing Pack

Before any dimension analysis, assemble the full context. Sub-agents reviewing code without context hallucinate or miss important design intent.

**Collect:**
- **PR metadata**: title, description, author, labels, linked issues/specs
- **Existing PR comments and reviews**: what has already been discussed — don't duplicate existing feedback
- **Referenced issues and design documents**: if the PR links to a spec or issue, read them for design intent
- **Changed files list**: `pull_request_read` → `get_files` for paths, additions, deletions

**Compute the correct diff:**

The PR diff must reflect only the PR's own changes — not unrelated commits on `main` since the branch was created. Agents often get this wrong (e.g., they see "deletions" that are actually new `main` commits not in the branch).

Use `gh` CLI — it computes the diff correctly against the merge base:
```bash
# Correct diff via gh CLI (uses GitHub's merge-base computation)
gh pr diff {pr_number} --repo {owner}/{repo}

# Or via API (same correct merge-base diff)
gh api repos/{owner}/{repo}/pulls/{pr_number} --jq '.diff_url' | xargs curl -sL
```

Alternatively, use the MCP tool `pull_request_read` → `get_diff` which GitHub also computes correctly against the merge base.

**Do NOT use raw `git diff main..branch`** — this includes unrelated main commits and produces a wrong diff.

**Save the briefing pack** to a file (e.g., `pr-{number}-briefing.md`). Every Wave 1 sub-agent receives this file as context.

### Wave 1: Find

Launch **one sub-agent per dimension** (parallel batches of 6). Each evaluates exactly one dimension against the PR diff.

Sub-agent instructions:

> Report `$DimensionName — LGTM` when the dimension is genuinely clean. Do not explain away real issues to produce a clean result.
>
> Report an ISSUE only when you can construct a **concrete failing scenario**: a specific input, a specific call sequence, a specific state that triggers the bug. No hypotheticals — "this might be a problem in theory" is not a finding.
>
> Read the **PR diff**, not main — new files only exist in the PR branch. Never verify findings against `main`; the code you're reviewing only exists in `refs/pull/{pr}/head`.
>
> Include exact file path and line range. Verify by tracing actual code flow.

### Wave 2: Validate

For each non-LGTM finding, actively prove or disprove it:

- **Code flow tracing**: Read full source from PR branch (`refs/pull/{pr}/head`). Trace callers, callees, state mutations, error paths.
- **Write and run tests for claims**: Write a minimal test that demonstrates the claimed issue. Run it against the PR branch. If the test fails as predicted → confirmed. If it passes → disputed.
- **Proof-of-concept snippet**: When a full test is too complex to run inline, write pseudocode or partial code demonstrating the issue. Include in PR feedback as evidence — enough for another engineer to implement.
- **Scenario simulation**: For complex issues (concurrency, state machines, protocol interactions), write a step-by-step execution trace showing how the bug manifests.
- **Multi-model consensus**: For borderline findings, validate with 3 models (Opus, Codex, Gemini). Keep findings confirmed by ≥2/3.

A finding is confirmed only with concrete evidence. Never validate against `main` — PR code only exists in the PR branch.

### Wave 3: Post

Post confirmed findings as inline review comments at exact file:line via GitHub CLI or MCP tools:

```markdown
**[$SEVERITY] $DimensionName**

$Concrete scenario that triggers the bug.

**Execution trace:** (when helpful)
Step 1: caller invokes X with input Y (line N)
Step 2: control reaches Z without validation (line M) ← bug

**Proof-of-concept test:**
```csharp
[Fact]
public void Scenario_Demonstrates_Issue() { ... }
```

**Recommendation:** $Fix.
```

Post design-level concerns (not tied to a line) as a single PR comment — one bullet each.

### Wave 4: Summary

Post a dimension checkbox table as the review body:

```markdown
| # | Dimension | Verdict |
|---|-----------|---------|
| 1 | Dimension Name | ✅ LGTM |
| 2 | Another | ⚠️ 1 MAJOR |

- [x] Dimension Name
- [ ] Another — description of issue
```

`[x]` = LGTM or NITs only. `[ ]` = MAJOR or BLOCKING.
All `[x]` → APPROVE. Any BLOCKING → REQUEST_CHANGES. Otherwise → COMMENT.

---

## Phase 5: Final Quality Gate

### 5.1 Anthropic guide compliance

Verify all artifacts against best practices:
- YAML frontmatter: name (gerund, ≤64), description (third person, ≤1024, triggers)
- No verbose explanations (the model is smart)
- No time-sensitive info
- Consistent terminology
- Progressive disclosure respected

### 5.2 Flow coherence

Verify the three layers work together:
- Instructions trigger on file edits → folder-specific rules
- Skills trigger on topic keywords → overarching guidance
- Agent triggers on `@{agent_name}` → full review workflow
- No concept explained in two places
- Skills point to agent for review, not duplicate it
- Instructions don't repeat skills or AGENTS.md

### 5.3 Link and path verification

- All `applyTo` globs match existing folders (`Test-Path`)
- All relative doc links resolve to existing files
- No stale references to deleted files

### 5.4 Anonymization verification

- Zero occurrences of: the username, full name, comment counts, PR numbers, "distilled from", "extracted from", evidence statistics
- Content reads as authoritative guidance, not analysis output

### 5.5 Deduplication verification

- No trigger overlap between skills (unless cross-referenced as complementary)
- No body overlap between instructions and AGENTS.md/copilot-instructions.md
- Agent doesn't repeat AGENTS.md content

### 5.6 Codebase verification

> **Sub-agents:** 1 per dimension (parallelize)
> **Input:** `agent.md` (deduplicated) CHECK items, local repo (`src/`, `tests/`, `eng/`), CI config
> **Output:** `confusion_audit.json` (grade per item), `agent.md` (verified)
> **Feedback loop:** If >20% grade C/D → re-run Phase 2.2 + 2.3 + 3 with stronger anti-overfitting

For each dimension in the generated agent, dispatch a fresh-context sub-agent that reads:
1. The dimension's CHECK items
2. The actual codebase (`src/`, `tests/`, `eng/`)
3. The repo's CI configuration

The sub-agent answers for every CHECK item:
- **Does this term exist in the codebase?** `grep` every function name, type name, and concept. Zero matches = obsolete, remove or replace.
- **Does CI already enforce this?** If the rule says "test on multiple platforms" and CI runs on 3 OSes, drop the rule — CI handles it.
- **Is this generalizable?** Could a reviewer apply this to a PR implementing a feature that doesn't exist yet? If it only makes sense for one specific code path, either generalize it or move it to a code comment.
- **Is the "why" clear?** Would a developer who has never seen this codebase understand what goes wrong if they violate this rule? If not, add a one-sentence rationale.

Grade each item: **A** (clear, verified), **B** (needs rationale — add it), **C** (overfitted — generalize or remove), **D** (obsolete/contradictory — rewrite or remove).

**Targets:** ≥80% grade A, 0% grade C/D. Fix all B/C/D items before finalizing.

**Feedback loop:** If >20% of items are grade C/D, the problem is in classification (Phase 2), not just in the artifact. Re-run §2.2 classification for the affected categories with strengthened anti-overfitting prompts, then re-run §2.3 synthesis and §3 artifact generation. Fixing artifacts alone treats symptoms.

**Commit** after verification fixes.

### 5.7 Overfitting verification

> **Input:** All `*.md` (verified), `dimension_evidence` table from §2.3
> **Output:** Final artifacts — committed to repo

Final check on the complete artifact set:
- No rules that reproduce what CI already enforces
- No rules referencing specific function names or line numbers unless those functions are long-lived stable APIs (verified by grep in 5.6)
- Every CHECK item is phrased as a generalizable principle, not a transcription of one PR's feedback
- Dimension frequency was counted by PRs, not by comments — a PR with 50 comments counts the same as one with 1 comment


