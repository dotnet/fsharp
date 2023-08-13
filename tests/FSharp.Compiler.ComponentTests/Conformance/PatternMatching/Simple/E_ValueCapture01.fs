// #Regression #Conformance #PatternMatching 
#light

//<Expects id="FS0039" status="error">The value or constructor 'ident1' is not defined</Expects>
//<Expects id="FS0039" status="error">The value or constructor 'ident2' is not defined</Expects>

// Verifing scoping of value captures
let test1() =
    let x = 1
    let _ =
        match x with
        | ident1 as ident2 -> ()
    
    if ident1 <> 1 then failwith "ident1 shouldn't be in scope!"
    if ident2 <> 2 then failwith "ident2 shouldn't be in scope!"

exit 1
