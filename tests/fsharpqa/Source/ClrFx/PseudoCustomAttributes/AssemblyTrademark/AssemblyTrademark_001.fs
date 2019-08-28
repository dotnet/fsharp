// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyTrademark
//<Expects status="success"></Expects>

#light

open System
open System.Reflection;
open System.Configuration.Assemblies

let CheckAssemblyAttribute () = 
        let path_to_myself = Assembly.GetExecutingAssembly().Location
        let actualvalue      = System.Diagnostics.FileVersionInfo.GetVersionInfo(path_to_myself).LegalTrademarks
        let expectedvalue    = "I'm a TradeMark"
        if actualvalue = expectedvalue then 
                                            0
                                       else
                                            printfn "FAIL: Expected %A; Actual: %A" expectedvalue actualvalue
                                            1

[<assembly:AssemblyTrademark("I'm a TradeMark")>]
do CheckAssemblyAttribute () |> exit
