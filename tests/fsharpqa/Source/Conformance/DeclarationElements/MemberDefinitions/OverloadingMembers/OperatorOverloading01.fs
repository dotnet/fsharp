// #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Sanity check operator overloading
type OverloadOp1(x : int) =
    
    member this.Value = x
    
    static member (+) (lhs : OverloadOp1, rhs : int) =
        new OverloadOp1(lhs.Value + rhs)

    static member (+) (lhs : OverloadOp1, rhs : OverloadOp1) =
        new OverloadOp1(lhs.Value + rhs.Value)
        
let test1() =
    let x = new OverloadOp1(0)
    let y = x + 2
    let z = y + y

    if x.Value <> 0 then exit 1
    if y.Value <> 2 then exit 2
    if z.Value <> 4 then exit 3
    ()

// Run the test    
test1() 

exit 0
