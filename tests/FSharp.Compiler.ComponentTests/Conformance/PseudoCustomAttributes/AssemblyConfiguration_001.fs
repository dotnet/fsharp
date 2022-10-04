// #Regression #Attributes #Assemblies 
// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyConfiguration

#light

open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly:AssemblyConfiguration("What's your configuration?")>]
do
    let actualvalue      =  try
                               let attr = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof<AssemblyConfigurationAttribute>, true (*ignored*))[0]
                               printfn "%A" attr
                               printfn "%s" (attr.ToString())
                               (attr :?> AssemblyConfigurationAttribute).Configuration  (* Need a dynamic downcast from obj *)
                            with
                               | _ -> "undefined!"
    let expectedvalue    = "What's your configuration?"
    printfn $"Expected {expectedvalue}; Actual: {actualvalue}"
    if not (actualvalue = expectedvalue) then raise(new Exception($"Expected {expectedvalue}; Actual: {actualvalue}"))
