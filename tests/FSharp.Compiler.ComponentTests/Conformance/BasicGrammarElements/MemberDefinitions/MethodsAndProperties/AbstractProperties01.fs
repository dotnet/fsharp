// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Verify ability to override / inherit properties

[<AbstractClass>]
type A() =
    abstract Value : string with get, set

[<AbstractClass>]
type B() =
   inherit A()

   override this.Value with get () = "B"

let mutable temp = ""

type C() =
    inherit B()

    override this.Value with set x = temp <- x; ()

type D() =
    inherit C()

    override this.Value with get () = "D"

type E() =
    inherit D()

    override this.Value with set x = temp <- x + x; ()

// Test
let c = new C()

if c.Value <> "B" then failwith "Failed: 1"
c.Value <- "::"
if temp <> "::" then failwith "Failed: 2"

// Derived class
let e = new E()
if e.Value <> "D" then failwith "Failed: 3"

e.Value <- "10"
if temp <> "1010" then failwith "Failed: 4"

let eCastAsC = e :> C
if eCastAsC.Value <> "D" then failwith "Failed: 5"
e.Value <- "!!"
if temp <> "!!!!" then failwith "Failed: 6"
