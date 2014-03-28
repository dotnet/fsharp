// #Regression #TypeProvider #Fields #Literals
// This is regression test for DevDiv:214000, but it also covers field literals in general

//<Expects status="success">val q : Quotations\.Expr<string> = Value \("Hello"\)</Expects>
//<Expects status="success">PASS</Expects>
//<Expects status="success">Call \(None, op_Equality,</Expects>
//<Expects status="success">\[Value \(18446744073709551615UL\),</Expects>
//<Expects status="success">Value \(18446744073709551615UL\)\]\), Value \(false\)\)</Expects>

// All the valid types (see ConstantObjToILFieldInit() in infos.fs)

// Spot-checking the field literals in quotations

let q = <@ N.T.Field_string @>;;

let u = <@ N.T.Field_bool && 
           N.T.Field_byte = System.Byte.MaxValue &&
           N.T.Field_char = 'A' &&
           N.T.Field_double = 1.2 &&
           N.T.Field_int = System.Int32.MaxValue &&
           N.T.Field_int16 = System.Int16.MaxValue &&
           N.T.Field_int64 = System.Int64.MaxValue &&
           N.T.Field_null =  null &&
           N.T.Field_sbyte = System.SByte.MaxValue &&
           N.T.Field_single = 2.1f &&
           N.T.Field_string = "Hello" &&
           N.T.Field_uint16 = System.UInt16.MaxValue &&
           N.T.Field_uint32 = System.UInt32.MaxValue &&
           N.T.Field_uint64 = System.UInt64.MaxValue
        @>

printfn "PASS"

#q;;

