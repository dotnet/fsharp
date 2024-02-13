module rec MyTestModule

type MaybeString = string | null
type MaybeMyCustomType = (MyCustomType | null)

type MyCustomType (x: MaybeString, y: string) = 

    static let mutable uglyGlobalMutableString : string = ""
    static let mutable uglyGlobalMutableNullableString : MaybeString = null
    static let mutable dict : Map<MaybeString,MaybeString> = Map.empty

    member val Nullable = x
    member val NonNullable = y
    member val JustSomeInt = 42

    static member GiveMeNull() : MaybeString = null
    static member GiveMeString() : string = ""

    member this.UnitFunc() = ()
    member this.GetThis() = this
    member this.GetThisOrNull() : MaybeMyCustomType = null

    member this.Item
        with get (index) = dict.[index]
        and set index value = dict <- dict.Add(index,value)