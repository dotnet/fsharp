// #Conformance #DeclarationElements #ObjectConstructors 
#light

type Foo(arg1:string, arg2:int) =
    let m_value = arg2
    member this.Property = m_value
    

let test1 = new Foo("foo", 42)
if test1.Property <> 42 then exit 1

let test2 = new Foo("king", -9)
if test2.Property <> -9 then exit 1

exit 0
