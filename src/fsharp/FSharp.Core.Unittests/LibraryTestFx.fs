// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.Unittests.LibraryTestFx

open System
open System.Collections.Generic

open NUnit.Framework

// Workaround for bug 3601, we are issuing an unnecessary warning
#nowarn "0004"

/// Check that the lamda throws an exception of the given type. Otherwise
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