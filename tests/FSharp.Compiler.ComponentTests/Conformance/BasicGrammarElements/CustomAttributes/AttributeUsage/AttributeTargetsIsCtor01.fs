// #Regression #Conformance #DeclarationElements #Attributes 
// FSharp1.0:4780 - Attributes targetting constructors are not allowed on explicit constructors 'new() = { ... }'
// Make sure custom attributes can be applied to explicit and implicit constructors

#light

open System

[<AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)>]
type TestAttribute1() =
    inherit Attribute()
    
[<AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)>]
type TestAttribute2() =
    inherit Attribute()
    
type TestType1 [<TestAttribute1>] (x) = 
    let a = x
    [<TestAttribute1>] new () = new TestType1(0)
    [<TestAttribute1>] new (a, b) = new TestType1(1)
    
type TestType2 [<TestAttribute2; TestAttribute2>] (x) = 
    let a = x
    [<TestAttribute2>]
    [<TestAttribute2>]
    new () = new TestType2(0)
    [<TestAttribute2; TestAttribute2>]
    new (a, b) = new TestType2(2)
