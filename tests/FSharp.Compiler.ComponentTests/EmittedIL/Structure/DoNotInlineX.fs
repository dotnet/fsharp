// #Regression #NoMT #CodeGen #Attributes

open System
open System.Diagnostics
open System.IO

let longtime = int(System.TimeSpan.FromSeconds(30.0).TotalMilliseconds)               // longtime is 30 seconds

let programFiles = 
    let pf86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)")
    if String.IsNullOrEmpty(pf86) then Environment.GetEnvironmentVariable("ProgramFiles") else pf86
let fsc =
    let overridePath = Environment.GetEnvironmentVariable("FSC")
    if not (String.IsNullOrEmpty(overridePath)) then 
        overridePath 
    else
        let fsc41_SxS = Path.Combine(programFiles, @"Microsoft SDKs\F#\4.1\Framework\v4.0\fsc.exe")
        let fsc40_SxS = Path.Combine(programFiles, @"Microsoft SDKs\F#\4.0\Framework\v4.0\fsc.exe")
        let fsc40 = Path.Combine(programFiles, @"Microsoft F#\v4.0\fsc.exe")
        let fsc20 = Path.Combine(programFiles, @"FSharp-2.0.0.0\bin\fsc.exe")

        match ([fsc41_SxS; fsc40_SxS; fsc40; fsc20] |> List.tryFind(fun x -> File.Exists(x))) with
        | Some(path) -> path
        | None -> "fsc.exe"  // just use what's on the PATH


let start (p1 : string) = Process.Start(p1)

let CompileFile file args = 
    let p = Process.Start(fsc, file + " " + args)
    p.WaitForExit()

[<EntryPoint>]
let main (args : string[]) =
    if args.Length = 0 then 0 else
        let baseFlag, derivedFlag, expectedResult1, expectedResult2 = args.[0], args.[1], int args.[2], int args.[3]
        let result = <|
            try
                CompileFile "BaseType.fs" ("-a --define:" + baseFlag)
                printfn "Compiled BaseType with %A" baseFlag
                CompileFile "DerivedType.fs" ("-r:BaseType.dll --define:" + derivedFlag)
                printfn "Compiled DerivedType with %A" derivedFlag

                let r1 = start "DerivedType.exe"
                r1.WaitForExit(longtime)
                printfn "Ran DerivedType.exe with result: %A" r1.ExitCode

                CompileFile "BaseType.fs" "-a"
                printfn "Compiled BaseType without %A" baseFlag
                let r2 = start "DerivedType.exe"
                r2.WaitForExit(longtime)
                printfn "Ran DerivedType.exe with result: %A" r2.ExitCode

                if r1.ExitCode = expectedResult1 && r2.ExitCode = expectedResult2 then 0 else 1
            with
                _ -> 1
raise (new Exception($"exit {result}"))
