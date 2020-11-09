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

let temp = ref ""

type C() =
    inherit B()

    override this.Value with set x = temp := x; ()

type D() =
    inherit C()

    override this.Value with get () = "D"

type E() =
    inherit D()

    override this.Value with set x = temp := x + x; ()

// Test
let c = new C()

if c.Value <> "B" then exit 1
c.Value <- "::"
if !temp <> "::" then exit 1

// Derived class
let e = new E()
if e.Value <> "D" then exit 1

e.Value <- "10"
if !temp <> "1010" then exit 1

let eCastAsC = e :> C
if eCastAsC.Value <> "D" then exit 1
e.Value <- "!!"
if !temp <> "!!!!" then exit 1



exit 0
