namespace Conformance.Types

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.ScriptHelpers

/// Tests for RFC FS-1043: Extension members become available to solve operator trait constraints.
module ExtensionConstraintsTests =

    let private testFileDir = Path.Combine(__SOURCE_DIRECTORY__, "testFiles")

    /// Create a compilation from a test file in the test directory.
    let private createTest fileName =
        FSharp(loadSourceFromFile (Path.Combine(testFileDir, fileName)))
        |> asExe

    /// Compile and run a test file with --langversion:preview. No warnings allowed.
    let private compileAndRunPreview fileName =
        createTest fileName
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    /// Compile and run a test file with --langversion:preview and --optimize-. No warnings allowed.
    /// Debug (unoptimized) builds specialize inline bodies through a different path that must still
    /// replay the scope-aware extension-operator solution.
    let private compileAndRunPreviewNoOptimize fileName =
        createTest fileName
        |> withLangVersionPreview
        |> withNoOptimize
        |> compileAndRun
        |> shouldSucceed

    // ========================================================================
    // Positive tests: compile AND run cleanly, zero warnings
    // ========================================================================

    [<Fact>]
    let ``Extension operators solve SRTP constraints`` () =
        compileAndRunPreview "BasicExtensionOperators.fs"

    [<Fact>]
    let ``Nested Traverse-Sequence extension SRTP dispatch does not ICE and runs`` () =
        compileAndRunPreview "NestedTraverseSequenceSRTP.fs"

    // Regression: an SRTP constraint solved by an extension member on a struct must pass the
    // struct receiver by value (extension members compile to static methods). Previously the
    // witness took the receiver's address, emitting invalid IL (System.InvalidProgramException).
    [<Fact>]
    let ``Struct extension members solve SRTP constraints and run without invalid IL`` () =
        compileAndRunPreview "StructExtensionMemberSRTP.fs"

    // @gusty's miniFSharpPlus reproduction as a single end-to-end mini-library: extension
    // operators (++ >>= <*> |>>) and extension members solve SRTP constraints, the
    // Default1/2/3 return-type mechanism selects witnesses, and user-written generic SRTP code
    // consumes them at specific types. See the file header for the three documented adaptations.
    // Uses the AllowOverloadOnReturnType attribute. VERIFIED in-repo: the compilation cannot find
    // the attribute (the SDK's FSharp.Core does not yet ship it), so this genuinely takes the clean
    // FS0039 path. The guard is not a false-green here — it compiles and runs only once the attribute
    // is available. Kept conditional so the test tracks whichever FSharp.Core the harness resolves.
    [<Fact>]
    let ``Extension members and operators solve SRTP across a mini functor-monad library`` () =
        createTest "MiniFSharpPlusExtensionSRTP.fs"
        |> withLangVersionPreview
        |> compileAndRunOrExpectMissingAttribute "Microsoft.FSharp.Core.AllowOverloadOnReturnTypeAttribute"

    [<Fact>]
    let ``Most recently opened extension wins`` () =
        compileAndRunPreview "ExtensionPrecedence.fs"

    [<Fact>]
    let ``Duplicate built-in operator extension opening one dispatches to it`` () =
        compileAndRunPreview "DuplicateBuiltinOperatorOpenOne.fs"

    [<Fact>]
    let ``Duplicate built-in operator extension shadowing picks most recently opened`` () =
        compileAndRunPreview "DuplicateBuiltinOperatorShadow.fs"

    [<Fact>]
    let ``Duplicate built-in operator extensions in distinct scopes each keep their own choice`` () =
        compileAndRunPreview "DuplicateBuiltinOperatorDistinctScopes.fs"

    // Regression (S-2): the debug (--optimize-) inline-specialization path kept the definition-site range of
    // the trait, so replaying the recorded scope-aware solution found no match when two scopes solved the
    // same operator key differently. Both inline sites then fell back to the throwing dynamic '*' stub
    // (silent NotSupportedException at run, Debug-only). The optimizer must also try the user call-site range.
    [<Fact>]
    let ``Duplicate built-in operator inline functions in distinct scopes run under optimize-`` () =
        compileAndRunPreviewNoOptimize "DuplicateBuiltinOperatorInlineDistinctScopes.fs"

    [<Fact>]
    let ``Duplicate built-in operator extensions with equal operand types each keep their own choice`` () =
        compileAndRunPreview "DuplicateBuiltinOperatorSymmetricOperands.fs"

    [<Fact>]
    let ``Duplicate built-in operator extension on a generic type replays the correct instantiation`` () =
        compileAndRunPreview "DuplicateBuiltinOperatorGenericInstantiations.fs"

    // Regression (miscompile): a generic inline extension operator (its parameter/result mention its own
    // method typar) dispatched through an inline function's abstract SRTP support was committed at the
    // definition site with that typar undetermined. It defaulted to obj and baked a `box ^T; unbox.any
    // List<obj>` coercion into the stored inline body -> InvalidCastException at the concrete call site.
    // Must run correctly under BOTH optimize modes (the def-site body carries the bad coercion regardless).
    [<Fact>]
    let ``Generic inline extension operator via SRTP runs under optimize+`` () =
        compileAndRunPreview "InlineExtensionOperatorGenericReturnSRTP.fs"

    [<Fact>]
    let ``Generic inline extension operator via SRTP runs under optimize-`` () =
        compileAndRunPreviewNoOptimize "InlineExtensionOperatorGenericReturnSRTP.fs"

    [<Fact>]
    let ``Duplicate built-in operator extension differing only by return type replays the correct instantiation`` () =
        compileAndRunPreview "DuplicateBuiltinOperatorReturnTypeInstantiations.fs"

    [<Fact>]
    let ``Duplicate built-in operator extension inlines the opened one, not the dynamic fallback`` () =
        // IL-level proof of the shadow/duplicate fix: two same-signature '*' extensions on string exist in
        // one compilation; opening exactly A must inline A's distinctive body (Array.replicate) at the
        // concrete call site. Before the fix the optimizer could not disambiguate and emitted FSharp.Core's
        // throwing dynamic-operator stub instead — that marker must be absent.
        FSharpWithFileName "Test.fs" """
module DuplicateBuiltinOperatorIL
module A =
    type System.String with
        static member ( * ) (s: string, n: int) : string = System.String.Concat(Array.replicate n s)
module B =
    type System.String with
        static member ( * ) (s: string, n: int) : string = s + string n
open A
let r : string = "ha" * 2
if r <> "haha" then failwith $"Expected 'haha', got '{r}'"
"""
        |> asExe
        |> withLangVersionPreview
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed
        // A's body (Array.replicate -> String.Concat) inlined directly at the concrete call site, proving A
        // specifically was chosen; not B's (s + string n), and not the dynamic fallback.
        |> verifyILContains ["""
          IL_0001:  ldstr      "ha"
          IL_0006:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Replicate<string>(int32,
                                                                                                             !!0)
          IL_000b:  call       string [runtime]System.String::Concat(string[])"""]
        |> verifyILNotPresent [ "Dynamic invocation of op_Multiply" ]

    [<Fact>]
    let ``Two same-signature extension operators in one scope are rejected, not crashed (FS0037)`` () =
        // Redeclaring the same extension operator signature in a single scope must give the ordinary
        // duplicate-member error, exactly as for any member — the SRTP feature must not turn it into a crash.
        FSharpWithFileName "Test.fs" """
module Test
type System.String with
    static member ( * ) (s: string, n: int) : string = System.String.Concat(Array.create n s)
    static member ( * ) (s: string, n: int) : string = "B:" + s
let r : string = "ha" * 3
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 37

    [<Fact>]
    let ``open type with homograph operators yields all overloads for SRTP`` () =
        compileAndRunPreview "OpenTypeOperatorHomographOrder.fs"

    [<Fact>]
    let ``open type homograph operators across multiple holder types accumulate`` () =
        compileAndRunPreview "OpenTypeOperatorHomographMultipleHolders.fs"

    [<Fact>]
    let ``open type nested in a module scopes extension operator correctly`` () =
        compileAndRunPreview "OpenTypeOperatorNestedModule.fs"

    [<Fact>]
    let ``local let binding shadows open type extension operator`` () =
        compileAndRunPreview "OpenTypeOperatorShadowing.fs"

    [<Fact>]
    let ``open type SRTP dispatch selects overload per argument type across holders`` () =
        compileAndRunPreview "OpenTypeOperatorSRTPDispatch.fs"

    [<Fact>]
    let ``open type homograph overloads on single holder differ by parameter type`` () =
        compileAndRunPreview "OpenTypeOperatorOverloadByParam.fs"

    [<Fact>]
    let ``open type operator with CompiledName attribute resolves by F# symbol`` () =
        compileAndRunPreview "OpenTypeOperatorCompiledName.fs"

    [<Fact>]
    let ``open type extension operator crosses assembly boundary`` () =
        let library =
            FSharp """
