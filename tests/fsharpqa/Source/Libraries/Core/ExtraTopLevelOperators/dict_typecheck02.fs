// #Regression #Llibraries 
// Regression test for FSHARP1.0:5365
//<Expects status="success"></Expects>

module N.M

 open System
 open System.Collections.Generic

 type ICloneable<'a> =
    abstract Clone : unit -> 'a

 type DictionaryFeature<'key> (dict: IDictionary<'key, int>) =
    member this.Add key value =
        dict.[key] <- value
