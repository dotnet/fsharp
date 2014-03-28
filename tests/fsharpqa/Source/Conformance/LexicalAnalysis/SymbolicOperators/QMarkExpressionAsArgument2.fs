// #Conformance #LexicalAnalysis #Operators 
#light

let mutable m : string = ""

let (?) (o:obj) (s:string) : (int -> int -> string) =
    m <- s
    fun a b -> sprintf "called with %d %d" a b
    
    
let test() =
    let s = "Hello" ? ("Contain" + "s") 10 10    
    s
    
let res1 = test()
let res2 =  m

if res1 = "called with 10 10" && res2 = "Contains" then exit 0
else exit 1
