// #Conformance #DeclarationElements #LetBindings #TypeTests 
#light

// Tests for sizeof<'a> type function

// This is a little complicated, but trust me it works out.
type T<'a>() = 
    static member P<'b>() = sizeof<'b>
    member x.M = T.P<'a>()

if (new T<byte>()).M  <> 1 then failwith "Failed: 1"
if (new T<int>()).M  <> 4 then failwith "Failed: 1"
if (new T<uint64>()).M <> 8 then failwith "Failed: 1"