module OpLib

[<AbstractClass; Sealed>]
type Ops =
    static member inline (+!) (a: int, b: int) = a + b + 7
    static member inline (+!) (a: string, b: string) = a + b + "_X"
            """
            |> withName "OpLib"
            |> asLibrary
            |> withLangVersionPreview

        FSharp """
module Consumer
open OpLib
open type Ops

let r1 : int = 10 +! 20
if r1 <> 37 then failwith (sprintf "Expected 37, got %d" r1)

let r2 : string = "a" +! "b"
if r2 <> "ab_X" then failwith (sprintf "Expected 'ab_X', got '%s'" r2)

let inline combine (a: ^T) (b: ^T) = a +! b
let r3 : int = combine 1 2
if r3 <> 10 then failwith (sprintf "Expected 10, got %d" r3)
        """
        |> asExe
        |> withLangVersionPreview
        |> withReferences [library]
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``open type named extension member crosses assembly boundary through SRTP`` () =
        // Companion to the operator cross-assembly test above, for a NAMED (non-operator)
        // extension member. Named members and operators take slightly different name-resolution
        // paths into SRTP, so this pins the named-member x cross-assembly x SRTP cell.
        let library =
            FSharp """
module NamedLib

[<AbstractClass; Sealed>]
type Ops =
    static member inline Widen (a: int) : int64 = int64 a + 100L
    static member inline Widen (a: string) : int64 = int64 a.Length + 200L
            """
            |> withName "NamedLib"
            |> asLibrary
            |> withLangVersionPreview

        FSharp """
module Consumer
open NamedLib
open type Ops

let inline widen (x: ^T) : int64 = ((^T or Ops) : (static member Widen : ^T -> int64) x)

