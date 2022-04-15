// #Regression #Conformance #TypesAndModules #Exceptions 
#light

// FSB 1384, Suggestion: allow explicit exception type definitions to override members - specifically the Message property

type Language = 
    | ``MC++``   = 0
    | ``C#``     = 1
    | ``VB.Net`` = 2

exception NonFSLangException of Language
    with 
        override this.Message = "Who would want to program in anything other than F#?"
    end
    
let testFunc() =
    try
        raise (NonFSLangException(Language.``C#``))
        "Exception not raised?"
    with
    | NonFSLangException (langUsed) as ex
        -> sprintf "Language [%s]. Exception text [%s]" (langUsed.ToString()) (ex.Message)
    | _ -> "Expected exception not caught."
    

let result = testFunc()
printfn "%s" result
if result <> "Language [C#]. Exception text [Who would want to program in anything other than F#?]" then failwith "Failed: 1"

