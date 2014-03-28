// #Conformance #DeclarationElements #MemberDefinitions #Overloading 
type Foo() =
    member this.DoStuff (x:int)    = "doStuff-1"
    member this.DoStuff (x:string) = "doStuff-2"
    member this.DoStuff (x:int, y:string)  = "doStuff-3"
    
let test = new Foo()
if test.DoStuff(0)      <> "doStuff-1" then exit 1
if test.DoStuff("")     <> "doStuff-2" then exit 1
if test.DoStuff(0, "")  <> "doStuff-3" then exit 1

exit 0    
