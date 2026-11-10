# Findings: references in instruction files

These findings come from the code of the tools on 2026-11-08. The documentation of the tools does not give all of these rules. Some statements of the documentation are incorrect.

| Tool | Version | Component that reads the instruction files |
|---|---|---|
| Visual Studio 2026 Insiders | 18.12.12328.396 | `Microsoft.VisualStudio.Copilot.Core.dll` and `Microsoft.VisualStudio.Copilot.Service.dll` |
| GitHub Copilot app | 1.1.6 | The Copilot CLI runtime 1.0.84-5: `runtime.node`, compiled from `custom_instructions.rs` |
| Claude Code | 2.1.319 | The JavaScript bundle in `claude.exe` |

Visual Studio contains the Copilot CLI 1.0.104-3. If the option "Use new Copilot chat experience" is on, Visual Studio starts this CLI. Then Visual Studio uses the rules of the GitHub Copilot app.

## What each tool does with a reference

| Text in an instruction file | Visual Studio | GitHub Copilot app | Claude Code |
|---|---|---|---|
| `#file:'<path>'` in a sentence | Text. The system prompt tells the model to read the file with `get_file`. | Text | Text |
| A Markdown link | Text. The system prompt tells the model to read the file with `get_file`. | Text | Text |
| A line with only `#file:'<name>.instructions.md'` | Loads the file | Text | Text |
| A line that ends with a link to a `*.instructions.md` file | Loads the file | Text | Text |
| `@<path>` | Text | An import in some files | An import in some files |
| The name of a skill | The model reads `SKILL.md` with `get_file` | The model calls the `skill` tool | The model calls the `Skill` tool |

A Markdown link is text in the three tools. Thus, the model reads the file only if the file is necessary for the task. GitHub and the IDEs show a relative Markdown link as a link that a person can click.

Visual Studio and Claude Code show the model the path of each instruction file. The GitHub Copilot app shows the path of an imported file and of each `*.instructions.md` file. The app does not show the path of `.github/copilot-instructions.md`, but this path is always the same.

## Visual Studio (default chat)

* Visual Studio reads `.github/copilot-instructions.md`, `.github/instructions/*.instructions.md`, and `%USERPROFILE%\copilot-instructions.md`.
* Visual Studio finds `.github` in the solution folder or in a parent folder. It stops at the folder that contains `.git`.
* Visual Studio reads only the top level of `.github/instructions`. It does not read the subfolders.
* In the frontmatter of a `*.instructions.md` file, Visual Studio reads only `applyTo`. For each other key, it writes a warning to its log.
* Visual Studio matches `applyTo` with `Microsoft.Extensions.FileSystemGlobbing` and ignores the case. The pattern `**/*.md` also matches the files in `.github` and `.claude`.
* Visual Studio does not read `AGENTS.md`, `CLAUDE.md`, or `.claude/rules`.
* Visual Studio loads a referenced instruction file from a line that starts with `#file:` and ends with `.instructions.md` or `.instructions.md'`. A list marker or a double quote on this line prevents the reference.
* Visual Studio also loads a file from a line that contains `](` and ends with `.instructions.md)`.
* The path starts in `.github/`. For a `*.instructions.md` file, the path starts in `.github/instructions/`. Visual Studio loads one level only.
* Visual Studio does not resolve other references in an instruction file. Its system prompt contains this text: "If the file is mentioned using markdown format or #file:'fileName' format and not already sent within the message, use get_file tool to acquire it."
* Visual Studio puts each instruction file into a code block. The first line of the code block has the relative path of the file.
* In the chat and in a prompt file, `#file:'<path>'` attaches the file. The characters `*`, `?`, `.`, and `,` end a path that has no quotes.
* Visual Studio reads skills from `.github/skills`, `.claude/skills`, and `.agents/skills` in the workspace. In the user profile, it reads `.copilot/skills`, `.claude/skills`, and `.agents/skills`.
* The system prompt lists each skill under "# Available Skills", with its name, its description, and the path of its `SKILL.md` file.
* Visual Studio loads a skill only if these conditions are true:
  * The `name` has 1 to 64 characters. It does not start or end with `-`, and it does not contain `--`.
  * The `description` has a maximum of 1,024 characters.
  * The file name is `SKILL.md`, the folder name is equal to `name`, and the name of the parent folder is `skills`.
