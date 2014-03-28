namespace Lib
 
module File1 =
 
    let mutable discState = System.DateTime.Now


module File2 =
    [<Struct>]
    type DiscState(rep : int) =
        member this.Rep = rep
 
    let mutable discState = DiscState(0)
 
