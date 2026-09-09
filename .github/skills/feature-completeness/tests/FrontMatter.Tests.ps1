#Requires -Modules powershell-yaml

Describe 'feature-completeness front matter' {
    It 'preserves the complete discovery description' {
        $markdown = Get-Content (Join-Path $PSScriptRoot '..\SKILL.md') -Raw -Encoding UTF8
        $frontMatter = ($markdown -split '(?m)^---\s*$', 3)[1]
        $description = (ConvertFrom-Yaml $frontMatter).description
        $expected = 'Use when about to defer useful work — when you catch yourself writing "follow-up", "future work", "out of scope", "v2", "next phase", "tracked in #NNN", "left as a TODO", "known limitation", or proposing to file an issue instead of finishing. Forces a binary decision: do the work now because it meets the product bar, or drop it with a real reason. Applies to feature scope, edge cases, missed optimizer/codegen cases, diagnostics gaps, and half-implemented APIs.'

        $description | Should -BeExactly $expected
    }
}
