// AssemblyAttributes
// See FSHARP1.0:832,1674,1675 and 2290
// Attribute under test:  AssemblyVersion
// In this test case the testharness compiles with the assembly version number --version:4.5.6.7 which overrides the attribute here
open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly:AssemblyVersion("9.8.7.6")>]
do
    let version = Assembly.GetExecutingAssembly().GetName().Version
    if not(version.Major = 4  && version.Minor = 5 && version.Build = 6 && version.Revision = 7) then raise (new Exception($"Version must be '4.5.6.7' provide by compiler arguments was actually: '{version}'"))

