module ``FSharp-Tests-Optimize``

open System
open System.IO
open NUnit.Framework

open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes

let testContext = FSharpTestSuite.testContext

module Analyses = 

    let ``fsc >a 2>&1`` cfg dir = 
        let ``exec >a 2>&1`` outFile p = 
            Command.exec dir cfg.EnvironmentVariables { Output = OutputAndErrorToSameFile(Overwrite(outFile)); Input = None; } p 
            >> checkResult
        Printf.ksprintf (fun flags sources out -> Commands.fsc (``exec >a 2>&1`` out) cfg.FSC flags sources)

    let fsdiff cfg dir a b = attempt {
        let out = new ResizeArray<string>()
        let redirectOutputToFile path args =
            log "%s %s" path args
            use toLog = redirectToLog ()
            Process.exec { RedirectOutput = Some (function null -> () | s -> out.Add(s)); RedirectError = Some toLog.Post; RedirectInput = None; } dir cfg.EnvironmentVariables path args
        do! (Commands.fsdiff redirectOutputToFile cfg.FSDIFF a b) |> (fun _ -> Success ())
        return out.ToArray() |> List.ofArray
        }

    [<Test; FSharpSuiteTest("optimize/analyses")>]
    let functionSizes () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let getfullpath = Commands.getfullpath dir
        let ``fsc >a 2>&1`` = ``fsc >a 2>&1`` cfg dir  
        let fsdiff = fsdiff cfg dir

        let outFile = "sizes.FunctionSizes.output.test.txt"
        let expectedFile = "sizes.FunctionSizes.output.test.bsl"

        // echo == FunctionSizes
        log "== FunctionSizes"
        // "%FSC%" %fsc_flags% --nologo -O --test:FunctionSizes sizes.fs >sizes.FunctionSizes.output.test.txt 2>&1
        do! ``fsc >a 2>&1`` "%s --nologo -O --test:FunctionSizes" cfg.fsc_flags ["sizes.fs"] outFile
        // if NOT EXIST sizes.FunctionSizes.output.test.bsl COPY sizes.FunctionSizes.output.test.txt sizes.FunctionSizes.output.test.bsl
        ignore "used only the first time when no expected output file exists, useless"
        // %FSDIFF% sizes.FunctionSizes.output.test.txt sizes.FunctionSizes.output.test.bsl > sizes.FunctionSizes.output.test.diff
        let! diff = fsdiff outFile expectedFile

        do! match diff with
            | [] -> Success
            | l ->
                NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath outFile) (getfullpath expectedFile) diff)
        })

    [<Test; FSharpSuiteTest("optimize/analyses")>]
    let totalSizes () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let ``fsc >a 2>&1`` = ``fsc >a 2>&1`` cfg dir  
        let fsdiff = fsdiff cfg dir
        let getfullpath = Commands.getfullpath dir

        let outFile = "sizes.TotalSizes.output.test.txt"
        let expectedFile = "sizes.TotalSizes.output.test.bsl"

        // echo == TotalSizes
        log "== TotalSizes"
        // "%FSC%" %fsc_flags% --nologo -O --test:TotalSizes sizes.fs >sizes.TotalSizes.output.test.txt 2>&1
        do! ``fsc >a 2>&1`` "%s --nologo -O --test:TotalSizes" cfg.fsc_flags ["sizes.fs"] outFile
        // if NOT EXIST sizes.TotalSizes.output.test.bsl COPY sizes.TotalSizes.output.test.txt sizes.TotalSizes.output.test.bsl
        ignore "used only the first time when no expected output file exists, useless"
        // %FSDIFF% sizes.TotalSizes.output.test.txt sizes.TotalSizes.output.test.bsl > sizes.TotalSizes.output.test.diff
        let! diff = fsdiff outFile expectedFile

        do! match diff with
            | [] -> Success
            | l ->
                NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath outFile) (getfullpath expectedFile) diff)
        })

    [<Test; FSharpSuiteTest("optimize/analyses")>]
    let hasEffect () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let ``fsc >a 2>&1`` = ``fsc >a 2>&1`` cfg dir  
        let fsdiff = fsdiff cfg dir
        let getfullpath = Commands.getfullpath dir

        let outFile = "effects.HasEffect.output.test.txt"
        let expectedFile = "effects.HasEffect.output.test.bsl"

        // echo == HasEffect
        log "== HasEffect"
        // "%FSC%" %fsc_flags% --nologo -O --test:HasEffect effects.fs >effects.HasEffect.output.test.txt 2>&1
        do! ``fsc >a 2>&1`` "%s --nologo -O --test:HasEffect" cfg.fsc_flags ["effects.fs"] outFile
        // if NOT EXIST effects.HasEffect.output.test.bsl COPY effects.HasEffect.output.test.txt effects.HasEffect.output.test.bsl
        ignore "used only the first time when no expected output file exists, useless"
        // %FSDIFF% effects.HasEffect.output.test.txt effects.HasEffect.output.test.bsl > effects.HasEffect.output.test.diff
        let! diff = fsdiff outFile expectedFile

        do! match diff with
            | [] -> Success
            | l ->
                NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath outFile) (getfullpath expectedFile) diff)
        })

    [<Test; FSharpSuiteTest("optimize/analyses")>]
    let noNeedToTailcall () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let ``fsc >a 2>&1`` = ``fsc >a 2>&1`` cfg dir  
        let fsdiff = fsdiff cfg dir
        let getfullpath = Commands.getfullpath dir

        let outFile = "tailcalls.NoNeedToTailcall.output.test.txt"
        let expectedFile = "tailcalls.NoNeedToTailcall.output.test.bsl"

        // echo == NoNeedToTailcall
        log "== NoNeedToTailcall"
        // "%FSC%" %fsc_flags% --nologo -O --test:NoNeedToTailcall tailcalls.fs >tailcalls.NoNeedToTailcall.output.test.txt 2>&1
        do! ``fsc >a 2>&1`` "%s --nologo -O --test:NoNeedToTailcall" cfg.fsc_flags ["tailcalls.fs"] outFile
        // if NOT EXIST tailcalls.NoNeedToTailcall.output.test.bsl COPY tailcalls.NoNeedToTailcall.output.test.txt tailcalls.NoNeedToTailcall.output.test.bsl
        ignore "used only the first time when no expected output file exists, useless"
        // %FSDIFF% tailcalls.NoNeedToTailcall.output.test.txt tailcalls.NoNeedToTailcall.output.test.bsl > tailcalls.NoNeedToTailcall.output.test.diff
        let! diff = fsdiff outFile expectedFile

        do! match diff with
            | [] -> Success
            | l ->
                NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath outFile) (getfullpath expectedFile) diff)
        })




