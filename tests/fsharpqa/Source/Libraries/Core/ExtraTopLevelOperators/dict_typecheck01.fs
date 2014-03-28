// #Regression #Llibraries 
// Regression test for FSHARP1.0:5365
//<Expects status="success"></Expects>

module N.M

 open System
 open System.Collections.Generic

 type ICloneable<'a> =
    abstract Clone : unit -> 'a

 type DictionaryFeature<'key, 'dict when 'dict :> IDictionary<'key, int> and 'dict :> ICloneable<'dict>> (dict: 'dict) =
    member this.Add key value =
        dict.[key] <- value  // use to give error: value must be local and mutable in order to mutate the contents of a value type, e.g. 'let mutable x = ...'

