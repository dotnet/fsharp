// #Regression #Conformance #DeclarationElements #Attributes 


[<assembly:System.Reflection.AssemblyVersion("1.2.*.4")>]
do 
    ()

let asm = System.Reflection.Assembly.GetExecutingAssembly().GetName()

exit <| if (asm.Version.ToString() = "0.0.0.0") then 0 else 1