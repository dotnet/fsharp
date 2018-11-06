﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open System.IO
open System.Diagnostics

open NUnit.Framework

module ILChecker =

    let private (++) a b = Path.Combine(a,b)

    let private getfullpath workDir path =
        let rooted =
            if Path.IsPathRooted(path) then path
            else Path.Combine(workDir, path)
        rooted |> Path.GetFullPath

    let private fileExists workDir path = 
        if path |> getfullpath workDir |> File.Exists then Some path else None

    let private requireFile nm = 
        if fileExists __SOURCE_DIRECTORY__ nm |> Option.isSome then nm else failwith (sprintf "couldn't find %s. Running 'build test' once might solve this issue" nm)

    let exec exe args =
        let startInfo = ProcessStartInfo(exe, String.concat " " args)
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false
        use p = Process.Start(startInfo)
        p.WaitForExit()
        p.StandardError.ReadToEnd(), p.ExitCode

    let checkContains sourceCode expectedILCode =
        let SCRIPT_ROOT = __SOURCE_DIRECTORY__
        let packagesDir = SCRIPT_ROOT ++ ".." ++ ".." ++ "packages"
#if DEBUG
        let configurationName = "debug"
#else
        let configurationName = "release"
#endif
        let fscBin = SCRIPT_ROOT ++ ".." ++ ".." ++ configurationName ++ "net40" ++ "bin"
        let fscExe = Path.Combine(fscBin, "fsc.exe")
        let Is64BitOperatingSystem = sizeof<nativeint> = 8
        let architectureMoniker = if Is64BitOperatingSystem then "x64" else "x86"
        let ildasmExe = requireFile (packagesDir ++ ("runtime.win-" + architectureMoniker + ".Microsoft.NETCore.ILDAsm.2.0.3") ++ "runtimes" ++ ("win-" + architectureMoniker) ++ "native" ++ "ildasm.exe")
        let coreclrDll = requireFile (packagesDir ++ ("runtime.win-" + architectureMoniker + ".Microsoft.NETCore.Runtime.CoreCLR.2.0.3") ++ "runtimes" ++ ("win-" + architectureMoniker) ++ "native" ++ "coreclr.dll")

        let tmp = Path.GetTempFileName()
        let tmpFs = Path.ChangeExtension(tmp, ".fs")
        let tmpDll = Path.ChangeExtension(tmp, ".dll")
        let tmpIL = Path.ChangeExtension(tmp, ".il")

        let mutable errorMsgOpt = None
        try
            // ildasm requires coreclr.dll to run which has already been restored to the packages directory
            File.Copy(coreclrDll, Path.GetDirectoryName(ildasmExe) ++ "coreclr.dll", overwrite=true)

            File.WriteAllText(tmpFs, sourceCode)

            let errors, exitCode = exec fscExe [ "--optimize+"; "-o"; tmpDll; "-a"; tmpFs ]

            if exitCode = 0 then
                exec ildasmExe [ sprintf "%s /out=%s" tmpDll tmpIL ] |> ignore

                let text = File.ReadAllText(tmpIL)
                let blockComments = @"/\*(.*?)\*/"
                let lineComments = @"//(.*?)\r?\n"
                let strings = @"""((\\[^\n]|[^""\n])*)"""
                let verbatimStrings = @"@(""[^""]*"")+"
                let textNoComments =
                    System.Text.RegularExpressions.Regex.Replace(text, 
                        blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings, 
                        (fun me -> 
                            if (me.Value.StartsWith("/*") || me.Value.StartsWith("//")) then
                                if me.Value.StartsWith("//") then Environment.NewLine else String.Empty
                            else
                                me.Value), System.Text.RegularExpressions.RegexOptions.Singleline)
                
                expectedILCode
                |> List.iter (fun ilCode ->
                    if String.IsNullOrWhiteSpace(ilCode) || not (textNoComments.Contains(ilCode)) then
                        errorMsgOpt <- Some("==EXPECTED CONTAINS==\n" + ilCode + "\n")
                )

                match errorMsgOpt with
                | Some(msg) -> errorMsgOpt <- Some(msg + "\n\n\n==ACTUAL==\n" + textNoComments)
                | _ -> ()
            else
                errorMsgOpt <- Some(errors)
        finally
            try File.Delete(tmp) with | _ -> ()
            try File.Delete(tmpFs) with | _ -> ()
            try File.Delete(tmpDll) with | _ -> ()
            try File.Delete(tmpIL) with | _ -> ()

            match errorMsgOpt with
            | Some(errorMsg) -> 
                Assert.Fail(errorMsg)
            | _ -> ()
            
