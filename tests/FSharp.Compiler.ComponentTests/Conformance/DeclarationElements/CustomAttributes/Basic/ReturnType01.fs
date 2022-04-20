// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:1165
// F# supports custom attributes on return type
// Note: the syntax is kind of awkward...
//<Expects status="success"></Expects>

#light

type TestAttribute(x:string) =
    inherit System.Attribute()
    member this.TestMessage = x

// This is a function that has a custom attribute on the return type.
let foo(a) : [<TestAttribute("Attribute on return type!")>] int
   = a + 5

// The rest of the code is a mere verification that the compiler thru reflection
let executingAssembly = System.Reflection.Assembly.GetExecutingAssembly()

// Method 'foo' is wrapped into a type called 'ReturnType01'
let programType = executingAssembly.GetType("ReturnType01")

// Extract message field from our custom attribute...
let message = (programType.GetMethod("foo").ReturnParameter.GetCustomAttributes(true).[0] :?> TestAttribute).TestMessage

// Final verification
if message = "Attribute on return type!" then () else failwith "Failed: 1"
