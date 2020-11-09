// #Conformance #SyntacticSugar 
#light

// Verify e1.[e2] is just syntactic sugar for calling the 'item' property.

type Foo() =
   let mutable m_lastSet = (0, 0)
   member this.LastSet = m_lastSet
   member this.Item with get x   = x * -1
                    and  set x y = m_lastSet <- (x, y)

let t = new Foo()

if t.[5]  <> -5 then exit 1
if t.[-1] <> 1 then exit 1

t.[3] <- 10
if t.LastSet <> (3, 10) then exit 1

exit 0
