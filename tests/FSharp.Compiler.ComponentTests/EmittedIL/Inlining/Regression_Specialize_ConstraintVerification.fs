namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Each test exercises one ILGenericParameterDef field cleared by stripILGenericParamConstraints.
/// If a constraint leaks onto the closure's Specialize<T> override the JIT throws TypeLoadException
/// ("weaker type parameter constraints"). See #14492.
module Regression_Specialize_ConstraintVerification =

    open Regression_TLR_MutualInnerRec_StructuralAssertions

    let private compileVerifyAndRun realsig source =
        source |> compileOptimized realsig |> compile |> shouldSucceed |> verifyPEAndRun

    [<Theory; InlineData(true); InlineData(false)>]
    let ``struct + equality`` (realsig: bool) =
        closureWithConstraint "'a : struct and 'a : equality" "a = b" "int" "42"
        |> compileVerifyAndRun realsig

    [<Theory; InlineData(true); InlineData(false)>]
    let ``not struct + equality`` (realsig: bool) =
        closureWithConstraint "'a : not struct and 'a : equality" "obj.ReferenceEquals(a, b)" "string" "\"ok\""
        |> compileVerifyAndRun realsig

    /// Specifically guards the `IsUnmanagedAttribute` (carried via CustomAttrsStored) clearing
    /// in `stripILGenericParamConstraints` — leaks of that attribute onto the Specialize override
    /// were the original #14492 trigger for unmanaged-constrained inlines.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``unmanaged + equality`` (realsig: bool) =
        let source = closureWithConstraint "'a : unmanaged and 'a : equality" "a = b" "int" "42"
        let compiled = source |> compileOptimized realsig |> compile |> shouldSucceed
        compiled
        |> verifyILNotPresent [
            // No IsUnmanagedAttribute / modreq leakage onto the Specialize override / T-suffix class.
            "Specialize<valuetype (class [runtime]System.ValueType modreq"
            "T<valuetype (class [runtime]System.ValueType modreq"
        ]
        verifyPEAndRun compiled

    [<Theory; InlineData(true); InlineData(false)>]
    let ``default constructor`` (realsig: bool) =
        closureWithConstraint "'a : (new : unit -> 'a) and 'a : equality" "a = b" "int" "42"
        |> compileVerifyAndRun realsig

    [<Theory; InlineData(true); InlineData(false)>]
    let ``interface IComparable`` (realsig: bool) =
        closureWithConstraint "'a :> System.IComparable" "(a :> System.IComparable).CompareTo(b) = 0" "int" "42"
        |> compileVerifyAndRun realsig

    [<Theory; InlineData(true); InlineData(false)>]
    let ``comparison`` (realsig: bool) =
        closureWithConstraint "'a : comparison" "compare a b = 0" "int" "42"
        |> compileVerifyAndRun realsig

    [<Theory; InlineData(true); InlineData(false)>]
    let ``struct + comparison + equality combined`` (realsig: bool) =
        closureWithConstraint "'a : struct and 'a : comparison and 'a : equality" "a >= b" "int" "42"
        |> compileVerifyAndRun realsig

    /// Constrained inline member call inside a closure caused CLR segfault. See #19075.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``SRTP member constraint with IDisposable`` (realsig: bool) =
        """module Test
open System
open System.IO
module Dispose =
    let inline action<'a when 'a: (member Dispose: unit -> unit) and 'a :> IDisposable>(a: 'a) = a.Dispose()
[<EntryPoint>]
let main _ =
    let ms = new MemoryStream()
    ms |> Dispose.action
    0
"""
        |> compileVerifyAndRun realsig
