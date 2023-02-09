module FSharp.Compiler.Service.Tests.SyntaxTreeTests.NestedModuleTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework


[<Test>]
let ``Range of attribute should be included in SynModuleSigDecl.NestedModule`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace SomeNamespace

[<Foo>]
module Nested =
    val x : int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.NestedModule _ as nm
    ]) as sigModule ])) ->
        assertRange (4, 0) (6, 15) nm.Range
        assertRange (2, 0) (6, 15) sigModule.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in SynModuleDecl.NestedModule`` () =
    let parseResults = 
        getParseResults
            """
module TopLevel

[<Foo>]
module Nested =
    ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.NestedModule _ as nm
    ]) ])) ->
        assertRange (4, 0) (6, 6) nm.Range
    | _ -> Assert.Fail "Could not get valid AST"
    
[<Test>]
let ``Range of equal sign should be present`` () =
    let parseResults = 
        getParseResults
            """
module X =
()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.NestedModule(trivia = { ModuleKeyword = Some mModule; EqualsRange = Some mEquals })
    ]) ])) ->
        assertRange (2, 0) (2, 6) mModule
        assertRange (2, 9) (2, 10) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of equal sign should be present, signature file`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace Foo

module X =
val bar : int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.NestedModule(trivia = { ModuleKeyword = Some mModule; EqualsRange = Some mEquals })
    ]) ])) ->
        assertRange (4, 0) (4, 6) mModule
        assertRange (4, 9) (4, 10) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of nested module in signature file should end at the last SynModuleSigDecl`` () =
    let parseResults =
        getParseResultsOfSignatureFile
            """namespace Microsoft.FSharp.Core

open System
open System.Collections.Generic
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open System.Collections


module Tuple =

    type Tuple<'T1,'T2,'T3,'T4> =
        interface IStructuralEquatable
        interface IStructuralComparable
        interface IComparable
        new : 'T1 * 'T2 * 'T3 * 'T4 -> Tuple<'T1,'T2,'T3,'T4>
        member Item1 : 'T1 with get
        member Item2 : 'T2 with get
        member Item3 : 'T3 with get
        member Item4 : 'T4 with get


module Choice =

    /// <summary>Helper types for active patterns with 6 choices.</summary>
    [<StructuralEquality; StructuralComparison>]
    [<CompiledName("FSharpChoice`6")>]
    type Choice<'T1,'T2,'T3,'T4,'T5,'T6> =
      /// <summary>Choice 1 of 6 choices</summary>
      | Choice1Of6 of 'T1
      /// <summary>Choice 2 of 6 choices</summary>
      | Choice2Of6 of 'T2
      /// <summary>Choice 3 of 6 choices</summary>
      | Choice3Of6 of 'T3
      /// <summary>Choice 4 of 6 choices</summary>
      | Choice4Of6 of 'T4
      /// <summary>Choice 5 of 6 choices</summary>
      | Choice5Of6 of 'T5
      /// <summary>Choice 6 of 6 choices</summary>
      | Choice6Of6 of 'T6



/// <summary>Basic F# Operators. This module is automatically opened in all F# code.</summary>
[<AutoOpen>]
module Operators =

    type ``[,]``<'T> with
        [<CompiledName("Length1")>]
        /// <summary>Get the length of an array in the first dimension  </summary>
        member Length1 : int
        [<CompiledName("Length2")>]
        /// <summary>Get the length of the array in the second dimension  </summary>
        member Length2 : int
        [<CompiledName("Base1")>]
        /// <summary>Get the lower bound of the array in the first dimension  </summary>
        member Base1 : int
        [<CompiledName("Base2")>]
        /// <summary>Get the lower bound of the array in the second dimension  </summary>
        member Base2 : int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
          SynModuleSigDecl.Open _
          SynModuleSigDecl.Open _
          SynModuleSigDecl.Open _
          SynModuleSigDecl.Open _
          SynModuleSigDecl.Open _
          SynModuleSigDecl.NestedModule(range=mTupleModule; moduleDecls=[ SynModuleSigDecl.Types([
              SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.ObjectModel(range=mTupleObjectModel); range=mTupleType)
          ], mTupleTypes) ])
          SynModuleSigDecl.NestedModule(range=mChoiceModule)
          SynModuleSigDecl.NestedModule(range=mOperatorsModule; moduleDecls=[ SynModuleSigDecl.Types([
              SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.Simple(range=mAugmentationSimple); range=mAugmentation)
          ], mOperatorsTypes) ])
      ]) ])) ->
        assertRange (10, 0) (20, 35) mTupleModule
        assertRange (12, 4) (20, 35) mTupleTypes
        assertRange (12, 9) (20, 35) mTupleType
        assertRange (13, 8) (20, 35) mTupleObjectModel
        assertRange (23, 0) (40, 25) mChoiceModule
        assertRange (44, 0) (60, 26) mOperatorsModule
        assertRange (48, 4) (60, 26) mOperatorsTypes
        assertRange (48, 9) (60, 26) mAugmentation
        assertRange (48, 9) (60, 26) mAugmentationSimple
    | _ -> Assert.Fail "Could not get valid AST"