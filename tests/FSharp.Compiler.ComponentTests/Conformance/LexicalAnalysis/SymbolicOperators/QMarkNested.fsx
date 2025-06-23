// #Conformance #LexicalAnalysis #Operators 
#light

let mutable members : string = ""

let (?) (o:obj) (s:string) : obj =
    members <- members + s + " "
    o
    
let test() =
    let _ = "Hello" ? Contains ? Startswith
    members
    
let res = test()
if res = "Contains Startswith " then exit 0
else exit 1
