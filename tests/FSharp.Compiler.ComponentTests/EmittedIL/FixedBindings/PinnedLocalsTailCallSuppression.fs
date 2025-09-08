module PinnedLocalsTailCallSuppression
open Microsoft.FSharp.NativeInterop

// Test case that should NOT emit .tail. prefix because method has pinned locals
let rec tailCallWithPinnedLocal x =
    if x <= 0 then 
        0
    else
        let mutable thing = x
        use ptr = fixed &thing
        tailCallWithPinnedLocal (x - 1)  // This should NOT be a tail call due to pinned local

// Test case that SHOULD emit .tail. prefix because method has no pinned locals  
let rec normalTailCall x =
    if x <= 0 then 
        0
    else
        normalTailCall (x - 1)  // This should be a tail call

// Test case with nested functions - outer has pinned local, inner should not emit tail calls
let outerWithPinnedLocal x =
    let mutable thing = x
    use ptr = fixed &thing
    
    let rec innerRecursive y =
        if y <= 0 then 0
        else innerRecursive (y - 1)  // This should NOT be tail call due to outer pinned local
    
    innerRecursive x