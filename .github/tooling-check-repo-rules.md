# Repo-specific rules for PR Tooling Safety Check
# Read by the labelops-pr-security-scan workflow at runtime.
# Edit this file to customize for your repo.

## Non-fork bypass

If the PR's head repository is `dotnet/fsharp` (not a fork), apply `AI-Tooling-Check-Bypassed`. This means the author has write access to the repo. Full scans are only for **fork PRs** where the contributor has no repo permissions.

Compare the `headRepository.owner.login` and `headRepository.name` fields from the GitHub API — not the author's username, not the PR title/body.

## Repo-specific categories

<category name="Affects-Bootstrap">
PR modifies anything in the compiler bootstrap chain. This repo's compiler builds itself — a PROTO compiler builds the new compiler, which then builds everything else. Any change that could influence which compiler binary is used, how the bootstrap stages work, or what tools (lexer/parser generators) produce during bootstrap belongs here.
</category>

<category name="Affects-Compiler-Output">
PR modifies anything that controls what bytes end up in compiled binaries — IL emission, code generation, binary serialization, or MSBuild tasks that ship with the compiler SDK. If the change could make compiled output differ from what a source review suggests, flag it.
</category>

<category name="Affects-Design-Time">
PR modifies anything that executes code at design time — type provider infrastructure (which loads and runs arbitrary assemblies), the `#r "nuget:..."` dependency manager (which resolves and loads packages at runtime in FSI), or IDE integration that runs code when a project is opened.
</category>

<category name="Affects-Test-Tooling">
PR modifies test infrastructure that controls how tests are built, discovered, or executed — not individual test cases. Changes to test runner configuration, test framework code that spawns external processes, or end-to-end build test infrastructure belong here. Adding a new test helper method or test case does not.
</category>
