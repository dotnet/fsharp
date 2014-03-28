// #Regression #Conformance #TypeInference #TypeConstraints 
#light

// Verify error message when an enum constraint fails.
//<Expects id="FS0001" status="error">The type 'int16' does not match the type 'int64'</Expects>

type EnumValUtils<'a when 'a : enum<int16>> =
    static member Print (x : 'a) = 
        let eVal = printfn "%A" x
        ()
        
type Int64Enum = 
    | Zero = 0L
    | One  = 1L
    
EnumValUtils<Int64Enum>.Print(Int64Enum.Zero)

exit 1
