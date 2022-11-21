module FSharp.Compiler.Service.Tests.SyntaxTreeTests.SignatureTypeTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

[<Test>]
let ``Range of Type should end at end keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """namespace GreatProjectThing

type Meh =
        class
        end


// foo"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types(range = r)]) ])) ->
        assertRange (3, 0) (5,11) r
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of SynTypeDefnSig record should end at last member`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """namespace X
type MyRecord =
    { Level: int }
    member Score : unit -> int"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([SynTypeDefnSig.SynTypeDefnSig(range=mSynTypeDefnSig)], mTypes)]) ])) ->
        assertRange (2, 0) (4, 30) mTypes
        assertRange (2, 5) (4, 30) mSynTypeDefnSig
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of SynTypeDefnSig object model should end at last member`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """namespace X
type MyRecord =
    class
    end
    member Score : unit -> int"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([SynTypeDefnSig.SynTypeDefnSig(range=mSynTypeDefnSig)], mTypes)]) ])) ->
        assertRange (2, 0) (5, 30) mTypes
        assertRange (2, 5) (5, 30) mSynTypeDefnSig
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of SynTypeDefnSig delegate of should start from name`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """namespace Y
type MyFunction =
    delegate of int -> string"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([SynTypeDefnSig.SynTypeDefnSig(range=mSynTypeDefnSig)], mTypes) ]) ])) ->
        assertRange (2, 0) (3, 29) mTypes
        assertRange (2, 5) (3, 29) mSynTypeDefnSig
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of SynTypeDefnSig simple should end at last val`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """namespace Z
type SomeCollection with
    val LastIndex : int
    val SomeThingElse : int -> string"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([SynTypeDefnSig.SynTypeDefnSig(range=mSynTypeDefnSig)], mTypes)]) ])) ->
        assertRange (2, 0) (4, 37) mTypes
        assertRange (2, 5) (4, 37) mSynTypeDefnSig
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in SynTypeDefnSig`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace SomeNamespace

[<Foo1>]
type MyType =
    class
    end
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types(types = [SynTypeDefnSig.SynTypeDefnSig(range = r)]) as t]) ])) ->
        assertRange (4, 0) (7, 7) r
        assertRange (4, 0) (7, 7) t.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attributes should be included in recursive types`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace SomeNamespace

type Foo =
    | Bar

and [<CustomEquality>] Bang =
    internal
        {
            LongNameBarBarBarBarBarBarBar: int
        }
        override GetHashCode : unit -> int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([
            SynTypeDefnSig.SynTypeDefnSig(range = r1)
            SynTypeDefnSig.SynTypeDefnSig(range = r2)
        ], mTypes)]) ])) ->
        assertRange (4, 5) (5, 9) r1
        assertRange (7, 4) (12, 42) r2
        assertRange (4, 0) (12, 42) mTypes
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in SynValSpfn and Member`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace SomeNamespace

type FooType =
    [<Foo2>] // ValSpfn
    abstract x : int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(decls =
            [ SynModuleSigDecl.Types(types = [
                SynTypeDefnSig.SynTypeDefnSig(typeRepr =
                    SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                        SynMemberSig.Member(range = mr; memberSig = SynValSig(range = mv)) ]))
            ]) ]) ])) ->
        assertRange (5, 4) (6, 20) mr
        assertRange (5, 4) (6, 20) mv
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefnSig with ObjectModel Delegate contains the range of the equals sign`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace Foo

type X = delegate of string -> string
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Types(
            types = [ SynTypeDefnSig(trivia = { EqualsRange = Some mEquals }
                                     typeRepr = SynTypeDefnSigRepr.ObjectModel(kind = SynTypeDefnKind.Delegate _)) ]
        )
    ]) ])) ->
        assertRange (4, 7) (4, 8) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefnSig with ObjectModel class contains the range of the equals sign`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace SomeNamespace

type Foobar =
    class
    end
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Types(
            types = [ SynTypeDefnSig(trivia = { EqualsRange = Some mEquals }
                                     typeRepr = SynTypeDefnSigRepr.ObjectModel(kind = SynTypeDefnKind.Class)) ]
        )
    ]) ])) ->
        assertRange (4, 12) (4, 13) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefnSig with Enum contains the range of the equals sign`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace SomeNamespace

