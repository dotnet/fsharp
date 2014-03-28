
module Library

module M :
  val internal internalF          : int -> int
  val internal signatureInternalF : int -> int    
  val publicF                     : int -> int

module internal P :
    type internal InternalClass =
        new : int -> InternalClass
        member X : int

    val internal InternalObject : InternalClass
