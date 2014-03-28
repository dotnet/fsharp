// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyFileVersion
//<Expects status=success></Expects>

#light

open System
open System.Reflection;
open System.Configuration.Assemblies

let CheckAssemblyAttribute () = 
        let path_to_myself = Assembly.GetExecutingAssembly().Location
        let actualvalue      = System.Diagnostics.FileVersionInfo.GetVersionInfo(path_to_myself).FileVersion
        let expectedvalue    = "1.2.3.4"
        if actualvalue = expectedvalue then 
                                            0
                                       else
                                            printfn "FAIL: Expected %A; Actual: %A" expectedvalue actualvalue
                                            1

[<assembly:AssemblyFileVersion("1.2.3.4")>]
do CheckAssemblyAttribute () |> exit
