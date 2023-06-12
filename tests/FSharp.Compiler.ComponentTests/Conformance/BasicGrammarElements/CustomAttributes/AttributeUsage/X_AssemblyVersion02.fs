// #Regression #Conformance #DeclarationElements #Attributes 
//<Expects id="FS2003" status="warning">The attribute System.Reflection.AssemblyFileVersionAttribute specified version '9\.8\.\*\.6', but this value is invalid and has been ignored</Expects>

[<assembly:System.Reflection.AssemblyVersion("1.2.3.4")>]
[<assembly:System.Reflection.AssemblyFileVersionAttribute("9.8.*.6")>]
do 
    ()
