// #Conformance #ObjectOrientedTypes #Classes #LetBindings 
// Scoping:
// identifier introduced by let is local
//<Expects status="success"></Expects>
#light    
type C() = class
             static let mutable m = [1;2;3]
             static let n = [1;2]
             member x.X = m.Head :: n.Tail          // m and n are visible here
           end
let verify = if (C().X.Length = 2) then 0 else 1

exit verify
