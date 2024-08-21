// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:1165
// F# supports custom attributes on return type.
// Multiple attributes on the same return value (same attribute repeated twice)
// Note: the syntax is kind of awkward...
//<Expects status="success"></Expects>

#light

[<System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)>]
type CA1 = 
    class
        inherit System.Attribute
        new (tt:string) = { t = tt }
        val t : string
    end                       

// This is a function that has a custom attribute on the return type.
let f(x) : [<CA1("A1")>] [<CA1("A2")>] int = x + 1

// The rest of the code is a mere verification that the compiler thru reflection
let executingAssembly = System.Reflection.Assembly.GetExecutingAssembly()

// Method 'f' is wrapped into a type called 'ReturnType03'
let programType = executingAssembly.GetType("ReturnType03")

// Get number of CAs
let count = programType.GetMethod("f").ReturnParameter.GetCustomAttributes(true)
