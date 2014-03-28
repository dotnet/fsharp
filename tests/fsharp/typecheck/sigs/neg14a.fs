
module Lib

type okType = X | Y 
type missingConstructorInSignature = A | B  | C
type missingFieldInSignature = { a: int; b: int }
type 'a missingTypeVariableInSignature = { f1: int }

type missingInterfaceInSignature<'t> =
   class
    interface System.IComparable with 
      member x.CompareTo(y) = 0
    end
   end

type missingInterfaceInImplementation<'t> = A | B


