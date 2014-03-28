module FSLib

type BigIntTest = 
    static member T(s) : bigint = NumericLiteralI.FromString(s)
    