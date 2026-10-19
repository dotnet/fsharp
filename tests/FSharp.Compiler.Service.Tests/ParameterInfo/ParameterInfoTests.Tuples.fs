module FSharp.Compiler.Service.Tests.ParameterInfoTuplesTests

open Xunit

[<Fact>]
let ``Regression.MethodInfo.WithColon.Bug4518_4`` () =
    assertFirstReturnTypeText ": string" """
           type T() =
                member this.Foo(a,b) = ""
           let t = new T()
           t.Foo({caret}"""

[<Fact>]
let ``ParameterInfo.NamesOfParams`` () =
    assertParameterInfoOverloads [["a: int"; "b: bool"; "c: int"; "d: int"; "?e int"]] """
type Foo =
    static member F(a:int, b:bool, c:int, d:int, ?e:int) = ()
let a = 42
Foo.F({caret}0,(a=42),d=3,?e=Some 4,c=2)"""

[<Fact>]
let ``LocationOfParams.Case2`` () =
    assertHasParameterInfo """System.Console.WriteLine({caret}"hello {0}"  , "Brian" )"""

[<Fact>]
let ``LocationOfParams.Case4`` () =
    assertHasParameterInfo """System.Console.WriteLine({caret}"hello {0}"  , ("tuples","don't confuse it") )"""

[<Fact>]
let ``LocationOfParams.Nested2`` () =
    assertHasParameterInfo """System.Console.WriteLine({caret}"hello {0}"  , sin  42.0 )"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``LocationOfParams.BY_DESIGN.WayThatMismatchedParensFailOver.Case1`` () =
    assertHasParameterInfo """
            type CC() =
                member this.M(a,b,c,d) = a+b+c+d
            let c = new CC()
            c.M({caret}1,2,3,
            c.M(1,2,3,4)"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``LocationOfParams.BY_DESIGN.WayThatMismatchedParensFailOver.Case2`` () =
    assertHasParameterInfo """
            type CC() =
                member this.M(a,b,c,d) = a+b+c+d
            let c = new CC()
            c.M({caret}1,2,3,
            c.M(1,2,3,4)
            c.M(1,2,3,4)
            c.M(1,2,3,4)"""

[<Fact>]
let ``LocationOfParams.Tuples.Bug91360.Case1`` () =
    assertHasParameterInfo """System.Console.WriteLine({caret} (42,43) ) // oops"""

[<Fact>]
let ``LocationOfParams.Tuples.Bug91360.Case2`` () =
    assertHasParameterInfo """System.Console.WriteLine({caret}(42,43) ) // oops"""

[<Fact>]
let ``LocationOfParams.InheritsClause.Bug192134`` () =
    assertHasParameterInfo """
            type B(x : int) =
               new(x1:int, x2: int) = new B(10)
            type A() =
               inherit B({caret}1,2)"""

[<Fact>]
let ``ParameterNamesInFunctionsDefinedByLetBindings`` () =
    assertParameterInfoOverloads [["n1: int"]] "let foo (n1 : int) (n2 : int) = n1 + n2\nfoo({caret}"
    assertParameterInfoOverloads [["n1: int"; "n2: int"]] "let foo (n1 : int, n2 : int) = n1 + n2\nfoo({caret}"
    assertParameterInfoOverloads [["'a -> 'b"]] "let foo = List.map\nfoo({caret}"
    assertParameterInfoOverloads [["int"]] "let foo x =\n    let bar y = x + y\n    bar({caret}"
    assertParameterInfoOverloads [["int option"]] "let f (Some x) = x + 1\nf({caret}"

[<Fact>]
let ``Multi.DotNet.StaticMethod`` () =
    assertParameterInfoContains ["format"; "arg0"] """System.Console.WriteLine({caret}"Today is {0:dd MMM yyyy}",System.DateTime.Today)"""

[<Fact>]
let ``Multi.Function.InTheClassMember`` () =
    assertParameterInfoOverloads [["int"; "int"]] """
            type Foo() =
                let foo1(a : int, b:int) = ()

                member this.A() =
                    foo1({caret}1,
                member this.A(a : string, b:int) = ()"""

[<Fact>]
let ``Multi.ParamAsTupleType`` () =
    assertParameterInfoOverloads [["int * int"; "int"]] """
            let tuple((a : int, b : int), c : int) = a * b + c
            let result = tuple({caret}(1, 2), 3)"""

[<Fact>]
let ``Multi.ParamAsCurryType`` () =
    assertParameterInfoOverloads [["x: float"]] """
            let multi (x : float) (y : float) = 0
            let sum(a, b) = a + b
            let rtnValue = sum(multi({caret}1.0) 3.0, 5)"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Multi.Function.WithOptionType`` () =
    assertParameterInfoOverloads [["int option"; "string ref"]] """
            let foo( a : int option, b : string ref) = 0
            let _ = foo({caret}Some(12),"""

[<Fact>]
let ``Multi.Function.WithOptionType2`` () =
    assertParameterInfoOverloads [["int option"; "float option"]] """
            let multi (x : float) (y : float) = x * y
            let sum(a : int, b) = a + b
            let options(a1 : int option, b1 : float option) = a1.ToString() + b1.ToString()
            let rtnOption = options({caret}Some(sum(1, 3)), Some(multi 3.1 5.0)) """

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Multi.Function.WithRefType`` () =
    assertParameterInfoOverloads [["int ref"; "string ref"]] """
            let foo( a : int ref, b : string ref) = 0
            let _ = foo({caret}ref 12,"""

[<Fact>]
let ``Multi.Overload.WithSameParameterCount`` () =
    assertParameterInfoOverloads [["int"; "int"; "string"; "bool"]; ["int"; "string"; "int"; "bool"]] """
            type Foo() =
              member this.A1(x1 : int, x2 : int, ?y : string, ?Z: bool) = ()
              member this.A1(x1 : int, X2 : string, ?y : int, ?Z: bool) = ()
            let foo = new Foo()
            foo.A1({caret}1,1,"""

[<Fact>]
let ``Multi.NoParameterInfo.OnFunctionDeclaration`` () =
    assertNoParameterInfo "let Foo(x : int, {caret}b : string) = ()"

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``LocationOfParams.Tuples.Bug123219`` () =
    assertHasParameterInfo """
type Expr = | Num of int
type T<'a>() =
    member this.M1(a:int*string, b:'a -> unit) = ()
let x = new T<Expr>()

x.M1((1,{caret} """
