type MyRec = { A : int; B : string }

[<AllowNullLiteral>]
type NullType(i:int) = 
    member __.X = i

let updateRecd x =
    try
       { x with A = 42 }
    with e ->
       { A = 0; B = "success" }
       
if (updateRecd Unchecked.defaultof<MyRec>).B <> "success" then exit 1

let touchType (x : NullType) =
    try
       x.X
    with e ->
       -1
       
if (touchType null) <> -1 then exit 1

exit 0
       


