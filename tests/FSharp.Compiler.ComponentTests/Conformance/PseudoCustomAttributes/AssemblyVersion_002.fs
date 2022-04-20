// #Regression #Attributes #Assemblies 
// Verify ability to put wildcards in assembly version

open System
open System.Reflection;
open System.Configuration.Assemblies

[<assembly:AssemblyVersion("1.2.*")>]
do
    // Ensure no is assemblyversionattribute embedded in the assembly
    let attrs = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof<AssemblyVersionAttribute>, true (*ignored*))
    if attrs.Length <> 0 then raise (new Exception($"AssemblyVersionAttribute must not be embedded in an assembly"))

    let version = Assembly.GetExecutingAssembly().GetName().Version
    if version.Major <> 1  && version.Minor <> 2 then raise (new Exception("Attribute version must start with 1.2. blah"))
    if version.Build = 0 && version.Revision = 0 then raise (new Exception("Wild card version not employed"))
