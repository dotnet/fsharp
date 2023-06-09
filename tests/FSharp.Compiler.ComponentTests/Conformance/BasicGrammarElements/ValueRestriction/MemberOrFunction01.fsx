// #Conformance #ObjectOrientedTypes #Classes #TypeInference #ValueRestriction

//<Expects status="success"></Expects>

// We expect no value restriciton here. The inferred signature is:
//     type C0 = 
//        new : unit -> C0
//        member ToList : unit -> ?1
// These are both members/functions, hence the value restriction is not applied to either.
// The type inference variable represents hidden state with no possible generalization point in the signature.
type C0() = 
    let mutable x = []
    member q.ToList() = x