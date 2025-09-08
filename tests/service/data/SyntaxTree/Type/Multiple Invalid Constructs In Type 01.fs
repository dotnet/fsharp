// Expected: Multiple warnings for module, type, exception, and open
module Module

type MultiTest =
    | Case1
    | Case2
    module NestedModule = begin end
    type NestedType = int
    exception NestedExc of string
    open System.Collections
