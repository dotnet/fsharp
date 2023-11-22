// <testmetadata>
// { "optimization": { "reported_in": "#6454", "reported_by": "@auduchinok", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>
// [<Struct>]
type S<'T>(x: 'T[]) = 
    static let empty = S<'T>(null)
    static member Empty = empty
    
[<Struct>]
type S2<'T>(x: 'T[]) = 
    static let empty = S2<'T>.M()
    static member M() = empty
    
System.Console.WriteLine()