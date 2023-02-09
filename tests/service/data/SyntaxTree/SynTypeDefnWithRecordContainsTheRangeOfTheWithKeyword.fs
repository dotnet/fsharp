type Foo =
    { Bar : int }
    with
        member this.Meh (v:int) = this.Bar + v