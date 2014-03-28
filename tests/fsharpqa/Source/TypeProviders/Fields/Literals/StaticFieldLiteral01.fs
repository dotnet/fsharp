// #Regression #TypeProvider #Fields #Literals
// This is regression test for DevDiv:214000, but it also covers field literals in general
//<Expects status="success"></Expects>

// All the valid types (see ConstantObjToILFieldInit() in infos.fs)

let e = N.T.Field_bool && 
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

exit <| if e then 0 else 1
