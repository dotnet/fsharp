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
        member x.XProxyIntrinsic with set (v:'a) = x.X  <- v
        member x.YProxyIntrinsic with set (v:'b) = x.Y  <- v
    module Extensions =
        type S<'a,'b> with
            member x.XProxyOptional with set (v:'a) = x.X  <- v
            member x.YProxyOptional with set (v:'b) = x.Y  <- v
    
    open Extensions
 
    // Standard construction
    let x1 = S<int,string>(1,"1", XProxyIntrinsic = 42, YProxyIntrinsic = "42")
    if x1.x <> 42   then failwith "Failed: 1"
    if x1.y <> "42" then failwith "Failed: 2"
    
    let x2 = S<_,_>(1,"1")
    x2.XProxyOptional <- 43
    x2.YProxyOptional <- "43"
    if x2.x <> 43   then failwith "Failed: 3"
    if x2.y <> "43" then failwith "Failed: 4"
 
    let x3 = S<_,_>(1,"1", XProxyOptional = 44, YProxyOptional = "44")
    if x3.x <> 44   then failwith "Failed: 5"
    if x3.y <> "44" then failwith "Failed: 6"
