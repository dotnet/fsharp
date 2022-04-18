// #Regression #Conformance #TypesAndModules #Modules 
#light

//<Expects id="FS0039" status="error">The value or constructor 'x' is not defined</Expects>

module A =
    [<AutoOpen>]
    module B = 
        let x = 0


// Since module A is not opened, B shouldn't
// be auto opened, leading to the build error.

if x <> 0 then failwith "Failed: 1"
