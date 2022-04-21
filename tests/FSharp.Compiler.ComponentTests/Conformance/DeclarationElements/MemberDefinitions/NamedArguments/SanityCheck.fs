// #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light

type EType = A = 1 | B = 2

type Foo() =
    member this.MethWithNamedParams (param1:int, param2:EType, param3:string) = 
        (param1 + (int param2) + param3.Length)
    
    
let x = new Foo()
let result = x.MethWithNamedParams(param3="stuff", param2=EType.A, param1 = 10)
if result <> 16 then failwith "Failed: 1"
