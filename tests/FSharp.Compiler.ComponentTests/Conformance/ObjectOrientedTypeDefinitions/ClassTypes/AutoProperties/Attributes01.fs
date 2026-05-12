// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #Attributes

type MyA() = inherit System.Attribute()

type A(x) =
    [<MyA>] 
    member val Property = x with get, set
    [<CompiledName("Hi")>]
    member val Property2 = x with get, set

let x = A(1)
x.Property <- 2
if x.Property <> 2 then exit 1