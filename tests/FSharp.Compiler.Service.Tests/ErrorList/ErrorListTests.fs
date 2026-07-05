module FSharp.Compiler.Service.Tests.ErrorListTests

open Xunit
open FSharp.Test

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``OverloadsAndExtensionMethodsForGenericTypes`` () =
    let _, checkResults = getParseAndCheckResults """
open System.Linq

type T =
    abstract Count : int -> bool
    default this.Count(_ : int) = true

    interface System.Collections.Generic.IEnumerable<int> with
        member this.GetEnumerator() : System.Collections.Generic.IEnumerator<int> = failwith "not implemented"
    interface System.Collections.IEnumerable with
        member this.GetEnumerator() : System.Collections.IEnumerator = failwith "not implemented"

let g (t : T) = t.Count()
"""
    assertNoDiagnostics checkResults

[<FactForDESKTOP>]
let ``ErrorsInScriptFile`` () =
    let _, checkResults = getParseAndCheckResults "#r \"System\"\n#r \"System2\"\n"
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "Assembly reference 'System2' was not found or is invalid" checkResults

[<Fact(Skip = "GetErrors function does not work for this case")>]
let ``LineDirective`` () =
    let _, checkResults = getParseAndCheckResults """
# 100 "foo.fs"
let x = y
"""
    assertDiagnosticsContain "The value or constructor 'y' is not defined" checkResults

[<Fact>]
let ``InvalidConstructorOverload`` () =
    let _, checkResults = getParseAndCheckResults """
type X private() =
    new(_ : int) = X()
    new(_ : bool) = X()
    new(_ : float, _ : int) = X()
X(1.0)
"""
    assertSingleDiagnosticContainingAll
        [ "No overloads match for method 'X'."
          "Available overloads:"
          "new: bool -> X"
          "new: int -> X" ]
        checkResults

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.InvalidJoinRelation.GroupJoin`` () =
    let _, checkResults = getParseAndCheckResults """
let x = query {
    for x in [1] do
    groupJoin y in [2] on ( x < y) into g
    select x }
"""
    assertDiagnosticsContain "Invalid join relation in 'groupJoin'." checkResults