type Bear =
    | BlackBear = 1
    | PolarBear = 2
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Types(
            types = [ SynTypeDefnSig(trivia = { EqualsRange = Some mEquals }
                                     typeRepr = SynTypeDefnSigRepr.Simple(repr =
                                         SynTypeDefnSimpleRepr.Enum(cases = [
                                            SynEnumCase(trivia={ EqualsRange = mEqualsEnumCase1 })
                                            SynEnumCase(trivia={ EqualsRange = mEqualsEnumCase2 })
                                     ]) )) ]
        )
    ]) ])) ->
        assertRange (4, 10) (4, 11) mEquals
        assertRange (5, 16) (5, 17) mEqualsEnumCase1
        assertRange (6, 16) (6, 17) mEqualsEnumCase2
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefnSig with Union contains the range of the equals sign`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace SomeNamespace

type Shape =
| Square of int 
| Rectangle of int * int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Types(
            types = [ SynTypeDefnSig(trivia = { EqualsRange = Some mEquals }
                                     typeRepr = SynTypeDefnSigRepr.Simple(repr = SynTypeDefnSimpleRepr.Union _)) ]
        )
    ]) ])) ->
        assertRange (4, 11) (4, 12) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefnSig should contains the range of the with keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace X

type Foo with
member Meh : unit -> unit
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents =[ SynModuleOrNamespaceSig(decls =[
        SynModuleSigDecl.Types(
            types=[ SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.Simple _
                                   trivia = { WithKeyword = Some mWithKeyword }) ]
        )
    ]) ])) ->
        assertRange (4, 9) (4, 13) mWithKeyword
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExceptionSig should contains the range of the with keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace X

exception Foo with
member Meh : unit -> unit
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Exception(
            exnSig=SynExceptionSig(withKeyword = Some mWithKeyword)
        )
    ]) ])) ->
        assertRange (4, 14) (4, 18) mWithKeyword
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``memberSig of SynMemberSig.Member should contains the range of the with keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace X

type Foo =
    abstract member Bar : int with get,set
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Types(
            types=[ SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.ObjectModel(memberSigs=[SynMemberSig.Member(memberSig=SynValSig(trivia = { WithKeyword = Some mWithKeyword }))])) ]
        )
    ]) ])) ->
        assertRange (5, 30) (5, 34) mWithKeyword
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in SynExceptionDefnRepr and SynExceptionSig`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module internal FSharp.Compiler.ParseHelpers

// The error raised by the parse_error_rich function, which is called by the parser engine
[<NoEquality; NoComparison>]
exception SyntaxError of obj * range: range


"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Exception(
                SynExceptionSig(exnRepr=SynExceptionDefnRepr(range=mSynExceptionDefnRepr); range=mSynExceptionSig), mException)
        ] ) ])) ->
        assertRange (5, 0) (6, 43) mSynExceptionDefnRepr
        assertRange (5, 0) (6, 43) mSynExceptionSig
        assertRange (5, 0) (6, 43) mException
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of members should be included in SynExceptionSig and SynModuleSigDecl.Exception`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module internal FSharp.Compiler.ParseHelpers

exception SyntaxError of obj * range: range with
    member Meh : string -> int

open Foo
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Exception(
                SynExceptionSig(exnRepr=SynExceptionDefnRepr(range=mSynExceptionDefnRepr); range=mSynExceptionSig), mException)
            SynModuleSigDecl.Open _
        ] ) ])) ->
        assertRange (4, 0) (4, 43) mSynExceptionDefnRepr
        assertRange (4, 0) (5, 30) mSynExceptionSig
        assertRange (4, 0) (5, 30) mException
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Val keyword is present in SynValSig`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module Meh

[<Foo>]
// meh
val a : int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Val(valSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.Val mVal }))
        ] ) ])) ->
        assertRange (6, 0) (6, 3) mVal
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Equals token is present in SynValSig value`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module Meh

val a : int = 9
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Val(valSig = SynValSig(trivia = { EqualsRange = Some mEquals }); range = mVal)
        ] ) ])) ->
        assertRange (4, 12) (4, 13) mEquals
        assertRange (4, 0) (4, 15) mVal
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Equals token is present in SynValSig member`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module Meh

