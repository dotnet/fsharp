// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.Unittests.LibraryTestFx

open System
open System.Collections.Generic

open NUnit.Framework

// Workaround for bug 3601, we are issuing an unnecessary warning
#nowarn "0004"

/// Check that the lambda throws an exception of the given type. Otherwise
/// calls Assert.Fail()
let CheckThrowsExn<'a when 'a :> exn> (f : unit -> unit) =
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
    else Assert.Fail()

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

// Verifies two sequences are equal (same length, equiv elements)
let VerifySeqsEqual seq1 seq2 =
    Assert.AreEqual(Seq.length seq1, Seq.length seq2, "Sequences are different lengths.")

    Seq.zip seq1 seq2
    |> Seq.iteri (fun i (a, b) -> if a <> b then Assert.Fail("Sequences are different in position {0}\n  Expected: {1}\n  But was: {2}", i, a, b))

let sleep(n : int32) =        
#if FX_NO_THREAD
    async { do! Async.Sleep(n) } |> Async.RunSynchronously
#else
    System.Threading.Thread.Sleep(n)
#endif

module SurfaceArea =
    open System.Reflection
    open System
    open System.Text.RegularExpressions
    
    // gets string form of public surface area for the currently-loaded FSharp.Core
    let private getActual () =
    
        // get current FSharp.Core
        let asm = 
            #if portable7 || portable78 || portable259 || coreclr
            typeof<int list>.GetTypeInfo().Assembly
            #else
            typeof<int list>.Assembly
            #endif
        
        // public types only
        let types =
            #if portable7 || portable78 || portable259 || coreclr
            asm.ExportedTypes |> Seq.filter (fun ty -> let ti = ty.GetTypeInfo() in ti.IsPublic || ti.IsNestedPublic) |> Array.ofSeq
            #else
            asm.GetExportedTypes()
            #endif

        // extract canonical string form for every public member of every type
        let getTypeMemberStrings (t : Type) =
            // for System.Runtime-based profiles, need to do lots of manual work
            #if portable7 || portable78 || portable259 || coreclr
            let getMembers (t : Type) =
                let ti = t.GetTypeInfo()
                let cast (info : #MemberInfo) = (t, info :> MemberInfo)
                seq {
                    yield! t.GetRuntimeEvents()     |> Seq.filter (fun m -> m.AddMethod.IsPublic) |> Seq.map cast
                    yield! t.GetRuntimeProperties() |> Seq.filter (fun m -> m.GetMethod.IsPublic) |> Seq.map cast
                    yield! t.GetRuntimeMethods()    |> Seq.filter (fun m -> m.IsPublic) |> Seq.map cast
                    yield! t.GetRuntimeFields()     |> Seq.filter (fun m -> m.IsPublic) |> Seq.map cast
                    yield! ti.DeclaredConstructors  |> Seq.filter (fun m -> m.IsPublic) |> Seq.map cast
                    yield! ti.DeclaredNestedTypes   |> Seq.filter (fun ty -> ty.IsNestedPublic) |> Seq.map cast
                } |> Array.ofSeq

            getMembers t
            |> Array.map (fun (ty, m) -> sprintf "%s: %s" (ty.ToString()) (m.ToString()))
            #else
            t.GetMembers()
            |> Array.map (fun v -> sprintf "%s: %s" (v.ReflectedType.ToString()) (v.ToString()))
            #endif
            
        let actual =
            types 
            |> Array.collect getTypeMemberStrings
            |> Array.sort
            |> String.concat "\r\n"

        asm,actual
    
    // verify public surface area matches expected
    let verify expected platform fileName =  
        let workDir = TestContext.CurrentContext.WorkDirectory
        let logFile = sprintf "%s\\CoreUnit_%s_Xml.xml" workDir platform
        let normalize (s:string) =
            Regex.Replace(s, "(\\r\\n|\\n)+", "\r\n").Trim([|'\r';'\n'|])
        let asm, actualNotNormalized = getActual ()
        let actual = actualNotNormalized |> normalize
        let expected = expected |> normalize
        
        Assert.AreEqual(expected, actual, sprintf "\r\nAssembly: %A\r\n--------------------- ACTUAL -------------------\r\n%s\r\n--------------------EXPECTED--------------------\r\n%s\r\n-----------------\r\n Expected and actual surface area don't match. To see the delta, run\r\nwindiff %s %s" asm actual expected fileName logFile)
