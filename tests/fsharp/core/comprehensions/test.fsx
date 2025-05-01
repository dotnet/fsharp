// #Conformance #Sequences #Regression #ControlFlow #SyntacticSugar #ComputationExpressions 
#if TESTS_AS_APP
module Core_comprehensions
#endif
#light
let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let _ = test "coic23a" (Seq.toList { 'a' .. 'c' } = ['a';'b';'c'])

let _ = test "coic23q" (Seq.toList {1 .. 0} = [])
let _ = test "coic23w" (Seq.toList {1 .. 1} = [1])
let _ = test "coic23e" (Seq.toList {1 .. 3} = [1;2;3])
let _ = test "coic23r" (Seq.toList {1L .. 3L} = [1L;2L;3L])
let _ = test "coic23t" (Seq.toList {1UL .. 3UL} = [1UL;2UL;3UL])
let _ = test "coic23y" (Seq.toList {1ul .. 3ul} = [1ul;2ul;3ul])
let _ = test "coic23a" (Seq.toList {1y .. 3y} = [1y;2y;3y])
let _ = test "coic23s" (Seq.toList {1uy .. 3uy} = [1uy;2uy;3uy])
let _ = test "coic23d" (Seq.toList {1s .. 3s} = [1s;2s;3s])
//test "coic23" (Seq.toList {1I .. 3I} = [1I;2I;3I])
//test "coic23" (Seq.toList {1N .. 3N} = [1N;2N;3N])
let _ = test "coic23d" (Seq.toList {1us .. 3us} = [1us;2us;3us])
let _ = test "coic23f" (Seq.toList {3 .. 1} = [])
let _ = test "coic23g" (Seq.toList  {1.0..3.0} = [1.0;2.0;3.0])
let _ = test "coic23h" (Seq.toList  {1.0 .. 1.0 .. 3.0} = [1.0;2.0;3.0])
let _ = test "coic23j" (Seq.toList  {1.0 .. 1.0 .. 2.01 } = [1.0;2.0])
let _ = test "coic23k" (Seq.toList  {3.0 .. -1.0 .. 0.0} = [3.0;2.0;1.0;0.0])
let _ = test "coic23l" (Seq.toList  {4.0 .. -2.0 .. 0.0} = [4.0;2.0;0.0])
let _ = test "coic23z" (Seq.toList  {3 .. -1 .. -3} = [3;2;1;0; -1; -2; -3])
let _ = test "coic23x" (Seq.toList  {3 .. -2 .. -3}= [3;1; -1; -3])

let _ = test "coic23c" ([ 'a' .. 'c' ] = ['a';'b';'c'])

let _ = test "coic23v" ([ 1 .. 0 ] = [])
let _ = test "coic23b" ([ 1 .. 1 ] = [1])
let _ = test "coic23n" ([ 1 .. 3 ] = [1;2;3])
let _ = test "coic23m" ([ 1L .. 3L ]= [1L;2L;3L])
let _ = test "coic2342" ([ 1UL .. 3UL ] = [1UL;2UL;3UL])
let _ = test "coic233d2" ([ 1ul .. 3ul ] = [1ul;2ul;3ul])
let _ = test "coic23t34" ([ 1y .. 3y ] = [1y;2y;3y])
let _ = test "coic23cwe" ([ 1uy .. 3uy ] = [1uy;2uy;3uy])
let _ = test "coic23cu" ([ 1s .. 3s ] = [1s;2s;3s])
//test "coic23" ([ 1I .. 3I) = [1I;2I;3I])
//test "coic23" ([ 1N .. 3N) = [1N;2N;3N])
let _ = test "coic238n7" ([ 1us .. 3us ] = [1us;2us;3us])
let _ = test "coic23we" ([ 3 .. 1 ] = [])
let _ = test "coic23v38c5" ([ 1.0..3.0 ] = [1.0;2.0;3.0])
let _ = test "coic23v5wq" ([ 1.0 .. 1.0 .. 3.0 ] = [1.0;2.0;3.0])
let _ = test "coic23cv42" ([ 1.0 .. 1.0 .. 2.01  ] = [1.0;2.0])
let _ = test "coic23cd2" ([ 3.0 .. -1.0 .. 0.0 ] = [3.0;2.0;1.0;0.0])
let _ = test "coic23vq423" ([ 4.0 .. -2.0 .. 0.0 ] = [4.0;2.0;0.0])
let _ = test "coic23jyi" ([ 3 .. -1 .. -3] = [3;2;1;0; -1; -2; -3])
let _ = test "coic23my7" ([ 3 .. -2 .. -3 ]= [3;1; -1; -3])


let test3198 = 
    let mutable count = 0 in 
    for i in seq {1 .. 3} do 
        count <- count + 1;
        printf "i = %d\n" i 
    done;
    test "fi3n" (count = 3)


let test3198b = 
    let mutable count = 0 in 
    let ie = {1 .. 3} in
    for i in ie do 
        count <- count + 1;
        printf "i = %d\n" i 
    done;
    test "fi3n" (count = 3)

let test3198x = 
    let mutable count = 0 in 
    let ie = ref {1 .. 3} in
    
    for i in !ie do 
        ie := {1 .. 2};
        count <- count + 1;
        printf "i = %d\n" i 
    done;
    test "fi3n" (count = 3)

let test342879 = 
    let mutable count = 0 in 
    for i in seq {1 .. 3} do 
        for j in seq {i .. 3} do 
            count <- count + 1;
            printf "i = %d\n" i 
        done;
    done;
    test "fi3n" (count = 6)

let _ = 
  for i in seq {1 .. 3} do 
      printf "i = %d\n" i 
  done


let _ = test "coic23v\w" (Seq.toList  (seq {for i in seq {1 .. 3} -> i,i*i}) = [1,1;2,4;3,9] )

//{for x,y in [| (1,2) |]  -> x+y)


let _ = 
  test "coic23va" (Seq.toList  (seq {for i in seq {1 .. 5} do
                                         if i % 2 = 0 then yield i+100})
                  = [102;104])

let _ = 
  test "coic23eq" (Seq.toList  (seq {for i in seq {1 .. 3} -> i}) = [1;2;3])

let _ = 
  test "coic23avwa" (Seq.toList  (seq {for i in seq {1 .. 3} do
                                           for j in seq {1 .. 4} do
                                              yield i,j}) = [1,1;1,2;1,3;1,4;2,1;2,2;2,3;2,4;3,1;3,2;3,3;3,4])

let _ = 
  test "coic23bvee" (Seq.toList  (seq {for i in seq {1 .. 3} do
                                           if i % 2 = 1 then
                                             for j in seq {1 .. 4} do yield i,j}) = [1,1;1,2;1,3;1,4;3,1;3,2;3,3;3,4])


let _ = 
  test "coic23wcwec" (Seq.toList  (seq {for i in seq {1 .. 3} do
                                            for j in seq {i .. 3} do yield i,j}) = [1,1;1,2;1,3;2,2;2,3;3,3])

let ie1 = [ 1 .. 1 .. 3 ]

let _ = test "colc23cwe" ([ for i in ie1 do 
                            for j in seq {i .. 3} do yield i,j ] = [1,1;1,2;1,3;2,2;2,3;3,3])

