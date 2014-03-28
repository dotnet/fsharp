// #Regression #Attributes #Assemblies 
// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyAlgorithmId
//<Expects status=success></Expects>

#light

open System
open System.Reflection;
open System.Configuration.Assemblies

let CheckAssemblyAttribute () = 
    if AssemblyHashAlgorithm.MD5 = Assembly.GetExecutingAssembly().GetName().HashAlgorithm then 0 else 1

[<assembly:AssemblyAlgorithmId(AssemblyHashAlgorithm.MD5)>]
do CheckAssemblyAttribute () |> exit
