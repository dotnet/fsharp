//
//Test set iteration
//

let s1 = Set.ofArray [|"1"|]
let s2 = Set.ofArray [|"1"|]
for x in s1 do
     for y in s2 do
         System.Console.WriteLine(x);;
             
printfn "Succeeded -- compile fail because file locked due to --shadowcopyreferences-"
use os = System.IO.File.CreateText "test1.ok" 
os.Close()

#quit;; 
