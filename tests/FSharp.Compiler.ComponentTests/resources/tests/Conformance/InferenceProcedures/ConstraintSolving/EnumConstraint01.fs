// #Conformance #TypeInference #TypeConstraints 
#light

// Verify happy case with an enum constraint

type EnumValUtils<'a, 'b when 'b : enum<'a>> =
    static member Test (x : 'b) = ()

type Int64Enum =
    | Zero = 0L
    | One  = 1L

type Int16Enum =
    | Zero = 0s
    | One  = 1s

EnumValUtils<int16, Int16Enum>.Test(Int16Enum.Zero)
EnumValUtils<int64, Int64Enum>.Test(Int64Enum.Zero)

exit 0
