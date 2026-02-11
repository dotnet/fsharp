// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression for 4643:
// infinite loop in typechecker - caused by recursive struct check via self typed static field

[<Struct>]
type RIP(x:int) =

   [<DefaultValue>]
   static val mutable private y : RIP


[<Struct>]
type arg_unused_is_RIP(x:RIP) = 
    struct 
    end

[<Struct>]
type arg_used_is_RIP(x:RIP) =
     member this.X = x

[<Struct>]
type field_is_RIP =
   val x :RIP

// If we have compiled and instantiated these structs
// then we are good to go.

let t1 = new RIP(), new RIP(1)
let t2 = new arg_unused_is_RIP(fst t1), new arg_unused_is_RIP(snd t1)
let t3 = new arg_used_is_RIP(fst t1),   new arg_used_is_RIP(snd t1)
let t4 = new field_is_RIP(), new field_is_RIP()

exit 0