let _ = test "colc23v\w" ([ for i in seq {1 .. 3} do yield i,i*i ] = [1,1;2,4;3,9])

//{for x,y in [| (1,2) |]  -> x+y)


let _ = test "colc23va" ([ for i in seq {1 .. 5} do
                           if i % 2 = 0 then yield i+100 ]
                        = [102;104])

let _ = test "colc23eq" ( [ for i in seq {1 .. 3} -> i ] = [1;2;3])

let _ = test "colc23avwa" ([ for i in seq {1 .. 3}  do
                             for j in seq {1 .. 4} do yield i,j ] = [1,1;1,2;1,3;1,4;2,1;2,2;2,3;2,4;3,1;3,2;3,3;3,4])

let _ = test "colc23bvee" ([ for i in seq {1 .. 3} do
                              if i % 2 = 1 then 
                                for j in seq {1 .. 4} do yield i,j ] = [1,1;1,2;1,3;1,4;3,1;3,2;3,3;3,4])


let _ = test "colc23wcwec" ([ for i in seq {1 .. 3} do
                              for j in seq {i .. 3} do yield i,j ] = [1,1;1,2;1,3;2,2;2,3;3,3])

let _ = test "colc23cwe" ([ for i in ie1 do
                            for j in seq {i .. 3} do yield i,j ] = [1,1;1,2;1,3;2,2;2,3;3,3])



