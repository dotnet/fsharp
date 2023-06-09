// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light

// FSB 1368, named arguments implicitly using property setters for generic class do not typecheck correctly

module GenericClass =
    type S<'a,'b> =
        class
           val mutable x : 'a
           val mutable y : 'b
           member obj.X with set(v) = obj.x <- v
           member obj.Y with set(v) = obj.y <- v
           new(a,b) = { x=a; y=b }
        end

    // Standard construction
    let x1 = S<int,string>(1,"1")
    if x1.x <> 1   then failwith "Failed: 1"
    if x1.y <> "1" then failwith "Failed: 2"
    
    // Named-argument constructor
    let x2 = S<int,string>(b="2",a=2)
    if x2.x <> 2   then failwith "Failed: 3"
    if x2.y <> "2" then failwith "Failed: 4"
    
    // Constructor with property setters afterwards
    let x3 = S<int,string>(3,"3",X= -3, Y= "-3")
    if x3.x <> -3   then failwith "Failed: 5"
    if x3.y <> "-3" then failwith "Failed: 6"
