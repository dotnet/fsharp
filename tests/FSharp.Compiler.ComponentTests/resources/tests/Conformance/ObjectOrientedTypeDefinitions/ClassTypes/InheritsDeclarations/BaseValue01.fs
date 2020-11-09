// #Conformance #ObjectOrientedTypes #Classes #Inheritance 
#light

// Verify that the 'base' keyword refers to base type

type Foo () =
   abstract DoStuff : unit -> string
   default this.DoStuff ()  = "Foo"
   override this.ToString() = "Foo"
   
type Bar () =
    inherit Foo()
    
    member this.BaseDoStuff   = base.DoStuff()
    member this.BaseToStringR = base.ToString()
    member this.ThisDoStuff   = this.DoStuff()
    member this.ThisToStringR = this.ToString()
    
    override this.DoStuff () = "Bar"
    override this.ToString() = "Bar"
    
    
let testFoo = new Foo()
let testBar = new Bar()

if testBar.BaseDoStuff   <> "Foo" then exit 1
if testBar.BaseToStringR <> "Foo" then exit 1

if testBar.ThisDoStuff   <> "Bar" then exit 1
if testBar.ThisToStringR <> "Bar" then exit 1

exit 0
