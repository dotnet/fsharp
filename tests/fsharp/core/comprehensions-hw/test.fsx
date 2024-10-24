// #Conformance #Sequences #Regression #ControlFlow #SyntacticSugar #ComputationExpressions 

//#nowarn "57"


open System.Text.RegularExpressions
open System.IO
open System.Xml

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check s b1 b2 = test s (b1 = b2)


test "coic23a" (Seq.toList { 'a' .. 'c' } = ['a';'b';'c'])

test "coic23q" (Seq.toList {1 .. 0} = [])
test "coic23w" (Seq.toList {1 .. 1} = [1])
test "coic23e" (Seq.toList {1 .. 3} = [1;2;3])
test "coic23r" (Seq.toList {1L .. 3L} = [1L;2L;3L])
test "coic23t" (Seq.toList {1UL .. 3UL} = [1UL;2UL;3UL])
test "coic23y" (Seq.toList {1ul .. 3ul} = [1ul;2ul;3ul])
test "coic23a" (Seq.toList {1y .. 3y} = [1y;2y;3y])
test "coic23s" (Seq.toList {1uy .. 3uy} = [1uy;2uy;3uy])
test "coic23d" (Seq.toList {1s .. 3s} = [1s;2s;3s])
//test "coic23" (Seq.toList {1I .. 3I} = [1I;2I;3I])
//test "coic23" (Seq.toList {1N .. 3N} = [1N;2N;3N])
test "coic23d" (Seq.toList {1us .. 3us} = [1us;2us;3us])
test "coic23f" (Seq.toList {3 .. 1} = [])
test "coic23g" (Seq.toList  (seq {1.0..3.0}) = [1.0;2.0;3.0])
test "coic23h" (Seq.toList  (seq {1.0 .. 1.0 .. 3.0}) = [1.0;2.0;3.0])
test "coic23j" (Seq.toList  (seq {1.0 .. 1.0 .. 2.01 }) = [1.0;2.0])
test "coic23k" (Seq.toList  (seq {3.0 .. -1.0 .. 0.0}) = [3.0;2.0;1.0;0.0])
test "coic23l" (Seq.toList  (seq {4.0 .. -2.0 .. 0.0}) = [4.0;2.0;0.0])
test "coic23z" (Seq.toList  (seq {3 .. -1 .. -3}) = [3;2;1;0; -1; -2; -3])
test "coic23x" (Seq.toList  (seq {3 .. -2 .. -3})= [3;1; -1; -3])

test "coic23c" ([ 'a' .. 'c' ] = ['a';'b';'c'])

test "coic23v" ([ 1 .. 0 ] = [])
test "coic23b" ([ 1 .. 1 ] = [1])
test "coic23n" ([ 1 .. 3 ] = [1;2;3])
test "coic23m" ([ 1L .. 3L ]= [1L;2L;3L])
test "coic2342" ([ 1UL .. 3UL ] = [1UL;2UL;3UL])
test "coic233d2" ([ 1ul .. 3ul ] = [1ul;2ul;3ul])
test "coic23t34" ([ 1y .. 3y ] = [1y;2y;3y])
test "coic23cwe" ([ 1uy .. 3uy ] = [1uy;2uy;3uy])
test "coic23cu" ([ 1s .. 3s ] = [1s;2s;3s])
//test "coic23" ([ 1I .. 3I) = [1I;2I;3I])
//test "coic23" ([ 1N .. 3N) = [1N;2N;3N])
test "coic238n7" ([ 1us .. 3us ] = [1us;2us;3us])
test "coic23we" ([ 3 .. 1 ] = [])
test "coic23v38c5" ([ 1.0..3.0 ] = [1.0;2.0;3.0])
test "coic23v5wq" ([ 1.0 .. 1.0 .. 3.0 ] = [1.0;2.0;3.0])
test "coic23cv42" ([ 1.0 .. 1.0 .. 2.01  ] = [1.0;2.0])
test "coic23cd2" ([ 3.0 .. -1.0 .. 0.0 ] = [3.0;2.0;1.0;0.0])
test "coic23vq423" ([ 4.0 .. -2.0 .. 0.0 ] = [4.0;2.0;0.0])
test "coic23jyi" ([ 3 .. -1 .. -3] = [3;2;1;0; -1; -2; -3])
test "coic23my7" ([ 3 .. -2 .. -3 ]= [3;1; -1; -3])


let test3198 = 
    let mutable count = 0 in 
    for i in {1 .. 3} do 
        count <- count + 1;
        printf "i = %d\n" i 
    done;
    test "fi3n" (count = 3)


let test3198b = 
    let mutable count = 0 in 
    let ie = {1 .. 3} 
    for i in ie do 
        count <- count + 1;
        printf "i = %d\n" i 
    done;
    test "fi3n" (count = 3)

let test3198x = 
    let mutable count = 0 in 
    let mutable ie = {1 .. 3} 
    
    for i in ie do 
        ie <- {1 .. 2}
        count <- count + 1;
        printf "i = %d\n" i 
    done;
    test "fi3n" (count = 3)

let test342879 = 
    let mutable count = 0 in 
    for i in {1 .. 3} do 
        for j in {i .. 3} do 
            count <- count + 1;
            printf "i = %d\n" i 
    test "fi3n" (count = 6)

for i in {1 .. 3} do 
    printf "i = %d\n" i 
done


