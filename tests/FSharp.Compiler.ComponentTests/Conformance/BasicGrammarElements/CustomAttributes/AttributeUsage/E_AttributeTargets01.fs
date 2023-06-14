// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:4673
// Title: AttributeTargets from AttributeUsage are not being checked/honored
// Descr: Verify an attribute targeting methods, can't be applied to any other language elements.

//<Expects id="FS0842" span="(21,21)" status="error">This attribute is not valid for use on this language element</Expects>
//<Expects id="FS0842" span="(24,28)" status="error">This attribute is not valid for use on this language element</Expects>

open System

#nowarn "0046"

[<AttributeUsage(AttributeTargets.Method)>]
type MAttribute() = inherit Attribute()

type A() =
    
    [<M>]
    let someVal = 100

    [<DefaultValue; M>]
    val mutable m_Index : int
    
    [<DefaultValue; field: M>]
    val mutable m_Name : string
    
    [<method: M>]
    member this.Index with [<method: M>] get () = 5
                       and [<method: M>] set (x : int) = ()
                       
    [<method: M>]
    static member (+) (op1 : A, op2 : A) = new A()