let r1 = widen 5
if r1 <> 105L then failwith (sprintf "Expected 105, got %d" r1)

let r2 = widen "abc"
if r2 <> 203L then failwith (sprintf "Expected 203, got %d" r2)
        """
        |> asExe
        |> withLangVersionPreview
        |> withReferences [library]
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``cross-assembly SRTP resolution uses consumer scope, not definition-site capture`` () =
        // Cross-assembly, extension solutions are resolved from the CONSUMER's scope, not
        // captured from the library's definition site. This is the intended model per
        // RFC FS-1043 (Binary compatibility / pickling): the trait's possible extension
        // solutions and accessor domain are deliberately NOT serialized into compiled DLLs —
        // they exist only during in-process constraint solving — so a consumer of a compiled
        // inline function uses the extensions available at its OWN call site.
        //
        // The passing companion test above (`open type extension operator crosses assembly
        // boundary`) shows the supported path: the consumer opens the extension. Here the
        // consumer opens GenericLib but NOT StringOps, so — unlike the intra-assembly
        // ScopeCapture.fs, where definition-site capture travels within one assembly — the
        // String.(*) extension is not in the consumer's scope and resolution fails as designed.
        // This is a deliberate consistency trade-off, not a defect. If the design is ever
        // changed to serialize definition-site capture across assemblies (a pickle-format
        // change + RFC amendment), flip this to |> compileAndRun |> shouldSucceed ("hahaha").
        let library =
            FSharp """
module ScopeCaptureLib

module StringOps =
    type System.String with
        static member (*)(s: string, n: int) = System.String.Concat(Array.replicate n s)

module GenericLib =
    open StringOps
    let inline multiply (x: ^T) (n: int) = x * n
            """
            |> withName "ScopeCaptureLib"
            |> asLibrary
            |> withLangVersionPreview

        FSharp """
