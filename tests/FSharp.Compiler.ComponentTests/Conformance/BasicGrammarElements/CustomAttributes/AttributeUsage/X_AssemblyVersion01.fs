// #Regression #Conformance #DeclarationElements #Attributes 


[<assembly:System.Reflection.AssemblyVersion("1.2.3.4")>]
[<assembly:System.Reflection.AssemblyFileVersion("9.8.7.6.5")>]
do 
    ()

let asm = System.Reflection.Assembly.GetExecutingAssembly().GetName()
