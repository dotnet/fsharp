// #Regression #Attributes #Assemblies 
// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyConfiguration
//<Expects status="success"></Expects>

#light

open System
open System.Reflection;
open System.Configuration.Assemblies

let CheckAssemblyAttribute () = 
        let myself = Assembly.GetExecutingAssembly()
        
        let actualvalue      =  try
                                   let attr = myself.GetCustomAttributes(typeof<AssemblyConfigurationAttribute>, true (*ignored*)).[0]  
                                   (attr :?> AssemblyConfigurationAttribute).Configuration  (* Need a dynamic downcast from obj *)
                                with
                                   | _ -> "undefined!"
        let expectedvalue    = "What's your configuration?"
        
        if actualvalue = expectedvalue then 
                                            0
                                       else
                                            printfn "FAIL: Expected %A; Actual: %A" expectedvalue actualvalue
                                            1

[<assembly:AssemblyConfiguration("What's your configuration?")>]
do CheckAssemblyAttribute () |> exit
