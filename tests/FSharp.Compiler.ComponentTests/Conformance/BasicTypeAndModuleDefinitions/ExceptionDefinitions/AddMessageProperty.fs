exception MyCustomExc of field:int
let f() =
    try
        raise (MyCustomExc(42))
    with
        | MyCustomExc _ as e  -> e.Message
    

let result = f()
printfn "%s" result
if result <> "MyCustomExc 42" then failwith "Failed: 1"

