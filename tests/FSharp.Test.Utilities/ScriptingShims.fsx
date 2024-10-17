namespace global

[<AutoOpen>]
module GlobalShims = 
    let exit (code:int) = 
        if code = 0 then 
            () 
        else failwith $"Script called function 'exit' with code={code}."