module Consumer
open ScopeCaptureLib.GenericLib
// Opens GenericLib only (not StringOps) — exactly as in ScopeCapture.fs.
let r = multiply "ha" 3
if r <> "hahaha" then failwith (sprintf "Expected 'hahaha', got '%s'" r)
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [library]
            |> compile
            |> shouldFail
            |> withErrorCode 1
            |> withDiagnosticMessageMatches "None of the types .*support the operator '\*'"

    [<Fact>]
    let ``Public extension operator solves an SRTP constraint`` () =
        compileAndRunPreview "ExtensionAccessibility.fs"

    [<Fact>]
    let ``internal extension member does not solve an SRTP constraint within an assembly`` () =
        // Only a public member may solve an SRTP constraint: a public inline function can be inlined
        // into another assembly where the internal member is inaccessible (MethodAccessException), so
        // it is rejected with FS0001 'is not public' at compile time. Internal-only-same-assembly SRTP
        // is a deferred use-site-accessibility design.
        FSharp """
module InternalIntraNoLeak
module InternalExt =
    type System.Int32 with
        static member internal Pong (x: int) = x + 200
open InternalExt
let inline pong (x: ^T) = (^T : (static member Pong : ^T -> ^T) x)
let r = pong 5
        """
        |> asExe
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "is not public"

    [<Fact>]
    let ``private extension member does not leak to a sibling module through SRTP`` () =
        // A 'private' extension is visible only in its defining module, so a sibling module that
        // opens A still cannot use it as a witness: the candidate is dropped ('does not support').
        FSharp """
module PrivateNoLeak
module A =
    type System.Int32 with
        static member private Secret (x: int) = x + 1
module B =
    open A
    let inline useSecret (x: ^T) = (^T : (static member Secret : ^T -> ^T) x)
    let r = useSecret 5
        """
        |> asExe
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "does not support the operator 'Secret'"

    [<Fact>]
    let ``private extension member captured at def-site does not solve an inline SRTP constraint`` () =
        // Regression: 'useSecret' and the private 'Secret' share module A, so def-site capture would
        // pick the private witness and inline it into B (inaccessible) -> MethodAccessException under
        // --realsig+ --optimize-. Found-but-rejected here yields FS0001 'is not public'.
        FSharp """
module PrivateCaptureNoLeak
module A =
    type System.Int32 with
        static member private Secret (x: int) = x + 1
    let inline useSecret (x: ^T) = (^T : (static member Secret : ^T -> ^T) x)
module B =
    let r = A.useSecret 5
        """
        |> asExe
        |> withLangVersionPreview
        |> withOptions [ "--realsig+"; "--optimize-" ]
        |> compile
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "is not public"

    [<Fact>]
    let ``private intrinsic member captured at def-site does not solve an inline SRTP constraint`` () =
        // Regression: the leak is not extension-specific. 'UseSecret' is declared INSIDE 'Foo', so it
        // has private def-site access to 'Foo.Secret'; capturing it would inline an inaccessible call
        // into B. Proven: pre-fix this compiled and crashed at run time with MethodAccessException.
        FSharp """
module IntrinsicCaptureNoLeak
type Foo =
    { V: int }
    static member private Secret (x: Foo) = { V = x.V + 1 }
    static member inline UseSecret (x: ^T) = (^T : (static member Secret : ^T -> ^T) x)
module B =
    let r = Foo.UseSecret { V = 5 }
        """
        |> asExe
        |> withLangVersionPreview
        |> withOptions [ "--realsig+"; "--optimize-" ]
        |> compile
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "is not public"

    [<Fact>]
    let ``private op_Implicit conversion does not solve an SRTP constraint`` () =
        // The op_Implicit/op_Explicit conversion arm must honor the same public-only rule; it is a
        // different resolution path than named members, so it is pinned separately.
        FSharp """
module PrivImplicitNoLeak
module A =
    type Wrap = { X: int }
    type Wrap with
        static member private op_Implicit (w: Wrap) : int = w.X
    let inline conv (x: ^T) : int = ((^T) : (static member op_Implicit : ^T -> int) x)
module B =
    let r = A.conv { A.Wrap.X = 5 }
        """
        |> asExe
        |> withLangVersionPreview
        |> withOptions [ "--realsig+"; "--optimize-" ]
        |> compile
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "does not support a conversion"

    [<Fact>]
    let ``internal op_Implicit conversion does not solve an SRTP constraint`` () =
        // Companion to the 'private op_Implicit' test for the conversion arm, and the conversion
        // analogue of the public-only witness rule: an 'internal' op_Implicit extension is dropped
        // as an SRTP witness even though it is accessible within the same assembly, because a public
        // inline function carrying it could be inlined into another assembly where it is inaccessible.
        // The candidate is filtered out, so the constraint reports 'does not support a conversion'.
        FSharp """
module InternalConvNoLeak
module ConvExt =
    type Wrap = { X: int }
    type Wrap with
        static member internal op_Implicit (w: Wrap) : int = w.X
open ConvExt
let inline toInt (x: ^T) : int = ((^T) : (static member op_Implicit : ^T -> int) x)
let r = toInt { X = 5 }
        """
        |> asExe
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 9, Col 15, Line 9, Col 24, "The type 'Wrap' does not support a conversion to the type 'int'")
        ]

    [<Fact>]
    let ``public member in an internal container does not solve an SRTP constraint`` () =
        // The public-only witness rule uses EFFECTIVE accessibility: a 'public' member declared inside
        // an 'internal' module has effective accessibility 'internal', so it is rejected as an SRTP
        // witness (FS0001 'is not public') just like a directly-internal member — making the module
        // public instead compiles and runs. Guards that the rule looks through the container, not only
        // the member's own accessibility keyword.
        FSharp """
module PubInInternalContainer
module internal Hidden =
    type System.Int32 with
        static member Boost (x: int) = x + 50
open Hidden
let inline boost (x: ^T) = (^T : (static member Boost : ^T -> ^T) x)
let r = boost 5
        """
        |> asExe
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 8, Col 15, Line 8, Col 16, "The member or object constructor 'Boost' is not public. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
        ]

    [<Fact>]
    let ``internal extension member does not leak across an assembly boundary through SRTP`` () =
        // An 'internal' extension member is not accessible from another assembly (absent IVT), so
        // it must not solve an SRTP constraint in a referencing assembly. Pins that accessibility
        // is honored across the assembly boundary, not only within one compilation unit.
        let library =
            FSharp """
module InternalNoLeakLib
type System.Int32 with
    static member internal Hidden (x: int) = x + 1
            """
            |> withName "InternalNoLeakLib"
            |> asLibrary
            |> withLangVersionPreview

        FSharp """
module Consumer
open InternalNoLeakLib
let inline useHidden (x: ^T) = (^T : (static member Hidden : ^T -> ^T) x)
let r = useHidden 5
            """
        |> asExe
        |> withLangVersionPreview
        |> withReferences [library]
        |> compile
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "does not support the operator 'Hidden'"

    [<Fact>]
    let ``internal extension witness is rejected across an assembly boundary even with InternalsVisibleTo`` () =
        // Sharper than the non-IVT companion above: here [<InternalsVisibleTo>] makes the 'internal'
        // extension member genuinely ACCESSIBLE in the friend consumer, so it is no longer 'does not
        // support the operator' (candidate invisible) — instead it is FOUND and then rejected by the
        // public-only witness rule with FS0001 'is not public'. IVT accessibility does not make an
        // internal member a valid cross-assembly SRTP witness. (Control: making the member public
        // compiles and runs; without IVT it reverts to 'does not support the operator'.)
        let library =
            FSharp """
module IvtLib
open System.Runtime.CompilerServices
[<assembly: InternalsVisibleTo("friend")>]
do ()
module Ext =
    type System.Int32 with
        static member internal Hidden (x: int) = x + 1
            """
            |> withName "IvtLib"
            |> asLibrary
            |> withLangVersionPreview

        FSharp """
module Consumer
open IvtLib.Ext
let inline useHidden (x: ^T) = (^T : (static member Hidden : ^T -> ^T) x)
let r = useHidden 5
            """
        |> withName "friend"
        |> asExe
        |> withLangVersionPreview
        |> withReferences [library]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 5, Col 19, Line 5, Col 20, "The member or object constructor 'Hidden' is not public. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
        ]

    [<Fact>]
    let ``multiple SRTP constraints solved by public extension members run under realsig`` () =
        // Positive counterpart to the realsig negatives above: two SRTP constraints in one inline
        // function, both solved by public extension members, must compile and run correctly under
        // --realsig+ --optimize- (the strict-codegen mode where earlier accessibility leaks surfaced).
        FSharp """
module MultiExtRealsig
module Ops =
    type System.Int32 with
        static member Inc (x: int) = x + 1
        static member Dbl (x: int) = x * 2
open Ops
let inline transform (x: ^T) =
    let a = (^T : (static member Inc: ^T -> ^T) x)
    (^T : (static member Dbl: ^T -> ^T) a)
if transform 10 <> 22 then failwith "wrong result"
        """
        |> asExe
        |> withLangVersionPreview
        |> withOptions [ "--realsig+"; "--optimize-" ]
        |> compileAndRun
        |> shouldSucceed


    // declared in one file and consumed from another. Written once and reused by the plain and the
    // signature-file variants, which exercise the exact same impl and consumer.
    let private extStringMultiplyImpl = """
module Lib.Ext
type System.String with
    static member ( * ) (s: string, n: int) = System.String.Concat(Array.create n s)
"""

    let private consumerUsesStar = """
module Consumer
open Lib.Ext
let r : string = "ha" * 3
if r <> "hahaha" then failwith (sprintf "Expected 'hahaha', got '%s'" r)
"""

    [<Fact>]
    let ``Extrinsic extension captured at definition site resolves across modules`` () =
        // Intra-assembly definition-site capture: an inline function carries the extensions in scope at its
        // own definition to a caller that never opened them. Positive counterpart to the cross-assembly
        // test above, which shows the capture stops at the assembly boundary.
        compileAndRunPreview "ScopeCapture.fs"

    [<Fact>]
    let ``Cross-file extension operator in same assembly emits the extension, not the dynamic fallback`` () =
        // Regression: an extension operator in an earlier file was invisible to trait-witness generation for
        // a later file of the same assembly, so the constraint fell back to FSharp.Core's dynamic operator
        // and threw at runtime. The IL must not contain the dynamic-fallback marker.
        FSharpWithFileName "Ext.fs" extStringMultiplyImpl
        |> withAdditionalSourceFile (FsSourceWithFileName "Consumer.fs" consumerUsesStar)
        |> asExe
        |> withLangVersionPreview
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed
        // The extension body must be inlined at the use site: literal "ha" -> Array.create -> String.Concat,
        // i.e. the resolved witness is the extension, not FSharp.Core's dynamic operator.
        |> verifyILContains ["""
          IL_0001:  ldstr      "ha"
          IL_0006:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<string>(int32,
                                                                                                           !!0)
          IL_000b:  call       string [runtime]System.String::Concat(string[])"""]
        |> verifyILNotPresent [ "Dynamic invocation of op_Multiply" ]

    [<Fact>]
    let ``Cross-file inline SRTP function is solved by an extension operator in an earlier file`` () =
        // Same two-file assembly, but the later file calls an inline SRTP function whose constraint the
        // earlier extension solves — proving the witness inlines across files, not only within one.
        FSharpWithFileName "Ext.fs" """
module Lib.Ext
type System.String with
    static member ( * ) (s: string, n: int) = System.String.Concat(Array.create n s)
let inline multiply (x: ^T) (n: int) : ^T = x * n
        """
        |> withAdditionalSourceFile (FsSourceWithFileName "Consumer.fs" """
module Consumer
open Lib.Ext
let r : string = multiply "ha" 3
if r <> "hahaha" then failwith (sprintf "Expected 'hahaha', got '%s'" r)
        """)
        |> asExe
        |> withLangVersionPreview
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed
        // The inline SRTP function's witness inlines across the file boundary all the way to the call site:
        // the consumer's static initializer is the extension body, not a dynamic-operator dispatch.
        |> verifyIL ["""
          IL_0001:  ldstr      "ha"
          IL_0006:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<string>(int32,
                                                                                                           !!0)
          IL_000b:  call       string [runtime]System.String::Concat(string[])"""]

    [<Fact>]
    let ``Cross-file extension operator with an explicit signature file resolves through SRTP`` () =
        // Sharpest signature/impl split: the extension is declared in an explicit .fsi, so its signature val
        // identity differs most from the impl val. The witness must reference the signature val (what code
        // generation binds across files), not the impl val.
        Fsi """
module Lib.Ext
type System.String with
    static member ( * ) : s: string * n: int -> string
        """
        |> withFileName "Ext.fsi"
        |> withAdditionalSourceFiles
            [ FsSourceWithFileName "Ext.fs" extStringMultiplyImpl
              FsSourceWithFileName "Consumer.fs" consumerUsesStar ]
        |> asExe
        |> withLangVersionPreview
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Extension operator from an earlier FSI submission solves an SRTP constraint in a later one`` () =
        // In FSI each ';;' submission compiles to its own dynamic assembly. An extension operator defined
        // in one submission must stay visible to trait-witness generation in later submissions; otherwise
        // the constraint falls back to FSharp.Core's dynamic operator and throws NotSupportedException at
        // runtime. The optimizer's incremental (FSI) path binds contents vals, so earlier submissions must
        // contribute their contents (not signatures) to the trait context.
        use script = new FSharpScript(langVersion = LangVersion.Preview)

        let _, defErrors =
            script.Eval """
type System.String with
    static member ( * ) (s: string, n: int) : string = System.String.Concat(Array.create n s)"""
        Assert.Empty defErrors

        let result, useErrors = script.Eval """ "ha" * 3 """
        Assert.Empty useErrors
        match result with
        | Result.Ok (Some value) -> Assert.Equal("hahaha", value.ReflectionValue :?> string)
        | Result.Ok None -> failwith "Expected a value from the second submission"
        | Result.Error ex -> raise ex

    [<Fact>]
    let ``Extrinsic extension not captured without ExtensionConstraintSolutions`` () =
        // Without the feature the (*) extension is not captured into the SRTP constraint, so 'multiply'
        // resolves x*n as int arithmetic and the string call site mismatches. Pin that specific int/string
        // mismatch so the test cannot pass on some unrelated FS0001.
        createTest "ScopeCapture.fs"
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "(?s)expected to have type.*'int'.*but here has type.*'string'"

    [<Fact>]
    let ``Sequentialized InvokeMap pattern compiles and runs`` () =
        compileAndRunPreview "WeakResolution.fs"

    [<Fact>]
    let ``op_Explicit return type disambiguation`` () =
        compileAndRunPreview "OpExplicitReturnType.fs"

    [<Fact>]
    let ``optional extension op_Explicit solves an SRTP constraint by return type`` () =
        compileAndRunPreview "OpExplicitOptionalExtension.fs"

    [<Fact>]
    let ``AllowOverloadOnReturnType resolves through SRTP`` () =
        // VERIFIED in-repo: the compilation cannot find AllowOverloadOnReturnType (the SDK's
        // FSharp.Core does not yet ship it), so this genuinely takes the clean FS0039 path — not a
        // false-green. It compiles and runs only once the attribute is available; kept conditional
        // so the test tracks whichever FSharp.Core the harness resolves.
        createTest "AllowOverloadOnReturnType.fs"
        |> withLangVersionPreview
        |> compileAndRunOrExpectMissingAttribute "Microsoft.FSharp.Core.AllowOverloadOnReturnTypeAttribute"

    [<Fact>]
    let ``Issue 9382 and 9416 regressions compile and run`` () =
        compileAndRunPreview "IssueRegressions.fs"

    [<Fact>]
    let ``DateTime plus y compiles and runs with preview`` () =
        // Prior to RFC-1043, weak resolution eagerly resolved this to
        // DateTime -> TimeSpan -> DateTime. Now it stays generic because
        // weak resolution is deferred for inline code.
        // H1: Prove y is truly generic by calling f1 with two different types.
        // M4: Exercises a runtime-verified extension operator on DateTime.
        FSharp """
module WeakResDateTime
open System

type MyOffset = { Hours: float }

type System.DateTime with
    static member (+) (dt: DateTime, off: MyOffset) = dt.AddHours(off.Hours)

let inline f1 (x: DateTime) y = x + y

// Call 1: y = TimeSpan (built-in DateTime + TimeSpan)
let r1 = f1 DateTime.MinValue (TimeSpan.FromHours(1.0))

// Call 2: y = MyOffset (extension DateTime + MyOffset)
// This ONLY compiles if y is generic — proves weak resolution deferral works
let r2 = f1 DateTime.MinValue { Hours = 2.0 }

// Verify both calls produce correct results
let expected1 = DateTime.MinValue.Add(TimeSpan.FromHours(1.0))
if r1 <> expected1 then failwith (sprintf "r1: Expected %A, got %A" expected1 r1)

let expected2 = DateTime.MinValue.AddHours(2.0)
if r2 <> expected2 then failwith (sprintf "r2: Expected %A, got %A" expected2 r2)
        """
        |> asExe
        |> withLangVersionPreview
        |> withOptions ["--nowarn:52"]
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``FSharpPlus Default1 Default2 priority pattern fails without explicit constraint`` () =
        // M3: FSharpPlus inheritance-based overload priority (Default1 inherits Default2).
        // Currently, the SRTP constraint on (^T or Default1) does not resolve
        // the Default2 fallback overload for non-int types. This test documents
        // the limitation: the pattern requires the constraint witness type to
        // directly declare the member, inheritance alone is not sufficient.
        FSharp """
module Test

type Default2 = class end
type Default1 = inherit Default2

type Resolver =
    static member Resolve(_: 'T, _: Default2) = "default"
    static member Resolve(x: int, _: Default1) = sprintf "int:%d" x

let inline resolve (x: ^T) =
    let d = Unchecked.defaultof<Default1>
    ((^T or Default1) : (static member Resolve: ^T * Default1 -> string) (x, d))

let r1 = resolve 42
let r2 = resolve "hello"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 887, Line 5, Col 17, Line 5, Col 33, "The type 'Default2' is not an interface type")
            (Error 1, Line 15, Col 18, Line 15, Col 20, "None of the types 'int, Default1' support the operator 'Resolve'")
            (Error 1, Line 16, Col 18, Line 16, Col 25, "None of the types 'string, Default1' support the operator 'Resolve'")
        ]

    [<Fact>]
    let ``Built-in operator wins over extension on same type`` () =
        FSharp """
