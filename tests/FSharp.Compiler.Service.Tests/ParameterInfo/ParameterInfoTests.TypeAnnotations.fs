module FSharp.Compiler.Service.Tests.ParameterInfoTypeAnnotationsTests

open Xunit

[<Fact>]
let ``Regression.StaticVsInstance.Bug3626.Case1`` () =
    assertParameterInfoOverloads [["staticReturnsInt: int"]] """
type Foo() =
    member this.Bar(instanceReturnsString:int) = "hllo"
    static member Bar(staticReturnsInt:int) = 13
let z = Foo.Bar({caret})"""

[<Fact>]
let ``Regression.StaticVsInstance.Bug3626.Case2`` () =
    assertParameterInfoOverloads [["instanceReturnsString: int"]] """
type Foo() =
    member this.Bar(instanceReturnsString:int) = "hllo"
    static member Bar(staticReturnsInt:int) = 13
let Hoo = new Foo()
let y = Hoo.Bar({caret}"""

[<Fact>]
let ``NoArguments`` () =
    assertParameterInfoOverloads [[]] """
type T =
    static member F() = 42
let r1 = T.F({caret})"""
    assertParameterInfoOverloads [[]] """
type T =
    static member G(x:unit) = 42
let r2 = T.G({caret})"""
    assertParameterInfoOverloads [[]] """
let h((x:unit)) = 42
let r3 = h({caret})"""
    assertParameterInfoOverloads [[]] """
let g() = 42
let r4 = g({caret})"""

[<Fact>]
let ``Single.DotNet.OneParameter`` () =
    assertParameterInfoOverloads [["value: int"]] "System.DateTime.Today.AddYears({caret}"

[<Fact>]
let ``Single.DotNet.RefTypeValueType`` () =
    assertParameterInfoOverloads [ []; ["name: string"; "salary: float"; "dob: System.DateTime"]; ["name: string"; "dob: System.DateTime"] ] """
type Emp =
    val mutable private m_Name : string
    val mutable private m_Salary : float
    val mutable private m_DoB : System.DateTime
    public new() = { m_Name = System.String.Empty; m_Salary = 0.0; m_DoB = System.DateTime.Today }
    public new(name, salary, dob) = { m_Name = name; m_Salary = salary; m_DoB = dob }
    public new(name, dob) = new Emp(name, 0.0, dob)
let _ = Emp({caret}"""
