module Test

// Two same-signature extension operators on System.String that differ ONLY by a return-type-determined
// instantiation. Regression for a return-type-blind optimizer-replay key/identity: both call sites
// shared a key and identity, so the first-recorded concrete solution (its method instantiation) was
// replayed at the other site, producing a ResizeArray<string> backed by a runtime List<int> ->
// heap type-confusion when a string was stored.
module A =
    type System.String with
        static member ( * ) (s: string, n: int) : ResizeArray<'T> = ResizeArray<'T>()

module B =
    type System.String with
        static member ( * ) (s: string, n: int) : ResizeArray<'T> = ResizeArray<'T>()

open A
open B

let a : ResizeArray<int> = "x" * 2
let b : ResizeArray<string> = "y" * 3

// If the wrong instantiation is replayed, b's backing store is List<int> and this corrupts the heap.
b.Add("hello")
if b.Count <> 1 then failwithf "expected b.Count=1, got %d" b.Count
if b.[0] <> "hello" then failwithf "expected \"hello\", got %s" b.[0]
a.Add(42)
if a.[0] <> 42 then failwithf "expected 42, got %d" a.[0]