module Test
type System.Int32 with
    static member (+) (a: int, b: int) = a * b  // deliberately wrong

let r1 = 1 + 2  // built-in must win, not the extension
if r1 <> 3 then failwith (sprintf "Expected 3, got %d" r1)

let inline addGeneric (x: ^T) (y: ^T) = x + y
let r2 = addGeneric 1 2  // built-in must win even through SRTP
if r2 <> 3 then failwith (sprintf "Expected 3, got %d" r2)
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    // ========================================================================
    // Negative tests: assert specific diagnostics
    // ========================================================================

    [<Fact>]
    let ``numeric widening via extension operators does not compose with built-in operators (by design)`` () =
        // RFC FS-1043 "Widening to specific type" example. Design point 5: for a type that
        // already carries a built-in operator, the built-in solution is committed before any
        // extension member is considered. So `1 + 2L` resolves the built-in int (+), which forces
        // both operands to int and rejects 2L — the widening extension on int64 is never reached.
        // This is why numeric widening is documented as aspirational / NOT IMPLEMENTED
        // (docs/RFC_Changes.md § Examples, docs/srtp-guide.md § Aspirational Patterns). This test
        // guards that scope-out so the behavior can't silently change unnoticed.
        FSharp """
module Test
let inline widen_to_int64 (x: ^T) : int64 = (^T : (static member widen_to_int64 : ^T -> int64) (x))
type System.Int32 with
    static member inline widen_to_int64 (a: int32) : int64 = int64 a
type System.Int64 with
    static member inline (+)(a: int64, b: 'T) : int64 = a + widen_to_int64 b
    static member inline (+)(a: 'T, b: int64) : int64 = widen_to_int64 a + b

let r = 1 + 2L
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "The type 'int64' does not match the type 'int'"

    [<Fact>]
    let ``FS1215 warning suppressed when ExtensionConstraintSolutions is active`` () =
        Fsx """
