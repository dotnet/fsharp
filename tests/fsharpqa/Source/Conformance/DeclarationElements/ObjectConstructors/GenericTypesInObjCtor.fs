// #Conformance #DeclarationElements #ObjectConstructors 
#light

// Verify ability to create a generic class with a generic type in its object ctor.

type Foo1<'a, 'b>(param1 : 'a, param2 : 'b) =
    let mutable m_a = param1
    let mutable m_b = param2
    
    member this.A with get () = m_a and set x = m_a <- x
    member this.B with get () = m_b and set x = m_b <- x

let test1 = new Foo1<int, list<string>>(42, [])
if test1.A <> 42 then exit 1
if test1.B <> [] then exit 1

test1.A <- -7
if test1.A <> -7 then exit 1
test1.B <- ["fever";"property"]
if test1.B <> ["fever";"property"] then exit 1

exit 0
