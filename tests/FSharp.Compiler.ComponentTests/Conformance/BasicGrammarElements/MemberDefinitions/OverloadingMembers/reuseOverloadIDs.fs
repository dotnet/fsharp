// #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// This test used to be about the [<OverloadID>], which is now gone
// Reuse the same overload IDs, but with different method sets
type Foo() =
    member this.DoStuff1 (x:int)    = "doStuff1-1"
    member this.DoStuff1 (x:string) = "doStuff1-2"
    
    member this.DoStuff2 (x:int)    = "doStuff2-1"
    member this.DoStuff2 (x:string) = "doStuff2-2"


let test = new Foo()
if test.DoStuff1(0)      <> "doStuff1-1" then failwith "Failed: 1"
if test.DoStuff1("")     <> "doStuff1-2" then failwith "Failed: 2"

if test.DoStuff2(0)      <> "doStuff2-1" then failwith "Failed: 3"
if test.DoStuff2("")     <> "doStuff2-2" then failwith "Failed: 4"
