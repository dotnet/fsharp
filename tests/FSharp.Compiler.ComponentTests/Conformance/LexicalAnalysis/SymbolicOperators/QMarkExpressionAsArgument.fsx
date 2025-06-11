// #Conformance #LexicalAnalysis #Operators 
#light

let mutable m : string = ""

let (?) (o:obj) (s:string) : (int -> string) =
    m <- s
    fun n -> sprintf "called with %d" n
    
    
let test() =
    let s = "Hello" ? ("Contains") (10)
    s
    
let res1 = test()
let res2 =  m

if res1 = "called with 10" && res2 = "Contains" then exit 0
else exit 1
