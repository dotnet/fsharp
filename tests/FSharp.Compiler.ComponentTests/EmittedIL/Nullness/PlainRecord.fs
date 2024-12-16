module MyTestModule


[<NoComparison;NoEquality>]
type MyRecord = {
    NonNullableString: string
    NullableString: string | null
}