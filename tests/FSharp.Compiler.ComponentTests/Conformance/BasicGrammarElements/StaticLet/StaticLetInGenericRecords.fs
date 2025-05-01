module Test

type MyRecord<'T> = 
    {
        X: 'T
        Y: int
    }

    static let sizeOfT = sizeof<'T>
    static let cachedVal =
        printfn "Creating cached val for %s" (typeof<'T>.Name)
        { X = Unchecked.defaultof<'T> ; Y = 15}
    static let mutable perTyparInstMutableCounter = 0

    static member IncBySize() = 
        perTyparInstMutableCounter <- perTyparInstMutableCounter + sizeOfT

    static member GetCounter() = perTyparInstMutableCounter



MyRecord<int>.IncBySize()
MyRecord<int>.IncBySize()

printfn "2x sizeof<int> = %i" (MyRecord<int>.GetCounter())

MyRecord<string>.IncBySize()
MyRecord<string>.IncBySize()

printfn "2x sizeof<string> = %i" (MyRecord<string>.GetCounter())

MyRecord<System.DateTime>.IncBySize()
MyRecord<System.DateTime>.IncBySize()

printfn "2x sizeof<System.DateTime> = %i" (MyRecord<System.DateTime>.GetCounter())

MyRecord<System.Uri>.IncBySize()
MyRecord<System.Uri>.IncBySize()

printfn "2x sizeof<System.Uri> = %i" (MyRecord<System.Uri>.GetCounter())