let _ = test "coac23cwe" ([|for i in ie1 do
                            for j in seq {i .. 3} do yield i,j|] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

let _ = test "coac23v\w" ([|for i in seq {1 .. 3} -> i,i*i|] = [|1,1;2,4;3,9|])

//{for x,y in [| (1,2) |]  -> x+y)


let _ = test "coac23va" ([|for i in seq {1 .. 5} do
                            if i % 2 = 0 then yield i+100|]
                        = [|102;104|])

let _ = test "coac23eq" ( [|for i in seq {1 .. 3} -> i|] = [|1;2;3|])

let _ = test "coac23avwa" ([|for i in seq {1 .. 3} do
                             for j in seq {1 .. 4} do yield i,j|] = [|1,1;1,2;1,3;1,4;2,1;2,2;2,3;2,4;3,1;3,2;3,3;3,4|])

let _ = test "coac23bvee" ([|for i in seq {1 .. 3} do
                              if i % 2 = 1 then 
                                for j in seq {1 .. 4} do yield i,j|] = [|1,1;1,2;1,3;1,4;3,1;3,2;3,3;3,4|])


let _ = test "coac23wcwec" ([|for i in seq {1 .. 3} do
                                for j in seq {i .. 3} do yield i,j|] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

let _ = test "coac23cwe" ([|for i in ie1 do
                              for j in seq {i .. 3} do yield i,j|] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

let _ = test "coac23cwe" ([|for i in ie1 do
                              for j in seq {i .. 3} do
                                yield (i,j)
                              done 
                            done |] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

let _ = test "coac23cwe" ([| for i in ie1 do
                               for j in seq {i .. 3} do
                                 yield (i,j)
                               done 
                             done |] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

let _ = test "coac23cwe" ([|for i in ie1 do
                              for j in seq {i .. 3} do
                                if true then 
                                    yield (i,j)
                                else 
                                    yield (i,j)
                              done
                            done |] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

let _ = test "coac23cwe" ([|for i in ie1 do
                              for j in seq {i .. 3} do
                                if true then 
                                    yield (i,j)
                                else 
                                    yield (i,j)
                              done
                            done |] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

let _ = test "coac23cwe" ([|for i in ie1 do
                              for j in seq {i .. 3} do
                                if i = j then 
                                    yield (i+i,j+j)
                                else 
                                    yield (i,j)
                              done
                            done |] = [|2,2;1,2;1,3;4,4;2,3;6,6|])

let _ = test "coac23cwe" ([|for i in ie1 do
                              for j in seq {i .. 3} do
                                if i = j then 
                                    yield (i+i,j+j)
                                else 
                                    yield (i,j)
                              done
                            done |] = [|2,2;1,2;1,3;4,4;2,3;6,6|])

module TestSet2 = begin
    let _ =  Seq.toList  (seq {for i in seq {1 .. 3} -> 1})

    let _ =  Seq.toList  (seq {for i in seq {1 .. 30} do if i % 2 = 0 then yield i+100})

    let _ =  Seq.toList  (seq {for i in seq {1 .. 3} -> i})

    let _ =  Seq.toList  (seq {for i in seq {1 .. 3} do for j in seq {1 .. 4} -> i,j})

    let _ =  Seq.toList  (seq {for i in seq {1 .. 3} do if i % 2 = 1 then for j in seq {1 .. 4} -> i,j})

    let _ =  Seq.toList  (seq {for i in seq {1 .. 3} do for j in seq {1 .. i} do yield i,j})

    let _ =  Seq.toList  (seq {for i,j in (Seq.ofList [(1,2);(3,4)]) -> i+j })

    let _ =  Seq.toList  (seq {for i,j in [(1,2);(3,4)] -> i+j})

    let _ =  Seq.toList  (seq  {for i in [Some "a"; None; Some "b"] -> i})

    let _ =  Seq.toList  (seq  {for opt in [Some "a"; None; Some "b"] do
                                     match opt with 
                                     | Some r -> yield r
                                     | None -> ()
                                   done })

    let _ =  Seq.toList  (seq  {for opt in [Some "a"; None; Some "b"] do
                                     match opt with 
                                     | Some r -> yield r
                                     | None -> ()
                                   done })

    let _ =  Seq.toList  (seq
                            { while false do
                                     let opt = Some "a" in
                                     match opt with 
                                     | Some r -> yield r
                                     | None -> ()
                              done })


    let _ =  Seq.toList  (seq {for r in [Some "a"; None; Some "b"] -> r})

    let _ =  Seq.toList  (seq {for i,j in [(1,2);(3,4)] do if i % 3 = 0 then yield i+j})

    let _ =  Seq.toList  (seq {for i,j in [| (1,2);(3,4) |] -> i+j})


    let _ =  Seq.toList  (seq {for i in ie1 do for j in seq {1 .. i} -> i,j})
end


open System.Text.RegularExpressions
let ie2 = Regex.Matches("all the pretty horses","[a-z][a-z]") 
let _ = test "coic23" 
           (Seq.toList  (seq {for i in ie2 -> i.Value, i.Length }) = 
              [("al", 2); ("th", 2); ("pr", 2); ("et", 2); ("ty", 2); ("ho", 2); ("rs", 2); ("es", 2)])



module IEnumeratorInterfaceForLoopTest = begin

    open System.Collections.Generic
    type DisposableEnumeratorInterface = 
      class
        new (counter) = { counter = counter } 
        val counter : int ref 
        interface IEnumerator<string> with 
            member x.Current  = "DisposableEnumeratorInterface: Current"
        end
        interface System.Collections.IEnumerator with 
            member x.MoveNext() = false
            member x.Current = box "whoa"
            member x.Reset() = ()
        end
        member x.Current  = "DisposableEnumeratorInterface: Current"
        interface System.IDisposable with 
          member x.Dispose() = incr x.counter 
        end 
      end
      
    type DisposableCollectionInterface = 
      class
        new (counter) = { counter = counter } 
        val counter : int ref 
        interface IEnumerable<string> with 
          member x.GetEnumerator() =  (new DisposableEnumeratorInterface(x.counter)  :> IEnumerator<string>)
        end 
        interface System.Collections.IEnumerable with 
          member x.GetEnumerator() =  (new DisposableEnumeratorInterface(x.counter) :> System.Collections.IEnumerator)
        end 
      end


    let _ = 
        let counter = ref 0 in 
        for i in (new DisposableCollectionInterface(counter) :> IEnumerable<string>) do
          failwith "No, this should not get executed, but IDisposable cleanup should occur"
        done;
        test "1f3oiver-90a" (!counter = 1)

    let _ = 
        let counter = ref 0 in 
        let ie = seq { for i in (new DisposableCollectionInterface(counter) :> IEnumerable<string>) -> failwith "No, this should not get executed, but IDisposable cleanup should occur" } in
        ie |> Seq.iter (fun _ -> failwith "this should be empty");
        test "1f3oiver-90b" (!counter = 1)

    let _ = 
        let counter = ref 0 in 
        for i in (new DisposableCollectionInterface(counter)) do
          failwith "No, this should not get executed, but IDisposable cleanup should occur"
        done;
        test "1f3oiver-90a" (!counter = 1)

    let _ = 
        let counter = ref 0 in 
        let ie = seq { for i in (new DisposableCollectionInterface(counter)) -> failwith "No, this should not get executed, but IDisposable cleanup should occur" } in
        ie |> Seq.iter (fun _ -> failwith "this should be empty");
        test "1f3oiver-90b" (!counter = 1)

    let _ = 
        let counter = ref 0 in 
        let _ = (new DisposableCollectionInterface(counter)) |> Seq.iter (fun i -> failwith "No, this should not get executed, but IDisposable cleanup should occur") in
        test "1f3oiver-90c" (!counter = 1)

end

module IEnumeratorPatternForLoopTest = begin

    open System.Collections.Generic
    type DisposableEnumeratorPattern = 
      class
        new (counter) = { counter = counter } 
        val counter : int ref 
        member x.MoveNext() = false
        member x.Current  = "DisposableEnumeratorPattern: Current"
        interface System.IDisposable with 
          member x.Dispose() = incr x.counter 
        end 
      end
      
    type DisposableCollectionPattern = 
      class
        new (counter) = { counter = counter } 
        val counter : int ref 
        member x.GetEnumerator() = new DisposableEnumeratorPattern(x.counter)
      end

    let _ = 
        let counter = ref 0 in 
        for i in (new DisposableCollectionPattern(counter)) do
          failwith "No, this should not get executed, but IDisposable cleanup should occur"
        done;
        test "1f3oiver-90a" (!counter = 1)

    let _ = 
        let counter = ref 0 in 
        let ie = seq { for i in (new DisposableCollectionPattern(counter)) -> failwith "No, this should not get executed, but IDisposable cleanup should occur" } in
        ie.GetEnumerator().Dispose()
        test "1f3oiver-90b-dispose" (!counter = 0)
        ie |> Seq.iter (fun _ -> failwith "this should be empty");
        test "1f3oiver-90b" (!counter = 1)

end

module SequenceComputationExpressions = begin
  type SequenceBuilder() =
      class
            member b.Zero() = Seq.empty
            member b.ReturnFrom(x) = x
            member b.Return(x) = Seq.singleton x
            member b.Combine(x,y) = Seq.append x y
            member b.Compose(p1,rest) = Seq.collect rest p1 
            member b.Using(rf,rest) = Microsoft.FSharp.Core.CompilerServices.RuntimeHelpers.EnumerateUsing rf rest
      end
  
end

module MaxIntMinIntBOundaryCases = begin
    open System
    let check s x y = test s (x=y)

    do check "r3111" [ Int64.MinValue .. Int64.MinValue] [Int64.MinValue]
    do check "r3112" [ Int64.MaxValue .. Int64.MaxValue] [Int64.MaxValue]
    do check "r3113" [ Int64.MaxValue - 1L .. Int64.MaxValue] [Int64.MaxValue-1L; Int64.MaxValue]
    do check "r3114" [ Int64.MinValue.. Int64.MinValue+1L] [Int64.MinValue; Int64.MinValue+1L]
    do check "r3115" [ Int64.MaxValue .. Int64.MaxValue-1L] []
    do check "r3116" [ Int64.MinValue+1L.. Int64.MinValue] []

    do check "r3117" [ Int32.MinValue .. Int32.MinValue] [Int32.MinValue]
    do check "r3118" [ Int32.MaxValue .. Int32.MaxValue] [Int32.MaxValue]
    do check "r3119" [ Int32.MaxValue - 1 .. Int32.MaxValue] [Int32.MaxValue-1; Int32.MaxValue]
    do check "r3110" [ Int32.MinValue.. Int32.MinValue+1] [Int32.MinValue; Int32.MinValue+1]
    do check "r311-" [ Int32.MaxValue .. Int32.MaxValue-1] []
    do check "r311a" [ Int32.MinValue+1.. Int32.MinValue] []


    do check "r311q" [ Int16.MinValue .. Int16.MinValue] [Int16.MinValue]
    do check "r311w" [ Int16.MaxValue .. Int16.MaxValue] [Int16.MaxValue]
    do check "r311e" [ Int16.MaxValue - 1s .. Int16.MaxValue] [Int16.MaxValue-1s; Int16.MaxValue]
    do check "r311r" [ Int16.MinValue .. Int16.MinValue+1s] [Int16.MinValue; Int16.MinValue+1s]
    do check "r311t" [ Int16.MaxValue .. Int16.MaxValue-1s] []
    do check "r311y" [ Int16.MinValue+1s .. Int16.MinValue] []


    do check "r311u" [ SByte.MinValue .. SByte.MinValue] [SByte.MinValue]
    do check "r311i" [ SByte.MaxValue .. SByte.MaxValue] [SByte.MaxValue]
    do check "r311o" [ SByte.MaxValue - 1y .. SByte.MaxValue] [SByte.MaxValue-1y; SByte.MaxValue]
    do check "r311p" [ SByte.MinValue .. SByte.MinValue+1y] [SByte.MinValue; SByte.MinValue+1y]
    do check "r311s" [ SByte.MaxValue .. SByte.MaxValue-1y] []
    do check "r311d" [ SByte.MinValue+1y .. SByte.MinValue] []

    do check "r311f" [ UInt64.MinValue .. UInt64.MinValue] [UInt64.MinValue]
    do check "r311g" [ UInt64.MaxValue .. UInt64.MaxValue] [UInt64.MaxValue]
    do check "r311h" [ UInt64.MaxValue - 1UL .. UInt64.MaxValue] [UInt64.MaxValue-1UL; UInt64.MaxValue]
    do check "r311j" [ UInt64.MinValue.. UInt64.MinValue+1UL] [UInt64.MinValue; UInt64.MinValue+1UL]
    do check "r311k" [ UInt64.MaxValue .. UInt64.MaxValue-1UL] []
    do check "r311l" [ UInt64.MinValue+1UL.. UInt64.MinValue] []

    do check "r311z" [ UInt32.MinValue .. UInt32.MinValue] [UInt32.MinValue]
    do check "r311x" [ UInt32.MaxValue .. UInt32.MaxValue] [UInt32.MaxValue]
    do check "r311c" [ UInt32.MaxValue - 1u .. UInt32.MaxValue] [UInt32.MaxValue-1u; UInt32.MaxValue]
    do check "r311v" [ UInt32.MinValue.. UInt32.MinValue+1u] [UInt32.MinValue; UInt32.MinValue+1u]
    do check "r311b" [ UInt32.MaxValue .. UInt32.MaxValue-1u] []
    do check "r311n" [ UInt32.MinValue+1u.. UInt32.MinValue] []


    do check "r311m" [ UInt16.MinValue .. UInt16.MinValue] [UInt16.MinValue]
    do check "r311Q" [ UInt16.MaxValue .. UInt16.MaxValue] [UInt16.MaxValue]
    do check "r311W" [ UInt16.MaxValue - 1us .. UInt16.MaxValue] [UInt16.MaxValue-1us; UInt16.MaxValue]
    do check "r311E" [ UInt16.MinValue .. UInt16.MinValue+1us] [UInt16.MinValue; UInt16.MinValue+1us]
    do check "r311R" [ UInt16.MaxValue .. UInt16.MaxValue-1us] []
    do check "r311T" [ UInt16.MinValue+1us .. UInt16.MinValue] []


    do check "r311A" [ Byte.MinValue .. Byte.MinValue] [Byte.MinValue]
    do check "r311S" [ Byte.MaxValue .. Byte.MaxValue] [Byte.MaxValue]
    do check "r311D" [ Byte.MaxValue - 1uy .. Byte.MaxValue] [Byte.MaxValue-1uy; Byte.MaxValue]
    do check "r311F" [ Byte.MinValue .. Byte.MinValue+1uy] [Byte.MinValue; Byte.MinValue+1uy]
    do check "r311G" [ Byte.MaxValue .. Byte.MaxValue-1uy] []
    do check "r311Z" [ Byte.MinValue+1uy .. Byte.MinValue] []

    let checkr s x y = test s (x= List.rev y)

    do checkr "r3121" [ Int64.MinValue .. -1L .. Int64.MinValue] [Int64.MinValue]
    do checkr "r3122" [ Int64.MaxValue .. -1L .. Int64.MaxValue] [Int64.MaxValue]
    do checkr "r3123" [ Int64.MaxValue .. -1L .. Int64.MaxValue - 1L] [Int64.MaxValue-1L; Int64.MaxValue]
    do checkr "r3124" [ Int64.MinValue+1L.. -1L .. Int64.MinValue] [Int64.MinValue; Int64.MinValue+1L]
    do checkr "r3125" [ Int64.MaxValue-1L .. -1L .. Int64.MaxValue] []
    do checkr "r3126" [ Int64.MinValue.. -1L .. Int64.MinValue+1L] []

    do checkr "r3127" [ Int32.MinValue .. -1 .. Int32.MinValue] [Int32.MinValue]
    do checkr "r3128" [ Int32.MaxValue .. -1 .. Int32.MaxValue] [Int32.MaxValue]
    do checkr "r3129" [ Int32.MaxValue .. -1 .. Int32.MaxValue-1] [Int32.MaxValue-1; Int32.MaxValue]
    do checkr "r3120" [ Int32.MinValue+1.. -1 .. Int32.MinValue] [Int32.MinValue; Int32.MinValue+1]
    do checkr "r312-" [ Int32.MaxValue-1 .. -1 .. Int32.MaxValue] []
    do checkr "r312a" [ Int32.MinValue.. -1 .. Int32.MinValue+1] []


    do checkr "r312q" [ Int16.MinValue .. -1s .. Int16.MinValue] [Int16.MinValue]
    do checkr "r312w" [ Int16.MaxValue .. -1s .. Int16.MaxValue] [Int16.MaxValue]
    do checkr "r312e" [ Int16.MaxValue  .. -1s .. Int16.MaxValue-1s] [Int16.MaxValue-1s; Int16.MaxValue]
    do checkr "r312r" [ Int16.MinValue+1s .. -1s .. Int16.MinValue] [Int16.MinValue; Int16.MinValue+1s]
    do checkr "r312t" [ Int16.MaxValue-1s .. -1s .. Int16.MaxValue] []
    do checkr "r312y" [ Int16.MinValue .. -1s .. Int16.MinValue+1s] []


    do checkr "r312u" [ SByte.MinValue .. -1y .. SByte.MinValue] [SByte.MinValue]
    do checkr "r312i" [ SByte.MaxValue .. -1y .. SByte.MaxValue] [SByte.MaxValue]
    do checkr "r312o" [ SByte.MaxValue .. -1y .. SByte.MaxValue-1y] [SByte.MaxValue-1y; SByte.MaxValue]
    do checkr "r312p" [ SByte.MinValue+1y .. -1y .. SByte.MinValue] [SByte.MinValue; SByte.MinValue+1y]
    do checkr "r312s" [ SByte.MaxValue-1y .. -1y .. SByte.MaxValue] []
    do checkr "r312d" [ SByte.MinValue .. -1y .. SByte.MinValue+1y] []

end
 
#if !NETCOREAPP
open System.IO
open System.Xml

let pickering() = 
    let files = Directory.GetFiles(@"C:\Program Files\Microsoft Enterprise Library January 2006\") in
    for file in files do
        let fileInfo = new FileInfo(file) in
        fileInfo.Attributes <- FileAttributes.Archive;
        let doc = new XmlDocument() in
        doc.Load(file);
        let nm = new XmlNamespaceManager(doc.NameTable) in
        nm.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");
        // need to pass in the namespace manager as the project files are in a name space
        let nodes = doc.SelectNodes("/msbuild:Project/msbuild:PropertyGroup/msbuild:TreatWarningsAsErrors", nm) in
        for node in nodes do
            node.InnerText <- bool.FalseString
        done;
        doc.Save(file);
    done;
    stdin.ReadLine()
#endif
 
(* Specification and discussion.

   Given the comprehension [x0 .. dx .. x1].

   For the purpose of specification, consider x0,dx,x1 as real numbers (in order to specify with exact arithmetic).
   Degenerate cases:
     When x0=x1, return [x0].
     When dx=0,  currently, it fails.
                 It could return either [] or [x0;x0;x0;...].

   Remaining cases:

   a) When interval and step are both increasing (have x0<x1 and dx>0)
      it's intended that the sequence should be:

        {zi while zi < x1} where zi = x0 + i.dx

   b) When interval increases but step decrease:
      Here are some examples:
        [0.0 .. -1.0 .. 20.0]
        [0.0 .. -1.0 .. infinity]
        [0   .. -1   .. System.Int32.MaxValue]

      There are two choices:
      C1) Return an empty sequence.
      C2) Return a sequence heading in the wrong direction until running out of values/precision.

      If the later sequences are required, they can be written directly, e.g.
        [0.0 .. -1.0 .. -infinity]
        [0   .. -1   .. min_int]
        
      Choosing C1 (which is the intended current behaviour).

   c) When interval and step are both decreasing, then dual of a).
   d) When internal decrease and step increase, then dual of b).

   Turning this into a brief specification for {x0 .. dx .. x1} on reals.
   
     If x0=x1 then [x0]
     If dx=0 then fail.
     If x0<x1. If dx>0 then {x0 + i.dx while <= x1}. If dx<0 then {}.
     If x0>x1. If dx<0 then {x0 + i.dx while >= x1}. If dx>0 then {}.
     
   However, the types are not real, rather int or float types.   
   The range sequence should agree with the real sequence under it's natural embedding into reals (+).   
   (+) So, UInt32.MaxValue is considered +ve, not -1.

   Rounding error optimization:
     With floating arithmetic (single/double),
     when (x1-x1)/dx is clearly integral (and fits inside int32 say),
     then can generate from the underlying int range via {x0 + i.dx}.
     This should ensure that the endpoint is generated (in this case, assuming no multiplication error).
   -----------------------------------------------------------------------

   Problems:
   * Unsigned types do not support negative dx. (reported by Mort).
   
   -----------------------------------------------------------------------
   Implementation issues:
   * Integer arithmetic can wrap, so termination conditions need care, e.g.   
     When x0<x1 and dx>0 then all of the following are possible:
             x0 + dx < x0  -- advanced beyond range, and wrapped.
             x0 + dx = x0  -- no change, dx=0
        x0 < x0 + dx < x1  -- advanced, still in range.
             x0 + dx = x1  -- advanced, hit end point
        x1 < x0 + dx       -- advanced beyond range.
        
   * Floating arithmetic can lose precision.
   * Floating arithmetic can break to Infinity, (e.g. System.Double.MaxValue + System.Double.MaxValue).
   * Floating values can include, +/- infinity and NaN. These could be x0,dx or x1.
   
 @"" <-- keeps emacs mode happy, please do not delete!
*)

