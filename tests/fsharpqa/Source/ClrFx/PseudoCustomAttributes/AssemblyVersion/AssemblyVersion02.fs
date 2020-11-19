// #Regression #Attributes #Assemblies 
// Verify ability to put wildcards in assembly version
// See FSHARP1.0:1675

#light

open System
open System.Reflection;
open System.Configuration.Assemblies

let TestAttribute() = 
    let myself = Assembly.GetExecutingAssembly()
        
    let actualValue      =  try
                               let attr = myself.GetCustomAttributes(typeof<AssemblyVersionAttribute>, true (*ignored*)).[0]  
                               (attr :?> AssemblyVersionAttribute).Version  (* Need a dynamic downcast from obj *)
                            with
                               | _ -> "undefined!"
    match actualValue with
    | "1.0.0.0" -> exit 1
    | "1.0.0.*" -> exit 2
    | x when x.StartsWith("1.0") -> ()
    | _ -> exit 3
                                            1

[<assembly:AssemblyVersion("1.0.*")>]
do ()

TestAttribute()

exit 0
