// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyCopyrightAttribute

open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly:AssemblyCopyrightAttribute("Copyright © Microsoft 3456")>]
do
    let path_to_myself = Assembly.GetExecutingAssembly().Location
    let actualvalue      = System.Diagnostics.FileVersionInfo.GetVersionInfo(path_to_myself).LegalCopyright
    let expectedvalue    = "Copyright © Microsoft 3456"
    if not (actualvalue = expectedvalue) then  raise (new Exception($"Expected: {expectedvalue}; Actual: {actualvalue}"))
