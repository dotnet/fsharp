// #Regression #Conformance #DeclarationElements #Attributes 

[<assembly:System.Reflection.AssemblyVersion("1.2.3")>]
do 
    ()

let asm = System.Reflection.Assembly.GetExecutingAssembly().GetName()
printfn "%s" <| asm.Version.ToString()
if (asm.Version.ToString() = "1.2.3.0") then () else failwith "Failed: 1"