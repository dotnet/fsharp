// #Conformance #LexicalAnalysis #Operators 
#light

let mutable result : string = ""

let (?<-) (o : obj) (m : string) (v : obj) =
    result <- sprintf "%s = %A" m v
    
let test() = 
    "hello" ? Contains <- 10
    
test()
if result = "Contains = 10" then exit 0
else exit 1
