// #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments 
#light

// Test optional parameters with prim types, obj types, disc unions, option types, ref types, mutable types, etc.

type People =
    | Jane of People
    | John of string
    | Joe of int

type Foo() =
    let mutable m_primType  = 1.0
    let mutable m_objType = "default"
    let mutable m_duType    = Joe 5
    let mutable m_ilo : int list option = None

    member this.Prim = m_primType
    member this.Obj = m_objType
    member this.DU = m_duType
    member this.ILO = m_ilo
   
    member this.MegaOptParams (?prim:float, ?obj:string, ?du:People, ?ilo:int list option, ?x:int) =
        let getArg (x:'a option) def:'a =
            match x with 
            | Some(value) -> value
            | None        -> def
            
        m_primType  <- getArg prim 100.0
        m_objType   <- getArg obj ""
        m_duType    <- getArg du (Jane(Joe(0)))
        m_ilo       <- getArg ilo (Some([1..10]))
        
let test = new Foo()

// Verify initial values for properties
if test.Prim <> 1.0         then failwith "Failed: 1"
if test.DU   <> Joe 5       then failwith "Failed: 2"
if test.Obj  <> "default"   then failwith "Failed: 3"
if test.ILO  <> None        then failwith "Failed: 4"

let x = 42
test.MegaOptParams(x = x)

// Verify default values were obtained, and modifed
if test.Prim <> 100.0           then failwith "Failed: 1"
if test.DU   <> Jane(Joe(0))    then failwith "Failed: 2"
if test.Obj  <> ""              then failwith "Failed: 3"
if test.ILO  <> Some([1..10])   then failwith "Failed: 4"
