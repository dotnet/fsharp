// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

open System
open System.IO
open System.Text.RegularExpressions

open Xunit
open TestFramework

[<RequireQualifiedAccess>]
module ILChecker =
    let config = initialConfig

    let private exec exe args =
        let arguments = args |> String.concat " "
        let exitCode, _output, errors = Commands.executeProcess exe arguments ""
        let errors = errors |> String.concat Environment.NewLine
        errors, exitCode

    /// Filters i.e ['The system type \'System.ReadOnlySpan`1\' was required but no referenced system DLL contained this type']
    let private filterSpecialComment (text: string) =
        let pattern = @"(\[\'(.*?)\'\])"
        Regex.Replace(
            text,
            pattern,
            (fun me -> String.Empty)
        )

    let normalizeILText assemblyName (ilCode: string) =
        let blockComments = @"/\*(.*?)\*/"
        let lineComments = @"//(.*?)\r?\n"
        let lineCommentsEof = @"//(.*?)$"
        let strings = @"""((\\[^\n]|[^""\n])*)"""
        let verbatimStrings = @"@(""[^""]*"")+"
        let methodSingleLine = "^(\s*\.method.*)(?: \s*)$[\r?\n?]^(\s*\{)"
        let methodDoubleLine = "^(\s*\.method.*)(?: \s*)$[\r?\n?]^(?: \s*)(.*)\s*$[\r?\n?]^(\s*\{)"
        let methodTripleLine = "^(\s*\.method.*)(?: \s*)$[\r?\n?]^(?: \s*)(.*)\s*$[\r?\n?]^(?: \s*)(.*)\s*$[\r?\n?]^(\s*\{)"
        let normalizeNewLines (text: string) = text.Replace("\r\n", "\n").Replace("\r\n", "\r")
        let resourceMultiLine = @"(?<resource>\.mresource\s+.*)(?<block>\s*\{[^}]*\})"

        let stripComments (text:string) =
            Regex.Replace(text,
                $"{blockComments}|{lineComments}|{lineCommentsEof}|{strings}|{verbatimStrings}",
                (fun me ->
                    if (me.Value.StartsWith("/*") || me.Value.StartsWith("//")) then
                        if me.Value.StartsWith("//") then Environment.NewLine else String.Empty
                    else
                        me.Value), RegexOptions.Singleline)
            |> filterSpecialComment

        let unifyMethodLine (text:string) =
            let text1 = Regex.Replace(text, $"{methodSingleLine}", (fun me -> $"{me.Groups[1].Value}\n{me.Groups[2].Value}"), RegexOptions.Multiline)
            let text2 = Regex.Replace(text1, $"{methodDoubleLine}", (fun me -> $"{me.Groups[1].Value} {me.Groups[2].Value}\n{me.Groups[3].Value}"), RegexOptions.Multiline)
            let text3 = Regex.Replace(text2, $"{methodTripleLine}", (fun me -> $"{me.Groups[1].Value} {me.Groups[2].Value} {me.Groups[3].Value}\n{me.Groups[4].Value}"), RegexOptions.Multiline)
            text3

        let replace input (pattern, replacement: string) = Regex.Replace(input, pattern, replacement, RegexOptions.Singleline)

        let unifyRuntimeAssemblyName ilCode =
            List.fold replace ilCode [
                "\[System\.Runtime\]|\[System\.Console\]|\[System\.Runtime\.Extensions\]|\[mscorlib\]|\[System\.Memory\]", "[runtime]"
                "(\.assembly extern (System\.Runtime|System\.Console|System\.Runtime\.Extensions|mscorlib|System\.Memory)){1}([^\}]*)\}", ".assembly extern runtime { }"
                "(\.assembly extern (FSharp.Core)){1}([^\}]*)\}", ".assembly extern FSharp.Core { }" ]

        let unifyImageBase ilCode = replace ilCode ("\.imagebase\s*0x\d*", ".imagebase {value}")

        let unifyingAssemblyNames (text: string) =
            match assemblyName with
            | Some name -> text.Replace(name, "assembly")
            | None -> text
            |> unifyRuntimeAssemblyName
            |> unifyImageBase

        let stripManagedResources (text: string) =
            let result = Regex.Replace(text, "\.mresource public .*\r?\n{\s*}\r?\n", "", RegexOptions.Multiline)
            result
        
        // This lets the same test be used when targeting both netfx and netcore.
        let unifyNetStandardVersions (text: string) = text.Replace(".ver 2:0:0:0", ".ver 2:1:0:0")

        let unifyResourceBlock text =
            let text2 = Regex.Replace(text, resourceMultiLine, (fun (res: Match) -> $"""{res.Groups["resource"].Value} {{ }}"""), RegexOptions.Multiline)
            text2

        ilCode.Trim()
        |> normalizeNewLines
        |> stripComments
        |> unifyingAssemblyNames
        |> unifyMethodLine
        |> stripManagedResources
        |> unifyNetStandardVersions
        |> unifyResourceBlock

    let private generateIlFile dllFilePath ildasmArgs =
        let ilFilePath = Path.ChangeExtension(dllFilePath, ".il")

        let ildasmPath = config.ILDASM

        let ildasmFullArgs = [ dllFilePath; $"-out=%s{ilFilePath}"; yield! ildasmArgs  ]

        let stdErr, exitCode =
            let ildasmCommandPath = Path.ChangeExtension(dllFilePath, ".ildasmCommandPath")
            File.WriteAllLines(ildasmCommandPath, [| $"{ildasmPath} {ildasmFullArgs}" |] )
            exec ildasmPath ildasmFullArgs

        if exitCode <> 0 then
            failwith $"ILASM Expected exit code \"0\", got \"%d{exitCode}\"\nSTDERR: %s{stdErr}"

        if not (String.IsNullOrWhiteSpace stdErr) then
            failwith $"ILASM Stderr is not empty:\n %s{stdErr}"

        ilFilePath

    let private generateIL (dllFilePath: string) ildasmArgs =
        let assemblyName = Some (Path.GetFileNameWithoutExtension dllFilePath)
        let ilFilePath = generateIlFile dllFilePath ildasmArgs
        let normalizedText = normalizeILText assemblyName (File.ReadAllText(ilFilePath))
        File.WriteAllText(ilFilePath, normalizedText)
        normalizedText

    let private compareIL assemblyName (actualIL: string) expectedIL =

        let mutable errorMsgOpt = None

        let prepareLines (s: string) =
            s.Split('\n')
                // Skip emitted managed resources
                |> Array.map(fun e -> e.Trim('\r'))
                |> Array.skipWhile(String.IsNullOrWhiteSpace)
                |> Array.rev
                |> Array.skipWhile(String.IsNullOrWhiteSpace)
                |> Array.rev

        match expectedIL with
        | [] -> errorMsgOpt <- Some "No Expected IL"
        | expectedIL ->
            let (|Trimmed|) (ilCode: string) = ilCode.Trim()

            for Trimmed ilCode in expectedIL do
                let expectedLines = ilCode |> normalizeILText (Some assemblyName) |> prepareLines

                if expectedLines.Length = 0 then
                    errorMsgOpt <- Some("ExpectedLines length invalid: 0")
                else
                    let startIndex =
                        let index = actualIL.IndexOf(expectedLines[0].Trim())
                        if index > 0 then
                            index
                        else
                            0
                    let actualLines = actualIL.Substring(startIndex) |> prepareLines

                    let errors = ResizeArray()
                    if actualLines.Length < expectedLines.Length then
                        let msg = $"\nExpected at least %d{expectedLines.Length} lines but found only %d{actualLines.Length}\n"
                        errorMsgOpt <- Some(msg + "\nExpected:\n" + ilCode + "\n")
                    else
                        for i = 0 to expectedLines.Length - 1 do
                            let expected = expectedLines[i].Trim()
                            let actual = actualLines[i].Trim()
                            if expected <> actual then
                                errors.Add $"\n==\nName: '%s{actualLines[0]}'\n\nExpected:\t %s{expected}\nActual:\t\t %s{actual}\n=="

                        if errors.Count > 0 then
                            let msg = String.concat "\n" errors + "\n\n\Expected:\n" + ilCode + "\n"
                            errorMsgOpt <- Some(msg + "\n\n\nActual:\n" + String.Join("\n", actualLines, 0, expectedLines.Length))

        match errorMsgOpt with
        | Some msg ->
            let msg = msg + "\n\n\nEntire actual:\n" + actualIL
            (false, msg, actualIL)
        | _ -> (true, String.Empty, actualIL)

    let private checkILPrim ildasmArgs dllFilePath =
        let actualIL = generateIL dllFilePath ildasmArgs
        compareIL (Path.GetFileNameWithoutExtension dllFilePath) actualIL

    let private checkILAux ildasmArgs dllFilePath expectedIL =
        let (success, errorMsg, _) = checkILPrim ildasmArgs dllFilePath expectedIL
        if not success then
            Assert.Fail(errorMsg)

    // This doesn't work because the '/linenum' is being ignored by
    // the version of ILDASM we are using, which we acquire from a nuget package
    //let checkILWithDebugPoints dllFilePath expectedIL =
    //    checkILAux [ "/linenum" ] dllFilePath expectedIL

    let checkIL dllFilePath expectedIL =
        checkILAux [] dllFilePath expectedIL

    let verifyIL (dllFilePath: string) (expectedIL: string) =
        checkIL dllFilePath [expectedIL]

    let verifyILAndReturnActual args dllFilePath expectedIL =
        checkILPrim args dllFilePath expectedIL

    let checkILNotPresent dllFilePath unexpectedIL =
        let actualIL = generateIL dllFilePath []
        if unexpectedIL = [] then
            Assert.Fail $"No unexpected IL given. This is actual IL: \n{actualIL}"
        let errors =
            unexpectedIL
            |> Seq.map (normalizeILText None)
            |> Seq.filter actualIL.Contains
            |> Seq.map (sprintf "Found in actual IL: '%s'")
            |> String.concat "\n"
        if errors <> "" then
            Assert.Fail $"{errors}\n\n\nEntire actual:\n{actualIL}"

    let reassembleIL ilFilePath dllFilePath =
        let ilasmPath = config.ILASM
        let errors, _ = exec ilasmPath [ $"%s{ilFilePath} /output=%s{dllFilePath} /dll" ]
        errors
