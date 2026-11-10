---
applyTo:
  - "**/*.md"
---

# File references

For the causes of these rules, refer to [FileReferences.findings.md](FileReferences.findings.md).

* To refer to a file, write a Markdown link. Start the path in the folder of the file that contains the link.
* To refer to a skill, write a Markdown link to its `SKILL.md` file. Use the name of the skill as the text of the link.
* Do not use `#file:'<path>'`.
* Do not write `@` before a path, except in an import in [CLAUDE.md](../../CLAUDE.md). Do not change an import in `CLAUDE.md` into a link.
* Do not put a link to a `*.instructions.md` file at the end of a line.

Examples in [.github/copilot-instructions.md](../copilot-instructions.md):

```markdown
[global.json](../global.json) sets the version of the .NET SDK.
To write the description of a pull request, use the [pr-description](skills/pr-description/SKILL.md) skill.
```
