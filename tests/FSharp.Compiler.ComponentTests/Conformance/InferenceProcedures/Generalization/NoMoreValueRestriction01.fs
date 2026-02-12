// #Conformance #TypeInference 
// Verify error if you only specify some, but not all, type args
//<Expects status="success"></Expects>

let f<'a> x (y : 'a) = (x, y)         // used to be error "Value restriction...". In Beta2, it is ok (val f: obj -> 'a -> obj * 'a)

let p = f 'a' 1

(if p = ('a',1) then 0 else 1) |> exit

