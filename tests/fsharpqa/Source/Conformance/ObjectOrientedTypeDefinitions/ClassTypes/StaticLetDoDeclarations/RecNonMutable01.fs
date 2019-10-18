// #Conformance #ObjectOrientedTypes #Classes #LetBindings 
// rec (non-mutable)
//<Expects status="success"></Expects>
#light
type C() = class
             static let rec f a = if a = 0 then 1 else a*f(a-1) 
             member x.X a = f a
           end
let verify = if (C().X 4 = 24) then 0 else 1

exit verify

