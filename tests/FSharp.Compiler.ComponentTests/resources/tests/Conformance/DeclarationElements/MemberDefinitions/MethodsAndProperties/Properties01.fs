// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Verify readonly properties

type DiscUnion =
    | A of int
    | B
    | C of string * int
    member this.Value =
        match this with
        | A(x) -> x
        | B    -> 0
        | C(s, x) -> s.Length + x


if A(42).Value         <> 42 then exit 1
if B.Value             <>  0 then exit 1
if C("Steve", 2).Value <>  7 then exit 1

exit 0
