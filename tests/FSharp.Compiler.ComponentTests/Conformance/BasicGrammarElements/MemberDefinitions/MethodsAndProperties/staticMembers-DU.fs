// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

let resultString = "Butter and Cheese"

type VanillaDU =
    | A of int
    | B of string
    | C
    static member GetValue (param1, param2, param3) = 
        let add x y : float = x + y        
        add 1.0 (42 |> float) |> ignore
        resultString

if VanillaDU.GetValue ((), [], None) <> resultString then failwith "Failed: 1"
