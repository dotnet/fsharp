// #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light

type Foo() =
    static member MethodWithNamedArgs (one:int, two:int, three:float, four:float) = 
        let onetwo = (one |> float) + (two * two |> float)
        onetwo + three ** 3.0 + four ** 4.0
        
let r1 = Foo.MethodWithNamedArgs(1, 2, 3.0, 4.0)
let r2 = Foo.MethodWithNamedArgs(1, 2, 3.0, four=4.0)
let r3 = Foo.MethodWithNamedArgs(1, 2, three=3.0, four=4.0)
let r4 = Foo.MethodWithNamedArgs(1, two=2, three=3.0, four=4.0)
let r5 = Foo.MethodWithNamedArgs(one=1, two=2, three=3.0, four=4.0)
let r6 = Foo.MethodWithNamedArgs(two=2, one=1, four=4.0, three=3.0)
let r7 = Foo.MethodWithNamedArgs(four=4.0, two=2, one=1,  three=3.0)

if r1 <> r2 || r2 <> r3 || r3 <> r4 || r4 <> r5 || r6 <> r7 then
    failwith "Failed: 1"
