// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:4727
// Title: once we start compiling them as real mutable fields, you should not be able to target both "property" for "val mutable" fields in classes

//<Expects id="FS0842" span="(24,14)" status="error">This attribute is not valid for use on this language element</Expects>

open System

#nowarn "0046"
#nowarn "0044"

type B() = 
    [<DefaultValue; field: Obsolete>]
    [<property: ObsoleteAttribute>]
    val mutable y : int

type C() =

    [<field: DefaultValue(true)>]
    static val mutable private mf4 : int


    // field shouldn't work here
    [<field: System.Obsolete("foo")>]
    member x.mf5 = 0

    
    member x.Foo 
       with [<property: System.Obsolete("foo")>] get() = 1
       and set(v) = ()

let c = C()
printfn "%d" c.Foo
