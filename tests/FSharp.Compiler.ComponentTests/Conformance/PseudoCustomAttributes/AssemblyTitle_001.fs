// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyTitle

open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly:AssemblyTitleAttribute("I'm a title")>]
do
    let path_to_myself = Assembly.GetExecutingAssembly().Location
    let actualvalue      = System.Diagnostics.FileVersionInfo.GetVersionInfo(path_to_myself).FileDescription
    let expectedvalue    = "I'm a title"
    if not (actualvalue = expectedvalue) then raise (new Exception($"FAIL: Expected {expectedvalue }; Actual: {actualvalue}"))