printf "Regressions: 1017\n"

let fails f = try f() |> ignore; false with e -> true
let check str x = if x then printf "OK: %s\n" str  else (printf "FAILED: %s\n" str;  report_failure (str))

let rec steps x dx n = if n=0 then [] else x :: steps (x+dx) dx (n-1)

(* Regression tests: integer ranges *)
[ 10 .. 20]         = steps 10 1 11     |> check    "10..20"
[ 10 .. 0 ]         = []                |> check    "10..0"
[ 10 .. 10 ]        = [10]              |> check    "10..10"
fails (fun () -> [ 10 .. 0 .. 10])      |> check    "10..0..10"

(* Bug: wrapped arithmetic unsigned *)
[ 10u .. 4294967294u .. 0u  ] = []      |> check    "overflow uint contrary"
[ 10u .. 4294967294u .. 20u ] = [10u]   |> check    "overflow uint //"

(* Bug: wrapped arithmetic signed *)
[ 10 .. 2147483647 .. 21 ] = [10]       |> check    "overflow int contrary"
[ 10 .. 2147483647 .. 0  ] = []         |> check    "overflow int //"

(* Bug: wrapped arithmetic signed, valid inputs, x0<x1 and dx>0, but termination is incorrect *)
[ System.Int32.MaxValue-3 .. 2 .. System.Int32.MaxValue ] = [System.Int32.MaxValue-3;System.Int32.MaxValue-1]   |> check "overflow miss endpoint"