* Visual Studio reads the frontmatter with YamlDotNet. Thus, a `description` on more than one line (`>-`) is correct.

## GitHub Copilot app

* The app reads `.github/copilot-instructions.md`, `AGENTS.md`, `CLAUDE.md`, `.claude/CLAUDE.md`, and `GEMINI.md`. It reads them in the git root, in the current folder, and in the folders between them.
* The app also reads `.github/instructions/**/*.instructions.md` and `~/.copilot/copilot-instructions.md`.
* The app does not put a `*.instructions.md` file with a specific `applyTo` into the context. It gives the model a table of patterns and paths, and the model reads the file with `view`.
* The app resolves an import only in `copilot-instructions.md`, `AGENTS.md`, and `CLAUDE.md`. It does not resolve an import in `GEMINI.md`, in `*.instructions.md`, in agent files, or in skills.
* The path starts in the folder of the file. The file must be in the repository. The app does not import an absolute path or a path with `~`. It writes a warning to its log.
* A word is an import if it starts with `./`, `../`, or `/`, or if it contains `/` or `\`. A word without a slash is an import only if it contains a period and the file exists.
* The app does not resolve an import in a code span or in a code block. It removes `!`, `)`, `,`, `.`, `:`, `;`, `?`, `]`, and `}` from the end of the path.
* The limits for each top-level file are 8 levels, 64 files, and 524,288 bytes.
* The app puts the text of the file after the import, in an `<imported_custom_instruction>` element. The attribute `source` of this element has the path of the file.
* The app puts the text of a file that the app reads directly into a `<custom_instruction>` element. This element has no path.
* If an import refers to a file that the app loaded before, the app writes `skipped="already-loaded"` and no text. The strings of the app do not show if this check includes the files that the app reads directly, for example `.github/copilot-instructions.md`.
* The app reads skills from `.github/skills`, `.claude/skills`, and `.agents/skills`, from the current folder to the git root. In the user profile, it reads `~/.copilot/skills` and `~/.agents/skills`.
* The model starts a skill with the `skill` tool. A user starts a skill with `/<name>`.
* For a skill, the app adds the line "Base directory for this skill: <folder>". The model resolves relative links from this folder. The app has no variable for the folder of a skill.
* In the prompt of the app, `@` opens a list of files. A file from this list becomes an attachment. The model gets the path and the number of lines, and reads the file with `view`.

## Claude Code

* Claude Code reads `CLAUDE.md`, `.claude/CLAUDE.md`, `CLAUDE.local.md`, and `.claude/rules/**/*.md` in the start folder and in its parent folders.
* Claude Code loads these files of a subfolder when the model reads a file in that subfolder.
* Claude Code loads a rule with `paths` when the model reads a file that matches. Claude Code matches `paths` with the `ignore` package, which uses the rules of `.gitignore`. The pattern `**/*.md` also matches the files in `.github` and `.claude`.
* Claude Code writes "Contents of <path>" before the text of each instruction file.
* Claude Code reads `AGENTS.md` only if there is no `CLAUDE.md`. It does not read `.github/copilot-instructions.md` without an import.
* Claude Code resolves an import in `CLAUDE.md`, `CLAUDE.local.md`, `.claude/rules/*.md`, and the files in `~/.claude`. The regular expression is `(?:^|\s)@((?:[^\s\\]|\\ )+)`.
* The path starts in the folder of the file. Claude Code accepts `~/` and absolute paths. For a file outside the project, Claude Code asks for an approval one time.
* Write a space in a path as `\ `. Quotes do not work.
* Claude Code does not resolve an import in a code span, in a code block, or in an HTML comment.
* The maximum depth is 4 levels below the file that Claude Code loads.
* Claude Code puts the text of an imported file into a separate block after the file that imports it. The import stays in the text.
* Claude Code does not apply `paths` to an import. It reads each rule together with its imports. At the start of a session, it keeps each file that has no `paths`. An imported file has no `paths`. Thus, each session gets the imported file. When a path matches, Claude Code adds only the text of the rule.
* The code of this filter is `Mo.filter((No)=>h?No.globs:!No.globs)`, where `h` is `conditionalRule`. On 2026-11-09, a new session in FSharp.Collections.Immutable had the import of a rule with `paths: tests/**/*.fs` in its context before the model read a file.
* Claude Code does not read `applyTo`. Thus, the `applyTo` of an imported `*.instructions.md` file does not limit the import.
* Thus, a rule with `paths` must refer to its instruction with a Markdown link. If a file must be in each session, import the file in `CLAUDE.md`.
* Claude Code imports a file only if `path.extname(path).toLowerCase()` is on a fixed list of 107 extensions. A file with no extension passes, for example `Makefile` or `.editorconfig`.
* If an extension is not on the list, Claude Code writes only one line to its debug log: `Skipping non-text file in @include: <path>`.
* A punctuation mark after the path becomes a part of the path. Thus, `@../global.json.` gets the extension `.`, and Claude Code does not import the file ([anthropics/claude-code#86060](https://github.com/anthropics/claude-code/issues/86060)).
* No setting and no environment variable changes the list. If the model must have a file with a different extension in each session, use a `SessionStart` hook. The hook returns the text in `hookSpecificOutput.additionalContext`.
* Claude Code reads skills only from `.claude/skills` in the start folder and in its parent folders to the git root, from `~/.claude/skills`, and from plugins. It does not read `.github/skills`.
* At the start of a session, the model gets only the name and the description of each skill.
* When the model starts a skill, Claude Code adds the line "Base directory for this skill: <folder>". It replaces `${CLAUDE_SKILL_DIR}`, `$ARGUMENTS`, and `$1`, and it runs `` !`command` ``.
* Then Claude Code attaches the file of each word that starts with `@`. The path starts in the current folder. If a backtick is before the `@`, Claude Code does not attach a file.

The list of extensions:

```text
.md .txt .text .json .yaml .yml .toml .xml .csv .html .htm .css .scss .sass .less .js .ts .tsx .jsx .mjs
.cjs .mts .cts .py .pyi .pyw .rb .erb .rake .go .rs .java .kt .kts .scala .c .cpp .cc .cxx .h .hpp .hxx
.cs .swift .sh .bash .zsh .fish .ps1 .bat .cmd .env .ini .cfg .conf .config .properties .sql .graphql
.gql .proto .vue .svelte .astro .ejs .hbs .pug .jade .php .pl .pm .lua .r .R .dart .ex .exs .erl .hrl
.clj .cljs .cljc .edn .hs .lhs .elm .ml .mli .f .f90 .f95 .for .cmake .make .makefile .gradle .sbt .rst
.adoc .asciidoc .org .tex .latex .lock .log .diff .patch
```

## Upstream issues

* [anthropics/claude-code#86060](https://github.com/anthropics/claude-code/issues/86060): a punctuation mark after the path stops an import. The issue is open.
* [anthropics/claude-code#40202](https://github.com/anthropics/claude-code/issues/40202) (`.gms`) and [anthropics/claude-code#18518](https://github.com/anthropics/claude-code/issues/18518) (`.mdc`) asked for one more extension. The bot closed the two issues as stale.
* [anthropics/claude-code#104510](https://github.com/anthropics/claude-code/issues/104510) (`.qmd`) asks for one more extension. The issue is open.
* On 2026-11-09, no issue asked for a list that users can change.