type System.String with
    static member (*) (s: string, n: int) = System.String.Concat(Array.replicate n s)
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``FS1215 warning emitted without ExtensionConstraintSolutions`` () =
        Fsx """
type System.String with
    static member (*) (s: string, n: int) = System.String.Concat(Array.replicate n s)
        """
        |> withLangVersion80
        |> compile
        |> withDiagnostics [
            Warning 1215, Line 3, Col 19, Line 3, Col 22, "Extension members cannot provide operator overloads.  Consider defining the operator as part of the type definition instead."
        ]

    [<Fact>]
    let ``FSharpPlus Sequence pattern fails to compile`` () =
        Fsx """
let inline CallReturn< ^M, ^R, 'T when (^M or ^R) : (static member Return : unit -> ('T -> ^R))> () =
    ((^M or ^R) : (static member Return : unit -> ('T -> ^R)) ())

let inline CallApply< ^M, ^I1, ^I2, ^R when (^M or ^I1 or ^I2) : (static member Apply : ^I1 * ^I2 -> ^R)> (input1: ^I1, input2: ^I2) =
    ((^M or ^I1 or ^I2) : (static member Apply : ^I1 * ^I2 -> ^R) input1, input2)

let inline CallMap< ^M, ^F, ^I, ^R when (^M or ^I or ^R) : (static member Map : ^F * ^I -> ^R)> (mapping: ^F, source: ^I) : ^R =
    ((^M or ^I or ^R) : (static member Map : ^F * ^I -> ^R) mapping, source)

let inline CallSequence< ^M, ^I, ^R when (^M or ^I) : (static member Sequence : ^I -> ^R)> (b: ^I) : ^R =
    ((^M or ^I) : (static member Sequence : ^I -> ^R) b)

type Return = class end
type Apply = class end
type Map = class end
type Sequence = class end

let inline InvokeReturn (x: 'T) : ^R = CallReturn< Return, ^R, 'T> () x
let inline InvokeApply (f: ^I1) (x: ^I2) : ^R = CallApply<Apply, ^I1, ^I2, ^R>(f, x)
let inline InvokeMap (mapping: ^F) (source: ^I) : ^R = CallMap<Map, ^F, ^I, ^R> (mapping, source)

type Sequence with
    static member inline Sequence (t: list<option<'t>>) : ^R =
        List.foldBack (fun (x: 't option) (ys: ^R) -> InvokeApply (InvokeMap (fun x y -> x :: y) x) ys) t (InvokeReturn [])

type Map with
    static member Map (f: 'T->'U, x: option<_>) = Option.map f x

type Apply with
    static member Apply (f: option<_>, x: option<'T>) : option<'U> = failwith ""

type Return with
    static member Return () = fun x -> Some x : option<'a>

let res = CallSequence<Sequence, _, _> [Some 3; Some 2; Some 1]
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            // Weak resolution deferral leaves the inner InvokeMap return type unresolved before
            // generalization, triggering the value restriction on `res`.
            (Error 30, Line 36, Col 5, Line 36, Col 8, "Value restriction: The value 'res' has an inferred generic type
val res: '_a list option
However, values cannot have generic type variables like '_a in \"let x: '_a\". You can do one of the following:
- Define it as a simple data term like an integer literal, a string literal or a union case like \"let x = 1\"
- Add an explicit type annotation like \"let x : int\"
- Use the value as a non-generic type in later code for type inference like \"do x\"
or if you still want type-dependent results, you can define 'res' as a function instead by doing either:
- Add a unit parameter like \"let x()\"
- Write explicit type parameters like \"let x<'a>\".
This error is because a let binding without parameters defines a value, not a function. Values cannot be generic because reading a value is assumed to result in the same everywhere but generic type parameters may invalidate this assumption by enabling type-dependent results.")
        ]

    [<Fact>]
    let ``Issue 8794 - Shadowing member return type produces ambiguity error`` () =
        // When Daughter shadows Mother.Hello() with a different return type,
        // the member constraint finds both overloads and reports ambiguity.
        // Not directly RFC FS-1043 — documents current member constraint behavior.
        Fsx """
