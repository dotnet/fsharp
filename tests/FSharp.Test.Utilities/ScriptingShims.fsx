namespace global

[<AutoOpen>]
module GlobalShims = 

    let errorStringWriter = new System.IO.StringWriter()        
    let oldConsoleError = System.Console.Error
    do System.Console.SetError(errorStringWriter)

    let exit (code:int) = 
        System.Console.SetError(oldConsoleError)
        if code=0 then 
            () 
        else failwith $"Script called function 'exit' with code={code} and collected in stderr: {errorStringWriter.ToString()}"