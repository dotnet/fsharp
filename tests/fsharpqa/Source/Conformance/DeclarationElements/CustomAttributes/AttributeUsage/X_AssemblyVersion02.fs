// #Regression #Conformance #DeclarationElements #Attributes 
//<Expects id="FS2003" status="warning">An System.Reflection.AssemblyInformationalVersionAttribute specified version '6\.5\.\*\.3', but this value is invalid and has been ignored</Expects>
//<Expects id="FS2003" status="warning">An System.Reflection.AssemblyFileVersionAttribute specified version '9\.8\.\*\.6', but this value is invalid and has been ignored</Expects>

[<assembly:System.Reflection.AssemblyVersion("1.2.3.4")>]
[<assembly:System.Reflection.AssemblyInformationalVersion("6.5.*.3")>]
[<assembly:System.Reflection.AssemblyFileVersionAttribute("9.8.*.6")>]
do 
    ()

exit 0