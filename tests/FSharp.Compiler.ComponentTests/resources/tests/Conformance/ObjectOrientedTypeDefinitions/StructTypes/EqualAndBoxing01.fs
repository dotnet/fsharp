// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression test for FSHARP1.0:5535
// Struct type

type S1(x:int) = struct
                    member __.y = x
                end

type S2(x:int) = struct
                    member __.y = x + 1
                 end

let s1 = new S1(0)
let s2 = new S2(1)

let t1 = s1.Equals(box s2)        // expect: false, no exception!
let t2 = s1.Equals(box s1)        // expect: true, no exception!

(if not t1 && t2 then 0 else 1) |> exit