module Inline = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)


        // "%FSC%" %fsc_flags% -g --optimize- --target:library -o:lib.dll lib.fs lib2.fs
        do! fsc "%s -g --optimize- --target:library -o:lib.dll" cfg.fsc_flags ["lib.fs"; "lib2.fs"]

        // "%FSC%" %fsc_flags% -g --optimize- --target:library -o:lib3.dll -r:lib.dll lib3.fs
        do! fsc "%s -g --optimize- --target:library -o:lib3.dll -r:lib.dll " cfg.fsc_flags ["lib3.fs"]

        // "%FSC%" %fsc_flags% -g --optimize- -o:test.exe test.fs -r:lib.dll -r:lib3.dll
        do! fsc "%s -g --optimize- -o:test.exe -r:lib.dll -r:lib3.dll" cfg.fsc_flags ["test.fs "]

        // "%FSC%" %fsc_flags% --optimize --target:library -o:lib--optimize.dll -g lib.fs  lib2.fs
        do! fsc "%s --optimize --target:library -o:lib--optimize.dll -g" cfg.fsc_flags ["lib.fs"; "lib2.fs"]

        // "%FSC%" %fsc_flags% --optimize --target:library -o:lib3--optimize.dll -r:lib--optimize.dll -g lib3.fs  
        do! fsc "%s --optimize --target:library -o:lib3--optimize.dll -r:lib--optimize.dll -g" cfg.fsc_flags ["lib3.fs"]

        // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fs -r:lib--optimize.dll  -r:lib3--optimize.dll
        do! fsc "%s --optimize -o:test--optimize.exe -g -r:lib--optimize.dll  -r:lib3--optimize.dll" cfg.fsc_flags ["test.fs "]

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let ildasm = Printf.ksprintf (Commands.ildasm exec cfg.ILDASM)
        let getfullpath = Commands.getfullpath dir

        // if not exist "%ILDASM%" (
        //    @echo '%ILDASM%' not found.
        //     goto Error 
        // )
        ignore "already checked at suite startup"

        // "%ILDASM%" /nobar /out=test.il test.exe
        do! ildasm "/nobar /out=test.il" "test.exe"

        // "%ILDASM%" /nobar /out=test--optimize.il test--optimize.exe
        do! ildasm "/nobar /out=test--optimize.il" "test--optimize.exe"

        // type test--optimize.il | find /C ".locals init" > count--optimize
        let ``test--optimize.il`` = 
            File.ReadLines (getfullpath "test--optimize.il")
            |> Seq.filter (fun line -> line.Contains(".locals init"))
            |> List.ofSeq

        // for /f %%c IN (count--optimize) do (if NOT "%%c"=="0" (
        do! match ``test--optimize.il`` with
            | [] -> Success
            | lines -> 
                // echo Error: optimizations not removed.  Relevant lines from IL file follow:
                // type test--optimize.il | find ".locals init"
                // goto SetError)
                NUnitConf.genericError (sprintf "Error: optimizations not removed.  Relevant lines from IL file follow: %A" lines)
        // )

        // type test.il | find /C ".locals init" > count
        // for /f %%c IN (count) do (
        //    set NUMELIM=%%c
        // )
        let numElim = 
            File.ReadLines (getfullpath "test.il")
            |> Seq.filter (fun line -> line.Contains(".locals init"))
            |> Seq.length

        log "Ran ok - optimizations removed %d textual occurrences of optimizable identifiers from target IL" numElim 

        }

    [<Test; FSharpSuiteTest("optimize/inline")>]
    let ``inline`` () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })

module Stats = 

    [<Test; FSharpSuiteTest("optimize/stats")>]
    let stats () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let ildasm = Commands.ildasm exec cfg.ILDASM
        let getfullpath = Commands.getfullpath dir

        // if not exist "%ILDASM%" (goto Error)
        ignore "already checked at suite startup"

        // where sd.exe 2> NUL
        // if not ERRORLEVEL 1 ( sd edit stats.txt ) else (attrib -r stats.txt )
        ignore "dont know"

        // "%ILDASM%" /nobar /out=FSharp.Core.il "%FSCOREDLLPATH%"
        do! ildasm "/nobar /out=FSharp.Core.il" cfg.FSCOREDLLPATH

        let ``FSharp.Core.il`` = File.ReadLines(getfullpath "FSharp.Core.il") |> Seq.toList

        let contains text (s: string) = if s.Contains(text) then 1 else 0

        // echo Counting TypeFuncs...
        // type FSharp.Core.il | find /C "extends Microsoft.FSharp.TypeFunc"        > count-Microsoft.FSharp-TypeFunc
        let typeFunc = ``FSharp.Core.il`` |> List.sumBy (contains "extends Microsoft.FSharp.TypeFunc")
        // echo Counting classes...
        // type FSharp.Core.il | find /C ".class"                                   > count-Microsoft.FSharp-.class
        let classes = ``FSharp.Core.il`` |> List.sumBy (contains ".class")
        // echo Counting methods...
        // type FSharp.Core.il | find /C ".method"                                  > count-Microsoft.FSharp-.method
        let methods = ``FSharp.Core.il`` |> List.sumBy (contains ".method")
        // echo Counting fields...
        // type FSharp.Core.il | find /C ".field"                                   > count-Microsoft.FSharp-.field
        let fields = ``FSharp.Core.il`` |> List.sumBy (contains ".field")

        // for /f %%c IN (count-Microsoft.FSharp-TypeFunc) do (
        //  for /f %%d IN (count-Microsoft.FSharp-.class) do (
        //   for /f %%e IN (count-Microsoft.FSharp-.method) do (
        //    for /f %%f IN (count-Microsoft.FSharp-.field) do (
        //          echo %date%, %time%, Microsoft.FSharp-TypeFunc, %%c, Microsoft.FSharp-classes, %%d,  Microsoft.FSharp-methods, %%e, ,  Microsoft.FSharp-fields, %%f,  >> stats.txt
        let date = DateTime.Today.ToString("dd/MM/yyyy") // 23/11/2006
        let time = DateTime.Now.ToString("HH:mm:ss.ff") // 16:03:23.40
        let m = sprintf "%s, %s, Microsoft.FSharp-TypeFunc, %d, Microsoft.FSharp-classes, %d,  Microsoft.FSharp-methods, %d, ,  Microsoft.FSharp-fields, %d,  " date time typeFunc classes methods fields

        log "now:"
        log "%s" m

        //    )
        //   )
        //  )
        // )
                
        })