module RegressionInAppend = begin
    let processFile file fileHandle =
       seq { try
               yield (file := fileHandle; ())
             finally
               // Close the file.
               file := 0 }

    let process2Files file =
       seq { for _ in processFile file 1 do
               yield ()
             for _ in processFile file 2 do
               // On yield, the file should still be "open" with 2
               //   but instead, the finally clause is called
               //   and sets it to 0 before yield.
               yield () }

    let file = ref 0 in
    
    test "ce030932jc2" ([ for _ in process2Files file do yield !file 
                          yield !file ] = [1;2;0])


    let test1 n = 
        let count = ref 0 
        let s = 
            Seq.append 
               (seq { use x = { new System.IDisposable with member self.Dispose() = incr count; System.Console.WriteLine("Dispose1") } in 
                      yield! [1;2] })
               (Seq.append 
                   (seq { use x = { new System.IDisposable with member self.Dispose() = incr count; System.Console.WriteLine("Dispose2") } in 
                          yield! [3;4] })
                   (seq { use x = { new System.IDisposable with member self.Dispose() = incr count; System.Console.WriteLine("Dispose3") } in 
                          yield! [5;6] }));
        let res = s |> Seq.take n |> Seq.toList in
        res, !count
        
     
    test "coc3n09" (test1(0) = ([], 0))
    test "coc3n09" (test1(1) = ([1], 1))
    test "coc3n09" (test1(2) = ([1;2], 1))
    test "coc3n09" (test1(3) = ([1;2;3], 2))
    test "coc3n09" (test1(4) = ([1;2;3;4], 2))
    test "coc3n09" (test1(5) = ([1;2;3;4;5], 3))
    test "coc3n09" (test1(6) = ([1;2;3;4;5;6], 3))

    let test2 n = 
        let count = ref 0 
        let s = 
            Seq.append 
               (Seq.append 
                   (seq { use x = { new System.IDisposable with member self.Dispose() = incr count; System.Console.WriteLine("Dispose1") } in 
                      yield! [1;2] })
                   (seq { use x = { new System.IDisposable with member self.Dispose() = incr count; System.Console.WriteLine("Dispose2") } in 
                          yield! [3;4] }))
               (seq { use x = { new System.IDisposable with member self.Dispose() = incr count; System.Console.WriteLine("Dispose3") } in 
                      yield! [5;6] });
        let res = s |> Seq.take n |> Seq.toList in
        res, !count
        
     
    test "coc3n09" (test2(0) = ([], 0))
    test "coc3n09" (test2(1) = ([1], 1))
    test "coc3n09" (test2(2) = ([1;2], 1))
    test "coc3n09" (test2(3) = ([1;2;3], 2))
    test "coc3n09" (test2(4) = ([1;2;3;4], 2))
    test "coc3n09" (test2(5) = ([1;2;3;4;5], 3))
    test "coc3n09" (test2(6) = ([1;2;3;4;5;6], 3))
end


test "coic23v34w" (Seq.toList  (seq {for i in {1 .. 3} -> i,i*i}) = [1,1;2,4;3,9] )

//{for x,y in [| (1,2) |]  -> x+y)


test "coic23va" (Seq.toList  (seq {for i in {1 .. 5} do
                                       if i % 2 = 0 then yield i+100})
                = [102;104])

test "coic23eq" (Seq.toList  (seq {for i in {1 .. 3} -> i}) = [1;2;3])

test "coic23avwa" (Seq.toList  (seq {for i in {1 .. 3} do
                                         for j in {1 .. 4} -> i,j}) = [1,1;1,2;1,3;1,4;2,1;2,2;2,3;2,4;3,1;3,2;3,3;3,4])

test "coic23bvee" (Seq.toList  (seq {for i in {1 .. 3} do 
                                         if i % 2 = 1 then 
                                           for j in {1 .. 4} -> i,j}) = [1,1;1,2;1,3;1,4;3,1;3,2;3,3;3,4])


test "coic23wcwec" (Seq.toList  (seq {for i in {1 .. 3} do
                                          for j in {i .. 3} -> i,j}) = [1,1;1,2;1,3;2,2;2,3;3,3])

let ie1 = [ 1 .. 1 .. 3 ]

test "colc23cwe" ([ for i in ie1 do
                    for j in {i .. 3} -> i,j ] = [1,1;1,2;1,3;2,2;2,3;3,3])

test "colc23v\w" ([ for i in {1 .. 3} -> i,i*i ] = [1,1;2,4;3,9])

//{for x,y in [| (1,2) |]  -> x+y)


test "colc23va" ([ for i in {1 .. 5} do
                     if i % 2 = 0 then yield i+100 ]
                = [102;104])

test "colc23eq" ( [ for i in {1 .. 3} -> i ] = [1;2;3])

test "colc23avwa" ([ for i in {1 .. 3} do
                     for j in {1 .. 4} -> i,j ] = [1,1;1,2;1,3;1,4;2,1;2,2;2,3;2,4;3,1;3,2;3,3;3,4])

test "colc23bvee" ([ for i in {1 .. 3}  do
                     if i % 2 = 1 then 
                        for j in {1 .. 4} -> i,j ] = [1,1;1,2;1,3;1,4;3,1;3,2;3,3;3,4])


test "colc23wcwec" ([ for i in {1 .. 3} do
                      for j in {i .. 3} -> i,j ] = [1,1;1,2;1,3;2,2;2,3;3,3])

