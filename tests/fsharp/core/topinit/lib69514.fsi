namespace Lib

module File1 = 
    val mutable discState : System.DateTime


module File2 =
    [<Struct>]
    type DiscState =
        new : int -> DiscState
        member Rep : int
 
    val mutable discState : DiscState


 
