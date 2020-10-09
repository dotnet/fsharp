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
        let p = Process.Start(startInfo)

        let errors = p.StandardError.ReadToEnd()

        let timeout = 30000
        let exited = p.WaitForExit(timeout)

        if not exited then
            failwith (sprintf "Process hasn't exited after %d milliseconds" timeout)

        errors, p.ExitCode

    /// Filters i.e ['The system type \'System.ReadOnlySpan`1\' was required but no referenced system DLL contained this type']
    let private filterSpecialComment (text: string) =
        let pattern = @"(\[\'(.*?)\'\])"
        System.Text.RegularExpressions.Regex.Replace(text, pattern,
            (fun me -> String.Empty)
        )

    let private checkILAux' ildasmArgs dllFilePath expectedIL =
        let ilFilePath = Path.ChangeExtension(dllFilePath, ".il")

        let mutable errorMsgOpt = None
        let mutable actualIL = String.Empty
        try
            let ildasmPath = config.ILDASM

            let stdErr, exitCode = exec ildasmPath (ildasmArgs @ [ sprintf "%s -out=%s" dllFilePath ilFilePath ])

            if exitCode <> 0 then
                failwith (sprintf "ILASM Expected exit code \"0\", got \"%d\"\nSTDERR: %s" exitCode stdErr)

            if not (String.IsNullOrWhiteSpace stdErr) then
                failwith (sprintf "ILASM Stderr is not empty:\n %s" stdErr)

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
                    errorMsgOpt <- Some("\nExpected:\n" + ilCode + "\n")
                else
                    let errors = ResizeArray()
                    let actualLines = textNoComments.Substring(startIndex, textNoComments.Length - startIndex).Split('\n')
                    if actualLines.Length < expectedLines.Length then
                        let msg = sprintf "\nExpected at least %d lines but found only %d\n" expectedLines.Length actualLines.Length
                        errorMsgOpt <- Some(msg + "\nExpected:\n" + ilCode + "\n")
                    else
                        for i = 0 to expectedLines.Length - 1 do
                            let expected = expectedLines.[i].Trim()
                            let actual = actualLines.[i].Trim()
                            if expected <> actual then
                                errors.Add(sprintf "\n==\nName: '%s'\n\nExpected:\t %s\nActual:\t\t %s\n==" actualLines.[0] expected actual)

                        if errors.Count > 0 then
                            let msg = String.concat "\n" errors + "\n\n\Expected:\n" + ilCode + "\n"
                            errorMsgOpt <- Some(msg + "\n\n\nActual:\n" + String.Join("\n", actualLines, 0, expectedLines.Length))
            )

            if expectedIL.Length = 0 then
                errorMsgOpt <- Some ("No Expected IL")

            actualIL <- textNoComments

            match errorMsgOpt with
            | Some(msg) -> errorMsgOpt <- Some(msg + "\n\n\nEntire actual:\n" + textNoComments)
            | _ -> ()
        finally
            try File.Delete(ilFilePath) with | _ -> ()

        match errorMsgOpt with
        | Some(errorMsg) -> (false, errorMsg, actualIL)
        | _ -> (true, String.Empty, String.Empty)

    let private checkILAux ildasmArgs dllFilePath expectedIL =
        let (success, errorMsg, _) = checkILAux' ildasmArgs dllFilePath expectedIL
        if not success then
            Assert.Fail(errorMsg)
        else ()

    let checkILItem item dllFilePath expectedIL =
        checkILAux [ sprintf "/item:%s" item ] dllFilePath expectedIL

    let checkILItemWithLineNumbers item dllFilePath expectedIL =
        checkILAux [ sprintf "/item:\"%s\"" item; "/linenum" ] dllFilePath expectedIL

    let checkIL dllFilePath expectedIL =
        checkILAux [] dllFilePath expectedIL

    let verifyIL (dllFilePath: string) (expectedIL: string) =
        checkIL dllFilePath [expectedIL]
    let verifyILAndReturnActual (dllFilePath: string) (expectedIL: string) = checkILAux' [] dllFilePath [expectedIL]
    let reassembleIL ilFilePath dllFilePath =
        let ilasmPath = config.ILASM
        let errors, _ = exec ilasmPath ([ sprintf "%s /output=%s /dll" ilFilePath dllFilePath ])
        errors
