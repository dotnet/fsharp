// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open System.IO
open System.Diagnostics

open NUnit.Framework

open FSharp.Compiler.SourceCodeServices

open TestFramework

[<RequireQualifiedAccess>]
module ILChecker =

    let checker = CompilerAssert.checker

    let config = initializeSuite ()

    let private exec exe args =
        let startInfo = ProcessStartInfo(exe, String.concat " " args)
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false
        use p = Process.Start(startInfo)
        p.WaitForExit()
        p.StandardError.ReadToEnd(), p.ExitCode

    /// Filters i.e ['The system type \'System.ReadOnlySpan`1\' was required but no referenced system DLL contained this type']
    let private filterSpecialComment (text: string) =
        let pattern = @"(\[\'(.*?)\'\])"
        System.Text.RegularExpressions.Regex.Replace(text, pattern,
            (fun me -> String.Empty)
        )

    let private checkAux extraDlls source expectedIL =
        let tmp = Path.GetTempFileName()
        let tmpFs = Path.ChangeExtension(tmp, ".fs")
        let tmpDll = Path.ChangeExtension(tmp, ".dll")
        let tmpIL = Path.ChangeExtension(tmp, ".il")

        let mutable errorMsgOpt = None
        try
            let ildasmPath = config.ILDASM

            File.WriteAllText(tmpFs, source)

            let extraReferences = extraDlls |> Array.ofList |> Array.map (fun reference -> "-r:" + reference)

#if NETCOREAPP
            // Hack: Currently a hack to get the runtime assemblies for netcore in order to compile.
            let runtimeAssemblies =
                typeof<obj>.Assembly.Location
                |> Path.GetDirectoryName
                |> Directory.EnumerateFiles
                |> Seq.toArray
                |> Array.filter (fun x -> x.ToLowerInvariant().Contains("system."))
                |> Array.map (fun x -> sprintf "-r:%s" x)

            let extraReferences = Array.append runtimeAssemblies extraReferences

            let errors, exitCode = checker.Compile(Array.append [| "fsc.exe"; "--optimize+"; "-o"; tmpDll; "-a"; tmpFs; "--targetprofile:netcore"; "--noframework" |] extraReferences) |> Async.RunSynchronously
#else
            let errors, exitCode = checker.Compile(Array.append [| "fsc.exe"; "--optimize+"; "-o"; tmpDll; "-a"; tmpFs |] extraReferences) |> Async.RunSynchronously
#endif
            let errors =
                String.concat "\n" (errors |> Array.map (fun x -> x.Message))

            if exitCode = 0 then
                exec ildasmPath [ sprintf "%s /out=%s" tmpDll tmpIL ] |> ignore

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
                    |> filterSpecialComment
                
                expectedIL
                |> List.iter (fun (ilCode: string) ->
                    let expectedLines = ilCode.Split('\n')
                    let startIndex = textNoComments.IndexOf(expectedLines.[0])
                    if startIndex = -1 || textNoComments.Length < startIndex + ilCode.Length then
                        errorMsgOpt <- Some("==EXPECTED CONTAINS==\n" + ilCode + "\n")
                    else
                        let errors = ResizeArray()
                        let actualLines = textNoComments.Substring(startIndex, textNoComments.Length - startIndex).Split('\n')
                        for i = 0 to expectedLines.Length - 1 do
                            let expected = expectedLines.[i].Trim()
                            let actual = actualLines.[i].Trim()
                            if expected <> actual then
                                errors.Add(sprintf "\n==\nName: %s\n\nExpected:\t %s\nActual:\t\t %s\n==" actualLines.[0] expected actual)

                        if errors.Count > 0 then
                            let msg = String.concat "\n" errors + "\n\n\n==EXPECTED==\n" + ilCode + "\n"
                            errorMsgOpt <- Some(msg + "\n\n\n==ACTUAL==\n" + String.Join("\n", actualLines, 0, expectedLines.Length))
                )

                if expectedIL.Length = 0 then
                    errorMsgOpt <- Some ("No Expected IL")

                match errorMsgOpt with
                | Some(msg) -> errorMsgOpt <- Some(msg + "\n\n\n==ENTIRE ACTUAL==\n" + textNoComments)
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

    let getPackageDlls name version framework dllNames =
        dllNames
        |> List.map (fun dllName ->
            requireFile (packagesDir ++ name ++ version ++ "lib" ++ framework ++ dllName)
        )
            
    /// Compile the source and check to see if the expected IL exists.
    /// The first line of each expected IL string is found first.
    let check source expectedIL =
        checkAux [] source expectedIL

    let checkWithDlls extraDlls source expectedIL =
        checkAux extraDlls source expectedIL

