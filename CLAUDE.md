@.github/copilot-instructions.md

## Visual Studio first

The `vs` MCP server reaches the Visual Studio instance holding this repo's solution. Where it and the shell both work, use it — it reports what the IDE's compiler and symbol graph know, not what the text files say.

- Build with `build_solution` / `build_project` / `build_clean` rather than `dotnet build` or `msbuild`; the Build section above is the fallback for when no solution is loaded (`ide_get_workspace_folders` is empty).
- Read errors from `ide_get_diagnostics`, not by parsing build output.
- Navigate and rename with `nav_go_to_definition`, `nav_find_references`, `nav_search_workspace_symbols`, `nav_rename_symbol` — the symbol graph, not `grep` plus hand edits.
- Format what you touched with `document_format` / `document_organize_imports` before reaching for `dotnet fantomas`.

Before editing a file on disk, `document_check_dirty` it. An unsaved VS buffer is the real content: read it with `document_read_buffer` and `document_save` first, or the edit lands on stale text and the user's next save reverts it.

`build_*` compiles whichever configuration the IDE has active, and `solution_set_configuration` changes it for the user's next manual build too — read `solution_get_configuration` before assuming `Debug`.

`project_add_file` appends without a position, so it cannot place a new `<Compile Include>` correctly in an order-sensitive F# project: add those to the `.fsproj` by hand.

Tests have no IDE path — the `vs` server exposes no Test Explorer tools (https://github.com/Corsinvest/cv4vs-agents/issues/192), so they run through `dotnet test` as described above.
