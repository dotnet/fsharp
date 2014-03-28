module Pos01a
let x : int list ref = ref []
let y : string list ref = ref []

let f () = 3

module X1 = begin 
  type x = X | Y
  let y = 3
end


module X2 = begin 
  type x = X | Y
  let x = 3
end


module X3 = begin 
  let y = X2.X
end



type fieldsInDifferentOrder = { b:int; a: int }

(*
module TestModuleAbbreviatingNamespace = begin
    module M = System

    let v = 3
end

module TestModuleAbbreviatingNamespaceOCamlSig = begin
    module M = System

    let v = 3
end
*)

type C() = 
    class
        [<DefaultValue>]
        val mutable x : int
    end
    