test "colc23cwe" ([ for i in ie1 do
                    for j in {i .. 3} -> i,j ] = [1,1;1,2;1,3;2,2;2,3;3,3])



test "coac23cwe" ([|for i in ie1 do
                    for j in {i .. 3} -> i,j|] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

test "coac23v\w" ([|for i in {1 .. 3} -> i,i*i|] = [|1,1;2,4;3,9|])

//{for x,y in [| (1,2) |]  -> x+y)


test "coac23va" ([|for i in {1 .. 5} do
                    if i % 2 = 0 then yield i+100|]
                = [|102;104|])

test "coac23eq" ( [|for i in {1 .. 3} -> i|] = [|1;2;3|])

test "coac23avwa" ([|for i in {1 .. 3} do
                     for j in {1 .. 4} -> i,j|] = [|1,1;1,2;1,3;1,4;2,1;2,2;2,3;2,4;3,1;3,2;3,3;3,4|])

test "coac23bvee" ([|for i in {1 .. 3} do
                       if i % 2 = 1 then 
                         for j in {1 .. 4} -> i,j|] = [|1,1;1,2;1,3;1,4;3,1;3,2;3,3;3,4|])


test "coac23wcwec" ([|for i in {1 .. 3} do
                      for j in {i .. 3} -> i,j|] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

test "coac23cwe" ([|for i in ie1 do
                    for j in {i .. 3} -> i,j|] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

module TestSet2 = begin
    let _ =  Seq.toList  (seq {for i in {1 .. 3} -> 1})

    let _ =  Seq.toList  (seq {for i in {1 .. 30} do if i % 2 = 0 then yield i+100})

    let _ =  Seq.toList  (seq {for i in {1 .. 3} -> i})

    let _ =  Seq.toList  (seq {for i in {1 .. 3} do for j in {1 .. 4} -> i,j})

    let _ =  Seq.toList  (seq {for i in {1 .. 3} do if i % 2 = 1 then for j in {1 .. 4} -> i,j})

    let _ =  Seq.toList  (seq {for i in {1 .. 3} do for j in {1 .. i} -> i,j})

    let _ =  Seq.toList  (seq {for i,j in (Seq.ofList [(1,2);(3,4)]) -> i+j })

    let _ =  Seq.toList  (seq {for i,j in [(1,2);(3,4)] -> i+j})

    let _ =  Seq.toList  (seq {for i in [Some "a"; None; Some "b"] -> i})

    let _ =  Seq.toList  (seq {for opt in [Some "a"; None; Some "b"] do
                                  let r = opt 
                                  yield r})

    let _ =  Seq.toList  (seq {for opt in [Some "a"; None; Some "b"] do
                                let r = opt 
                                yield r})

    let _ =  Seq.toList  (seq {for r in [Some "a"; None; Some "b"]
                                -> r})

    let _ =  Seq.toList  (seq {for r in [Some "a"; None; Some "b"] do
                                  yield r})

(*
    let _ =  Seq.toList  (seq {for opt in [Some "a"; None; Some "b"]
                                   match opt with 
                                   | Some r -> r})


    let _ =  Seq.toList  (seq {for opt in [Some "a"; None; Some "b"]
                                   match opt with 
                                   | (Some r) ->> { for j in [1;2] -> j+j }
                                   //| None ->> {for j in [1;2] -> j+j}) 
                                   })

*)

(* TODO: see bug in PS - this should work *)
(*    let _ =  Seq.toList  (seq {for opt in [Some "a"; None; Some "b"] match opt with (Some r) -> r) *)

    let _ =  Seq.toList  (seq {for i,j in [(1,2);(3,4)] do if i % 3 = 0 then yield i+j})

    let _ =  Seq.toList  (seq {for i,j in [| (1,2);(3,4) |] -> i+j})

    let ie1 = [ 3 .. 1 .. 5 ]

    let _ =  Seq.toList  (seq {for i in ie1 do for j in {1 .. i} do yield i,j})
end


module SingleLineTestSet = begin
    let test3198 = 
        let mutable count = 0 in         for i in 1 .. 3 do             count <- count + 1;            printf "i = %d\n" i         done;        test "fi3fwfe2a" (count = 3)


    let test3198b = 
        let mutable count = 0 in         let ie = seq {1 .. 3} in       for i in ie do             count <- count + 1;            printf "i = %d\n" i         done;        test "fi3fwfe2b" (count = 3)

    let test3198x = 
        let mutable count = 0 in         let ie = ref (seq {1 .. 3}) in                for i in !ie do             ie := {1 .. 2};             count <- count + 1;            printf "i = %d\n" i;         done;        test "fi3fwfe2c" (count = 3)

    let test342879 = 
        let mutable count = 0 in         
        for i in 1 .. 3 do             for j in i .. 3 do                 count <- count + 1;                printf "i = %d\n" i ;       
        test "fi3fwfe2d" (count = 6)

    let _ = for i in 1 .. 3 do         printf "i = %d\n" i     done


    let _ = test "coic23v\w" (Seq.toList  (seq {for i in 1 .. 3 -> i,i*i}) = [1,1;2,4;3,9])

    //{for x,y in [| (1,2) |]  -> x+y)


    let _ = test "coic23va" (Seq.toList  (seq {for i in 1 .. 5 do if  i % 2 = 0 then yield i+100}) = [102;104])

    let _ = test "coic23eq" (Seq.toList  (seq {for i in {1 .. 3} -> i}) = [1;2;3])

    let _ = test "coic23avwa" (Seq.toList  (seq {for i in {1 .. 3} do for j in {1 .. 4} -> i,j}) = [1,1;1,2;1,3;1,4;2,1;2,2;2,3;2,4;3,1;3,2;3,3;3,4])

    let _ = test "coic23bvee" (Seq.toList  (seq {for i in {1 .. 3} do if i % 2 = 1 then for j in {1 .. 4} -> i,j}) = [1,1;1,2;1,3;1,4;3,1;3,2;3,3;3,4])


    let _ =  test "coic23wcwec" (Seq.toList  (seq {for i in {1 .. 3} do for j in {i .. 3} -> i,j}) = [1,1;1,2;1,3;2,2;2,3;3,3])

    let ie1 = [ 1 .. 1 .. 3]

    let _ = test "colc23cwe" ([ for i in ie1 do for j in {i .. 3} -> i,j ] = [1,1;1,2;1,3;2,2;2,3;3,3])

    let _ =   test "colc23v\w" ([ for i in {1 .. 3} -> i,i*i ] = [1,1;2,4;3,9])

    //{for x,y in [| (1,2) |]  -> x+y)


    let _ =  test "colc23va" ([ for i in {1 .. 5} do if i % 2 = 0 then yield  i+100 ] = [102;104])

    let _ =  test "colc23eq" ( [ for i in {1 .. 3} -> i ] = [1;2;3])

    let _ =  test "colc23avwa" ([ for i in {1 .. 3} do for j in {1 .. 4} -> i,j ] = [1,1;1,2;1,3;1,4;2,1;2,2;2,3;2,4;3,1;3,2;3,3;3,4])

    let _ =  test "colc23bvee" ([ for i in {1 .. 3} do if i % 2 = 1 then for j in {1 .. 4} -> i,j ] = [1,1;1,2;1,3;1,4;3,1;3,2;3,3;3,4])


    let _ = test "colc23wcwec" ([ for i in {1 .. 3} do for j in {i .. 3} -> i,j ] = [1,1;1,2;1,3;2,2;2,3;3,3])

    let _ = test "colc23cwe" ([ for i in ie1 do for j in {i .. 3} -> i,j ] = [1,1;1,2;1,3;2,2;2,3;3,3])



    let _ =  test "coac23cwe" ([|for i in ie1 do for j in {i .. 3} -> i,j|] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

    let _ =  test "coac23v\w" ([|for i in {1 .. 3} -> i,i*i|] = [|1,1;2,4;3,9|])

    //{for x,y in [| (1,2) |]  -> x+y)


    let _ =  test "coac23va" ([|for i in {1 .. 5} do if i % 2 = 0 then yield i+100|] = [|102;104|])

    let _ =  test "coac23eq" ( [|for i in {1 .. 3} -> i|] = [|1;2;3|])

    let _ =  test "coac23avwa" ([|for i in {1 .. 3}  do for j in {1 .. 4} -> i,j|] = [|1,1;1,2;1,3;1,4;2,1;2,2;2,3;2,4;3,1;3,2;3,3;3,4|])

    let _ =  test "coac23bvee" ([|for i in {1 .. 3} do if i % 2 = 1 then for j in {1 .. 4} -> i,j|] = [|1,1;1,2;1,3;1,4;3,1;3,2;3,3;3,4|])


    let _ =  test "coac23wcwec" ([|for i in {1 .. 3} do for j in {i .. 3} -> i,j|] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

    let _ =  test "coac23cwe" ([|for i in ie1 do for j in {i .. 3} -> i,j|] = [|1,1;1,2;1,3;2,2;2,3;3,3|])

    let _ =  Seq.toList  (seq {for i in {1 .. 3} -> 1})

    let _ =  Seq.toList  (seq {for i in 1 .. 30 do if i % 2 = 0 then yield i+100})

    let _ =  Seq.toList  (seq {for i in {1 .. 3} -> i})

    let _ =  Seq.toList  (seq {for i in {1 .. 3} do for j in {1 .. 4} do yield i,j})

    let _ =  Seq.toList  (seq {for i in {1 .. 3} do if i % 2 = 1 then for j in {1 .. 4} -> i,j})

    let _ =  Seq.toList  (seq {for i in {1 .. 3} do for j in 1 .. i -> i,j})

    let _ =  Seq.toList  (seq {for i,j in (Seq.ofList [(1,2);(3,4)]) -> i+j})

    let _ =  Seq.toList  (seq {for i,j in [(1,2);(3,4)] -> i+j})

    let _ =  Seq.toList  (seq {for i in [Some "a"; None; Some "b"] -> i})

    let _ =  Seq.toList  (seq {for opt in [Some "a"; None; Some "b"] do let r = opt in yield r})

    let _ =  Seq.toList  (seq {for opt in [Some "a"; None; Some "b"]  do
                                   let r = opt in 
                                   yield r})

    let _ =  Seq.toList  (seq {for r in [Some "a"; None; Some "b"] -> r})

//    let _ =  Seq.toList  (seq {for opt in [Some "a"; None; Some "b"] match opt with (Some r) -> r}

    let _ =  Seq.toList  (seq {for i,j in [(1,2);(3,4)] do if i % 3 = 0 then yield i+j})


    let _ =  Seq.toList  (seq {for i,j in [| (1,2);(3,4) |] -> i+j})
    let ie2 = [ 3 .. 1 .. 5 ]

    let _ =  Seq.toList  (seq {for i in ie2 do for j in 1 .. i -> i,j})
end

let ie2 = Regex.Matches("all the pretty horses","[a-z][a-z]") 
test "coic23" 
   (Seq.toList  (seq {for i in ie2 -> i.Value, i.Length}) = 
      [("al", 2); ("th", 2); ("pr", 2); ("et", 2); ("ty", 2); ("ho", 2); ("rs", 2); ("es", 2)])



#if !NETCOREAPP
let pickering() = 
    let files = Directory.GetFiles(@"C:\Program Files\Microsoft Enterprise Library January 2006\", "*.csproj", SearchOption.AllDirectories)
    for file in files do
        let fileInfo = new FileInfo(file)
        fileInfo.Attributes <- FileAttributes.Archive 
        let doc = new XmlDocument()
        doc.Load(file)
        let nm = new XmlNamespaceManager(doc.NameTable)
        nm.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003")
        // need to pass in the namespace manager as the project files are in a name space
        let nodes = doc.SelectNodes("/msbuild:Project/msbuild:PropertyGroup/msbuild:TreatWarningsAsErrors", nm) 
        for node in nodes do
            node.InnerText <- bool.FalseString
        doc.Save(file)
    stdin.ReadLine()
 
#endif
for i,j in [(1,1);(2,1);(3,2)] do
   printf "i = %d,j = %d\n" i j

for Some(i,j) in [Some(1,1);None;Some(3,2)] do
   printf "i = %d,j = %d\n" i j
   

let countc23 = ref 0 
for i,j in [(1,1);(2,1);(3,2)] do
   printf "i = %d,j = %d\n" i j;
   incr countc23 

for Some(i,j) in [Some(1,1);None;Some(3,2)] do
   printf "i = %d,j = %d\n" i j
   incr countc23

test "coic23" (!countc23 = 5)

let checkerboardCoordinates n = 
    seq { for row in 1 .. n do
           for col in 1 .. n do
              if (row+col) % 2 = 0 then
                  yield (row,col) }

let fileInfo dir =
    seq { for file in Directory.GetFiles(dir) do
            let creationTime = File.GetCreationTime(file) 
            let lastAccessTime = File.GetLastAccessTime(file) 
            yield (file,creationTime,lastAccessTime) }

let rec allFiles dir =
    seq { for file in Directory.GetFiles(dir) do yield file
          for subdir in Directory.GetDirectories dir do yield! (allFiles subdir) }

module Attempt = 
    type Attempt<'a> = (unit -> 'a option)
    let succeed x = (fun () -> Some(x)) 
    let fail      = (fun () -> None) 
    let apply (a:Attempt<'a>) = a() 
    let bind p rest = (fun () -> match apply p with None -> None | Some r -> apply (rest r))
    let delay f = (fun () -> apply (f ()))

    type AttemptBuilder() =
        member b.Return(x) = succeed x
        member b.ReturnFrom(x) = x
        member b.Bind(p,rest) = bind p rest
        member b.Delay(f) = delay f
        member b.Let(p,rest) = rest p 
    let attempt = new AttemptBuilder()

    let failIfBig n = attempt { if n > 1000 then return! fail else return n }

    let failIfEitherBig (inp1,inp2) = 
            attempt { let! n1 = failIfBig inp1
                      let! n2 = failIfBig inp2 
                      return (n1,n2) }

module RandomWalk = 
    let rnd = new System.Random() 
    let dice p = (rnd.NextDouble() < p )
    let walk initial upP = [| let mutable state = initial 
                              for i in 0 .. 100 do
                                 do if dice upP then state <- (state + 1) else (state <- state - 1)
                                 yield float state |]


module StringExtensionTest = 
    module StringExtensions = 
        type System.String with
            member i.IsPrime = (i = "2") || (i = "3")


    let s = "2"
    open StringExtensions

    printfn "res = %b" s.IsPrime

module RandomSmallIfThenElseTest = 
    let foo() = 
     async 
      { let a = 5
        if true then  
            do ()
        return a }

    let foo2() = 
     async 
      { let a = 5
        if true then  
            do ()
        return a }

#if !NETCOREAPP
module MoreExtensions =

    open Microsoft.FSharp.Control
    open System
    open System.Threading
    open System.Net
    open System.Net.Security
    open System.Net.Cache
    open System.Security.Principal

    type System.Data.SqlClient.SqlCommand with 
        
        member cmd.ExecuteReaderAsync() = 
            Async.FromContinuations (fun (cont,econt,ccont) -> 
                        // Note: The call to EndExecuteReader is not protected in the
                        // exception chain. But it should not fail.
                        cmd.BeginExecuteReader(callback=AsyncCallback(fun x -> match (try Choice1Of2(cmd.EndExecuteReader x) with e -> Choice2Of2(e)) with 
                                                                               | Choice1Of2(res) -> cont res 
                                                                               | Choice2Of2(exn) -> econt exn), 
                                               stateObject=0) 
                        |> ignore)


    type WebRequestParameters(requestUri:Uri,AuthenticationLevel, CachePolicy, ConnectionGroupName, 
                              ContentLength, Credentials, ImpersonationLevel, 
                              Method, PreAuthenticate, Timeout,
                              UseDefaultCredentials) =
        member obj.RequestUri = requestUri
        member obj.Apply(x: System.Net.WebRequest) = 
            (match AuthenticationLevel with None -> () | Some(v:AuthenticationLevel) -> x.AuthenticationLevel <- v)
            (match CachePolicy with None -> () | Some(v:RequestCachePolicy) -> x.CachePolicy <- v)
            (match ConnectionGroupName with None -> () | Some(v) -> x.ConnectionGroupName <- v)
            (match ContentLength with None -> () | Some(v) -> x.ContentLength <- v)
            (match Credentials with None -> () | Some(v:ICredentials) -> x.Credentials <- v)
            (match ImpersonationLevel with None -> () | Some(v:TokenImpersonationLevel) -> x.ImpersonationLevel <- v)
            (match Method with None -> () | Some(v) -> x.Method <- v)
            (match PreAuthenticate with None -> () | Some(v:AuthenticationLevel) -> x.AuthenticationLevel <- v)
            (match Timeout with None -> () | Some(v) -> x.Timeout <- v)
            (match UseDefaultCredentials with None -> () | Some(v) -> x.UseDefaultCredentials <- v)


        
    let trylet f x = (try Choice1Of2 (f x) with exn -> Choice2Of2(exn))
    
    let protect cont econt f x = 
        match trylet f x with 
        | Choice1Of2 v -> cont v
        | Choice2Of2 exn -> econt exn
    
    type System.Net.WebRequest with 
        static member Async(requestUri:Uri,?AuthenticationLevel, ?CachePolicy, ?ConnectionGroupName, 
                            ?ContentLength, ?Credentials, ?ImpersonationLevel, 
                            ?Method, ?PreAuthenticate, ?Timeout,
                            ?UseDefaultCredentials) =
  
           let parameters = WebRequestParameters(requestUri,AuthenticationLevel, CachePolicy, ConnectionGroupName, 
                                                 ContentLength, Credentials, ImpersonationLevel, 
                                                 Method, PreAuthenticate, Timeout,
                                                 UseDefaultCredentials)
                                       
           Async.FromContinuations (fun (cont,econt,ccont) -> 
               let req = System.Net.WebRequest.Create(parameters.RequestUri) 
               parameters.Apply(req);
               req.BeginGetResponse(callback=AsyncCallback(protect cont econt req.EndGetResponse), state=0) |> ignore)

        static member Async(requestUriString:string,?AuthenticationLevel, ?CachePolicy, ?ConnectionGroupName, 
                            ?ContentLength, ?Credentials, ?ImpersonationLevel, 
                            ?Method, ?PreAuthenticate, ?Timeout,
                            ?UseDefaultCredentials) =
  
           let parameters = WebRequestParameters(Uri(requestUriString),AuthenticationLevel, CachePolicy, ConnectionGroupName, 
                                                 ContentLength, Credentials, ImpersonationLevel, 
                                                 Method, PreAuthenticate, Timeout,
                                                 UseDefaultCredentials)
                                       
           Async.FromContinuations (fun (cont,econt,ccont) -> 
               let req = System.Net.WebRequest.Create(parameters.RequestUri) 
               parameters.Apply(req);
               req.BeginGetResponse(callback=AsyncCallback(protect cont econt req.EndGetResponse), state=0) |> ignore)



module SimpleAsyncWebCrawl = 

    open System.Windows.Forms
    open System.Collections.Generic
    open System.Net
    open System.IO
    open MoreExtensions
    open Microsoft.FSharp.Control
    open System.Text.RegularExpressions

    let limit = 50
    let linkPat = "href=\s*\"[^\"h]*(http://[^&\"]*)\""
    let getLinks (txt:string) =  [ for m in Regex.Matches(txt,linkPat)  -> m.Groups.Item(1).Value ]
         
    let (<--) (mp: MailboxProcessor<_>) x = mp.Post(x)




    let webRequestSemaphore = new System.Threading.Semaphore(initialCount=2,maximumCount=2)
    webRequestSemaphore.WaitOne() |> ignore
    
    /// Fetch the URL, and post the results to the collector. 
    let collectLinksAsync (url:string) : Async<string list> =
        async { do printfn "requesting %s" url
                // Use an Async web request.
                let! rsp = 
                    async { do printfn "wait for semaphore for %s..." url
                            let! ok = Async.AwaitWaitHandle webRequestSemaphore
                            do printfn "got semaphore, ok = %A" ok
                            if ok then 
                                try 
                                   do printfn "starting web request for %s" url
                                   let! res = WebRequest.Async(url,Timeout=5) 
                                   do printfn "done web request for %s" url
                                   return res
                                finally 
                                   do printfn "releasing semaphore for %s" url
                                   webRequestSemaphore.Release() |> ignore
                            elif ok then
                                return! failwith "hey, I thought we weren't OK"
                            else 
                                return! failwith "couldn't acquire a semaphore" }
                // Get the response stream and read it
                let! html = 
                    async { use reader = new StreamReader(rsp.GetResponseStream()) 
                            do printfn "reading %s" url
                            // Note: this read should be asynchronous (todo)
                            return reader.ReadToEnd()  }
                // Get the links
                do printfn "finished reading %s, scraping HTML for links" url
                let links = getLinks html
                // We're done
                do printfn "got %d links" (List.length links)
                return links }

    /// 'collector' is a single global agent that receives URLs as messages. It creates processes
    /// that send messages back to itself, hence it needs to refer to itself, hence it is marked 'rec'.
    let collector = 
        new Control.MailboxProcessor<_>(fun inbox ->
            // The states of an agent form a set of mutually recursive functions
            let rec crawl(visited : Set<string>) = 
               async { do printfn "waiting for url ..." 
                       //spawn all the requests
                       let! url = inbox.Receive() 
                       do printfn "got url %s, #visited = %d" url visited.Count
                       // Check the limit
                       if visited.Count < limit then
                           // Which ones are new?
                           if not (visited.Contains(url)) then                  
                               // Spawn off a new asynchronous computation for the new url. Each new job collects
                               // links and sends those links back as messages to the collector.
                               do! Async.StartChild(async { let! links = collectLinksAsync url
                                                                 // Send the links as messages
                                                            for link in links do 
                                                                    inbox <-- link }) |> Async.Ignore
                           // Recurse into the waiting state
                           return! crawl(visited.Add(url)) }

            // the initial state            
            crawl(Set.empty))
                       
    let main() = 
        collector.Start()
        collector <-- "http://news.google.com"
        Async.CancelDefaultToken()
#endif

module TryFinallySequenceExpressionTests = 

    type RunExactlyOncePoint(s) = 
        let mutable count = 0
        member x.Run() = 
            if count > 0 then 
                failwithf "point %s got run more than once" s
            else
                printfn "got to point %s ok" s
             
            count <- count + 1
        interface System.IDisposable with 
            member x.Dispose() =  
                if count = 0 then 
                    failwithf "point %s never got run" s
                else
                    printfn "got to point %s exactly once" s

    type RunNeverPoint(s) = 
        member x.Run() = failwithf "never-run point %s got run " s
        interface System.IDisposable with 
            member x.Dispose() =  
                printfn "never got to point %s - good!" s

    let test3926() = 
       use point1 = new RunExactlyOncePoint("clknnew1")
       use point2 = new RunExactlyOncePoint("clknnew1")
       seq { point1.Run() 
             try 
                 ()
             finally 
                  point2.Run() } |> Seq.iter ignore

    test3926()

    let test3926b() = 
       use point1 = new RunExactlyOncePoint("clknnew1")
       use point2 = new RunExactlyOncePoint("clknnew1")
       seq { point1.Run() 
             try 
                 yield 1
             finally 
                  point2.Run() } |> Seq.iter ignore

    test3926b()

    let test3926c() = 
       use point1 = new RunExactlyOncePoint("clknnew1")
       use point2 = new RunExactlyOncePoint("clknnew1")
       seq { point1.Run() 
             yield 1
             try 
                 yield 1
             finally 
                  point2.Run() } |> Seq.iter ignore

    test3926c()

    let test3926d() = 
       use point1 = new RunExactlyOncePoint("clknnew1")
       use point2 = new RunExactlyOncePoint("clknnew2")
       use point3 = new RunExactlyOncePoint("clknnew3")
       use point4 = new RunExactlyOncePoint("clknnew4")
       use point5 = new RunExactlyOncePoint("clknnew5")
       seq { point1.Run() 
             yield 1
             try 
                 yield 1
                 point2.Run() 
                 try 
                     yield 1
                     point3.Run() 
                 finally 
                      point4.Run() 
             finally 
                  point5.Run() } |> Seq.iter ignore

    test3926d()

    let test3927() = 
       use point1 = new RunExactlyOncePoint("clknnew1")
       use point2 = new RunExactlyOncePoint("clknnew1")
       seq { try 
                 ()
             finally 
                  point1.Run() 
             try 
                 ()
             finally 
                  point2.Run() } |> Seq.iter ignore

    test3927()

    let test3928() = 
       use point1 = new RunExactlyOncePoint("clknnew1")
       use point2 = new RunExactlyOncePoint("clknnew1")
       seq { try 
                 point1.Run()
             finally 
                  point2.Run() } |> Seq.iter ignore

    test3928()

    let test3929() = 
       use point1 = new RunExactlyOncePoint("clknnew1")
       seq { try 
                 ()
             finally 
                  point1.Run() } |> Seq.iter ignore

    test3929()

    let test3930() = 
       use point1 = new RunExactlyOncePoint("clknnew1")
       use point2 = new RunExactlyOncePoint("clknnew2")
       use point3 = new RunExactlyOncePoint("clknnew3")
       use point4 = new RunExactlyOncePoint("clknnew4")
       try 
        seq { point1.Run()
              try 
                 point2.Run()
              finally 
                  point3.Run()
                  failwith "fail" } |> Seq.iter ignore
       with _ -> 
           point4.Run()

    test3930()

    let test3931() = 
       use point1 = new RunExactlyOncePoint("clknnew11")
       use point2 = new RunExactlyOncePoint("clknnew22")
       use never1 = new RunNeverPoint("clknnew33")
       use never2 = new RunNeverPoint("clknnew44")
       use never3 = new RunNeverPoint("clknnew55")
       use never4 = new RunNeverPoint("clknnew66")
       use never5 = new RunNeverPoint("clknnew77")
       use never6 = new RunNeverPoint("clknnew88")
       try 
            seq { point1.Run()
                  failwith ""
                  try 
                      never1.Run()
                  finally 
                      never2.Run()
                  never3.Run()
                  try 
                      never4.Run()
                  finally 
                      never5.Run()
                  never6.Run()
                  yield 1  } |> Seq.iter ignore
        with _ -> 
            point2.Run()
      
    test3931()

    let testve932() = 
       use point1 = new RunExactlyOncePoint("zlknnew11")
       use point2 = new RunExactlyOncePoint("zlknnew22")
       use never1 = new RunNeverPoint("zlknnew33")
       use never2 = new RunNeverPoint("zlknnew44")
       use never3 = new RunNeverPoint("zlknnew55")
       use never4 = new RunNeverPoint("zlknnew66")
       use never5 = new RunNeverPoint("zlknnew77")
       use never6 = new RunNeverPoint("zlknnew88")
       try 
            seq { point1.Run()
                  try 
                      failwith ""
                      try 
                          never1.Run()
                      finally 
                          never2.Run()
                      never3.Run()
                      try 
                          never4.Run()
                      finally 
                          never5.Run()
                      never6.Run()
                  finally
                      point2.Run()
                  yield 1  } |> Seq.iter ignore
        with _ -> ()
          
    testve932() 
    let testve934() = 
       use point1 = new RunExactlyOncePoint("1zlknnew11")
       use point2 = new RunExactlyOncePoint("1zlknnew22")
       use point3 = new RunExactlyOncePoint("1zlknnew33")
       use never1 = new RunNeverPoint("1zlknnew3")
       use never2 = new RunNeverPoint("1zlknnew44")
       use never3 = new RunNeverPoint("1zlknnew55")
       use never4 = new RunNeverPoint("1zlknnew66")
       use never5 = new RunNeverPoint("1zlknnew77")
       use never6 = new RunNeverPoint("1zlknnew88")
       try 
        seq { point1.Run()
              try 
                  try 
                      failwith ""
                      never1.Run()
                  finally 
                      point2.Run()
                  never2.Run()
                  try 
                      never3.Run()
                  finally 
                      never4.Run()
                  never5.Run()
              finally
                  point3.Run()
              yield 1  } |> Seq.iter ignore
       with _ -> ()
      
    testve934()
      
    let testve935() = 
       use point1 = new RunExactlyOncePoint("1zlknnew11")
       use point2 = new RunExactlyOncePoint("1zlknnew22")
       use point3 = new RunExactlyOncePoint("1zlknnew33")
       use point4 = new RunExactlyOncePoint("1zlknnew44")
       use point5 = new RunExactlyOncePoint("1zlknnew55")
       use point6 = new RunExactlyOncePoint("1zlknnew55")
       use never1 = new RunNeverPoint("1zlknnew3")
       use never2 = new RunNeverPoint("1zlknnew4")
       use never3 = new RunNeverPoint("1zlknnew55")
       try 
            seq { point1.Run() 
                  try 
                      try 
                          point2.Run()
                      finally 
                          point3.Run()
                      point4.Run()
                      try 
                          failwith ""
                          never1.Run()
                      finally 
                          point5.Run()
                      never3.Run()
                  finally
                      point6.Run()
                  yield 1  } |> Seq.iter ignore
       with _ -> ()

    testve935() 

    let testve936()  = 
       use point1 = new RunExactlyOncePoint("2zlknnew11")
       use point2 = new RunExactlyOncePoint("2zlknnew22")
       use point3 = new RunExactlyOncePoint("2zlknnew33")
       use point4 = new RunExactlyOncePoint("2zlknnew44")
       use point5 = new RunExactlyOncePoint("2zlknnew55")
       use point6 = new RunExactlyOncePoint("2zlknnew66")
       use never1 = new RunNeverPoint("2zlknnew3")
       use never2 = new RunNeverPoint("2zlknnew4")
       use never3 = new RunNeverPoint("2zlknnew55")
       try 
            seq { point1.Run()
                  for x in 0..0 do
                      try 
                          try 
                              point2.Run()
                          finally 
                              point3.Run()
                          point4.Run()
                          try 
                              failwith ""
                              never1.Run()
                          finally 
                              point5.Run()
                          never2.Run()
                      finally
                          point6.Run()
                      yield 1  } |> Seq.iter ignore
       with _ -> ()

    testve936() 
    let testve937()  = 
       use point1 = new RunExactlyOncePoint("3zlknnew11")
       use point2 = new RunExactlyOncePoint("3zlknnew22")
       use point3 = new RunExactlyOncePoint("3zlknnew33")
       use point4 = new RunExactlyOncePoint("3zlknnew44")
       use point5 = new RunExactlyOncePoint("3zlknnew55")
       use point6 = new RunExactlyOncePoint("3zlknnew66")
       use never1 = new RunNeverPoint("3zlknnew3")
       use never2 = new RunNeverPoint("3zlknnew4")
       use never3 = new RunNeverPoint("3zlknnew55")
       try 
            seq { point1.Run()
                  let mutable x = 0
                  while x = 0 do
                      x <- x + 1 
                      try 
                          try 
                              point2.Run()
                          finally 
                              point3.Run()
                          point4.Run()
                          try 
                              failwith ""
                              never1.Run()
                          finally 
                              point5.Run()
                          never2.Run()
                      finally
                          point6.Run()
                      yield 1  } |> Seq.iter ignore
       with _ -> ()

    testve937() 

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

