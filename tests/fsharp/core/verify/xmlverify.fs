// #Conformance 
open System.Xml

try
   let doc = new XmlDocument() 
   let file = System.Environment.GetCommandLineArgs().[1]
   printf "Testing %s..." file
   doc.Load(file)
   printfn "OK!"
with 
   | e -> printfn "ERROR LOADING XML!: %A" e.Message
          exit 1
