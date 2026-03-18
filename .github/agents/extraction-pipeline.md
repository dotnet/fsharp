---
name: extraction-pipeline
description: "Extracts review expertise from a GitHub user's history and generates Copilot instructions, skills, and a review agent. Invoke when setting up expert review capabilities for a repository based on a specific reviewer's historical feedback patterns."
---

# Expert Reviewer Extraction Pipeline

Generate folder-scoped instructions, topic-scoped skills, and a multi-dimensional review agent from a GitHub user's public review history. Produces anonymized, deduplicated, Copilot-compatible `.github/` artifacts.

## Scale Warning

This pipeline processes **thousands** of GitHub items (typically 3,000–10,000+ issues, PRs, discussions, and review comments spanning a decade). It will not fit in a single context window.

**Use sub-agents for everything.** The orchestrator manages SQLite state and dispatches work. Sub-agents do the heavy lifting:
- **Collection**: one sub-agent per repo × date range chunk. Parallelize aggressively (6+ concurrent agents).
- **Comment fetching**: one sub-agent per ~200 items. This is the most API-call-intensive phase.
- **Semantic analysis**: one sub-agent per ~200 comments. Each classifies and extracts rules.
- **Synthesis**: one sub-agent to read all analysis summaries (not raw data) and produce dimensions/principles.
- **Artifact generation**: one sub-agent per artifact type (instructions, skills, agent).
- **Validation/improvement**: one sub-agent per file or per concern.

Store all intermediate results in **SQLite** (queryable) and **JSON backup files** (recoverable). Sub-agents write results to files; the orchestrator imports into SQLite and dispatches next phase. Never rely on passing large datasets through context — use the filesystem.

Use `model: "claude-opus-4.6"` for all sub-agents. Use background mode (`mode: "background"`) so agents run in parallel and the orchestrator is notified on completion.

## Inputs

Collect these before starting. If any are missing, ask the user via the `ask_user` tool.

| Parameter | Required | Example |
|-----------|----------|---------|
| `landing_repo` | yes | `dotnet/fsharp` — the repo receiving the artifacts |
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

For EVERY indexed item (not a sample), fetch the user's actual words. **All comment types matter:**

- **PR descriptions** (when user is author): `pull_request_read` → `get` → save body. These reveal design intent and priorities — often the most valuable content.
- **PRs — general comments**: `pull_request_read` → `get_comments` → filter to username. This is the primary comment channel for many reviewers.
- **PRs — review comments** (code-level, with file path + diff hunk): `pull_request_read` → `get_review_comments` → filter to username
- **PRs — reviews** (approval/request-changes with summary body): `pull_request_read` → `get_reviews` → filter to username. These carry the reviewer's top-level verdict and summary — often the most opinionated content. Skip reviews with empty bodies.
- **Issues — body** (when user is author): save the issue body as a comment.
- **Issues — comments**: `issue_read` → `get_comments` → filter to username
- **Discussions**: Use GraphQL to fetch comment nodes filtered to username.

Store in SQLite (`user_comments` table): comment_id, activity_id, repo, comment_type (pr_description, issue_description, issue_comment, review_comment, pr_comment, review, discussion_comment), body, created_at, file_path, diff_hunk, url.

This is the most API-intensive phase. Batch into sub-agents of ~15 items each — small batches ensure each agent completes reliably. Parallelize aggressively. Handle rate limits with retry.

### 1.3 Collect PR context

For PRs with review comments, also collect:
- Files changed (`get_files`): path, additions, deletions, status
- PR labels and description

This maps comments to code areas.

### 1.4 Cross-validate against current codebase

Collected data references files and folders as they existed at the time of the comment — migrations and refactorings happen. Before enrichment, reconcile all file paths:

1. Extract all unique file paths from collected comments (review comments have `file_path`, PR files have `path`).
2. For each path, check if it exists in the current repo (`Test-Path` or `glob`).
3. If missing, search for the filename in its current location (files get moved between folders). Update the path if found.
4. If the file was deleted entirely, keep the comment's essence (the rule it teaches) but drop the file pointer. The rule may still apply to successor code.

This prevents generating instructions that point at nonexistent files.

### 1.5 Backup

Write all collected data as JSON to a backup directory (e.g., `{landing_repo}-analysis/`). The SQLite database is the working copy; JSON is the safety net.

---

## Phase 2: Data Enrichment and Catalogization