[<Theory(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
[<InlineData("""
let t =
    query {
        for x in [1] do
        join y in [""] on (x ?=? y)
        select 1  }
""")>]
[<InlineData("""
let t =
    query {
        for x in [1] do
        groupJoin y in [""] on (x ?=? y) into g
        select 1  }
""")>]
let ``Query.NonOpenedNullableModule - nullable operator cannot be resolved`` (source: string) =
    let _, checkResults = getParseAndCheckResults source
    assertDiagnosticsContain "The operator '?=?' cannot be resolved." checkResults

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.InvalidJoinRelation.Join`` () =
    let _, checkResults = getParseAndCheckResults """
let x =
    query {
        for x in [1] do
        join y in [""] on (x > y)
        select 1
    }
"""
    assertDiagnosticsContain "Invalid join relation in 'join'." checkResults

let invalidMethodOverloadCases: obj[] seq =
    [
        [| box """
System.Console.WriteLine(null)
"""
           box [ "A unique overload for method 'WriteLine' could not be determined"
                 "Candidates:"
                 "System.Console.WriteLine(value: obj) : unit"
                 "System.Console.WriteLine(value: string) : unit" ] |]
        [| box """
type A<'T>() =
    member this.Do(a : int, b : 'T) = ()
    member this.Do(a : int, b : int) = ()
type B() =
    inherit A<int>()

let b = B()
b.Do(1, 1)
"""
           box [ "A unique overload for method 'Do' could not be determined"
                 "Candidates:"
                 "member A.Do: a: int * b: 'T -> unit"
                 "member A.Do: a: int * b: int -> unit" ] |]
    ]

[<Theory; MemberData(nameof invalidMethodOverloadCases)>]
let ``InvalidMethodOverload`` (source: string) (expectedParts: string list) =
    let _, checkResults = getParseAndCheckResults source
    assertSingleDiagnosticContainingAll expectedParts checkResults

[<Fact>]
let ``NoErrorInErrList`` () =
    let _, checkResults = getParseAndCheckResults """
module NoErrors2

module DictionaryExtension =

    type System.Collections.Generic.IDictionary<'k,'v> with
        member this.TryLookup(key : 'k) =
            let mutable value = Unchecked.defaultof<'v>
            if this.TryGetValue(key, &value) then
                Some value
            else
                None

open DictionaryExtension
"""
    assertNoDiagnostics checkResults

[<Fact>]
let ``NoLevel4Warning`` () =
    let _, checkResults = getParseAndCheckResults """
namespace testerrorlist
module nolevel4warnings =
    let x = System.DateTime.Now - System.DateTime.Now
    x.Add(x) |> ignore
"""
    assertNoDiagnostics checkResults

[<Fact>]
let ``TestWrongKeywordInInterfaceImplementation`` () =
    let _, checkResults = getParseAndCheckResults """
type staticInInterface =
    class
        interface System.IDisposable with
            static member Foo() = ()
            member x.Dispose() = ()
        end
    end
"""
    assertDiagnosticsContain "No static abstract member was found that corresponds to this override" checkResults

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.MultipleErrors`` () =
    let _, checkResults = getParseAndCheckResults "type Err = TPErrors.TP<1>"
    assertDiagnosticsContain "type provider" checkResults

[<Fact>]
let ``Records.ErrorList.IncorrectBindings1`` () =
    for code in [ "{_}"; "{_ = }" ] do
        let _, checkResults = getParseAndCheckResults code
        assertDiagnosticCount 2 checkResults
        assertDiagnosticsContain "Field bindings must have the form 'id = expr;'" checkResults
        assertDiagnosticsContain "'_' cannot be used as field name" checkResults

[<Fact>]
let ``Records.ErrorList.IncorrectBindings2`` () =
    let _, checkResults = getParseAndCheckResults "{_ = 1}"
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "'_' cannot be used as field name" checkResults

[<Fact>]
let ``Records.ErrorList.IncorrectBindings3`` () =
    let _, checkResults = getParseAndCheckResults "{a = 1; _; _ = 1}"
    assertDiagnosticCount 3 checkResults
    let messages = dumpDiagnostics checkResults |> List.distinct
    Assert.Equal(2, messages |> List.filter (fun m -> m.Contains "'_' cannot be used as field name") |> List.length)
    Assert.Equal(1, messages |> List.filter (fun m -> m.Contains "Field bindings must have the form 'id = expr;'") |> List.length)

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.StaticParameters.IncorrectType`` () =
    let _, checkResults = getParseAndCheckResults """type foo = N1.T< const 42,2>"""
    assertDiagnosticsContain "but here has type" checkResults

[<Fact(Skip = "This is ignored because currently the Mock Type Provider is not evaluating the static parameter.")>]
let ``TypeProvider.StaticParameters.Incorrect`` () =
    let _, checkResults = getParseAndCheckResults """type foo = N1.T< const " ",2>"""
    assertDiagnosticsContain "An error occurred applying the static arguments to a provided type" checkResults

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.StaticParameters.IncorrectNumberOfParameter`` () =
    let _, checkResults = getParseAndCheckResults """type foo = N1.T< const "Hello World">"""
    assertDiagnosticsContain "requires a value" checkResults

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.ProhibitedMethods`` () =
    let _, checkResults = getParseAndCheckResults "let x = BadMethods.Arr.GetFirstElement([||])"
    assertDiagnosticsContain "reported an error in the context of provided type" checkResults

[<Fact>]
let ``TypeProvider.StaticParameters.ErrorListItem`` () =
    let _, checkResults = getParseAndCheckResults """type foo = N1.T< const "Hello World",2>"""
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "The namespace or module 'N1' is not defined." checkResults

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.StaticParameters.NoErrorListCount`` () =
    let _, checkResults = getParseAndCheckResults """type foo = N1.T< const "Hello World",2>"""
    assertNoDiagnostics checkResults

[<Fact>]
let ``NoError.FlagsAndSettings.TargetOptionsRespected`` () =
    let _, checkResults =
        getParseAndCheckResultsWithOptions [| "--nowarn:44" |] """
[<System.Obsolete("x")>]
let fn x = 0
let y = fn 1
"""
    assertNoDiagnostics checkResults

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``UnicodeCharacters`` () =
    let _, checkResults = getParseAndCheckResults "namespace 新規baApplication5"
    assertDiagnosticsContain "新規" checkResults

[<Fact(Skip = "GetErrors function does not work for this case")>]
let ``NoWarn.Bug5424`` () =
    let _, checkResults = getParseAndCheckResults """
#nowarn "67" // this type test or downcast will always hold
#nowarn "66" // this upcast is unnecessary - the types are identical
namespace Namespace1
    module Test =
        open System
        let a = ((5 :> obj) :?> Object)
        let b = a :> obj
"""
    assertNoDiagnostics checkResults

[<Fact>]
let ``FlagsAndSettings.ErrorsInFlagsDisplayed`` () =
    let _, checkResults =
        getParseAndCheckResultsWithOptions [| "--versionfile:nonexistent" |] """
let x = 1
"""
    assertDiagnosticsContain "Invalid version file" checkResults
    assertDiagnosticsContain "nonexistent" checkResults

[<Fact>]
let ``BackgroundComplier`` () =
    let _, checkResults = getParseAndCheckResults """
module Test

module M =
    let func (args : string[]) =
        if(args.Length=1 && args.[0]="Hello") then 0 else 1

    [<EntryPoint>]
    let main2 args =
        let res = func(args)
        exit(res)

    let f x =
        let p = x
        p + 1

    let g x =
        let p = x
        p + 1
"""
    assertDiagnosticCount 2 checkResults
    assertDiagnosticsContain "must be the last declaration in the last file in the compilation sequence" checkResults

[<Fact>]
let ``CompilerErrorsInErrList1`` () =
    let _, checkResults = getParseAndCheckResults """
namespace Errorlist
module CompilerError =

    let a = NoVal
"""
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "The value or constructor 'NoVal' is not defined" checkResults

[<Fact>]
let ``CompilerErrorsInErrList5`` () =
    let checkResults =
        checkAsFsFile """module Test
#r "D:\\x\\Absent.dll"

let x = 0
"""
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "may only be used in F# script files" checkResults

[<Fact>]
let ``CompilerErrorsInErrList6`` () =
    let _, checkResults = getParseAndCheckResults """
type EnumOfBigInt =
    | A = 0I
    | B = 0I

type EnumOfNatNum =
    | A = 0N
    | B = 0N
"""
    assertDiagnosticCount 2 checkResults
    assertDiagnosticsContain "is not a valid value for an enumeration literal" checkResults

[<Fact>]
let ``CompilerErrorsInErrList7`` () =
    let _, checkResults = getParseAndCheckResults """
type EnumType =
    | A = 1
    | B = 2

type CustomAttrib(a:int, b:string, c:float, d:EnumType) =
    inherit System.Attribute()

let a = 42
let b = "str"
let c = 3.141
let d = EnumType.A

[<CustomAttrib(a, b, c, d)>]
type SomeClass() =
    override this.ToString() = "SomeClass"

[<EntryPoint>]
let main0 args = ()

let foo = 1
"""
    assertDiagnosticCount 5 checkResults
    assertDiagnosticsContain "is not a valid constant expression or custom attribute value" checkResults

[<Fact>]
let ``CompilerErrorsInErrList8`` () =
    let _, checkResults = getParseAndCheckResults """
type EnumInt8s      = | A1 = - 10y
"""
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "Unexpected symbol '-' in union case" checkResults

[<Fact>]
let ``CompilerErrorsInErrList9`` () =
    let _, checkResults = getParseAndCheckResults """
namespace NS
    [<AbstractClass>]
    type Lib() =
        class
            abstract M : int -> int
        end

namespace NS
    module M =
        type Lib with
            override x.M i = i
"""
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "Method overrides and interface implementations are not permitted here" checkResults

[<FactForNETCOREAPP>]
let ``CompilerErrorsInErrList10`` () =
    let _, checkResults = getParseAndCheckResults """
namespace Errorlist
module CompilerError =

    printfn "%A" System.Windows.Forms.Application.UserAppDataPath
"""
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "'Forms' is not defined" checkResults

[<Fact>]
let ``DoubleClickErrorListItem`` () =
    let _, checkResults = getParseAndCheckResults """
let x = x
"""
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "The value or constructor 'x' is not defined" checkResults

[<Fact>]
let ``FixingCodeAfterBuildRemovesErrors01`` () =
    let _, checkResults = getParseAndCheckResults """
let x = 4 + "x"
"""
    assertDiagnosticCount 2 checkResults
    assertDiagnosticsContain "does not match the type" checkResults

[<Fact>]
let ``FixingCodeAfterBuildRemovesErrors02`` () =
    let _, checkResults = getParseAndCheckResults "let x = 4"
    assertNoDiagnostics checkResults

[<Fact>]
let ``IncompleteExpression`` () =
    let checkResults =
        checkAsFsFile """module Test

printfn "%A"

List.map (fun x -> x + 1)
"""
    assertDiagnosticCount 2 checkResults
    assertDiagnosticsContain "This expression is a function value, i.e. is missing arguments" checkResults

[<Fact>]
let ``IntellisenseRequest`` () =
    let _, checkResults = getParseAndCheckResults """
type Foo() =
    member a.B(*Marker*) : int = "1"
"""
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "This expression was expected to have type 'int' but here has type 'string'" checkResults

[<Theory>]
[<InlineData("""
open System

module Foo  =
    type Thread(thread) =
        let mutable next : Thread option = thread
        member t.Next with get() = next and set(thread) = next - thread

module Bar =
    let x = new Foo.Thread(None)

    x.Next <- Some x  """)>]
[<InlineData("""
open System

module Foo  =
    type Thread(thread) =
        let mutable next : Thread option = thread
        member t.Next with get() = next and set(thread) = next - thread

module Bar =
    let x = new Foo.Thread(None)
    x.Next <- Some 1 """)>]
let ``TypeChecking - error count`` (source: string) =
    let _, checkResults = getParseAndCheckResults source
    assertDiagnosticCount 1 checkResults

[<Theory>]
[<InlineData("""
open System

module Foo  =
    type Thread(thread) =
        let mutable next : Thread option = thread
        member t.Next with get() = next and set(thread) = next - thread

module Bar =
    let x = new Foo.Thread(None)

    x.Next <- Some x  """, "Foo.Thread option")>]
[<InlineData("""
open System

module Foo  =
    type Thread(thread) =
        let mutable next : Thread option = thread
        member t.Next with get() = next and set(thread) = next - thread

module Bar =
    let x = new Foo.Thread(None)
    x.Next <- Some 1 """, "operator '-'")>]
let ``TypeChecking - error message`` (source: string) (expected: string) =
    let _, checkResults = getParseAndCheckResults source
    assertDiagnosticsContain expected checkResults

[<Fact>]
let ``Warning.ConsistentWithLanguageService`` () =
    let _, checkResults = getParseAndCheckResults """
open System
mixin mixin mixin mixin mixin mixin mixin mixin mixin mixin
mixin mixin mixin mixin mixin mixin mixin mixin mixin mixin"""
    assertWarningCount 20 checkResults
    assertDiagnosticsContain "is reserved for future use by F#" checkResults

[<Fact>]
let ``Warning.ConsistentWithLanguageService.Comment`` () =
    let _, checkResults = getParseAndCheckResults """
open System
//mixin mixin mixin mixin mixin mixin mixin mixin mixin mixin
//mixin mixin mixin mixin mixin mixin mixin mixin mixin mixin"""
    assertWarningCount 0 checkResults

[<Fact(Skip = "GetErrors function does not work for this case")>]
let ``Errorlist.WorkwithoutNowarning`` () =
    let _, checkResults = getParseAndCheckResults """
type Fruit (shelfLife : int) as x =
    let mutable m_age = (fun () -> x)
#nowarn "47"
"""
    assertDiagnosticCount 1 checkResults

[<Fact(Skip = "disabled for F#8; covered in FCS tests instead")>]
let ``CompilerErrorsInErrList4`` () =
    let _, checkResults = getParseAndCheckResults """
#nowarn "47"

type Fruit (shelfLife : int) as x =

        let mutable m_age = (fun () -> x)


#nowarn "25" // FS0025: Incomplete pattern matches on this expression. For example, the value 'C'

type DU = A | B | C
let f x = function A -> true | B -> false



let _fsyacc_gotos = [| 0us; 1us; 2us|]
"""
    assertNoDiagnostics checkResults
