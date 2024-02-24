// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Verify you can define properties with the get and set keyword

let mutable lastUsed = ("", 0)

type Foo =
    static member ReadOnly1 = lastUsed <- ("ReadOnly1", 0); 1
    static member ReadOnly2 with get () = lastUsed <- ("ReadOnly2", 0); 2

    static member WriteOnly with set  x = lastUsed <- ("WriteOnly", x)

    static member ReadWrite1 
        with get () = lastUsed <- ("ReadWrite1", 0); 3
        and  set  x = lastUsed <- ("ReadWrite1", x)

    static member ReadWrite2 
        with set  x = lastUsed <- ("ReadWrite2", x)
        and  get () = lastUsed <- ("ReadWrite2", 0); 4
 
if Foo.ReadOnly1 <> 1           then failwith "Failed: 1"
if lastUsed <> ("ReadOnly1", 0) then failwith "Failed: 2"

if Foo.ReadOnly2 <> 2           then failwith "Failed: 3"
if lastUsed <> ("ReadOnly2", 0) then failwith "Failed: 4"

Foo.WriteOnly <- -123
if lastUsed <> ("WriteOnly", -123) then failwith "Failed: 5"

if Foo.ReadWrite1 <> 3           then failwith "Failed: 6"
if lastUsed <> ("ReadWrite1", 0) then failwith "Failed: 7"

Foo.ReadWrite1 <- -456
if lastUsed <> ("ReadWrite1", -456) then failwith "Failed: 8"

if Foo.ReadWrite2 <> 4           then failwith "Failed: 9"
if lastUsed <> ("ReadWrite2", 0) then failwith "Failed: 10"

Foo.ReadWrite2 <- -789
if lastUsed <> ("ReadWrite2", -789) then failwith "Failed: 11"
