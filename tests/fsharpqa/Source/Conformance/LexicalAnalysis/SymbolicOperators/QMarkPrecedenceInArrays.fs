// #Conformance #LexicalAnalysis #Operators 
#light

let (?) (o:obj) (s:string) : System.String =
    s
    
let test() =
    let s = [ "Hello" ? Contains; "Hello" ? Empty ]
    s
    
let res = test()
if res.Head = "Contains" && res.Tail.Head = "Empty" then exit 0
else exit 1
