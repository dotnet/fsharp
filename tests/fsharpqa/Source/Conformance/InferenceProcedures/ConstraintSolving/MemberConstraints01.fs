// #Conformance #TypeInference #TypeConstraints 
// Verify you can overload operators on a type and not add all the extra jazz
// such as inlining and the ^ operator.

type Foo(x : int) = 
    member this.Val = x
    
    static member (-->) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (-->) ((src : Foo), (target : int)) = new Foo(src.Val + target)
    
    static member (+) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (+) ((src : Foo), (target : int)) = new Foo(src.Val + target)
    
let x = Foo(3) --> 4
let y = Foo(3) --> Foo(4)
let x2 = Foo(3) + 4
let y2 = Foo(3) + Foo(4)

if x.Val <> 7 then exit 1
if y.Val <> 7 then exit 1
if x2.Val <> 7 then exit 1
if y2.Val <> 7 then exit 1

exit 0
