// #Regression #Conformance #DeclarationElements #Attributes 
//<Expects id="FS2003" status="warning" span="(4,46-4,55)">The attribute System.Reflection.AssemblyVersionAttribute specified version '1\.2\.\*\.4', but this value is invalid and has been ignored</Expects>

[<assembly:System.Reflection.AssemblyVersion("1.2.*.4")>]
do 
    ()

let asm = System.Reflection.Assembly.GetExecutingAssembly().GetName()

exit <| if (asm.Version.ToString() = "0.0.0.0") then 0 else 1