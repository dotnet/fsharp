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
    type S<'a,'b> with
        member x.XProxy with set v = x.X  <- v
        member x.YProxy with set v = x.Y  <- v

    // Standard construction
    let x1 = S<int,string>(1,"1", XProxy = 2, YProxy = "2")
    if x1.x <> 2   then failwith "Failed: 1"
    if x1.y <> "2" then failwith "Failed: 2"

