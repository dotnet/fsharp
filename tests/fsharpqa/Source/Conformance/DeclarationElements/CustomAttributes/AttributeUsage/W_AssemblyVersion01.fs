// #Regression #Conformance #DeclarationElements #Attributes 
//<Expects id="FS2003" status="warning">An AssemblyVersionAttribute specified version '1\.2\.3\.4\.5\.6', but this value is invalid and has been ignored</Expects>

[<assembly:System.Reflection.AssemblyVersion("1.2.3.4.5.6")>]
do 
    ()

let asm = System.Reflection.Assembly.GetExecutingAssembly().GetName()

exit <| if (asm.Version.ToString() = "0.0.0.0") then 0 else 1