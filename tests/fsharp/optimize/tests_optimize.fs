module ``FSharp-Tests-Optimize``

open System
open System.IO
open NUnit.Framework
open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes

[<Test>]
let functionSizes () = check (attempt {
    let cfg = testConfig "optimize/analyses"

    let outFile = "sizes.FunctionSizes.output.test.txt"
    let expectedFile = "sizes.FunctionSizes.output.test.bsl"

    log "== FunctionSizes"
    do! fscBothToOut cfg outFile "%s --nologo -O --test:FunctionSizes" cfg.fsc_flags ["sizes.fs"] 

    let! diff = fsdiff cfg outFile expectedFile

    do! match diff with
        | [] -> Success
        | _ ->
            NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile) (getfullpath cfg expectedFile) diff)
    })

[<Test>]
let totalSizes () = check (attempt {
    let cfg = testConfig "optimize/analyses"

    let outFile = "sizes.TotalSizes.output.test.txt"
    let expectedFile = "sizes.TotalSizes.output.test.bsl"

    log "== TotalSizes"
    do! fscBothToOut cfg outFile "%s --nologo -O --test:TotalSizes" cfg.fsc_flags ["sizes.fs"] 

    let! diff = fsdiff cfg outFile expectedFile

    do! match diff with
        | [] -> Success
        | _ ->
            NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile) (getfullpath cfg expectedFile) diff)
    })

[<Test>]
let hasEffect () = check (attempt {
    let cfg = testConfig "optimize/analyses"

    let outFile = "effects.HasEffect.output.test.txt"
    let expectedFile = "effects.HasEffect.output.test.bsl"

    log "== HasEffect"
    do! fscBothToOut cfg outFile "%s --nologo -O --test:HasEffect" cfg.fsc_flags ["effects.fs"] 

    let! diff = fsdiff cfg outFile expectedFile

    do! match diff with
        | [] -> Success
        | _ ->
            NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile) (getfullpath cfg expectedFile) diff)
    })

[<Test>]
let noNeedToTailcall () = check (attempt {
    let cfg = testConfig "optimize/analyses"

    let outFile = "tailcalls.NoNeedToTailcall.output.test.txt"
    let expectedFile = "tailcalls.NoNeedToTailcall.output.test.bsl"

    log "== NoNeedToTailcall"
    do! fscBothToOut cfg outFile "%s --nologo -O --test:NoNeedToTailcall" cfg.fsc_flags ["tailcalls.fs"] 

    let! diff = fsdiff cfg outFile expectedFile

    do! match diff with
        | [] -> Success
        | _ ->
            NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile) (getfullpath cfg expectedFile) diff)
    })




[<Test>]
let ``inline`` () = check (attempt {
    let cfg = testConfig "optimize/inline"

    do! fsc cfg "%s -g --optimize- --target:library -o:lib.dll" cfg.fsc_flags ["lib.fs"; "lib2.fs"]

    do! fsc cfg "%s -g --optimize- --target:library -o:lib3.dll -r:lib.dll " cfg.fsc_flags ["lib3.fs"]

    do! fsc cfg "%s -g --optimize- -o:test.exe -r:lib.dll -r:lib3.dll" cfg.fsc_flags ["test.fs "]

    do! fsc cfg "%s --optimize --target:library -o:lib--optimize.dll -g" cfg.fsc_flags ["lib.fs"; "lib2.fs"]

    do! fsc cfg "%s --optimize --target:library -o:lib3--optimize.dll -r:lib--optimize.dll -g" cfg.fsc_flags ["lib3.fs"]

    do! fsc cfg "%s --optimize -o:test--optimize.exe -g -r:lib--optimize.dll  -r:lib3--optimize.dll" cfg.fsc_flags ["test.fs "]

    do! ildasm cfg "/nobar /out=test.il" "test.exe"

    do! ildasm cfg "/nobar /out=test--optimize.il" "test--optimize.exe"

    let ``test--optimize.il`` = 
        File.ReadLines (getfullpath cfg "test--optimize.il")
        |> Seq.filter (fun line -> line.Contains(".locals init"))
        |> List.ofSeq

    do! match ``test--optimize.il`` with
        | [] -> Success
        | lines -> 
            NUnitConf.genericError (sprintf "Error: optimizations not removed.  Relevant lines from IL file follow: %A" lines)

    let numElim = 
        File.ReadLines (getfullpath cfg "test.il")
        |> Seq.filter (fun line -> line.Contains(".locals init"))
        |> Seq.length

    log "Ran ok - optimizations removed %d textual occurrences of optimizable identifiers from target IL" numElim 
                
    })

[<Test>]
let stats () = check (attempt {
    let cfg = testConfig "optimize/stats"

    do! ildasm cfg "/nobar /out=FSharp.Core.il" cfg.FSCOREDLLPATH

    let fscore = File.ReadLines(getfullpath cfg "FSharp.Core.il") |> Seq.toList

    let contains text (s: string) = if s.Contains(text) then 1 else 0

    let typeFunc = fscore |> List.sumBy (contains "extends Microsoft.FSharp.TypeFunc")
    let classes = fscore |> List.sumBy (contains ".class")
    let methods = fscore |> List.sumBy (contains ".method")
    let fields = fscore |> List.sumBy (contains ".field")

    let date = DateTime.Today.ToString("dd/MM/yyyy") // 23/11/2006
    let time = DateTime.Now.ToString("HH:mm:ss.ff") // 16:03:23.40
    let m = sprintf "%s, %s, Microsoft.FSharp-TypeFunc, %d, Microsoft.FSharp-classes, %d,  Microsoft.FSharp-methods, %d, ,  Microsoft.FSharp-fields, %d,  " date time typeFunc classes methods fields

    log "now:"
    log "%s" m
                
    })
