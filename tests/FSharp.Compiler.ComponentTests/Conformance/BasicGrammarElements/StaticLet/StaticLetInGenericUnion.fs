module Test

[<Struct>]
type MyUnion<'A,'B> = 
    | A of aval:'A
    | B of bval:'B
    | C

    static let sizeOfTCached = 
        printfn "Creating cached val for %s * %s" (typeof<'A>.Name) (typeof<'B>.Name)
        sizeof<MyUnion<'A,'B>>
        

    static let mutable perTyparInstMutableCounter = 0

    static member IncBySize() = 
        perTyparInstMutableCounter <- perTyparInstMutableCounter + sizeOfTCached

    static member GetCounter() = perTyparInstMutableCounter



MyUnion<int,int>.IncBySize()
printfn "sizeof MyUnion<int,int> = %i" (MyUnion<int,int>.GetCounter())

MyUnion<int,string>.IncBySize()
printfn "sizeof MyUnion<int,string> = %i" (MyUnion<int,string>.GetCounter())

MyUnion<string,string>.IncBySize()
printfn "sizeof MyUnion<string,string> = %i" (MyUnion<string,string>.GetCounter())

