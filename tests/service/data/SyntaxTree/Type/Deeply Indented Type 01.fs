// Testing: Invalid constructs in deeply nested type
module OuterModule

module InnerModule =
    module DeeplyNested =
        type IndentedType =
            type NestedType = int
            module NestedModule =
                let x = 1
            exception NestedExc