(* Bug: floating point inconsistencies *)
(* Bug: sequence not invariant under scaling *)
(* Bug: sequence include/exclude end point when known "exact number of steps" depending on rounding error *)
[| 0.0 .. 0.1      .. 10.0 |].Length = 101              |> check    "float exact range 101"
[| 0.0 .. 0.01     .. 1.0  |].Length = 101              |> check    "float exact range 101 v2"
[| 0.0 .. 0.001    .. 1.0  |].Length = 1001             |> check    "float exact range 1001"
[| 0.0 .. 0.0001   .. 1.0  |].Length = 10001            |> check    "float exact range 10001"
[| 0.0 .. 0.00001  .. 1.0  |].Length = 100001           |> check    "float exact range 100001"

(* Float degenerate cases *)
[ -infinity .. 1.1 .. infinity ] = [-infinity]          |> check    "-inf .. 1.1 .. inf"
[ -infinity .. infinity .. 2.0]  = [-infinity]          |> check    "-inf .. inf .. _"


fails (fun () -> [nan .. 1.1 .. 2.2])                   |> check    "nan .. x .. y"
fails (fun () -> [0.0 .. nan .. 2.2])                   |> check    "x .. nan .. y"
fails (fun () -> [0.0 .. 1.1 .. nan])                   |> check    "x .. y .. nan"


module M = 
    do printfn "hello"

module M2 = begin 
    System.DateTime.Now
end


