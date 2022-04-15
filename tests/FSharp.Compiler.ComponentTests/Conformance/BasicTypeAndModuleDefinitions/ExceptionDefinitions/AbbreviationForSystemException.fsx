// #Conformance #TypesAndModules #Exceptions 
// exn is an abbreviation for System.Exception
#light

let p = (typeof<exn>).FullName = "System.Exception"

if not (p) then failwith "Failed: 1"
