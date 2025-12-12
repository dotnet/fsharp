// #Conformance #TypeRelatedExpressions #TypeAnnotations 

//
// Expressions
//
// Rigid type annotation
// expr : ty
//
// Misc test on 'null'
//<Expects status="success"></Expects>


let a = null : string
let b = [(null : _); box 1]
let c = [(null : System.Nullable); null]

exit 0
