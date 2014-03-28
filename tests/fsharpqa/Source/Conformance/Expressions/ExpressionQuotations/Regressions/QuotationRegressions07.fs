// #Regression #Conformance #Quotations 
// See also FSHARP1.0:4710

type C = 
    class 
        val x : int 
        val mutable y : int 
        new () = { x = 12; y = 13 }
    end 

let c = new C() 

let test1 = <@@ c.x @@>      |> sprintf "%A"
let test2 = <@@ c.y @@>      |> sprintf "%A"
let test3 = <@@ c.y <- 3 @@> |> sprintf "%A"

printfn "Comparing [%s]" test1
if test1 <> "PropertyGet (Some (PropertyGet (None, c, [])), x, [])"          then exit 1

printfn "Comparing [%s]" test2
if test2 <> "FieldGet (Some (PropertyGet (None, c, [])), y)"             then exit 1

printfn "Comparing [%s]" test3
if test3 <> "FieldSet (Some (PropertyGet (None, c, [])), y, Value (3))"  then exit 1

exit 0
