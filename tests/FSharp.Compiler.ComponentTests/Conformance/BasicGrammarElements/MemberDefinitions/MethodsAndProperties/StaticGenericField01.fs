// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Verify we can handle static generic fields

type Counter1<'a> = 
    static member P = sizeof<'a>

type Counter2<'a>() = 
    static let x = sizeof<'a>
    static member P = x
    member this.P2 = Counter2<'a>.P


// Static field
if Counter1<int32>.P <> 4 then failwith "Failed: 1"
if Counter1<int64>.P <> 8 then failwith "Failed: 2"

// Static field
if Counter2<int32>.P <> 4 then failwith "Failed: 3"
if Counter2<int64>.P <> 8 then failwith "Failed: 4"

// Instance member access static field
let c32 = new Counter2<int32>()
let c64 = new Counter2<int64>()

if c32.P2 <> 4 then failwith "Failed: 5"
if c64.P2 <> 8 then failwith "Failed: 6"
