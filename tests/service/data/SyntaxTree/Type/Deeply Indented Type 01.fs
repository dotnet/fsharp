// Expected: Multiple warnings for nested invalid constructs
module OuterModule

module InnerModule =
    module DeeplyNested =
        type IndentedType =
            | Case1
            | Case2
            type NestedType = int  // Should warn
            module NestedModule =  // Should warn
                let x = 1
            exception NestedExc of string  // Should warn
