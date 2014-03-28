module Pos01a
val x : int list ref
val f : unit -> int

module X1 : sig val y : int  end 
module X2 : sig type x = X | Y  end 

module X3 : sig val y : X2.x  end 

type fieldsInDifferentOrder = { b:int; a: int }

//module TestModuleAbbreviatingNamespace : begin
//    module M = System
//
//    val v : M.Int32
//end
//
//module TestModuleAbbreviatingNamespaceOCamlSig : sig
//    module M = System
//
//    val v : M.Int32
//end

type C = 
    class
        [<DefaultValue>]
        val mutable x : int
        new : unit -> C
    end