module MoreSequenceSyntaxTests = 
    let x0i1 = seq { yield 1
                     yield 2 }  
    let x0i2 = seq { yield 1;
                     yield 2 }  
    let x0i3 = seq { yield 1
                     yield 2; }  
    let x0i4 = seq { yield 1;
                     yield 2; }  

    // Note this is currently accepted but may be restricted at a later date.
    let x0i5 = seq { yield 1;
                       yield 2; }  

    // Note this is currently accepted but may be restricted at a later date.
    let x0i6 = seq { yield 1;
                    yield 2; }  

    // Note this is currently accepted but may be restricted at a later date.
    let x0i7 = seq { yield 1;
                  yield 2; }  

    // Note this is currently accepted but may be restricted at a later date.
    let x0i8 = seq { yield 1;
               yield 2; }  

    let y0i1 =     [ yield 1
                     yield 2 ]  
    let y0i2 =     [ yield 1;
                     yield 2 ]  
    let y0i3 =     [ yield 1
                     yield 2; ]  
    let y0i4 =     [ yield 1;
                     yield 2; ]  

    // Note this is currently accepted but may be restricted at a later date.
    let y0i5 =     [ yield 1;
                       yield 2; ]  

    // Note this is currently accepted but may be restricted at a later date.
    let y0i6 =     [ yield 1;
                    yield 2; ]  

    // Note this is currently accepted but may be restricted at a later date.
    let y0i7 =     [ yield 1;
                  yield 2; ]  

    // Note this is currently accepted but may be restricted at a later date.
    let y0i8 =     [ yield 1;
               yield 2; ]  

                
    module SeqTests = 

        let x0a0 : seq<int> = seq { do printfn "ello hello" }  
        let x0a0a = do ()
        let x0a0b = do ()

        let counter = ref 0

        let doTest id (e:System.Collections.Generic.IEnumerable<_>) =
            counter := 0
            e.GetEnumerator().Dispose()
            test id (!counter  = 0)
            let en = e.GetEnumerator()
            en.MoveNext() |> ignore
            en.Dispose()
            test id (!counter = 1)

        let x0a1 =
            seq { use e = { new System.IDisposable with member x.Dispose() = counter := !counter + 1 } 
                  for i in 0 .. 3 do
                      if true  then
                          do printfn "hello"
                      yield i*2 }
        do doTest "x0a1" x0a1
        let x0a2 =
            seq { use e = { new System.IDisposable with member x.Dispose() = counter := !counter + 1 } 
                  for i in 0 .. 3 do
                      if true  then
                          printfn "hello"
                      yield i*2 }
        do doTest "x0a2" x0a2
        let x0a3 =
            seq { use e = { new System.IDisposable with member x.Dispose() = counter := !counter + 1 } 
                  for i in 0 .. 3 do
                      if true  then
                          printfn "hello"
                          printfn "hello"
                          printfn "hello"
                          yield i*2 
                      else 
                          yield i*2
                          printfn "hello"
                          do printfn "hello"
                          printfn "hello"
                      yield i*2 
                      yield i*2 }
        do doTest "x0a1" x0a3
        let x0a = seq { yield 1 }  
        let x0b = seq { for x in 1..2 -> 3 }  
        let x0c = seq { for x in 1..2 do yield 3 }  
        let x0c2 = seq { for x = 1 to 3 do yield 3 }  
        let x0d = seq { while false do yield 3 }  
        let x0e = seq { let x = 1
                        yield 1 }  
        let x0f = seq { let f x = x + 1 
                        yield 1 }  
        let x0g = seq { let rec f x = f x + 1 
                        yield 1 }  
        let x0h = seq { do printfn "hello"
                        yield 1 }  
        let x0i = seq { printfn "hello"
                        yield 1 }  
        let x0i1 = seq { yield 1
                         yield 2 }  
        let x0i2 = seq { printfn "hello"
                         yield 1
                         yield 2 }  
        let x0i3 = seq { yield 1
                         printfn "hello"
                         yield 2 }  
        let x0i4 = seq { yield 1
                         printfn "hello" }  
        let x0j = seq { use x = { new System.IDisposable with 
                                      member x.Dispose() = counter := !counter + 1 } 
                        yield 1 } 
        do doTest "x0j" x0j
        let x0k = seq { try 
                            yield 1
                        finally 
                            printfn "hello" }  

        let x0l = [ yield 1
                    if true then 
                        yield 1 ] 

        let evens1 n =
            seq { for x in 1 .. n do if x % 2 = 0 then yield x }

        let evens2 n =
            seq { for x in 1 .. n do
                   if x % 2 = 0 then yield x }

        let evens3 n =
            seq { for x in 1 .. n do
                   if x % 2 = 0  then 
                     for y in 1 .. n do
                       if y % 2 = 0 then yield (x,y) }

    module AsyncTests = 

        let x0a = async { return 1 }  
        let x0c = async { for x in 1..2 do return () }  
        let x0d = async { while false do return () }  
        let x0e = async { let x = 1
                          return 1 }  
        let x0f = async { let f x = x + 1
                          return 1 }  
        let x0g = async { let rec f x = f x + 1
                          return 1 }
        let x0h = async { do printfn "hello"
                          return 1 } 
        let x0i = async { printfn "hello"
                          return 1 } 
        let x0i2 = async { printfn "hello"
                           return () } 
        let x0j = async { use x = { new System.IDisposable with 
                                        member x.Dispose() = () } 
                          return 1 } 
        let x0k = async { try 
                            return 1
                          finally 
                            printfn "hello" }  
        let x0l = async { try 
                            return 1
                          with _ ->  
                            return 2 }  
        let x0m = async { printfn "hello" }

#if TESTS_AS_APP
#else
        let f103 () = 
            async { do! Async.SwitchToNewThread()
                    do! Async.SwitchToNewThread() }
#endif


    module AmbiguityTests1 = 
        type r = { a : int; b : int }
        let r1 = { a = 1; b = 2 } 
        let r2 = { r1 with a = 3 } 

    module AmbiguityTests2 = 
        let x1 = [ ] 
        let x2 = [ 1;2;3 ] 

    module ExpressionTests = 

        module M = 
           do printfn "hello" 
           let x = 1
           
        let f101 () = 
            do printfn "hello" 

        let f102 () = 
            do printfn "hello" 
            do printfn "hello" 
            1+1

        let f104 () = 
            if 1>2 then 
                do printfn "hello"
            else
                do printfn "world"


module SyncMonad = 
    type Sync<'a> = (unit -> 'a)
    type SyncBuilder() = 
        member b.Bind(x,f) = f (x())
        member b.Using(x,f) = (fun () -> use r = x in f r ())
        member b.TryFinally(x,f) = (fun () -> try x() finally f())
        member b.TryWith(x,f) = (fun () -> try x() with e -> f e ())
        member b.Combine(f1,g) = (fun () -> f1(); g()())
        member b.Delay(f) = (fun () -> f()())
        member b.Zero() = (fun () -> ())
        member b.Return x = (fun () -> x)
        member b.ReturnFrom x = x
        member b.For(e,f) = (fun () -> for x in e do f x ())
        member b.While(gd,x) = (fun () -> while gd() do x())

    let sync = SyncBuilder()


    module SyncTests = 

        let x0a : Sync<int> = sync { return 1 }  
        let x0c : Sync<unit>  = sync { for x in 1..2 do return () }  
        let x0d  : Sync<unit> = sync { while false do return () }  
        let x0e  : Sync<int> = 
            sync { let x = 1
                   return 1 }  
        let x0f  : Sync<int> = 
            sync { let f x = x + 1
                   return 1 }  
        let x0g : Sync<int> = 
            sync { let rec f x = f x + 1
                   return 1 }
        let x0h  : Sync<int> = 
            sync { do printfn "hello"
                   return 1 } 
        let x0i  : Sync<int> = 
            sync { printfn "hello"
                   return 1 } 
        let x0i2  : Sync<unit> = 
            sync { printfn "hello"
                   return () } 
        let x0i2b  : Sync<unit> = 
            sync { if true then 
                      return () } 
        let x0j  : Sync<int> = 
            sync { use x = { new System.IDisposable with 
                                 member x.Dispose() = () } 
                   return 1 } 
        let x0k  : Sync<int> =
            sync { try 
                      return 1
                   finally 
                      printfn "hello" }  
        let x0l : Sync<int> = 
            sync { try 
                       return 1
                   with _ ->  
                       return 2 }  
        let x0m : Sync<unit> = 
           sync { printfn "hello" }


#if TESTS_AS_APP
#else
    type ThreadBuilder () = 
        inherit SyncBuilder()
        member x.Run(f) = async { do! Async.SwitchToNewThread()
                                  return f() } |> Async.RunSynchronously
            
    let thread = new ThreadBuilder()
    
    module ThreadTests = 

        let x0a : int = thread { return 1 }  
        let x0c : unit  = thread { for x in 1..2 do return () }  
        let x0d  : unit = thread { while false do return () }  
        let x0e  : int = 
            thread { let x = 1
                     return 1 }  
        let x0f  : int = 
            thread { let f x = x + 1
                     return 1 }  
        let x0g : int = 
            thread { let rec f x = f x + 1
                     return 1 }
        let x0h  : int = 
            thread { do printfn "hello"
                     return 1 } 
        let x0i  : int = 
            thread { printfn "hello"
                     return 1 } 
        let x0i2  : unit = 
            thread { printfn "hello"
                     return () } 
        let x0i2b  : unit = 
            thread { if true then 
                      return () } 
        let x0j  : int = 
            thread { use x = { new System.IDisposable with 
                                 member x.Dispose() = () } 
                     return 1 } 
        let x0k  : int =
            thread { try 
                      return 1
                     finally 
                      printfn "hello" }  
        let x0l : int = 
            thread { try 
                       return 1
                     with _ ->  
                       return 2 }  
        let x0m : unit = 
           thread { printfn "hello" }
