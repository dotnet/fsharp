module StructPrivateField =
    [<Struct>]
    [<NoComparison;NoEquality>]
    type C =
        [<DefaultValue>]
        val mutable (* private. uncomment the private modifier to see an error *) goo : byte []        
        member this.P with set(x) = this.goo <- x