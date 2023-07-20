// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:2087
// Function parameter attributes are ignored and do not appear in the resulting assembly
// Attribute is placed on a let-binding function definition
//<Expects status="success"></Expects>
#light
open System.Reflection

type A1() = class
               inherit System.Attribute()
            end

type A2() = class
               inherit System.Attribute()
            end

[<A1>] 
let foo ([<A2>]x) = x

// The rest of the code is a mere verification that the compiler thru reflection
let executingAssembly = System.Reflection.Assembly.GetExecutingAssembly()

// Method 'foo' is wrapped into a type called 'Function01'
let programType = executingAssembly.GetType("Function01")

// Extract message field from our custom attribute...
let res = match (programType.GetMethod("foo").GetCustomAttributes(true).[0]) with
          | :? A1 -> 0
          | _ -> 1

// Final verification
if res <> 0 then failwith $"Failed: {res}"
