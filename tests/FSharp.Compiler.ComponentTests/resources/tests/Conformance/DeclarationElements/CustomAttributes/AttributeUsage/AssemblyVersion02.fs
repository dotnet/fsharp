// #Regression #Conformance #DeclarationElements #Attributes 

[<assembly:System.Reflection.AssemblyVersion("1.2.3")>]
do 
    ()

let asm = System.Reflection.Assembly.GetExecutingAssembly().GetName()
printfn "%s" <| asm.Version.ToString()
exit <| if (asm.Version.ToString() = "1.2.3.0") then 0 else 1