// #Conformance #LexicalAnalysis #Operators 
#light

let (?) (o : obj) (s : string) : string =
    let ty = o.GetType()
    sprintf "type: %s, member: %s" ty.Name s
    
    
let testSimple() =
    let res = "Hello" ? Length
    res
    
let res = testSimple()
if res = "type: String, member: Length" then exit 0
else exit 1
