type Foo() =
    member val MinValue: int = (open type System.Int32; MinValue) with get, set
