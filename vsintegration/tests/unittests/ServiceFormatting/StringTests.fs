// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.StringTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``triple-quoted strings``() =
    formatSourceString false "let xmlFragment2 = \"\"\"<book author=\"Milton, John\" title=\"Paradise Lost\">\"\"\"" config
    |> should equal "let xmlFragment2 = \"\"\"<book author=\"Milton, John\" title=\"Paradise Lost\">\"\"\"
"

[<Test>]
let ``string literals``() =
    formatSourceString false """
let xmlFragment1 = @"<book author=""Milton, John"" title=""Paradise Lost"">"
let str1 = "abc"
    """ config 
    |> prepend newline
    |> should equal """
let xmlFragment1 = @"<book author=""Milton, John"" title=""Paradise Lost"">"
let str1 = "abc"
"""

[<Test>]
let ``multiline strings``() =
    formatSourceString false """
let alu =
        "GGCCGGGCGCGGTGGCTCACGCCTGTAATCCCAGCACTTTGG\
        GAGGCCGAGGCGGGCGGATCACCTGAGGTCAGGAGTTCGAGA\
        CCAGCCTGGCCAACATGGTGAAACCCCGTCTCTACTAAAAAT\
        ACAAAAATTAGCCGGGCGTGGTGGCGCGCGCCTGTAATCCCA\
        GCTACTCGGGAGGCTGAGGCAGGAGAATCGCTTGAACCCGGG\
        AGGCGGAGGTTGCAGTGAGCCGAGATCGCGCCACTGCACTCC\
  AGCCTGGGCGACAGAGCGAGACTCCGTCTCAAAAA"B
    """ config
    |> prepend newline
    |> should equal """
let alu = "GGCCGGGCGCGGTGGCTCACGCCTGTAATCCCAGCACTTTGG\
        GAGGCCGAGGCGGGCGGATCACCTGAGGTCAGGAGTTCGAGA\
        CCAGCCTGGCCAACATGGTGAAACCCCGTCTCTACTAAAAAT\
        ACAAAAATTAGCCGGGCGTGGTGGCGCGCGCCTGTAATCCCA\
        GCTACTCGGGAGGCTGAGGCAGGAGAATCGCTTGAACCCGGG\
        AGGCGGAGGTTGCAGTGAGCCGAGATCGCGCCACTGCACTCC\
  AGCCTGGGCGACAGAGCGAGACTCCGTCTCAAAAA"B
"""

[<Test>]
let ``preserve uncommon literals``() =
    formatSourceString false """
let a = 0xFFy
let c = 0b0111101us
let d = 0o0777
let e = 1.40e10f
let f = 23.4M
let g = '\n'
    """ config 
    |> prepend newline
    |> should equal """
let a = 0xFFy
let c = 0b0111101us
let d = 0o0777
let e = 1.40e10f
let f = 23.4M
let g = '\n'
"""

[<Test>]
let ``should preserve triple-quote strings``() =
    formatSourceString false "
    type GetList() =
        let switchvox_users_voicemail_getList_response = \"\"\"
            </response>\"\"\"

        let switchvox_users_voicemail_getList = \"\"\"
            </request>\"\"\"

        member self.X = switchvox_users_voicemail_getList_response
"    config 
    |> prepend newline
    |> should equal "
type GetList() =
    let switchvox_users_voicemail_getList_response = \"\"\"
            </response>\"\"\"
    let switchvox_users_voicemail_getList = \"\"\"
            </request>\"\"\"
    member self.X = switchvox_users_voicemail_getList_response
"

[<Test>]
let ``should keep triple-quote strings``() =
    formatSourceString false "
[<EntryPoint>]
let main argv = 
    use fun1 = R.eval(R.parse(text = \"\"\"
    function(i) {
        x <- rnorm(1000)
        y <- rnorm(1000)
        m <- lm(y~x)
        m$coefficients[[2]]
    }
    \"\"\"))
    0
"    config 
    |> prepend newline
    |> should equal "
[<EntryPoint>]
let main argv =
    use fun1 = R.eval (R.parse (text = \"\"\"
    function(i) {
        x <- rnorm(1000)
        y <- rnorm(1000)
        m <- lm(y~x)
        m$coefficients[[2]]
    }
    \"\"\"))
    0
"