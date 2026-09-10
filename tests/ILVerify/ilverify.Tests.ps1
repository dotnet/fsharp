# Pester tests for ILVerify helper functions defined in ilverify.ps1.
# Compatible with Pester 3.x+ (ships with Windows PowerShell).
# Run with: Invoke-Pester ./tests/ILVerify/ilverify.Tests.ps1

# Extract and load only the function definitions from ilverify.ps1
# without executing the build/verification logic.
$scriptContent = Get-Content "$PSScriptRoot/ilverify.ps1" -Raw
$ast = [System.Management.Automation.Language.Parser]::ParseInput($scriptContent, [ref]$null, [ref]$null)
$functions = $ast.FindAll({ $args[0] -is [System.Management.Automation.Language.FunctionDefinitionAst] }, $false)
foreach ($fn in $functions) {
    Invoke-Expression $fn.Extent.Text
}

Describe "Normalize-IlverifyOutputLine" {
    It "removes closure suffixes" {
        Normalize-IlverifyOutputLine 'Foo+clo@924-516::Invoke()' | Should Be 'Foo+clo::Invoke()'
    }

    It "removes function suffixes with line numbers" {
        Normalize-IlverifyOutputLine 'parseOption@269::Bar()' | Should Be 'parseOption::Bar()'
    }

    It "removes 'at line NNNN'" {
        Normalize-IlverifyOutputLine 'something at line 1234 rest' | Should Be 'something rest'
    }

    It "removes pipe stage patterns" {
        Normalize-IlverifyOutputLine 'Foo+Pipe #1 stage #1 at line 1782@1782::Invoke()' | Should Be 'Foo+::Invoke()'
    }

    It "collapses multiple spaces" {
        Normalize-IlverifyOutputLine 'a  b   c' | Should Be 'a b c'
    }

    It "trims leading and trailing whitespace" {
        Normalize-IlverifyOutputLine '  hello world  ' | Should Be 'hello world'
    }
}

Describe "Remove-IlverifyOffsets" {
    It "strips a single offset from an error line" {
        $line = "[IL]: Error [StackByRef]: : Foo::Bar(int32)][offset 0x0000001E][found Native Int] Expected ByRef."
        $result = Remove-IlverifyOffsets @($line)
        $result | Should Be "[IL]: Error [StackByRef]: : Foo::Bar(int32)][found Native Int] Expected ByRef."
    }

    It "handles uppercase and lowercase hex digits" {
        $result = Remove-IlverifyOffsets @("prefix [offset 0xABcd0012] suffix")
        $result | Should Be "prefix  suffix"
    }

    It "trims trailing whitespace" {
        $result = Remove-IlverifyOffsets @("some text   ")
        $result | Should Be "some text"
    }

    It "returns empty array for empty input" {
        $result = @(Remove-IlverifyOffsets @())
        $result.Count | Should Be 0
    }

    It "processes multiple lines" {
        $lines = @(
            "[IL]: Error [X]: : A::M()][offset 0x00000011][found Y] Msg1.",
            "[IL]: Error [X]: : B::N()][offset 0x00000022][found Z] Msg2."
        )
        $result = Remove-IlverifyOffsets $lines
        $result.Count | Should Be 2
        $result[0] | Should Be "[IL]: Error [X]: : A::M()][found Y] Msg1."
        $result[1] | Should Be "[IL]: Error [X]: : B::N()][found Z] Msg2."
    }

    It "passes through lines without offsets unchanged" {
        $result = Remove-IlverifyOffsets @("no offsets here")
        $result | Should Be "no offsets here"
    }
}

Describe "Soft comparison (offset-tolerant)" {
    It "matches when only IL offsets differ" {
        $output   = @(
            "[IL]: Error [StackByRef]: : Foo::Bar()][offset 0x0000001E][found Native Int] Expected ByRef.",
            "[IL]: Error [ReturnPtrToStack]: : Baz::Qux()][offset 0x00000070] Return type is ByRef."
        )
        $baseline = @(
            "[IL]: Error [StackByRef]: : Foo::Bar()][offset 0x0000001A][found Native Int] Expected ByRef.",
            "[IL]: Error [ReturnPtrToStack]: : Baz::Qux()][offset 0x00000064] Return type is ByRef."
        )
        $cmp = Compare-Object (Remove-IlverifyOffsets $output) (Remove-IlverifyOffsets $baseline)
        $cmp | Should BeNullOrEmpty
    }

    It "detects real differences even when offsets also differ" {
        $output   = @(
            "[IL]: Error [StackByRef]: : Foo::Bar()][offset 0x0000001E][found Native Int] Expected ByRef.",
            "[IL]: Error [NEW_ERROR]: : New::Method()][offset 0x00000099][found X] New error."
        )
        $baseline = @(
            "[IL]: Error [StackByRef]: : Foo::Bar()][offset 0x0000001A][found Native Int] Expected ByRef.",
            "[IL]: Error [ReturnPtrToStack]: : Baz::Qux()][offset 0x00000064] Return type is ByRef."
        )
        $cmp = Compare-Object (Remove-IlverifyOffsets $output) (Remove-IlverifyOffsets $baseline)
        $cmp | Should Not BeNullOrEmpty
    }

    It "detects added errors" {
        $output   = @(
            "[IL]: Error [X]: : Foo::A()][offset 0x00000011] Msg.",
            "[IL]: Error [X]: : Foo::B()][offset 0x00000022] Msg.",
            "[IL]: Error [X]: : Foo::C()][offset 0x00000033] New."
        )
        $baseline = @(
            "[IL]: Error [X]: : Foo::A()][offset 0x00000011] Msg.",
            "[IL]: Error [X]: : Foo::B()][offset 0x00000022] Msg."
        )
        $cmp = Compare-Object (Remove-IlverifyOffsets $output) (Remove-IlverifyOffsets $baseline)
        $cmp | Should Not BeNullOrEmpty
    }

    It "detects removed errors" {
        $output   = @(
            "[IL]: Error [X]: : Foo::A()][offset 0x00000011] Msg."
        )
        $baseline = @(
            "[IL]: Error [X]: : Foo::A()][offset 0x00000011] Msg.",
            "[IL]: Error [X]: : Foo::B()][offset 0x00000022] Msg."
        )
        $cmp = Compare-Object (Remove-IlverifyOffsets $output) (Remove-IlverifyOffsets $baseline)
        $cmp | Should Not BeNullOrEmpty
    }

    It "handles trailing whitespace differences between output and baseline" {
        $output   = @("[IL]: Error [X]: : Foo::A()][offset 0x00000011] Msg.   ")
        $baseline = @("[IL]: Error [X]: : Foo::A()][offset 0x000000FF] Msg.")
        $cmp = Compare-Object (Remove-IlverifyOffsets $output) (Remove-IlverifyOffsets $baseline)
        $cmp | Should BeNullOrEmpty
    }
}
