// #Regression #Conformance #TypeInference 
// Verify error if you only specify some, but not all, type args
//<Expects span="(8,11-8,14)" status="error" id="FS0001">This expression was expected to have type    'char'    but here has type    'float32'</Expects>

let f<'a> x (y : 'a) = (x, y)         // used to be error "Value restriction...". In Beta2, it is ok (val f: obj -> 'a -> obj * 'a)

let p = f 'a' 1                       // At this point, (val f: char -> 'a -> char * 'a)
let q = f 1.f 1                       // This is an error!


