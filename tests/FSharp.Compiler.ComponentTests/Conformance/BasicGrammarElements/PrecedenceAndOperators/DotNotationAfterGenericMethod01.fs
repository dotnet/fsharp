// #Regression #Conformance #BasicGrammarElements #Operators 
// FSharp1.0:4760 - A method<instantiated>(with,arguments) can not appear mid-way down a path-lookup



type Variable<'a>() =
    static member New<'T>() = new Variable<'T>()
    static member NewDouble() = new Variable<double>()   
    member this.Named(x:string) = this

let v2  = Variable<int>.New<double>().Named("v")
let v3  = Variable<int>.NewDouble().Named("v")
