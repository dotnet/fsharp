// #Regression #NoMT #FSI 
// Enumeration gave error
//<Expects status="success"></Expects>

let s1 = Set.ofArray [|"1"|]
let s2 = Set.ofArray [|"1"|]
for x in s1 do
     for y in s2 do
         System.Console.WriteLine(x);;
#q;;

