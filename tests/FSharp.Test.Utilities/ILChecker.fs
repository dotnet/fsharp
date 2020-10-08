// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test.Utilities

open System
open System.IO
open System.Text.RegularExpressions
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

    let private replaceRe (re: string) (replacement: string) (where: string) : string =
        Regex.Replace(where, re, replacement)
 
    let private trimEnd (chars: char array) (where: string) : string =
        where.TrimEnd(chars)
        
    let private cleanupIL (il: string) : string =
        il
        |> replaceRe @"(.*\.line[^'$]*)('.+)?" ""
        |> replaceRe @"(.*\.ver )(.+)" "$1"
        |> replaceRe @"(.*\.publickeytoken )(.+)" "$1"
        |> replaceRe "\.language.+$" ""

        |> replaceRe " = \([0-9a-hA-H\s]+\)" ""

        // Filter 'indexed' locals:
        |> replaceRe "\[\d+\]\s" ""

        // Unify assembly names:
        |> replaceRe "\[System.Diagnostics.Debug\]|\[System.Runtime\]|\[System.Runtime.Extensions\]|\[mscorlib\]" "[runtime]"
        |> replaceRe "\.assembly extern (System\.Runtime|mscorlib)$" ".assembly extern runtime"
        |> replaceRe "\.assembly '?(\S+)'?$" ".assembly assembly"
        |> replaceRe ".module '(\S+)'$" ".module $1"
        |> replaceRe "\.mresource public '?(FSharpSignatureData|FSharpOptimizationData)\.\S+'?$" ".mresource public $1.assembly"

        // Replace comments: 
        |> replaceRe @"//.+$" "\n"
        |> replaceRe @"/\*(.*?)\*/" ""
        
        // Replace strings and verbatim strings:
        |> replaceRe @"""((\\[^\n]|[^""\n])*)""" "$1"
        |> replaceRe @"@(""[^""]*"")+" "$1"

        // Filter special comments (e.g. ['The system type \'System.ReadonlySpan`1\' was required but not refenced system DLL contained this type'])
        |> replaceRe @"(\[\'(.*?)\'\])" ""

        // generated identifier names can sometimes differ when using hosted compiler due to polluted environment
        // e.g. fresh compiler would generate identifier x@8
        //      but hosted compiler will generate 'x@8-3'
        // these are functionally the same, merely a naming difference
        // strip off the single quotes and -N tag.
               
        |> replaceRe @"'(\w+@\d+)-\d+'" "$1"

        // Trim any trailing spaces and tabs
        |> trimEnd [|' '; '\t'|]

    let private normalizeAssemblyName (assemblyName: string) (il: string) : string =
        il.Replace(assemblyName, "assembly")

    let private tryGetAssemblyName (il: string) : string option =
        let m = Regex.Match(il, ".module '?(\S+)'?\.dll")
        if m.Success then Some(m.Groups.[1].Value) else None
        
    let private checkILAux' ildasmArgs dllFilePath expectedIL =
        let ilFilePath = Path.ChangeExtension(dllFilePath, ".il")

        let errorMsgs = ResizeArray()
                   
        let mutable actualIL = String.Empty
        try
            let ildasmPath = config.ILDASM

            let stdErr, exitCode = exec ildasmPath (ildasmArgs @ [ sprintf "%s -out=%s" dllFilePath ilFilePath ])

            if exitCode <> 0 then failwith (sprintf "ILASM Expected exit code \"0\", got \"%d\"\nSTDERR: %s" exitCode stdErr)

            if not (String.IsNullOrWhiteSpace stdErr) then
                failwith (sprintf "ILASM Stderr is not empty:\n %s" stdErr)

            let unifyRuntimeAssemblyName ilCode =
                System.Text.RegularExpressions.Regex.Replace(ilCode,
                    "\[System.Diagnostics.Debug\]|\[System.Runtime\]|\[System.Runtime.Extensions\]|\[mscorlib\]","[runtime]",
                    System.Text.RegularExpressions.RegexOptions.Singleline)

            let asmName = Path.GetFileNameWithoutExtension(dllFilePath)
            
            let text =
                File.ReadAllText(ilFilePath)
                |> normalizeAssemblyName asmName
                |> unifyRuntimeAssemblyName
                
            let blockComments = @"/\*(.*?)\*/"
            let lineComments = @"//(.*?)\r?\n"
            let strings = @"""((\\[^\n]|[^""\n])*)"""
            let verbatimStrings = @"@(""[^""]*"")+"
            let textNoComments_ =
                System.Text.RegularExpressions.Regex.Replace(text,
                    blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                    (fun me ->
                        if (me.Value.StartsWith("/*") || me.Value.StartsWith("//")) then
                            if me.Value.StartsWith("//") then Environment.NewLine else String.Empty
                        else
                            me.Value), System.Text.RegularExpressions.RegexOptions.Singleline)
                |> filterSpecialComment

            let textNoComments =
                textNoComments_.Replace("\r\n", "\n").Split('\n')
                |> Array.map cleanupIL
                |> List.ofSeq
                |> String.concat "\n"
                
            expectedIL
            |> List.map (fun (ilCode: string) ->
                         let il = ilCode.Trim() |> unifyRuntimeAssemblyName
                         match tryGetAssemblyName il with
                         | Some name -> il |> normalizeAssemblyName name
                         | None -> il)
                         
            |> List.iter (fun (ilCode: string) ->

                let expectedLines = ilCode.Split('\n') |> Array.map cleanupIL

                let startIL = expectedLines.[0].Trim()
                let startIndex = textNoComments.IndexOf(startIL)
                          
                if startIndex = -1 then               
                    errorMsgs.Add("\nExpected IL was not found in the output:\n" + startIL + "\n")
                else
                    let errors = ResizeArray()
                    let actualLines = textNoComments.Substring(startIndex, textNoComments.Length - startIndex).Split('\n')
                    if actualLines.Length < expectedLines.Length then
                        failwith dllFilePath
                        let msg = sprintf "\nExpected at least %d lines but found only %d\n" expectedLines.Length actualLines.Length
                        errorMsgs.Add(msg + "\nExpected:\n" + ilCode + "\n")
                    else
                        for i = 0 to expectedLines.Length - 1 do
                            let expected = expectedLines.[i].Trim()
                            let actual = actualLines.[i].Trim()
                            if expected <> actual then
                                errors.Add(sprintf "\n==\nName: '%s'\n\nExpected:\t %s\nActual:\t\t %s\n==" actualLines.[0] expected actual)

                        if errors.Count > 0 then
                            let msg = String.concat "\n" errors + "\n\n\Expected:\n" + ilCode + "\n"
                            errorMsgs.Add(msg + "\n\n\nActual:\n" + String.Join("\n", actualLines, 0, expectedLines.Length))
            )

            if expectedIL.Length = 0 then
                errorMsgs.Add("No Expected IL")

            actualIL <- textNoComments

            //match errorMsgOpt with
            //| Some(msg) -> errorMsgOpt <- msg + "\n\n\nEntire actual:\n" + textNoComments
            //| _ -> ()
        finally
            try File.Delete(ilFilePath) with | _ -> ()

        match errorMsgs.Count with
        | 0 -> (true, String.Empty, String.Empty)
        | _ ->
            let messages = String.concat "\n" errorMsgs 
            (false, messages, actualIL)

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
        
    let verifyILAndReturnActual (dllFilePath: string) (expectedIL: string list) = checkILAux' [] dllFilePath expectedIL
    
    let reassembleIL ilFilePath dllFilePath =
        let ilasmPath = config.ILASM
        let errors, _ = exec ilasmPath ([ sprintf "%s /output=%s /dll" ilFilePath dllFilePath ])
        errors
