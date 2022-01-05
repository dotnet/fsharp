// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyInformationalVersion
//<Expects status="success"></Expects>

#light

open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly:AssemblyInformationalVersion("5.9.3.4")>]
do
    let path_to_myself = Assembly.GetExecutingAssembly().Location
    let actualvalue      = System.Diagnostics.FileVersionInfo.GetVersionInfo(path_to_myself).ProductVersion
    let expectedvalue    = "5.9.3.4"
    if not(actualvalue = expectedvalue) then
        raise( new Exception($"FAIL: Expected {expectedvalue }; Actual: {actualvalue}"))

