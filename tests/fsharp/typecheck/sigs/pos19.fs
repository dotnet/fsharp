
// This test is related to F# 4.0+ design change "https://fslang.uservoice.com/forums/245727-f-language/suggestions/6107641-make-microsoft-prefix-optional-when-using-core-f"
//
// In this test, we simulate user code that has defined types in FSharp.Collections. We check that the resolution
// of types in FSharp.Collections proceeds as previously for user-defined types.

namespace FSharp.Collections

    type List<'T> = A | B
    type Map<'T> = C | D


namespace Pos19

    module N = 
        open FSharp.Collections

        // check "List" resolves to the FSharp.Collections.List defined above
        let f (x: List<int>) = 
           match x with 
           | A -> 1 
           | B -> 2

        let y = f A 

namespace Test3

    module N = 
        open FSharp.Collections

        // check "List" resolves to the FSharp.Collections.List defined above
        let f1 (x: List<int>) = 
           match x with 
           | A -> 1 
           | B -> 2

        let y1 = f1 A 

        // check "List" resolves to the FSharp.Collections.List defined above
        let f2 (x: global.FSharp.Collections.List<int>) = 
           match x with 
           | A -> 1 
           | B -> 2

        let y2 = f2 A 

        // check "List" resolves to the FSharp.Collections.List defined above
        let f3 (x: FSharp.Collections.List<int>) = 
           match x with 
           | A -> 1 
           | B -> 2

        let y3 = f3 A 


        // check "Map" resolves to the FSharp.Collections.List defined above
        let g1 (x: Map<int>) = 
           match x with 
           | C -> 1 
           | D -> 2

        let z1 = g1 C 

        // check "List" resolves to the FSharp.Collections.List defined above
        let g2 (x: global.FSharp.Collections.Map<int>) = 
           match x with 
           | C -> 1 
           | D -> 2

        let z2 = g2 C 


        // check "List" resolves to the FSharp.Collections.List defined above
        let g3 (x: FSharp.Collections.Map<int>) = 
           match x with 
           | C -> 1 
           | D -> 2

        let z3 = g3 C 
