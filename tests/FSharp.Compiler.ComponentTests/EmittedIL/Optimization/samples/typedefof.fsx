// <testmetadata>
// { "optimization": { "reported_in": "#5019", "reported_by": "@rspeele,@ForNever", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>
open System.Collections.Generic
type C() =
    member this.M() =
        let x = typedefof<List<_>>
        ()
        

System.Console.WriteLine()