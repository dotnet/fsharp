// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
//<Expects status="error" span="(4,34-4,37)" id="FS3135">To indicate that this property can be set, use 'member val PropertyName = expr with get,set'\.$</Expects>
type T() =
    member val Property = 0 with set    // contrary to C#, we need both get and set

let x = T()
x.Property <- 1
