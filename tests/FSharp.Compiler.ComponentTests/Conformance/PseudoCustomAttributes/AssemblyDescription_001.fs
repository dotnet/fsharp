// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyDescriptionAttribute

open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly:AssemblyDescriptionAttribute("I'm a description")>]
do
    let path_to_myself = Assembly.GetExecutingAssembly().Location
    let actualvalue      = System.Diagnostics.FileVersionInfo.GetVersionInfo(path_to_myself).Comments
    let expectedvalue    = "I'm a description"
    if not (actualvalue = expectedvalue) then  raise (new Exception($"FAIL: Expected: {expectedvalue}; Actual: {actualvalue}"))
