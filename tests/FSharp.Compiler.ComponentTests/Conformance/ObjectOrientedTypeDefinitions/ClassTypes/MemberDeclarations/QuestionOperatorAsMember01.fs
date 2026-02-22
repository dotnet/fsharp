// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions #Operators
//Dev11:277085, used to give a warning here

type X() = 
    static member (?<-) (this, name, value) = printfn "%x: %s <- %d" (hash this) name value