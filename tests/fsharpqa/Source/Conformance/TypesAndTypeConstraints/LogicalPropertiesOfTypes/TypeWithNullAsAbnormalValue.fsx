// #Conformance #TypeConstraints 
#light

let x1 : int list = Unchecked.defaultof<int list>
let x2 : int[] = Unchecked.defaultof<int[]>

type R = { Name : string; Age : int }
let x3 = Unchecked.defaultof<R>

type C() = class end
let x4 = Unchecked.defaultof<C>
type I = interface end
let x5 = Unchecked.defaultof<I>

type U = A | B
let x6 = Unchecked.defaultof<U>

exit 0




