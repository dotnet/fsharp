// #Conformance #LexicalAnalysis #Operators 
#light

let mutable m : string = ""

let f a b c =
    sprintf "called with %d %d %d" a b c

let (?) (o:obj) (s:string) : (int -> int -> int -> string) =
    m <- s
    fun a b c -> sprintf "called with %d, %d, %d" a b c
    
    
let test() =
    let s = "Hello" ? Contains 10 10 10
    s
    
let res1 = test()
let res2 =  m

if res1 = "called with 10, 10, 10" && res2 = "Contains" then exit 0
else exit 1
