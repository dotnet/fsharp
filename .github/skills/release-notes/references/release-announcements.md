# F# release announcements

Use this workflow to draft or review one F# component article for a .NET milestone.
For per-PR changelog entries, use the [release-notes skill](../SKILL.md#version).

## 1. Establish scope

- Confirm the milestone, publication repository, article path, PR source branch, target branch, and draft state.
- Limit publication changes to the article, its component index link, and explicitly approved companion reports or media.
- Keep skill updates in a separate documentation PR, never in a release article PR.
- Store research, validation projects, and logs in session storage. Commit maintained fixtures only within explicit or agreed task scope.
- Do not delete existing author files without approval.
- Work directly by default. Use bounded, grouped research only when needed or requested, not one sub-agent per PR.
- Do not hardcode models or an automatic review panel. Honor an explicitly requested review setup.
- Do not generate whole-.NET metadata, schemas, or branch sets unless the task requires those publication contracts.

## 2. Prove release inclusion

Discover actual SDK branches rather than constructing a branch name from the milestone:

```bash
gh api repos/dotnet/sdk/branches --paginate --jq '.[].name'
```

Resolve the target release, preceding milestone, and parallel stable SDK band.
Confirm the release identity from official release metadata.
Resolve release tags to commit SHAs for each comparison:

| Source | Read at the resolved commit | Record |
| --- | --- | --- |
| `dotnet/sdk` | `eng/Version.Details.xml`, dependency `Microsoft.FSharp.Compiler` | Dependency repository URI and SHA |
| `dotnet/dotnet` | `src/source-manifest.json` at that pinned VMR SHA | `commitSha` for repository path `fsharp` |
| `dotnet/fsharp` | Source and `docs/release-notes/` at the F# SHA | Baseline and target source pins |

- Follow the dependency URI and SHA. If the source repository differs, verify the pin chain rather than assuming VMR.
- Use the official SDK's pinned payload, not a newer SDK or VMR branch head. Branch shorthand is not shipping proof.
- Compare target and baseline F# pins. Check the parallel stable band for shared or earlier availability.
- Read the whole commit range, merged PRs, and release-note files at both pins.
- Compare actual baseline changelog content. Random insertion and entry movement are not new features.
- Do not discover changes from added diff lines, manifest scores, or PR titles alone.
- Check implementation, tests, reverts, and required dependencies at the target pin. Exclude reverted and unshipped work.
- Confirm real user scenarios. Exclude maintenance, CI, localization, and test-only work without user impact.
- Include compiler-service APIs when they enable a user scenario. Scope IDE features to the product that actually ships them.
- Distinguish source-built reports from official SDK results. Name compiler, library, runtime, and packaging differences.

## 3. Select and explain the news

- Read multiple relevant published `fsharp.md` articles in `dotnet/core/release-notes/`.
- Compare voice, structure, size, and section proportions. Audit important omissions beyond the initial draft.
- Remove unchanged features already announced, including preview status reminders. Describe meaningful state changes as deltas.
- Include earlier-shipped, unannounced work only when requested. State its actual availability, not a false current-milestone introduction.
- Follow explicit user priorities. Otherwise, rank by reader impact and group related changes.
- Give major capabilities and broad, default-enabled application or library benefits suitable visibility.
- Treat library-author foundations as important when their enabled scenarios justify coverage.
- Use established feature names in headings and the opening table of contents. Keep both in the same order.
- Explain concrete benefits positively in the body. Match predecessor proportions, not a fixed highlight quota or one-sentence cap.
- Read implementation tests for meaningful capability scenarios. Adapt illustrative source-tested examples, not only the smallest syntax example.
- Do not treat negative or skipped tests, or unmet source conditions, as proof of successful behavior.
- A validated prototype or test-only integration can demonstrate shipped enabling capability. Label the integration as prototype or test-only.
- Do not present that integration as shipped stock-library support or guarantee its future release.
- State a library-author-first audience near the feature introduction.
- Distinguish shipped compiler mechanics and enabling APIs from stock-library support and prototype integrations.
- Explain the adoption path without dismissing foundations or promising integrations that do not ship.
- Define required `LangVersion` or feature opt-in once near the opening.
- Mark affected sections **Preview** and refer to that setup. Do not repeat XML or CLI setup in each section.
- Check exact gates in pinned source. For SDK availability claims, also check the exact milestone SDK.
- Keep default-enabled performance and FSharp.Core changes distinct from preview language features.
- Separate migration guidance for the previous stable release from changes affecting only earlier previews.
- Group related bug fixes by user effect. Link issues and PRs rather than listing implementation changes.

## 4. Present performance evidence

- Separate FSharp.Core savings in running applications from compiler, FCS, and IDE compilation or memory improvements.
- Group related PRs into coherent categories. Explain who benefits and how they adopt the change.
- Name the measured metric: allocated bytes, closure counts, retained heap, peak RSS, CPU time, or wall time.
- Identify baselines, workloads, hardware, and measured cohorts. Preserve regressions, uncertainty, and limits.
- Use representative comparisons. Do not add unrelated percentages or turn workload results into universal build-speed claims.
- Keep accurate author-provided rounded summaries. Do not invent precision or measurements.
- Distinguish checked author-provided presentation from independent reruns and public raw reproducibility.
- Label full-cycle companion reports when their baseline is broader than the article's milestone comparison.
- When adding a deeper report, keep the article's useful summary. Add a motivated relative link beside that summary.
- Add a prominent opening link when requested or important for report discovery.

## 5. Preserve attribution

- Check actual PR authors and contribution history. Do not infer community eligibility solely from labels.
- Credit community contributors with `Thanks to [**user**](https://github.com/user)`.
- Preserve credit for superseded contributions when they inspired the shipped work.
- Do not credit T-Gro, Copilot, or abonie as community contributors. Exclude bot accounts.

## 6. Validate the publication

- Validate changed examples and claims against the exact milestone SDK in scratch projects outside the publication repository.
- Check the claimed behavior, not only compilation. Keep commands, results, and logs in session storage.
- For prose or title edits, use scoped Markdown and link checks. Do not start new builds or unrelated benchmarks.
- Reuse valid completed evidence. Bound research to the task and required publication checks, especially during urgent edits.
- Report missing methodology or raw-result references while the author can still retrieve them. Do not invent URLs.
- When raw references are missing, require report-specific approval before publication. That approval is not a waiver for other reports.
- Preserve true author-provided or source-built scope. Missing references cannot support claims of independent verification or public reproducibility.
- If necessary evidence remains unavailable, report the gap. Do not mark an unverified claim as confirmed.
- Inspect supplied packages, article references, assets, and provenance before import. Do not execute package code.
- Import only the approved article and required media. Exclude archives, datasets, generators, extra READMEs, and scripts unless requested.
- Check SVGs for scripts and external content. Verify alt text and repository media paths.
- Preserve navigation-only milestone indexes by default. Change that convention only on explicit user request.
- Preserve other component links. Use the actual index, such as `README.md`, rather than inventing `index.md`.
- Check relative article links, anchors, the table of contents, and media references.
- After renames or deletions, repair affected references.
- Use Markdown links for every issue and PR reference.
- Verify remote PRs, source objects, and redirects. Use the authenticated GitHub API when needed to verify source objects.
- Report ignored URLs, rate limits such as HTTP 429, and unavailable checks separately. None confirms link availability.
- Review factual scope, omissions, examples, preview setup, metrics, attribution, and the complete publication diff.
- When reviewers are requested, check voice, style, and importance separately. Record predecessor files each reviewer actually read.
- A prompt that names files is not proof of reading them. Report multiple models only if those models actually participated.

## 7. Publish only through the intended PR

- Check the current PR head and source branch before edits or pushes.
- Use ordinary commits and non-force pushes to that source branch only.
- Preserve the PR base and draft state unless authorized to change them.
- Never push to `main` or the target branch. Do not enable agent merge or merge the PR.
- Verify the live remote branch and PR head SHA after a push.
- Report the exact branch and SHA, plus whole-PR file count and additions/deletions against the base.
- Distinguish local, committed, pushed, and merged work. Do not report only the latest patch as the complete PR.