type Mother() =
    member this.Hello() = Unchecked.defaultof<int>

type Daughter() =
    inherit Mother()
    member this.Hello() = Unchecked.defaultof<string>

type SomeoneHolder<'Someone when 'Someone: (member Hello : unit -> string)> =
    { Someone: 'Someone }

let someoneHolder = { Someone = Daughter() }
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            // Shadowing Mother.Hello() with Daughter.Hello() (different return type)
            // creates overload ambiguity for the member constraint.
            (Error 193, Line 12, Col 33, Line 12, Col 43, "A unique overload for method 'Hello' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known return type: string

Candidates:
 - member Daughter.Hello: unit -> string
 - member Mother.Hello: unit -> int")
        ]

    [<FactForNETCOREAPP>]
    let ``Extension does not satisfy IWSAM constraint`` () =
        // M1: Extension (+) should NOT make a type satisfy IAdditionOperators.
        // SRTP extension solutions and interface implementations are orthogonal.
        // IWSAM BCL type IAdditionOperators is only available on .NET 7+, so gate to NETCOREAPP.
        FSharp """
module Test
open System.Numerics

type MyNum = { V: int }

type MyNum with
    static member (+) (a: MyNum, b: MyNum) = { V = a.V + b.V }

let addViaIWSAM<'T when 'T :> IAdditionOperators<'T,'T,'T>> (a: 'T) (b: 'T) = a + b
let r = addViaIWSAM { V = 1 } { V = 2 }
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 11, Col 21, Line 11, Col 30, "The type 'MyNum' is not compatible with the type 'IAdditionOperators<MyNum,MyNum,MyNum>'")
        ]

    [<Fact>]
    let ``Extension not in scope is not resolved`` () =
        FSharp """
namespace Test

module Exts =
    type System.Int32 with
        static member Zing(x: int) = x + 999

module Consumer =
    let inline zing (x: ^T) = (^T : (static member Zing: ^T -> ^T) x)
    let r = zing 5  // Exts not opened, should fail
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 10, Col 18, Line 10, Col 19, "The type 'int' does not support the operator 'Zing'")
        ]

    [<Fact>]
    let ``Extension operator on FSharpFunc type`` () =
        FSharp """
module Test

type Microsoft.FSharp.Core.FSharpFunc<'t, 'u> with
    static member (|>>) (f: 't -> 'u, g: 'u -> 'v) : 't -> 'v = f >> g

let composed = string |>> List.singleton
let result = composed 5
if result <> ["5"] then failwith $"Expected [\"5\"], got {result}"
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Extension operator on FSharpFunc - piped usage`` () =
        FSharp """
