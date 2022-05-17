// #Conformance #ObjectOrientedTypes #Classes #TypeInference #ValueRestriction

//<Expects status="success"></Expects>

// We expect no value restriciton here. The inferred signature is:
//     val f1<'T> : ?1
// Here 'f1' is a type function, so the value restriction is not applied. The type inference variable
// represents hidden state with no possible generalization point in the signature.
let f1<'T> = 
    let x = ref []
    x
