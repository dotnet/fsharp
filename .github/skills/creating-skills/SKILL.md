---
name: creating-skills
description: Create custom agent capabilities when discovering novel tools, receiving task-agnostic tips from reviewers, or after researching specialized workflows not covered in existing instructions. Teaches structure, YAML optimization for LLM discoverability, and token efficiency.
---

# Creating GitHub Copilot Agent Skills

This skill teaches you how to create effective GitHub Copilot Agent Skills for this repository.

## Pre-Check: Avoid Duplication

**STOP** and check before creating a new skill:

1. **Does it exist already?**
   - List all skills: `ls -la .github/skills/`
   - Read existing skill frontmatter and content
   - If semantically similar skill exists, STOP

2. **Should an existing skill be expanded?**
   - If frontmatter semantically matches your use case → Update existing skill's description
   - Add keywords to improve discoverability rather than creating duplicate

3. **Should existing skill content change?**
   - If frontmatter matches but content incomplete → Add section to existing skill
   - Enhance with additional examples, procedures, or troubleshooting
   - Update frontmatter only if significantly broadening scope

**Only create new skill if:**
- No semantic overlap with existing skills
- Addresses distinct problem domain
- Has unique triggering conditions

## Skill Structure

### Directory Placement

Skills should be placed in `.github/skills/` directory:
- **Project skills** (repository-specific): `.github/skills/skill-name/`

Each skill must have its own subdirectory with a lowercase, hyphenated name that matches the `name` field in the frontmatter.

### File Requirements

Every skill directory must contain a `SKILL.md` file (case-sensitive) with:

1. **YAML Frontmatter** (required):

2. **Markdown Body** with clear instructions, examples, procedures, guidelines, and references

### Additional Resources

Skills can include:
- Scripts (e.g., `.sh`, `.fsx`, `.ps1`)
- Example files
- Templates
- Reference documentation

## YAML Frontmatter Best Practices

The frontmatter is critical for skill discoverability and token efficiency:

### Required Fields

- **name** (string): Unique identifier, lowercase with hyphens
  - Must match the directory name
  - Should be descriptive but concise
  - Example: `hypothesis-driven-debugging`, `github-actions-failure-debugging`

- **description** (string): When and why to use this skill
  - Should be 1-2 sentences
  - Include trigger keywords that help the AI recognize when to load the skill
  - Example: "Guide for debugging failing GitHub Actions workflows. Use this when asked to debug failing GitHub Actions workflows."
  - **SEO-like optimization for LLMs**: Include key terms that would appear in user requests

### Optional Fields

- **license** (string): License for the skill (e.g., MIT, Apache-2.0)

### Description Guidelines

The description is crucial for skill discoverability. Think of it like SEO for LLMs:

✅ **Good descriptions** (specific, actionable, keyword-rich):
- "Guide for debugging failing GitHub Actions workflows. Use this when asked to debug failing GitHub Actions workflows."
- "Systematic approach to investigating F# compiler performance issues using traces, dumps, and benchmarks."
- "Step-by-step process for analyzing test failures using hypothesis-driven debugging."

❌ **Poor descriptions** (vague, generic):
- "Helps with debugging"
- "Tool for testing"
- "Useful utility"

### Token Efficiency

Skills should be concise to avoid wasting context tokens:
- Keep instructions focused and relevant
- Use bullet points and numbered lists
- Avoid redundant information
- Reference external resources rather than duplicating content
- The agent will only load skills when relevant, so clear descriptions help prevent unnecessary loading

## Skill Content Best Practices

### Structure

1. **Title and Overview**: Brief introduction
2. **When to Use**: Clear triggering conditions
3. **Prerequisites**: Required tools, setup, or knowledge
4. **Step-by-Step Instructions**: Numbered procedures
5. **Examples**: Concrete use cases
6. **Troubleshooting**: Common issues
7. **References**: Links to related documentation

### Writing Style

- Use imperative mood ("Run the test", not "You should run the test")
- Be specific and actionable
- Include command examples with expected output
- Use code blocks with language identifiers
- Highlight warnings and critical information
- Reference tools and APIs that the agent has access to

### Examples

Always include concrete examples:
- Command invocations with flags and arguments
- Expected output and how to interpret it
- Common variations and edge cases
- Links to real-world usage in the repository

## Testing Your Skill

After creating a skill:

1. Verify the file structure:
   ```bash
   ls -la .github/skills/your-skill-name/
   # Should show SKILL.md and any additional resources
   ```

2. Validate YAML frontmatter:
   - Ensure proper YAML syntax
   - Required fields are present
   - Name matches directory name

3. Test skill invocation:
   - Ask Copilot a question that should trigger the skill
   - Verify the skill is loaded (check response for skill-specific guidance)
   - Ensure instructions are clear and actionable

4. Iterate based on usage:
   - Monitor how often the skill is used
   - Refine description for better discoverability
   - Update instructions based on feedback

## Examples from This Repository

See existing skills in `.github/skills/` for reference:
- `hypothesis-driven-debugging`: Systematic failure investigation
- Additional skills may be added over time

## References

- [GitHub Copilot Agent Skills Documentation](https://docs.github.com/en/copilot/concepts/agents/about-agent-skills)
- [Agent Skills Open Standard](https://github.com/agentskills/agentskills)
- [Community Skills Collection](https://github.com/github/awesome-copilot)
