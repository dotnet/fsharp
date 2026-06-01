namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Inline functions with constrained type params, when inlined into closures,
/// attach those constraints to the closure class's generic params. The closure's
/// Specialize<T> override (from FSharpFunc) must be unconstrained to match the base.
/// If constraints leak, the JIT throws at type-load time:
///   "TypeLoadException: Method 'Specialize' on type '...' tried to implicitly
///    override a method with weaker type parameter constraints."
///
/// Each test exercises a different ILGenericParameterDef field stripped by mkILSimpleTypar:
///   struct       → HasNotNullableValueTypeConstraint
///   not struct   → HasReferenceTypeConstraint
///   unmanaged    → CustomAttrsStored (IsUnmanagedAttribute)
///   new()        → HasDefaultConstructorConstraint
///   :> interface → Constraints list
///   comparison   → SRTP resolved to specific IL calls
///
/// Pipeline: compile (optimized) → ILVerify (metadata) → run (TypeLoadException guard).
/// See #14492.
module Regression_Specialize_ConstraintVerification =

    open Regression_TLR_MutualInnerRec_StructuralAssertions

    let private compileVerifyAndRun realsig source =
        let compiled = source |> compileOptimized realsig |> compile |> shouldSucceed
        compiled |> verifyPEFileWithSystemDlls |> shouldSucceed |> ignore
        compiled |> run |> shouldSucceed |> ignore

    [<Theory; InlineData(true); InlineData(false)>]
    let ``struct + equality`` (realsig: bool) =
        closureWithConstraint "'a : struct and 'a : equality" "a = b" "int" "42"
        |> compileVerifyAndRun realsig

    [<Theory; InlineData(true); InlineData(false)>]
    let ``not struct + equality`` (realsig: bool) =
        closureWithConstraint "'a : not struct and 'a : equality" "obj.ReferenceEquals(a, b)" "string" "\"ok\""
        |> compileVerifyAndRun realsig

    [<Theory; InlineData(true); InlineData(false)>]
    let ``unmanaged + equality`` (realsig: bool) =
        closureWithConstraint "'a : unmanaged and 'a : equality" "a = b" "int" "42"
        |> compileVerifyAndRun realsig

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
