// #Conformance #ObjectOrientedTypes #Classes #TypeInference #ValueRestriction

//<Expects status="success"></Expects>

// We expect no value restriciton here. The inferred signature is:
//     type SC1<'T> = 
//        new : unit -> SC0
//        static member ToList : unit -> ?1
// These are both members/functions, hence the value restriction is not applied to either.
// The type inference variable represents hidden state with no possible generalization point in the signature.
type SC1<'T>() = 
    static let mutable x = []
    static member ToList() = x