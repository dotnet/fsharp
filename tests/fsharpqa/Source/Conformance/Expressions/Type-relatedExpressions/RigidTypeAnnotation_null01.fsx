// #Conformance #TypeRelatedExpressions #TypeAnnotations 
#light
//
// Expressions
//
// Rigid type annotation
// expr : ty
//
// Misc test on 'null'
//<Expects status="success"></Expects>
#light

let a = null : string
let b = [(null : _); box 1]
let c = [(null : System.Nullable); null]

exit 0
