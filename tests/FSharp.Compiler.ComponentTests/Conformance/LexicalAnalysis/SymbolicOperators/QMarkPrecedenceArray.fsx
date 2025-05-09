// #Conformance #LexicalAnalysis #Operators 
#light

let mutable m : string = ""

let (?) (o:obj) (s:string) : string array =
    m <- s
    [| "Index0" |]
    
    
let test() =
    let s = "Hello" ? Contains.[0]
    s
    
let res1 = test()
let res2 =  m

if res1 = "Index0" && res2 = "Contains" then exit 0
else exit 1
