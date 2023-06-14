// #Regression #Conformance #DeclarationElements #Accessibility 
#light

// Regression test for FSHARP1.0:4137 - CTP: Encapsulation/accessibility in F#

module internal HelperModule = 
    type Data = private { Datum: int }
    let handle (data:Data): int = data.Datum
    let make x = { Datum = x }
    
module public Module =    
    type public Data = private { Thing: HelperModule.Data }
    let public getInt (data:Data): int = HelperModule.handle data.Thing
    let makeData x = { Thing = HelperModule.make x }
    
let internal a = HelperModule.make 2

let b = Module.makeData 1

if HelperModule.handle a <> 2 || Module.getInt b <> 1 then failwith "Failed: 1"
