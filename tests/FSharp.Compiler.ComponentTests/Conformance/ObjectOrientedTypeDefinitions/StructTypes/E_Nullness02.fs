// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Verify error struct contains a field which cannot be null.
// Verify this is chained correctly.
//<Expects id="FS0688" span="(19,9-19,28)" status="error">The default, zero-initializing constructor of a struct type may only be used if all the fields of the struct type admit default initialization</Expects>

type DiscUnion = A | B of int

type StructType =
    struct
        val m_x : DiscUnion
        member this.Value = this.m_x
    end

[<Struct>]
type StructTypeTwo =
    val m_x : StructType   // Which contains a field which shouldn't be null
     member this.Value = this.m_x

let x = new StructTypeTwo()
