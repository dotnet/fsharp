// #Conformance #TypesAndModules #Exceptions 
// Import C# exception in F#
//<Expects status="success"></Expects>
#light

// F# exception definition + abbreviation
exception E = CSharpException.CSharpException

let r1 = try
            raise( E )
         with
            | :? CSharpException.CSharpException as c -> c.Message = "C#Exception"
            | _ -> false

let r2 = try
            raise( E )
         with
            | E as c -> c.Message = "C#Exception"
            | _ -> false
    
if not (r1 && r2) then failwith "Failed: 1"