module Test

type Microsoft.FSharp.Core.FSharpFunc<'t, 'u> with
    static member (|>>) (f: 't -> 'u, g: 'u -> 'v) : 't -> 'v = f >> g

// This tests: 5 |> (string |>> List.singleton)
let x02 = 5 |> (string |>> List.singleton)
if x02 <> ["5"] then failwith $"Expected [\"5\"], got {x02}"
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Extension operator on FSharpFunc with nested SRTP - map squared`` () =
        // Tests |>>> ("map squared") which uses flip and |>> with FSharpFunc extension
        // This is the test case from miniFSharpPlus.fsx
        FSharp """
module Test

type List<'t> with
    static member (|>>) (x: list<'t>, f: 't -> 'u) : list<'u> = List.map f x

type Option<'t> with
    static member (|>>) (x: option<'t>, f: 't -> 'u) : option<'u> = Option.map f x

type Microsoft.FSharp.Core.FSharpFunc<'t, 'u> with
    static member (|>>) (f: 't -> 'u, g: 'u -> 'v) : 't -> 'v = f >> g

let inline flip f x y = f y x

type List<'t> with
    static member inline (|>>>) (x: list<'MonadT>, f) = (flip (|>>) >> flip (|>>)) f x

// Test: apply |>>> to a list of options
let x07 = [Some 1] |>>> string
if x07 <> [Some "1"] then failwith $"Expected [Some \"1\"], got {x07}"
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Extension operator on FSharpFunc with deeply nested SRTP - map cubed`` () =
        // Tests |>>>> ("map cubed") which uses three levels of flip and |>>
        // This previously caused "Undefined or unsolved type variable" regression
        FSharp """
module Test

type List<'t> with
    static member (|>>) (x: list<'t>, f: 't -> 'u) : list<'u> = List.map f x

type Option<'t> with
    static member (|>>) (x: option<'t>, f: 't -> 'u) : option<'u> = Option.map f x

type Microsoft.FSharp.Core.FSharpFunc<'t, 'u> with
    static member (|>>) (f: 't -> 'u, g: 'u -> 'v) : 't -> 'v = f >> g

let inline flip f x y = f y x

type List<'t> with
    static member inline (|>>>) (x: list<'MonadT>, f) = (flip (|>>) >> flip (|>>)) f x

type List<'t> with
    static member inline (|>>>>) (x: list<'Monad2T>, f) = (flip (|>>) >> flip (|>>) >> flip (|>>)) f x

// Test: apply |>>>> to a nested structure
let x08 = [[Some 1]] |>>>> string
if x08 <> [[Some "1"]] then failwith $"Expected [[Some \"1\"]], got {x08}"

let x09 = [Some [1]] |>>>> string
if x09 <> [Some ["1"]] then failwith $"Expected [Some [\"1\"]], got {x09}"
        """
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
