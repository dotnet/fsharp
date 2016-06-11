module ``FSharpQA-Tests-CompilerOptions-fsi``

open NUnit.Framework

open NUnitConf
open PlatformHelpers
open RunPlTest


module arguments =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/arguments")>]
    let arguments () = runpl |> check


module exename =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/exename")>]
    let exename () = runpl |> check


module help =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/help")>]
    let help () = check(attempt {

        let ``\$FSI_PIPE >{file} {args} 2>&1`` fileName args workDir (cfg: RunPl.RunPlConfig) = attempt {
            let ``exec >a 2>&1`` p = 
                Command.exec workDir cfg.envVars { Output = OutputAndError(Overwrite(fileName)); Input = None } p 
                >> checkResult

            do! Commands.fsi ``exec >a 2>&1`` cfg.FSI_PIPE args []
            }

        do! [ @"\$FSI_PIPE >help.txt -?     2>&1",          ``\$FSI_PIPE >{file} {args} 2>&1`` "help.txt" "-?"
              @"\$FSI_PIPE >help.txt --help 2>&1",          ``\$FSI_PIPE >{file} {args} 2>&1`` "help.txt" "--help"
              @"\$FSI_PIPE >help.txt /?     2>&1",          ``\$FSI_PIPE >{file} {args} 2>&1`` "help.txt" "/?"
              @"\$FSI_PIPE >help.txt --nologo -?     2>&1", ``\$FSI_PIPE >{file} {args} 2>&1`` "help.txt" "--nologo -?" ]
            |> Map.ofList
            |> runplWithCmds

        }) 


module highentropyva =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/highentropyva")>]
    let highentropyva () = runpl |> check


module nologo =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/nologo")>]
    let nologo () = runpl |> check


module subsystemversion =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/subsystemversion")>]
    let subsystemversion () = runpl |> check


module times =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/times")>]
    let times () = runpl |> check