### 2.1 Study the landing repo

Before analyzing comments, understand the codebase:
- Directory structure → feature area mapping
- Existing documentation (specs, wiki, guides)
- Existing `.github/` artifacts (instructions, skills, agents, copilot-instructions.md, AGENTS.md)
- Technology stack, conventions, key files

Store as a feature area reference table in SQLite.

### 2.2 Semantic analysis

For each collected comment, classify using a sub-agent (Opus). **Do not use a hardcoded category list** — derive categories from the data:

1. **Bootstrap pass**: Take a random sample of ~300 comments from diverse PRs (not just the PRs with the most comments). Ask a sub-agent to read them and propose a category taxonomy. The agent should identify recurring themes, name them, and define each in one sentence. Expect 15–40 categories to emerge.

2. **Classification pass**: Using the derived taxonomy, classify all comments in batches (~15 PR packets per sub-agent, where each packet includes all comments on that PR). For each comment extract:
   - **Categories** (one or more, from the derived taxonomy)
   - **Feature area**: map to the landing repo's code structure (from 2.1)
   - **File/folder**: which code path does this apply to
   - **Severity**: trivial, minor, moderate, major, critical
   - **Derived rule**: actionable rule extracted from the comment, phrased as a generalizable principle — not tied to the specific PR

3. **Taxonomy refinement**: After the first full pass, review category distribution. Merge categories with <5 occurrences into broader ones. Split categories with >500 occurrences if they contain distinct sub-themes. Re-classify affected comments.

**Anti-overfitting rules for classification:**
- **Normalize by PR, not by comment.** A PR with 50 comments gets weight=1, same as a PR with 1 comment. Count how many PRs a rule appears in, not how many comments mention it. The reviewer saying something once on a clean PR means the same as repeating it 10 times on a messy one.
- **Generalize, don't transcribe.** The derived rule must be applicable to a future PR the classifier has never seen. "Always call stripTyEqns before matching types" is good. "Call stripTyEqns on line 47 of CheckPatterns.fs" is overfitted.
- **Distinguish reviewer opinion from CI enforcement.** If the reviewer says "please add tests", that's a review rule. If the reviewer says "run tests on Linux and Windows", that might just mean "CI should cover this" — not a rule for human reviewers. Check: does the repo's CI already do this? If yes, don't encode it as a review rule.
- **Distinguish design guidance from implementation instruction.** "Gate features behind LanguageFeature flags" is design guidance (always applicable). "Use BindUnitVars after stripping the lambda" is an implementation instruction for a specific code path (only applicable when touching that code).

Store in SQLite (`comment_analysis` table).

Process in batches. Use sub-agents — each handles ~15 PR packets with full context. Run in parallel.

### 2.3 Clustering

Aggregate analysis results to identify:

1. **Review dimensions**: Recurring themes across hundreds of comments. Each dimension should be specific enough to act on, broad enough to apply across many PRs. Target 8–24 dimensions.

2. **Folder hotspots**: Which directories receive the most review feedback, and which dimensions apply there.

3. **Overarching principles**: Cross-cutting rules that apply everywhere.

4. **Repo-specific knowledge**: Rules that are unique to this codebase, not generic programming advice.

Use a synthesis sub-agent (Opus) that reads all analysis summaries and produces:
- Dimension list with rules, severity, evidence
- Folder → dimension mapping
- Principle list
- Knowledge area reference table

---

## Phase 3: Artifact Generation

### 3.1 Raw creation

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
- Single source of truth for the review methodology
- Contains: overarching principles, all dimensions inline (with rules + CHECK flags), folder hotspot mapping, review workflow
- The review workflow is 5 waves (see below)

**Commit** after raw creation.

### 3.2 Anonymize

Remove all personal names, comment counts, PR number references, evidence statistics, "distilled from" language. The artifacts should read as authoritative engineering guidance, not data analysis output.

**Commit** after anonymization.

### 3.3 Improve per Anthropic guide

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

Compare new artifacts against existing `.github/` content:
- Check trigger overlap between new and existing skills
- Check body overlap (same howto in two places)
- Resolve: if complementary (how vs when), add cross-references. If duplicate, merge or delete.
- Instructions must not repeat AGENTS.md or copilot-instructions.md
- All doc links verified to exist on disk

**Commit** after deduplication.

---

## Phase 4: Review Workflow (embedded in the agent)

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

