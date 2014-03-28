// #Conformance #TypesAndModules #Exceptions 
// exn is an abbreviation for System.Exception
#light

let p = (typeof<exn>).FullName = "System.Exception"

(if p then 0 else 1) |> exit
