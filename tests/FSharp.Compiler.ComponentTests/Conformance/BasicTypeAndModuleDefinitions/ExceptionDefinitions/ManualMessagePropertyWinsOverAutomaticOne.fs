exception MyCustomExc of Message:string
let f() =
    try
        raise (MyCustomExc("This should be the message!"))
    with
        | MyCustomExc m as e  -> e.Message
    

let result = f()
printfn "%s" result
if result <> "This should be the message!" then failwith $"Failed: 1. Message is '{result}' instead"