> Report `$DimensionName — LGTM` when the dimension is genuinely clean.
>
> Report an ISSUE only when you can construct a **concrete failing scenario**: a specific input, a specific call sequence, a specific state that triggers the bug. No hypotheticals.
>
> Read the **PR diff**, not main — new files only exist in the PR branch.
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

### 5.6 Overfitting verification

Run a confusion audit: give a fresh-context sub-agent (with NO knowledge of the source data) the generated agent file and ask it to grade every CHECK item on a confusion scale:

- **A (Clear)**: Any competent developer could apply this rule to a random future PR.
- **B (Needs context)**: Rule is correct but needs a "why" or "when" — add one sentence of rationale.
- **C (Overfitted)**: Rule is too specific to one historical scenario — generalize or remove.
- **D (Confusing)**: Rule is ambiguous or contradictory — rewrite.

Target: ≥80% grade A, 0% grade C/D. Fix all C/D items. Add "why" to all B items.

Also check:
- No rules that reproduce what CI already enforces (e.g., "run tests on Linux" when CI covers all platforms)
- No rules referencing specific function names or line numbers unless those functions are long-lived stable APIs
- Every CHECK item is phrased as a generalizable principle, not a transcription of one PR's feedback
- Dimension frequency is counted by PRs, not by comments — a PR with 50 comments counts the same as one with 1 comment

---

## Lessons Learned (encoded in this process)

Failure modes observed during development. The process above accounts for them, but they're listed for awareness:

1. **Nodder bias**: Telling sub-agents "LGTM is the best outcome" caused them to explain away real issues. The correct framing: "LGTM when genuinely clean. Do not explain away real issues."

2. **Nitpicker bias**: Without LGTM guidance, sub-agents generated 25+ findings including hypotheticals. The fix: require concrete failing scenarios — no "maybe in theory."

3. **Wrong branch verification**: Verification agents checked `main` instead of the PR branch, disputing real findings because new files didn't exist on `main`. The fix: always verify against PR diff or `refs/pull/{pr}/head`.

4. **Static-only analysis**: Reading code without tracing execution missed whether issues were real. The fix: Wave 2 requires active validation — build, test, write PoCs, simulate execution traces.

5. **Duplicate skills**: Two skills covering the same topic emerged. The fix: single source of truth in one file; others are slim pointers.

6. **Separate reference files get skipped**: Content in a separate file was not reliably read by the model. The fix: inline critical content in the agent file — it's loaded on invocation, guaranteed to be read.

7. **Description is everything for discovery**: The YAML `description` field is how the model picks from 100+ skills. Invest in keyword-rich, third-person, specific trigger descriptions.

8. **Sampling bias toward noisy PRs**: The first run sampled ~300 PRs from ~5000 and got 469 comments — biased toward high-comment PRs. The deep run collected ALL ~8000 comments from ALL PRs. The difference was 17x more data and qualitatively different dimensions emerged. Never sample.

9. **Comment-count overfitting**: A PR with 50 review comments dominated the taxonomy, producing CHECK items specific to that one PR. The fix: normalize by PR count, not comment count. One PR = one vote regardless of how many comments it has.

10. **Reproducing CI as review rules**: The classifier extracted "run tests on Linux and Windows" as a review rule — but CI already does this. Reviewers mention CI coverage in passing; it's not a rule for future reviewers. The fix: cross-reference with the repo's CI config and exclude rules that CI already enforces.

11. **Overfitted CHECK items from single PRs**: Rules like "cross-reference GenFieldInit in IlxGen.fs when modifying infos.fs" are useless for a reviewer who isn't modifying that specific file. The fix: the confusion audit (Phase 5.6) catches these. Every CHECK item must be applicable to a future PR about a feature that doesn't exist yet.

12. **Obsolete terminology**: The classifier picked up historical terms ("reactor thread") that no longer exist in the codebase. The fix: cross-validate all technical terms against the current codebase with grep. Zero matches = remove the term.

13. **Missing own-PR data**: The first run only searched `commenter:username`, missing PRs where the user was the author. PR descriptions by the target user are often the richest source of design philosophy. The fix: always search both `commenter:` AND `author:`.

14. **Multi-batch agent failures**: Sub-agents given 9+ batches of 15 PRs often gave up partway or wrote placeholder files. The fix: keep batch assignments to 4-5 batches per agent, validate output file sizes (>500 bytes), and re-dispatch failures.
