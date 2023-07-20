// #Regression #Conformance #DeclarationElements #Attributes 
// Regression for 6227

[<assembly:System.Reflection.AssemblyVersion("1.1.9365.33000")>]
do 
    ()

let asm = System.Reflection.Assembly.GetExecutingAssembly().GetName()

if (asm.Version.ToString() = "1.1.9365.33000") then () else failwith "Failed: 1"