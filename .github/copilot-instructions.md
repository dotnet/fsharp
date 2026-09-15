# F# Compiler

## Build

Default (set `BUILDING_USING_DOTNET=true` system-wide):
```bash
dotnet build <changed>.fsproj -c Debug
```
Get target framework: `dotnet msbuild <proj> -getProperty:TargetFrameworks`
FSharp.Core + compiler composite: `./build.sh -c Release`
FSharp.Build changes: `./build.sh -c Release`

## No bullshit

Build fails → 99% YOUR previous change broke it. You ARE the compiler.
DON'T say "pre-existing", "infra issue", "unrelated".
DO `git clean -xfd artifacts` and rebuild.
Bootstrap contamination: early commits break compiler → later "fixes" still use broken bootstrap. Clean fully.
Triage a build failure → `binlog-analysis` skill fetches the binlog (local build or failed AzDo PR build) and analyzes it live via the `binlog-mcp` MCP (structured errors, root-cause diagnose, MSBuild perf X-ray).

## Test

Default: `-c Debug`

Use `-c Release` for: EmittedIL tests, Optimizer tests, full component runs

spot check: `dotnet test <proj> [--filter-method|--filter-class] "<glob_pattern>" -c Debug`

full component: `dotnet test tests/FSharp.Compiler.ComponentTests -c Release`

IDE/service: `tests/FSharp.Compiler.Service.Tests`

VS integration: `vsintegration/` (Windows only)

update baselines: `TEST_UPDATE_BSL=1 <test command>`

## Spotcheck tests

- find new tests for bugfix/feature
- find preexisting tests in same area
- run siblings/related

## Final validation (Copilot Coding Agent only)

Before submitting: `./build.sh -c Release --testcoreclr`

## Code

.fs: implementation
.fsi: declarations, API docs, context comments

Abbreviations (`ad`, `cenv`, `m`, `tcref`, `eenv`, `cgbuf`, `ncenv`, `tau`, …): see `docs/coding-standards.md` for the canonical glossary before guessing what a short identifier means.

## Rules

Public API change → update .fsi
New diagnostic → update `src/Compiler/FSComp.txt`
API surface change → `TEST_UPDATE_BSL=1 dotnet test tests/FSharp.Compiler.Service.Tests --filter "SurfaceAreaTest" -c Release`
After code changes → `dotnet fantomas .`
When fully done → write release notes (see skill)

## Visual Studio first

The `vs` MCP server reaches the Visual Studio instance holding this repo's solution. Where it and the shell both work, use it — it reports what the IDE's compiler and symbol graph know, not what the text files say.

- Build with `build_solution` / `build_project` / `build_clean` rather than `dotnet build` or `msbuild`; the Build section above is the fallback for when no solution is loaded (`ide_get_workspace_folders` is empty).
- Read errors from `ide_get_diagnostics`, not by parsing build output.
- Navigate and rename with `nav_go_to_definition`, `nav_find_references`, `nav_search_workspace_symbols`, `nav_rename_symbol` — the symbol graph, not `grep` plus hand edits.
- Format what you touched with `document_format` / `document_organize_imports` before reaching for `dotnet fantomas`.

Before editing a file on disk, `document_check_dirty` it. An unsaved VS buffer is the real content: read it with `document_read_buffer` and `document_save` first, or the edit lands on stale text and the user's next save reverts it.

`build_*` compiles whichever configuration the IDE has active, and `solution_set_configuration` changes it for the user's next manual build too — read `solution_get_configuration` before assuming `Debug`.

`project_add_file` appends without a position, so it cannot place a new `<Compile Include>` correctly in an order-sensitive F# project: add those to the `.fsproj` by hand.
