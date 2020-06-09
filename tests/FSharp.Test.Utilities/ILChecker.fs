// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test.Utilities

open System
open System.IO
open System.Diagnostics

open NUnit.Framework
open TestFramework

[<RequireQualifiedAccess>]
module ILChecker =

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

    let private checkILAux ildasmArgs dllFilePath expectedIL =
        let ilFilePath = Path.ChangeExtension(dllFilePath, ".il")

        let mutable errorMsgOpt = None
        try
            let ildasmPath = config.ILDASM

            exec ildasmPath (ildasmArgs @ [ sprintf "%s /out=%s" dllFilePath ilFilePath ]) |> ignore

            let unifyRuntimeAssemblyName ilCode =
                System.Text.RegularExpressions.Regex.Replace(ilCode,
                    "\[System.Runtime\]|\[System.Runtime.Extensions\]|\[mscorlib\]","[runtime]",
                    System.Text.RegularExpressions.RegexOptions.Singleline)

            let text =
                let raw = File.ReadAllText(ilFilePath)
                let asmName = Path.GetFileNameWithoutExtension(dllFilePath)
                raw.Replace(asmName, "assembly")
                |> unifyRuntimeAssemblyName
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
            |> List.map (fun (ilCode: string) -> ilCode.Trim() |> unifyRuntimeAssemblyName )
            |> List.iter (fun (ilCode: string) ->
                let expectedLines = ilCode.Split('\n')
                let startIndex = textNoComments.IndexOf(expectedLines.[0].Trim())
                if startIndex = -1 then
                    errorMsgOpt <- Some("==EXPECTED CONTAINS==\n" + ilCode + "\n")
                else
                    let errors = ResizeArray()
                    let actualLines = textNoComments.Substring(startIndex, textNoComments.Length - startIndex).Split('\n')
                    if actualLines.Length < expectedLines.Length then
                        let msg = sprintf "==EXPECTED AT LEAST %d LINES BUT FOUND ONLY %d ==\n" expectedLines.Length actualLines.Length
                        errorMsgOpt <- Some(msg + "==EXPECTED CONTAINS==\n" + ilCode + "\n")
                    else
                        for i = 0 to expectedLines.Length - 1 do
                            let expected = expectedLines.[i].Trim()
                            let actual = actualLines.[i].Trim()
                            if expected <> actual then
                                errors.Add(sprintf "\n==\nName: '%s'\n\nExpected:\t %s\nActual:\t\t %s\n==" actualLines.[0] expected actual)

                        if errors.Count > 0 then
                            let msg = String.concat "\n" errors + "\n\n\n==EXPECTED==\n" + ilCode + "\n"
                            errorMsgOpt <- Some(msg + "\n\n\n==ACTUAL==\n" + String.Join("\n", actualLines, 0, expectedLines.Length))
            )

            if expectedIL.Length = 0 then
                errorMsgOpt <- Some ("No Expected IL")

            match errorMsgOpt with
            | Some(msg) -> errorMsgOpt <- Some(msg + "\n\n\n==ENTIRE ACTUAL==\n" + textNoComments)
            | _ -> ()
        finally
            try File.Delete(ilFilePath) with | _ -> ()

            match errorMsgOpt with
            | Some(errorMsg) ->
                Assert.Fail(errorMsg)
            | _ -> ()

    let checkILItem item dllFilePath expectedIL =
        checkILAux [ sprintf "/item:%s" item ] dllFilePath expectedIL

    let checkILItemWithLineNumbers item dllFilePath expectedIL =
        checkILAux [ sprintf "/item:\"%s\"" item; "/linenum" ] dllFilePath expectedIL

    let checkIL dllFilePath expectedIL =
        checkILAux [] dllFilePath expectedIL

    let reassembleIL ilFilePath dllFilePath =
        let ilasmPath = config.ILASM
        let errors, _ = exec ilasmPath ([ sprintf "%s /output=%s /dll" ilFilePath dllFilePath ])
        errors
