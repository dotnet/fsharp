
module Lib

type okType
type missingTypeInImplementation
type missingConstructorInSignature = A | B
type missingFieldInSignature = { a: int }
type fieldsInWrongOrder = { a: int; b:int }
type missingTypeVariableInSignature = { f1: int }
type missingInterfaceInSignature<'t> =
   class
   end
type missingInterfaceInImplementation<'t> =
   class
     interface System.IComparable
   end


type z = A | B
