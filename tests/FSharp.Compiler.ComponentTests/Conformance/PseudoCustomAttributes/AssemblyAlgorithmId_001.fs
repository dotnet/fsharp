// #Regression #Attributes #Assemblies 
// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyAlgorithmId
//<Expects status="success"></Expects>

#light

open System
open System.Reflection;
open System.Configuration.Assemblies

let CheckAssemblyAttribute () =
    let alg = Assembly.GetExecutingAssembly().GetName().HashAlgorithm
    printfn "%A" alg
    if not (AssemblyHashAlgorithm.MD5 = alg) then raise (new Exception("Invalid Assembly Hash Algorithm"))

[<assembly:AssemblyAlgorithmId(AssemblyHashAlgorithm.MD5)>]
CheckAssemblyAttribute ()
