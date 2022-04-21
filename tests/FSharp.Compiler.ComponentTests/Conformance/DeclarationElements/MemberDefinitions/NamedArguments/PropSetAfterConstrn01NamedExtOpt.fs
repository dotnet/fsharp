// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light


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
module GenericClassExt =
    
    module Extensions =
        open GenericClass
        type S<'a,'b> with
            member x.XProxyOptional with set (v:'a) = x.X  <- v
            member x.YProxyOptional with set (v:'b) = x.Y  <- v

module Test =
    open GenericClassExt.Extensions
    open GenericClass
    let x1 = S<_,_>(1,"1", XProxyIntrinsic = 44, YProxyIntrinsic = "44")
    if x1.x <> 44   then failwith "Failed: 1"
    if x1.y <> "44" then failwith "Failed: 2"

    let x3 = S<_,_>(1,"1", XProxyOptional = 44, YProxyOptional = "44")
    if x3.x <> 44   then failwith "Failed: 3"
    if x3.y <> "44" then failwith "Failed: 4"
