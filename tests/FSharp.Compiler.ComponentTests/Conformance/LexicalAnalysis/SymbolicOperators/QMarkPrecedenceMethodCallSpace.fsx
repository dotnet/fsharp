// #Conformance #LexicalAnalysis #Operators 
#light

let mutable m : string = ""

let (?) (o:obj) (s:string) : System.String =
    m <- s
    ""
    
    
let test() =
    let s = "Hello" ? Contains . GetType()
    s.Name
    
let res1 = test()
let res2 =  m

if res1 = "String" && res2 = "Contains" then exit 0
else exit 1