#endif
   
module ContMonad = 
    type Cont<'a> = (('a -> unit) * (exn -> unit) -> unit)
    type ContBuilder() = 
        member b.Bind(x:Cont<'a1>,f: 'a -> Cont<'b>) : Cont<'b> = 
            (fun (cont,econt) -> x ((fun xv -> f xv (cont,econt)), (fun exn -> econt exn)))
        member b.Using(r: ('r :> System.IDisposable),f : ('r -> Cont<'b>)) : Cont<'b> = 
            (fun (cont,econt) -> f r ((fun v -> r.Dispose(); cont v),(fun exn -> r.Dispose(); econt exn)))
        member b.TryFinally(x:Cont<'a2>,f: (unit -> unit)) = 
            (fun (cont,econt) -> x ((fun xv -> f(); cont xv),(fun exn -> f(); econt exn)))
        member b.TryWith(x:Cont<'a3>,f: (exn -> Cont<'a3>)) : Cont<'a3> = 
            (fun (cont,econt) -> x (cont,(fun exn -> f exn (cont,econt))))
        member b.Combine(x:Cont<unit>,g:Cont<'a4>) : Cont<'a4> = 
            (fun (cont,econt) -> x ((fun () -> g (cont,econt)),econt))
        member b.Delay(f:unit->Cont<'a5>) : Cont<'a5> = 
            (fun (cont,econt) -> f () (cont,econt))
        member b.Zero()  : Cont<unit> = 
            (fun (cont,econt) -> cont ())
        member b.Yield(cell: Cont<unit> ref) : Cont<unit> = 
            (fun (cont,econt) -> cell := (fun (_,_) -> cont()))
        member b.Return x = 
            (fun (cont,econt) -> cont x)
        //member b.For(e,f) = 
        //    (fun (cont,econt) -> cont x)        
        //member b.While(gd,x) = (fun () -> while gd() do x())

    let cont = ContBuilder()


    module ContTests = 

        let x0a : Cont<int> = cont { return 1 }  
        let x0e  : Cont<int> = 
            cont { let x = 1
                   return 1 }  
        let x0f  : Cont<int> = 
            cont { let f x = x + 1
                   return 1 }  
        let x0g : Cont<int> = 
            cont { let rec f x = f x + 1
                   return 1 }
        let x0h  : Cont<int> = 
            cont { do printfn "hello"
                   return 1 } 
        let x0i  : Cont<int> = 
            cont { printfn "hello"
                   return 1 } 
        let x0i2  : Cont<unit> = 
            cont { printfn "hello"
                   return () } 
        let x0i2b  : Cont<unit> = 
            cont { if true then 
                      return () } 
        let x0j  : Cont<int> = 
            cont { use x = { new System.IDisposable with 
                                 member x.Dispose() = () } 
                   return 1 } 
        let x0k  : Cont<int> =
            cont { try 
                      return 1
                   finally 
                      printfn "hello" }  
        let x0l : Cont<int> = 
            cont { try 
                       return 1
                   with _ ->  
                       return 2 }  
        let x0m : Cont<unit> = 
           cont { printfn "hello" }


    module ContDisposalTest0 = 

        let finished = ref false
        let disposed = ref false
        let failure = ref false
        let ccont = ref (cont { return () })
        ccont := cont { use r = { new System.IDisposable with member x.Dispose() = disposed := true } 
                        yield ccont 
                        yield ccont }        

        let move() = 
            !ccont ((fun v -> finished := true),(fun exn -> failure := true))
        test "vojwe9u0rw1" (move(); not !finished)
        test "vojwe9u0rw2" (not !disposed)
        test "vojwe9u0rw1" (move(); not !finished)
        test "vojwe9u0rw2" (not !disposed)
        test "vojwe9u0rw1" (move(); !finished)
        test "vojwe9u0rw2" (!disposed)


    module ContDisposalTest1 = 

        let finished = ref false
        let disposed = ref false
        let failure = ref false
        let ccont = ref (cont { return () })
        ccont := cont { use r = { new System.IDisposable with member x.Dispose() = disposed := true } 
                        if 6 > 0 then 
                          yield ccont
                        else 
                          yield ccont
                        if 6 < 0 then 
                          yield ccont
                        else 
                          yield ccont }        


        let move() = 
            !ccont ((fun v -> finished := true),(fun exn -> failure := true))
        test "vojwe9u0rw1" (move(); not !finished)
        test "vojwe9u0rw2" (not !disposed)
        test "vojwe9u0rw1" (move(); not !finished)
        test "vojwe9u0rw2" (not !disposed)
        test "vojwe9u0rw1" (move(); !finished)
        test "vojwe9u0rw2" (!disposed)


    module ContDisposalTest2 = 

        let finished = ref false
        let disposed = ref false
        let failure = ref false
        let ccont = ref (cont { return () })
        ccont := cont { use! r = cont { return ( { new System.IDisposable with member x.Dispose() = disposed := true }) } 
                        if 6 > 0 then 
                          yield ccont
                        else 
                          yield ccont
                        if 6 < 0 then 
                          yield ccont
                        else 
                          yield ccont }        


        let move() = 
            !ccont ((fun v -> finished := true),(fun exn -> failure := true))
        test "vojwe9u0rw1" (move(); not !finished)
        test "vojwe9u0rw2" (not !disposed)
        test "vojwe9u0rw1" (move(); not !finished)
        test "vojwe9u0rw2" (not !disposed)
        test "vojwe9u0rw1" (move(); !finished)
        test "vojwe9u0rw2" (!disposed)


   
module ExistsMonad = 
    type Exists = (unit -> bool)
    type ExistsBuilder() = 
        member b.Bind(x:Exists,f) :Exists = (fun () -> x() || f()())
        member b.Using(x,f) :Exists = (fun  () -> use r = x in f r ())
        member b.TryFinally(x:Exists,f) = (fun () -> try x() finally f())
        member b.TryWith(x:Exists,f) :Exists = (fun () -> try x() with e -> f e ())
        member b.Combine(x:Exists,g) :Exists = (fun () -> x() || g())
        member b.Delay(f:unit->Exists) : Exists = (fun () -> f()())
        member b.Zero() :Exists = (fun () -> false)
        member b.Yield(x:bool) :Exists = (fun () -> x)
        member b.YieldFrom(x:bool list) :Exists = (fun () -> List.exists id x)
        member b.For(e,f: 'a -> Exists) :Exists = (fun () -> Seq.exists (fun x -> f x ()) e)
        member b.While(gd,x:Exists) = (fun () -> let rec loop() = if gd() then x() || loop() else false in loop())
        member b.Run(x:Exists) : bool = x ()

    let exists = ExistsBuilder()

            
    for x in 0..1000 do
                if x % 100 = 0 then printfn "searching x = %d" x
                for y in 0..x do
                   let z = ref 3
                   while !z < 10 do
                      if (x + y + !z = 2009) then 
                          printfn "found it"
                      incr z 

    exists { for x in 0..1000 do
                if x % 100 = 0 then printfn "searching x = %d" x
                for y in 0..x do
                   let z = ref 3
                   while !z < 10 do
                      yield (x + y + !z = 2009)
                      incr z }

                      
    module ExistsTests = 

        let x0a : bool = exists.Run (exists.Delay(fun () -> exists.Yield(true)))
        let x0b : bool = exists { yield true }  
        let x0c : bool = exists { for x in 1..2 do yield true }  
        let x0d  : bool = exists { while false do yield true }  
        let x0e  : bool = 
            exists { let x = 1
                     yield true }  
        let x0f  : bool = 
            exists { let f x = x + 1
                     yield true }  
        let x0f_many  : bool = 
            exists { let f x = x + 1
                     yield! [true;false] }  
        let x0g : bool = 
            exists { let rec f x = f x + 1
                     yield true }
        let x0h  : bool = 
            exists { do printfn "hello"
                     yield true } 
        let x0i  : bool = 
            exists { printfn "hello"
                     yield true } 
        let x0i2  : bool = 
            exists { printfn "hello"
                     yield true } 
        let x0j  : bool = 
            exists { use x = { new System.IDisposable with 
                                 member x.Dispose() = () } 
                     yield true } 
        let x0k  : bool =
            exists { try 
                       yield true
                     finally 
                        printfn "hello" }  
        let x0l : bool = 
            exists { try 
                       yield true
                     with _ ->  
                       yield false }  
        let x0m : bool = 
           exists { printfn "hello" }



module DisposalTest0a = 

    let disposed = ref false
    let x = 
       Microsoft.FSharp.Core.CompilerServices.RuntimeHelpers.EnumerateUsing 
             { new System.IDisposable with member x.Dispose() = disposed := true } 
             (fun r -> [1;2])

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed

module DisposalTest0 = 

    let disposed = ref false
    let x = seq { use r = { new System.IDisposable with member x.Dispose() = disposed := true } 
                  yield! [1;2] }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed


module DisposalTest1 = 

    let disposed = ref false
    let x = seq { use r = { new System.IDisposable with member x.Dispose() = disposed := true } 
                  yield 1
                  yield 2 }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed

module DisposalTest2 = 

    let disposed = ref false
    let x = seq { use r = { new System.IDisposable with member x.Dispose() = disposed := true } 
                  for x in 0 .. 1 do 
                    yield 1 }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed

module DisposalTest3 = 

    let disposed = ref false
    let i = ref 0
    let x = seq { use r = { new System.IDisposable with member x.Dispose() = disposed := true } 
                  while !i < 2 do 
                    yield 1
                    incr i }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed
    
module DisposalTest4 = 

    let disposed = ref false
    let i = ref 0
    let x = seq { use r = { new System.IDisposable with member x.Dispose() = disposed := true } 
                  if !i > 0 then 
                    yield 1
                  else 
                    yield 2 
                  if !i < 0 then 
                    yield 1
                  else 
                    yield 2 }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed

module DisposalTest0b = 

    let disposed = ref false
    let x = seq { use r = { new System.IDisposable with member x.Dispose() = disposed := true } 
                  yield! [1;2] }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed


module DisposalTest1b = 

    let disposed = ref false
    let x = seq { let rec obj = { new System.IDisposable with member x.Dispose() = disposed := true } 
                  use r = obj
                  yield 1
                  yield 2 }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed

module DisposalTest2b = 

    let disposed = ref false
    let x = seq { let obj = { new System.IDisposable with member x.Dispose() = disposed := true } 
                  use r = obj
                  for x in 0 .. 1 do 
                    yield 1 }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed

module DisposalTest3b = 

    let disposed = ref false
    let i = ref 0
    let x = seq { let f() = disposed := true
                  use r = { new System.IDisposable with member x.Dispose() = f() } 
                  while !i < 2 do 
                    yield 1
                    incr i }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed
    
module DisposalTest4b = 

    let disposed = ref false
    let i = ref 0
    let x = seq { let rec f() = disposed := true
                  use r = { new System.IDisposable with member x.Dispose() = f() } 
                  if !i > 0 then 
                    yield 1
                  else 
                    yield 2 
                  if !i < 0 then 
                    yield 1
                  else 
                    yield 2 }        

    let e = x.GetEnumerator()
    test "vojwe9u0rw1" (e.MoveNext())
    test "vojwe9u0rw2" (not !disposed)
    test "vojwe9u0rw3" (e.MoveNext())
    test "vojwe9u0rw4" (not !disposed)
    test "vojwe9u0rw5" (not (e.MoveNext()))
    e.Dispose()
    test "vojwe9u0rw6" !disposed

module EnumPatternWithFunkyTypes_FSharp_1_0_13904 = 
    [<Struct>]
    type Struct =
        val x:int
        val y:int

    type En<'a>(mvNext:'a) =
        member x.Current = 1
        member x.MoveNext() = mvNext

    type T<'a>(mvNext:'a) =
        member x.GetEnumerator() = En mvNext

    // This is allowed - 'a is known to be "bool"
    let s = seq { for i in T true -> i }

module SideEffectListMonad =
    type SideEffectListWithReturnBuilder(onReturn, onZero) =
        member b.Bind(x:unit,f) :list<'b> = f()
        member b.Combine(x:list<'a>,y:list<'a>) :list<'a> = List.append x y
        member b.Delay(f:unit->list<'a>) :list<'a> = f()
        member b.Return _ :list<'a> = onReturn(); []
        member b.Zero() :list<'a> = onZero(); []
        member b.Yield(x:'a) :list<'a> = [x]

    let sideEffectListWithReturn onReturn onZero = SideEffectListWithReturnBuilder(onReturn, onZero)

    type SideEffectListWithZeroBuilder(onZero) =
        member b.Bind(x:unit,f) :list<'b> = f()
        member b.Combine(x:list<'a>,y:list<'a>) :list<'a> = List.append x y
        member b.Delay(f:unit->list<'a>) :list<'a> = f()
        member b.Zero() :list<'a> = onZero(); []
        member b.Yield(x:'a) :list<'a> = [x]

    let sideEffectListWithZero onZero = SideEffectListWithZeroBuilder(onZero)

    module SideEffectListTests =

        let x0a : list<int> * int * int =
            let calledReturn = ref 0
            let onReturn () = calledReturn := !calledReturn + 1
            let calledZero = ref 0
            let onZero () = calledZero := !calledZero + 1
            sideEffectListWithReturn onReturn onZero { yield 1
                                                       do! printfn "hello" }, !calledReturn, !calledZero
        test "x0a" (x0a = ([1], 1, 0))

        let x0b : list<int> * int =
            let calledZero = ref 0
            let onZero () = calledZero := !calledZero + 1
            sideEffectListWithZero onZero { yield 1
                                            do! printfn "hello" }, !calledZero
        test "x0b" (x0b = ([1], 1))


#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

