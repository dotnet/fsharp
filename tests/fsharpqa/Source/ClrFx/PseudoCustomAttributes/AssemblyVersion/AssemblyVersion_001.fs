// #Regression #Attributes #Assemblies 
// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyVersion
//<Expects status="success"></Expects>

#light

open System
open System.Reflection;
open System.Configuration.Assemblies

let CheckAssemblyAttribute () = 
        let myself = Assembly.GetExecutingAssembly()
        
        let actualvalue      =  try
                                   let attr = myself.GetCustomAttributes(typeof<AssemblyVersionAttribute>, true (*ignored*)).[0]  
                                   (attr :?> AssemblyVersionAttribute).Version  (* Need a dynamic downcast from obj *)
                                with
                                   | _ -> "undefined!"
        let expectedvalue    = "4.1.3.2"
        
        if actualvalue = expectedvalue then 
                                            0
                                       else
                                            printfn "FAIL: Expected %A; Actual: %A" expectedvalue actualvalue
                                            1

[<assembly:AssemblyVersion("4.1.3.2")>]
do CheckAssemblyAttribute () |> exit
