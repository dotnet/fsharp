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

### 1.1 Index all activity

Search each repo for issues, PRs, and discussions where `username` participated. GitHub search returns max 1000 results per query — split by date ranges to capture everything.

For each repo:
```
search_issues:        commenter:{username} created:{year_start}..{year_end}
search_pull_requests: commenter:{username} created:{year_start}..{year_end}
```

For discussions (if the repo uses them), use the GitHub GraphQL API:
```graphql
query {
  search(query: "repo:{owner}/{repo} commenter:{username} type:discussion", type: DISCUSSION, first: 100) {
    nodes { ... on Discussion { number title body createdAt url category { name } } }
  }
}
```

Store in SQLite (`gh_activity` table): repo, type (issue/pr/discussion), number, title, state, created_at, updated_at, labels, url, author.

Parallelize across repos and date ranges. Use sub-agents for large volumes.

### 1.2 Collect actual comments

For each indexed item, fetch the user's actual comments:
- **Issues**: `issue_read` → `get_comments` → filter to username
- **PRs — general comments**: `pull_request_read` → `get_comments` → filter to username
- **PRs — review comments** (code-level, with file path + diff hunk): `pull_request_read` → `get_review_comments` → filter to username
- **PRs — reviews** (approval/request-changes with summary body): `pull_request_read` → `get_reviews` → filter to username. These carry the reviewer's top-level verdict and summary — often the most opinionated content.
- **Discussions**: Use GraphQL to fetch comment nodes filtered to username. Discussion comments often contain design rationale and architectural decisions.

Store in SQLite (`user_comments` table): comment_id, activity_id, repo, comment_type (issue_comment, review_comment, pr_comment, review_summary, discussion_comment), body, created_at, file_path, diff_hunk, url.

This is the most API-intensive phase. Batch into sub-agents by date range. Handle rate limits with retry.

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

1. **Bootstrap pass**: Take a random sample of ~200 comments. Ask a sub-agent to read them and propose a category taxonomy that fits this specific reviewer and codebase. The agent should identify recurring themes, name them, and define each in one sentence. Expect 15–40 categories to emerge.

2. **Classification pass**: Using the derived taxonomy, classify all comments in batches (~200 per sub-agent). For each comment extract:
   - **Categories** (one or more, from the derived taxonomy)
   - **Feature area**: map to the landing repo's code structure (from 2.1)
   - **File/folder**: which code path does this apply to
   - **Sentiment**: approval, concern, suggestion, question, blocking
   - **Severity**: trivial, minor, moderate, major, critical
   - **Focus point**: what specifically is being addressed
   - **Derived rule**: actionable rule extracted from the comment

3. **Taxonomy refinement**: After the first full pass, review category distribution. Merge categories with <5 occurrences into broader ones. Split categories with >500 occurrences if they contain distinct sub-themes. Re-classify affected comments.

Store in SQLite (`comment_analysis` table).

Process in batches. Use sub-agents — each handles ~200 comments. Run in parallel.

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
