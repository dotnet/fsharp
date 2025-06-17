// #Regression #Conformance #LexicalAnalysis #Operators 
#light

//<Expects id="FS0717" status="error">Unexpected type arguments</Expects>

let mutable m : string = ""

let foo (x : 'a) =
    x.ToString()

let (?) (o:obj) (s:string) : ('a -> string) =
    m <- s
    fun n -> n.ToString()
    
let test() =
    let r = "Hello" ? Contains<int>(10)
    r
