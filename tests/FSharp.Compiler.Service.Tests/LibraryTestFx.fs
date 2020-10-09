// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.LibraryTestFx

open System
open System.Collections.Generic
open System.IO
open System.Reflection

open NUnit.Framework

// Verifies two sequences are equal (same length, equiv elements)
let VerifySeqsEqual (seq1 : seq<'T>) (seq2 : seq<'T>) =
    Assert.Equals(seq1, seq2)

let sleep(n : int32) =
    System.Threading.Thread.Sleep(n)

module SurfaceArea =
    open System.Reflection
    open System
    open System.Text.RegularExpressions

    // gets string form of public surface area for FSharp.CompilerService
    let private getActual () =

        // get local FSharp.CompilerService
        let path = Path.Combine(Path.GetDirectoryName(typeof<int list>.Assembly.Location), "FSharp.Compiler.Service.dll")
        let name = AssemblyName.GetAssemblyName (path)
        let asm = Assembly.Load(name)
        let fsCompilerServiceFullName = asm.FullName

        // public types only
        let types = asm.ExportedTypes |> Seq.filter (fun ty -> let ti = ty.GetTypeInfo() in ti.IsPublic || ti.IsNestedPublic) |> Array.ofSeq

        // extract canonical string form for every public member of every type
        let getTypeMemberStrings (t : Type) =
            // for System.Runtime-based profiles, need to do lots of manual work
            let getMembers (t : Type) =
                let ti = t.GetTypeInfo()
                let cast (info: #MemberInfo) = (t, info :> MemberInfo)
                let isDeclaredInFSharpCompilerService (m:MemberInfo) = m.DeclaringType.Assembly.FullName = fsCompilerServiceFullName
                seq {
                    yield! t.GetRuntimeEvents()     |> Seq.filter (fun m -> m.AddMethod.IsPublic && m |> isDeclaredInFSharpCompilerService) |> Seq.map cast
                    yield! t.GetRuntimeProperties() |> Seq.filter (fun m -> m.GetMethod.IsPublic && m |> isDeclaredInFSharpCompilerService) |> Seq.map cast
                    yield! t.GetRuntimeMethods()    |> Seq.filter (fun m -> m.IsPublic && m |> isDeclaredInFSharpCompilerService) |> Seq.map cast
                    yield! t.GetRuntimeFields()     |> Seq.filter (fun m -> m.IsPublic && m |> isDeclaredInFSharpCompilerService) |> Seq.map cast
                    yield! ti.DeclaredConstructors  |> Seq.filter (fun m -> m.IsPublic) |> Seq.map cast
                    yield! ti.DeclaredNestedTypes   |> Seq.filter (fun ty -> ty.IsNestedPublic) |> Seq.map cast
                } |> Array.ofSeq

            getMembers t
            |> Array.map (fun (ty, m) -> sprintf "%s: %s" (ty.ToString()) (m.ToString()))

        let actual =
            types |> Array.collect getTypeMemberStrings

        asm, actual

    // verify public surface area matches expected
    let verify expected platform (baseline: string) =
        printfn "Verify"
        let normalize (s:string) =
            Regex.Replace(s, "(\\r\\n|\\n|\\r)+", "\r\n").Trim()

        let asm, actualNotNormalized = getActual ()
        let actual = actualNotNormalized |> Seq.map normalize |> Seq.filter (String.IsNullOrWhiteSpace >> not) |> set
        
        let expected =
            // Split the "expected" string into individual lines, then normalize it.
            (normalize expected).Split([|"\r\n"; "\n"; "\r"|], StringSplitOptions.RemoveEmptyEntries)
            |> set

        //
        // Find types/members which exist in exactly one of the expected or actual surface areas.
        //

        /// Surface area types/members which were expected to be found but missing from the actual surface area.
        let unexpectedlyMissing = Set.difference expected actual

        /// Surface area types/members present in the actual surface area but weren't expected to be.
        let unexpectedlyPresent = Set.difference actual expected

        // If both sets are empty, the surface areas match so allow the test to pass.
        if Set.isEmpty unexpectedlyMissing
          && Set.isEmpty unexpectedlyPresent then
            // pass
            ()
        else

            let logFile =
                let workDir = TestContext.CurrentContext.WorkDirectory
                sprintf "%s\\FSharp.CompilerService.SurfaceArea.%s.txt" workDir platform

            System.IO.File.WriteAllText(logFile, String.Join("\r\n", actual))

            // The surface areas don't match; prepare an easily-readable output message.
            let msg =
                let inline newLine (sb : System.Text.StringBuilder) = sb.AppendLine () |> ignore
                let sb = System.Text.StringBuilder ()
                Printf.bprintf sb "Assembly: %A" asm
                newLine sb
                sb.AppendLine "Expected and actual surface area don't match. To see the delta, run:" |> ignore
                Printf.bprintf sb "    windiff %s %s" baseline logFile
                newLine sb
                newLine sb
                sb.AppendLine "To update the baseline run:" |> ignore
                Printf.bprintf sb "    copy /y %s %s" logFile baseline
                newLine sb
                newLine sb
                sb.Append "Unexpectedly missing (expected, not actual):" |> ignore
                for s in unexpectedlyMissing do
                    newLine sb
                    sb.Append "    " |> ignore
                    sb.Append s |> ignore
                newLine sb
                newLine sb
                sb.Append "Unexpectedly present (actual, not expected):" |> ignore
                for s in unexpectedlyPresent do
                    newLine sb
                    sb.Append "    " |> ignore
                    sb.Append s |> ignore
                newLine sb
                sb.ToString ()

            failwith msg
