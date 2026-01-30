// #Regression #Conformance #Quotations #ReflectedDefinition
// Regression for DevDiv:361318 
//<Expects status="success"></Expects>

open System.Reflection
open Microsoft.FSharp.Quotations.DerivedPatterns

type Iface1 =
    abstract Foo : int

type Iface2 =
    inherit Iface1
    abstract Bar : int

type Test() =
    interface Iface2 with
        [<ReflectedDefinition>]
        member this.Bar = 1
    interface Iface1 with
        [<ReflectedDefinition>]
        member this.Foo = 0

let bindingFlags = BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Static ||| BindingFlags.GetProperty ||| BindingFlags.Instance

try
    for ty in Assembly.GetExecutingAssembly().GetTypes() do
       for mthd in ty.GetMethods(bindingFlags) do
           match mthd with
           | MethodWithReflectedDefinition _ -> printfn "%s" mthd.Name
           | _ -> ()        
    0
with
| _ -> printfn "FAIL: exception detected (did DevDiv:361318 regress?)"
       1
|> exit

