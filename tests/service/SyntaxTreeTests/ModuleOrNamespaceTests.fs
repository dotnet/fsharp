module FSharp.Compiler.Service.Tests.SyntaxTreeTests.ModuleOrNamespaceTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``DeclaredNamespace range should start at namespace keyword`` () =
    let parseResults = 
        getParseResults
            """namespace TypeEquality

/// A type for witnessing type equality between 'a and 'b
type Teq<'a, 'b>
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.DeclaredNamespace; range = r) ])) ->
        assertRange (1, 0) (4, 8) r
    | _ -> Assert.Fail "Could not get valid AST"
    
[<Test>]
let ``Multiple DeclaredNamespaces should have a range that starts at the namespace keyword`` () =
    let parseResults = 
        getParseResults
            """namespace TypeEquality

/// A type for witnessing type equality between 'a and 'b
type Teq = class end

namespace Foobar

let x = 42
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.DeclaredNamespace; range = r1)
        SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.DeclaredNamespace; range = r2) ])) ->
        assertRange (1, 0) (4, 20) r1
        assertRange (6, 0) (8, 10) r2
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``GlobalNamespace should start at namespace keyword`` () =
    let parseResults = 
        getParseResults
            """// foo
// bar
namespace  global

type X = int
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.GlobalNamespace; range = r) ])) ->
        assertRange (3, 0) (5, 12) r
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Module range should start at first attribute`` () =
    let parseResults = 
        getParseResults
            """
[<  Foo  >]
module Bar

let s : string = "s"
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.NamedModule; range = r) ])) ->
        assertRange (2, 0) (5, 20) r
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Module should contain module keyword`` () =
    let parseResults = 
        getParseResults
            """
/// this file contains patches to the F# Compiler Service that have not yet made it into
/// published nuget packages.  We source-copy them here to have a consistent location for our to-be-removed extensions

module FsAutoComplete.FCSPatches

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FsAutoComplete.UntypedAstUtils
open FSharp.Compiler.CodeAnalysis

module internal SynExprAppLocationsImpl =
let a = 42
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.NamedModule; trivia = { ModuleKeyword = Some mModule; NamespaceKeyword = None }) ])) ->
        assertRange (5, 0) (5, 6) mModule
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Namespace should contain namespace keyword`` () =
    let parseResults = 
        getParseResults
            """
namespace Foo
module Bar =
let a = 42
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.DeclaredNamespace; trivia = { ModuleKeyword = None; NamespaceKeyword = Some mNamespace }) ])) ->
        assertRange (2, 0) (2, 9) mNamespace
    | _ -> Assert.Fail "Could not get valid AST"
