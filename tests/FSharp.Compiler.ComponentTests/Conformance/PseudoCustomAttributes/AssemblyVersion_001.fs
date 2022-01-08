// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyVersion

open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly:AssemblyVersion("9.8.7.6")>]
do
    let version = Assembly.GetExecutingAssembly().GetName().Version
    if version.Major <> 9  && version.Minor <> 8 && version.Build <> 7 && version.Revision <> 6 then raise (new Exception($"Version must be '9.8.7.6' was actually: '{version}'"))

