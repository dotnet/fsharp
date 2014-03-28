// #Regression #TypeProvider #Fields #Literals
// This is regression test for DevDiv:214000, but it also covers field literals in general
//<Expects status="success">val e : bool = true</Expects>

let t = new N.T()

let e = t.Instance_Field_bool && 
        t.Instance_Field_byte = System.Byte.MaxValue &&
        t.Instance_Field_char = 'A' &&
        t.Instance_Field_double = 1.2 &&
        t.Instance_Field_int = System.Int32.MaxValue &&
        t.Instance_Field_int16 = System.Int16.MaxValue &&
        t.Instance_Field_int64 = System.Int64.MaxValue &&
        t.Instance_Field_null =  null &&
        t.Instance_Field_sbyte = System.SByte.MaxValue &&
        t.Instance_Field_single = 2.1f &&
        t.Instance_Field_string = "Hello" &&
        t.Instance_Field_uint16 = System.UInt16.MaxValue &&
        t.Instance_Field_uint32 = System.UInt32.MaxValue &&
        t.Instance_Field_uint64 = System.UInt64.MaxValue

#q;;
