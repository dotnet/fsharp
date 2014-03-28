namespace Microsoft.FSharp.Compatibility

type permutation = int -> int

module Permutation =

    let invalidArg arg msg = raise (new System.ArgumentException((msg:string),(arg:string)))        

    let ofFreshArray (arr:_[]) = 
        let arr2 = Array.zeroCreate arr.Length
        for i = 0 to arr.Length - 1 do 
            let x = arr.[i] 
            if x < 0 || x >= arr.Length then invalidArg "arr" "invalid permutation" 
            arr2.[x] <- 1
        for i = 0 to arr.Length - 1 do 
            if arr2.[i] <> 1 then invalidArg "arr" "invalid permutation"
        (fun k -> arr.[k])

    let ofArray (arr:_[]) = arr |> Array.copy |> ofFreshArray

    let of_array (arr:_[]) = ofArray arr

    let ofPairs  (mappings: seq<int * int>) = 
      let p = dict mappings 
      (fun k -> if p.ContainsKey k then p.[k] else k)

    let of_pairs  (mappings: seq<int * int>) =  ofPairs mappings

    let swap (n:int) (m:int) = 
      (fun k -> if k = n then m elif k = m then n else k)

    let reversal size = 
      if size <= 0 then invalidArg "size" "a permutation size must be positive";
      (fun k -> (size - 1 - k))

    let rotation size distance = 
      if size <= 0 then invalidArg "size" "a permutation size must be positive";
      if abs distance >= size then invalidArg "distance" "the absolute value of the distance must be less than the size of the permutation";
      (fun k -> (k + size + distance) % size)

    let identity (k:int) = k
    
    let inverse size p =
        if size <= 0 then invalidArg "size" "a permutation size must be positive";
        let arr2 = Array.zeroCreate size
        for i = 0 to size - 1 do
             arr2.[p i] <- i
        ofFreshArray arr2

    