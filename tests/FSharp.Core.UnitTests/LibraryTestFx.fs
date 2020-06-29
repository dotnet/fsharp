// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.LibraryTestFx

open System
open System.Collections.Generic

open NUnit.Framework

// Workaround for bug 3601, we are issuing an unnecessary warning
#nowarn "0004"

/// Check that the lambda throws an exception of the given type. Otherwise
/// calls Assert.Fail()
let CheckThrowsExn<'a when 'a :> exn> (f : unit -> unit) =
    try
        let _ = f ()
        sprintf "Expected %O exception, got no exception" typeof<'a> |> Assert.Fail 
    with
    | :? 'a -> ()
    | e -> sprintf "Expected %O exception, got: %O" typeof<'a> e |> Assert.Fail

let private CheckThrowsExn2<'a when 'a :> exn> s (f : unit -> unit) =
    let funcThrowsAsExpected =
        try
            let _ = f ()
            false // Did not throw!
        with
        | :? 'a
            -> true   // Thew null ref, OK
        | _ -> false  // Did now throw a null ref exception!
    if funcThrowsAsExpected
    then ()
    else Assert.Fail(s)

// Illegitimate exceptions. Once we've scrubbed the library, we should add an
// attribute to flag these exception's usage as a bug.
let CheckThrowsNullRefException      f = CheckThrowsExn<NullReferenceException>   f
let CheckThrowsIndexOutRangException f = CheckThrowsExn<IndexOutOfRangeException> f
let CheckThrowsObjectDisposedException f = CheckThrowsExn<ObjectDisposedException> f

// Legit exceptions
let CheckThrowsNotSupportedException f = CheckThrowsExn<NotSupportedException>    f
let CheckThrowsArgumentException     f = CheckThrowsExn<ArgumentException>        f
let CheckThrowsArgumentNullException f = CheckThrowsExn<ArgumentNullException>    f
let CheckThrowsArgumentNullException2 s f  = CheckThrowsExn2<ArgumentNullException>  s  f
let CheckThrowsArgumentOutOfRangeException f = CheckThrowsExn<ArgumentOutOfRangeException>    f
let CheckThrowsKeyNotFoundException  f = CheckThrowsExn<KeyNotFoundException>     f
let CheckThrowsDivideByZeroException f = CheckThrowsExn<DivideByZeroException>    f
let CheckThrowsOverflowException     f = CheckThrowsExn<OverflowException>        f
let CheckThrowsInvalidOperationExn   f = CheckThrowsExn<InvalidOperationException> f
let CheckThrowsFormatException       f = CheckThrowsExn<FormatException>           f
let CheckThrowsArithmeticException   f = CheckThrowsExn<ArithmeticException>  f

// Verifies two sequences are equal (same length, equiv elements)
let VerifySeqsEqual (seq1 : seq<'T>) (seq2 : seq<'T>) =
    CollectionAssert.AreEqual(seq1, seq2)

let sleep(n : int32) =
    System.Threading.Thread.Sleep(n)

module SurfaceArea =
    open System.Reflection
    open System
    open System.Text.RegularExpressions

    // gets string form of public surface area for the currently-loaded FSharp.Core
    let private getActual () =

        // get current FSharp.Core
        let asm = typeof<int list>.Assembly
        let fsCoreFullName = asm.FullName

        // public types only
        let types = asm.ExportedTypes |> Seq.filter (fun ty -> let ti = ty.GetTypeInfo() in ti.IsPublic || ti.IsNestedPublic) |> Array.ofSeq

        // extract canonical string form for every public member of every type
        let getTypeMemberStrings (t : Type) =
            // for System.Runtime-based profiles, need to do lots of manual work
            let getMembers (t : Type) =
                let ti = t.GetTypeInfo()
                let cast (info: #MemberInfo) = (t, info :> MemberInfo)
                let isDeclaredInFSharpCore (m:MemberInfo) = m.DeclaringType.Assembly.FullName = fsCoreFullName
                seq {
                    yield! t.GetRuntimeEvents()     |> Seq.filter (fun m -> m.AddMethod.IsPublic && m |> isDeclaredInFSharpCore) |> Seq.map cast
                    yield! t.GetRuntimeProperties() |> Seq.filter (fun m -> m.GetMethod.IsPublic && m |> isDeclaredInFSharpCore) |> Seq.map cast
                    yield! t.GetRuntimeMethods()    |> Seq.filter (fun m -> m.IsPublic && m |> isDeclaredInFSharpCore) |> Seq.map cast
                    yield! t.GetRuntimeFields()     |> Seq.filter (fun m -> m.IsPublic && m |> isDeclaredInFSharpCore) |> Seq.map cast
                    yield! ti.DeclaredConstructors  |> Seq.filter (fun m -> m.IsPublic) |> Seq.map cast
                    yield! ti.DeclaredNestedTypes   |> Seq.filter (fun ty -> ty.IsNestedPublic) |> Seq.map cast
                } |> Array.ofSeq


            getMembers t
            |> Array.map (fun (ty, m) -> sprintf "%s: %s" (ty.ToString()) (m.ToString()))

        let actual =
            types |> Array.collect getTypeMemberStrings

        asm, actual

    // verify public surface area matches expected
    let verify expected platform (fileName : string) =
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
                sprintf "%s\\FSharp.Core.SurfaceArea.%s.txt" workDir platform
            System.IO.File.WriteAllText(logFile, String.Join("\r\n", actual))

            // The surface areas don't match; prepare an easily-readable output message.
            let msg =
                let inline newLine (sb : System.Text.StringBuilder) = sb.AppendLine () |> ignore
                let sb = System.Text.StringBuilder ()
                Printf.bprintf sb "Assembly: %A" asm
                newLine sb
                sb.AppendLine "Expected and actual surface area don't match. To see the delta, run:" |> ignore
                Printf.bprintf sb "    windiff %s %s" fileName logFile
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
