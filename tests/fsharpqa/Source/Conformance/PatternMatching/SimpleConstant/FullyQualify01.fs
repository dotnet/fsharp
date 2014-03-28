// #Conformance #PatternMatching #Constants 
#light

// Verify ability to pattern match against a fully qualified constant or literal

module Test =

    module A =
        module B = 
            module C = 
                module D = 
                     type DU =
                         | A of int
                         | B of string
                         | C

                     [<Literal>]
                     let literalValue = "AString"


    let test x =
        match x with
        | A.B.C.D.A(x)                               -> 1
        | A.B.C.D.B(x) when x = A.B.C.D.literalValue -> 2
        | A.B.C.D.B(x)                               -> 3
        | _ -> 0


    if test (A.B.C.D.A(5)) <> 1 then exit 1   
    if test (A.B.C.D.B(A.B.C.D.literalValue)) <> 2 then exit 1
    if test (A.B.C.D.C)    <> 0 then exit 1

    exit 0
