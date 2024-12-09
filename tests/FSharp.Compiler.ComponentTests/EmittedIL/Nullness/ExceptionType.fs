module MyLibrary

[<NoEquality;NoComparison>]
exception JustStringE of string
[<NoEquality;NoComparison>]
exception TwoStrings of M1:string * M2:(string|null)
[<NoEquality;NoComparison>]
exception NullableStringE of (string|null)
[<NoEquality;NoComparison>]
exception NullableMessage of Message:(string|null)