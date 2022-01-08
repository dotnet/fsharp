// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyCompany

open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly: AssemblyCompany("T_Soft")>]
do
    let path_to_myself = Assembly.GetExecutingAssembly().Location
    let info = System.Diagnostics.FileVersionInfo.GetVersionInfo(path_to_myself)
    let actualvalue      = info.CompanyName
    let expectedvalue    = "T_Soft"
    if not (actualvalue = expectedvalue) then  raise (new Exception($"FAIL: Expected: {expectedvalue}; Actual: {actualvalue}"))
