// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSharp1.0:4785
// Title: Search the IEnumerator pattern first

let ra = new ResizeArray<int>(100)
for i = 0 to 100 do ra.Add(i)


let test1()  = 
   let mutable z = 0
   for i = 0 to 10000000 do
     for x in ra do
         z <- z + 1
   printfn "z = %d" z
