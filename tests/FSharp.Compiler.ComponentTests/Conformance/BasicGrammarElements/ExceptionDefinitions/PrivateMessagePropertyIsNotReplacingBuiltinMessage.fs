exception MyCustomExc of int
    with 
        member private this.Message = "This must remain secret!"
    end

let f() =
    try
        raise (MyCustomExc(42))
    with
        |  e  -> e.Message
    

let result = f()
printfn "%s" result
if result = "This must remain secret!" then failwith $"Failed: 1. Secret private string was leaked."