type X =
    member a : int = 10
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                    SynMemberSig.Member(memberSig = SynValSig(trivia = { EqualsRange = Some mEquals }); range = mMember)
                ]))
            ])
        ] ) ])) ->
        assertRange (5, 19) (5, 20) mEquals
        assertRange (5, 4) (5, 23) mMember
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Trivia is present in SynTypeDefnSig`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module Meh

type X =
    member a : int = 10

/// Represents a line number when using zero-based line counting (used by Visual Studio)
#if CHECK_LINE0_TYPES

#else
type Y = int
#endif

type Z with
    static member P : int -> int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(trivia = { LeadingKeyword = SynTypeDefnLeadingKeyword.Type mType1
                                          EqualsRange = Some mEq1
                                          WithKeyword = None }) ])
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(trivia = { LeadingKeyword = SynTypeDefnLeadingKeyword.Type mType2
                                          EqualsRange = Some mEq2
                                          WithKeyword = None  }) ])
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(trivia = { LeadingKeyword = SynTypeDefnLeadingKeyword.Type mType3
                                          EqualsRange = None
                                          WithKeyword = Some mWith3 }) ])
        ] ) ])) ->
        ()
        assertRange (4, 0) (4, 4) mType1
        assertRange (4, 7) (4, 8) mEq1
        assertRange (11, 0) (11, 4) mType2
        assertRange (11, 7) (11, 8) mEq2
        assertRange (14, 0) (14, 4) mType3
        assertRange (14, 7) (14, 11) mWith3
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``SynValSig contains parameter names`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module Meh
val InferSynValData:
    memberFlagsOpt: SynMemberFlags option * pat: SynPat option * SynReturnInfo option * origRhsExpr: SynExpr ->
        x: string ->
            SynValData2
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Val(valSig = SynValSig(synType =
                    SynType.Fun(
                        argType =
                            SynType.Tuple(path = [
                                SynTupleTypeSegment.Type(SynType.SignatureParameter(id = Some memberFlagsOpt))
                                SynTupleTypeSegment.Star _
                                SynTupleTypeSegment.Type(SynType.SignatureParameter(id = Some pat))
                                SynTupleTypeSegment.Star _
                                SynTupleTypeSegment.Type(SynType.App _)
                                SynTupleTypeSegment.Star _
                                SynTupleTypeSegment.Type(SynType.SignatureParameter(id = Some origRhsExpr))
                            ])
                        returnType =
                            SynType.Fun(
                                argType = SynType.SignatureParameter(id = Some x)
                                returnType = SynType.LongIdent _
                            )
                    )
                ))
        ] ) ])) ->
        Assert.AreEqual("memberFlagsOpt", memberFlagsOpt.idText)
        Assert.AreEqual("pat", pat.idText)
        Assert.AreEqual("origRhsExpr", origRhsExpr.idText)
        Assert.AreEqual("x", x.idText)
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``Leading keyword in recursive types`` () =
    let parseResults =
        getParseResultsOfSignatureFile
             """
type A = obj
and B = int
 """

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(trivia = { LeadingKeyword = SynTypeDefnLeadingKeyword.Type mType })
                SynTypeDefnSig(trivia = { LeadingKeyword = SynTypeDefnLeadingKeyword.And mAnd })
            ])
        ])
    ])) ->
        assertRange (2, 0) (2, 4) mType
        assertRange (3, 0) (3, 3) mAnd
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"


[<Test>]
let ``Nested type has static type as leading keyword`` () =
    let parseResults =
        getParseResultsOfSignatureFile
             """
type A =
    static type B =
                    class
                    end
 """

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(
                    memberSigs = [
                        SynMemberSig.NestedType(nestedType =
                            SynTypeDefnSig(trivia = { LeadingKeyword = SynTypeDefnLeadingKeyword.StaticType(mStatic, mType) }))
                    ]
                ))
            ])
        ])
    ])) ->
        assertRange (3, 4) (3, 10) mStatic
        assertRange (3, 11) (3, 15) mType
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
