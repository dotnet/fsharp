// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyTrademark

open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly:AssemblyTrademark("I'm a TradeMark")>]
do
    let path_to_myself = Assembly.GetExecutingAssembly().Location
    let actualvalue      = System.Diagnostics.FileVersionInfo.GetVersionInfo(path_to_myself).LegalTrademarks
    let expectedvalue    = "I'm a TradeMark"
    if not (actualvalue = expectedvalue) then  raise (new Exception("Expected: {expectedvalue}; Actual: {actualvalue}"))
