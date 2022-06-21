// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

type Foo<'a>() = 
    let mutable m_val : 'a list = []
    
    member this.Add x = m_val <- m_val @ [x]
    member this.Length = List.length m_val
    
let f1 = new Foo<int>()
f1.Add 1; f1.Add 2; f1.Add 3; 
if f1.Length <> 3 then failwith "Failed: 1"

let f2 = new Foo<string>()
f2.Add "one"; f2.Add "two"
if f2.Length <> 2 then failwith "Failed: 2"
