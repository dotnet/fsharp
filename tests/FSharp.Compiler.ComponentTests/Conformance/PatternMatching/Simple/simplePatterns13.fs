// #Conformance #PatternMatching 
#light

module SomeModule =
    type DU =
        | A of int
        | B of string
        | C

module MainModule =
    let getCount thingy =
        match thingy with
        | SomeModule.DU.A x -> x
        | SomeModule.DU.B x -> x.Length
        | SomeModule.DU.C   -> 0
    
    if getCount (SomeModule.DU.A(42)) <> 42 then exit 1    
    if getCount (SomeModule.DU.B("cat")) <> 3 then exit 1
    
    exit 0
