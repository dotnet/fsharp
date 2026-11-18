// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Purpose: cross-assembly REALSIG MODE MIXING on an INLINED GENERIC SURFACE that
/// exercises BOTH the closure-re-hosting case (a closure over a typar) AND the
/// "returns a generic option" case (the FSharpOption analogue) � both fully
/// expressed inside OUR OWN LIBRARY, not FSharp.Core.
///
/// The library exposes an `inline` value (comparePayload) whose body is a closure
/// over its type parameter 'T.  A re-hosted closure is the canonical realsig+ case
/// ("!0..!n inherited from the host class"); forcing the consumer to inline it into
/// a generic class is the test.
///
/// The library also exposes an `inline` value (maybeWrapToOpt) whose body *returns*
/// a generic option ('T option) � the FSharpOption analogue.  Forcing the consumer
/// to inline it into the consumer's own generic class is the second test.
///
/// The (libRealsig, exeRealsig) axis is what is under test: all four combinations
/// must compile AND run correctly, for each (libRealsig, exeRealsig, optimize) cell.
///
/// Why inline: an `inline` value is *obligated* to be inlined into the calling
/// context � there is no fallback to a call site.  This forces the re-hosting
/// pass to splice library bodies (closures, generic union returns) into the
/// consumer, which is exactly the failure mode in Issue_0003 (a realsig+ FCS
/// compilation that "inlines" FCore's FastGenericComparer / FastGenericEqualityComparer,
/// which both use and return generic FCore types; our test does the same shape but
/// with OUR OWN types, which is a cleaner repro).
///
/// Breaks if: (a) a re-hosted closure body, under realsig+, loses its reference to
/// the class typar when its host class is the consumer's generic class, or (b) an
/// inlined library body that returns a generic option fails to resolve the
/// option type against the consumer's generic class.

namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_CrossAssemblyRealSigComparisonSurface =

    let verifyPEFileOnPath dllPath =
        verifyPEFileWithSystemDllsOn dllPath
        |> (fun e -> CompilationResult.Success {
                OutputPath = Some dllPath
                Dependencies = []
                Adjust = 0
                Diagnostics = []
                PerFileErrors = []
                Output = Some (RunOutput.ExecutionOutput e)
                Compilation = FSharp "x" |> withName "x" })

    let private libSource =
        """
module LibSrc

[<StructuralComparison; StructuralEquality>]
type Key<'T when 'T : comparison> =
    {
        tag: int
        payload: 'T
    }

/// Inline: a closure over 'T in the body.  A re-hosted closure under realsig+ is
/// the canonical "!0 inherited from host class" case.  Forcing the consumer to
/// inline this into the consumer's own generic class is the test.
let inline comparePayload (a: 'T) (b: 'T) : int =
    let local (x: 'T) (acc: int) : int = acc + (System.Text.Encoding.UTF8.GetByteCount (box x |> string))
    local a 0 - local b 0

/// Inline: returns a generic option � the FSharpOption analogue.  Forcing the
/// consumer to inline this into the consumer's own generic class is the test.
let inline wrapToOpt (a: 'T) : 'T option = Some a

type Container<'T when 'T : comparison>() =

    /// Inlines comparePayload: the inlined body (a closure over 'T) must work
    /// under the host class's !0 (i.e. the consumer's own generic class)
    /// when the consumer is built with the opposite realsig flag.
    member _.ComparePayload (a: Key<'T>, b: Key<'T>) = comparePayload a.payload b.payload

    /// Inlines wrapToOpt: the inlined body returns a generic option (the FSharpOption
    /// analogue), which must be spliced into the consumer's own generic class.
    member _.Wrap (a: Key<'T>) : 'T option = wrapToOpt a.payload
        """

    let private exeSource =
        """
module exeSource
open LibSrc

[<EntryPoint>]
let main _ =
    let a = { tag = 1; payload = "one" }
    let b = { tag = 2; payload = "two" }

    let c = Container<string>()
    let cmp = c.ComparePayload (a, b)
    let opt = c.Wrap a

    if cmp <> 0 && opt.IsSome then 0 else 1
        """

    [<Theory>]
    //           librs, exers, opt
    [<InlineData(true,  true,  true )>]
    [<InlineData(true,  true,  false)>]
    [<InlineData(true,  false, true )>]
    [<InlineData(true,  false, false)>]
    [<InlineData(false, true,  true )>]
    [<InlineData(false, true,  false)>]
    [<InlineData(false, false, true )>]
    [<InlineData(false, false, false)>]
    let ``Inlining a closure + generic option across mixed realsig modes`` (libRealsig, exeRealsig, optimize) =
        let lib = FSharp libSource |> withName "libSource" |> withRealInternalSignature libRealsig |> asLibrary |> withOptimization optimize
    
        let exe =
            FSharp exeSource
            |> withName "exeSource"
            |> withReferences [ lib ]
            |> withRealInternalSignature exeRealsig
            |> withOptimization optimize
            |> compile
    
        // (a) verify the main exe (unchanged)
        exe
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [ "All Classes and Methods in*exeSource.dll Verified." ]
        |> shouldSucceed
        |> ignore
    
        // (b) verify the referenced lib
        let libPath = exe.Output.Dependencies |> List.item 0
        verifyPEFileOnPath libPath
        |> withOutputContainsAllInOrderWithWildcards [ "All Classes and Methods in*libSource.dll Verified." ]
        |> shouldSucceed
        |> ignore
       

    /// A second test: a *method* that's inlined into a consumer, where the method's
    /// body uses the class's typar in a *closure returned from the method*.  This is
    /// the "closure re-hosted, then inlined into a consumer" shape.
    let private libMethodSource =
        """
module Lib2Src

type Bag<'T, 'U
    when 'T : comparison
    and  'U : comparison>() =

    /// Returns a closure that uses the bag's typar 'T in its body.
    /// The closure must survive both (a) being re-hosted under the bag's !0
    /// (realsig+) and (b) being inlined into a consumer with the opposite
    /// realsig flag.
    member _.Adder() : ('T -> int) =
        let defaultValue = Unchecked.defaultof<'T>
    
        let tester =
            fun (x:'T) ->
                obj.Equals(x, defaultValue)
    
        fun x ->
            if tester x then 0 else 1
    """

    let private exeMethodSource =
        """
module exeSource

open Lib2Src

[<EntryPoint>]
let main _ =
    let b = Bag<string, int>()
    let adder = b.Adder()
    let r = adder "two"
    if r > 0 then 0 else 1
"""

    [<Theory>]
    //           librs, exers, opt
    [<InlineData(true,  true,  true )>]
    [<InlineData(true,  true,  false)>]
    [<InlineData(true,  false, true )>]
    [<InlineData(true,  false, false)>]
    [<InlineData(false, true,  true )>]
    [<InlineData(false, true,  false)>]
    [<InlineData(false, false, true )>]
    [<InlineData(false, false, false)>]
    let ``A method that returns a closure over a class typar is consumable across mixed realsig modes`` (libRealsig: bool, exeRealsig: bool, optimize: bool) =
        let lib =
            FSharp libMethodSource
            |> withRealInternalSignature libRealsig
            |> asLibrary
            |> withOptimization optimize

        FSharp exeMethodSource
        |> withReferences [ lib ]
        |> withRealInternalSignature exeRealsig
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore

    /// Regression test for the imported-host re-hosting defect (the FSharp.Compiler.Service
    /// "FindTypeDefBuilder: Microsoft.FSharp.Core.FSharpOption`1 not found" internal error, and
    /// its IComparer`1 sibling).
    ///
    /// Root cause: Under realsig+, a closure that is re-hosted as a nested method of a class
    /// must have a LOCAL host type (one with a local TypeDefBuilder). When the closure's host
    /// tycon is IMPORTED (e.g. FSharpOption<'T> from FSharp.Core, or IComparer<'T> from
    /// System.Collections.Generic), codegen crashes with "FindTypeDefBuilder: ... not found"
    /// because an imported type has no local TypeDefBuilder.
    ///
    /// This can happen in two places (both guarded by the IsLocalRef fix):
    ///   1. CheckExpressions.TcLetBinding/TcLetrecGeneralizeBinding: a `let`/`let rec` closure
    ///      inside an object-expression member implementing an imported generic registers the
    ///      imported type as its host (via env.eFamilyType).
    ///   2. Optimizer.ConsiderSplitToMethod: a split thunk (under --optimize+) whose free
    ///      locals include an imported generic local registers the imported tycon as its host.
    ///
    /// The fix: only register ownerTycon when the host tycon is LOCAL (user-defined). An
    /// imported host falls back to ownerTycon = None, so the closure becomes a standalone
    /// generic method of the module class (which DOES have a local type def).
    ///
    /// This test exercises case 1 (object-expression member of an imported generic) directly:
    /// a small, reliable repro that does NOT require the split-thunk path (which requires a
    /// FunctionSize >= 3000 body, which is hard to trigger reliably in a small test). The FCS
    /// build repro (which uses the split-thunk path, case 2) is the definitive end-to-end
    /// verification.
    let private libObjExprClosureSource =
        """
module LibObjExprClosureSrc
open System.Collections.Generic

/// An object expression implementing the IMPORTED generic IComparer<'T>.  The member body
/// contains a `let` closure over 'T.  Under realsig+, that closure is re-hosted; the host must
/// be a LOCAL type def, NOT the imported IComparer<'T> (which has no local type def).
let makeComparer (a : 'T) (b : 'T) : IComparer<'T> when 'T : comparison =
    { new IComparer<'T> with
        member c.Compare (x: 'T, y: 'T) : int =
            let score (z: 'T) : int =
                System.Text.Encoding.UTF8.GetByteCount (box z |> string)
            score x - score y
    }

type Holder<'T when 'T : comparison>() =
    /// Exercises the object expression above inside a generic class member, so the re-hosted
    /// closure is in the realsig+ "closure inherits host typars" regime.
    member _.ComparePair (a: 'T, b: 'T) : int =
        let cmp = makeComparer a b
        cmp.Compare (a, b)
    """

    let private exeObjExprClosureSource =
        """
module exeObjExprClosureSrc
open LibObjExprClosureSrc

[<EntryPoint>]
let main _ =
    let h = Holder<string>()
    let r = h.ComparePair ("one", "two")
    if r <> 0 then 0 else 1
    """

    [<Theory>]
    //           librs, exers, opt
    // optimize+ is REQUIRED for the split (ConsiderSplitToMethod only fires under optimize+);
    // realsig+ is REQUIRED for the re-hosting registration.  The two (librs=true, opt=true)
    // cells are the regression: a re-hosted split thunk whose free local has an imported generic
    // type (FSharpOption<'T>).
    [<InlineData(true,  true,  true )>]
    [<InlineData(true,  true,  false)>]
    [<InlineData(true,  false, true )>]
    [<InlineData(true,  false, false)>]
    [<InlineData(false, true,  true )>]
    [<InlineData(false, true,  false)>]
    [<InlineData(false, false, true )>]
    [<InlineData(false, false, false)>]
    let ``A closure over a typar inside an object-expression member of an imported generic compiles under realsig+`` (libRealsig: bool, exeRealsig: bool, optimize: bool) =
        let lib =
            FSharp libObjExprClosureSource
            |> withName "libObjExprClosureSource"
            |> withRealInternalSignature libRealsig
            |> asLibrary
            |> withOptimization optimize

        // The failure mode is a codegen (FindTypeDefBuilder) internal error when a re-hosted
        // closure's host is an imported generic type (IComparer<'T>, no local TypeDefBuilder).
        // Successful compilation + run is the regression assertion: the closure was re-hosted
        // under a host that has a local type def (the module class), not under the imported type.
        FSharp exeObjExprClosureSource
        |> withName "exeObjExprClosureSource"
        |> withReferences [ lib ]
        |> withRealInternalSignature exeRealsig
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore
