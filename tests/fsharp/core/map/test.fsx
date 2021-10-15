// #Conformance #Regression #Collections 
#if TESTS_AS_APP
module Core_map
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

(* TEST SUITE FOR STANDARD LIBRARY *)
  
let test_eq_range n m x = 
  for i = n to m do 
    test "ew9wef" (Map.find i x = i * 100);
  done;
  for i = n to m do 
    test "ew9wef" (Map.tryFind i x = Some (i * 100));
    test "ew9wef" (x.TryGetValue(i) = (true, (i * 100)));
    let mutable res = 0
    test "ew9wef" (x.TryGetValue(i, &res) = true);
    test "ew9wef" (res = (i * 100));
  done;
  for i = m+1 to m+100 do 
    test "ew9wef" (Map.tryFind i x = None);
    test "ew9wef" (x.TryGetValue(i) = (false, 0));
    let mutable res = 0
    test "ew9wef" (x.TryGetValue(i,&res) = false);
  done;
  for i = m+1 to m+5 do 
    test "ew9cwef" ((try Some(Map.find i x) with :? System.Collections.Generic.KeyNotFoundException -> None) = None);
  done

let test39342() = 
  let x = Map.empty in 
  let x = Map.add 1 100 x in
  let x = Map.add 2 200 x in
  let x = Map.add 3 300 x in
  let x = Map.add 4 400 x in
  let x = Map.add 5 500 x in
  let x = Map.add 6 600 x in
  let x = Map.add 7 700 x in
  let x = Map.add 8 800 x in
  let x = Map.add 9 900 x in
  let x = Map.add 10 1000 x in
  let x = Map.add 11 1100 x in
  let x = Map.add 12 1200 x in
  let x = Map.add 13 1300 x in
  let x = Map.add 14 1400 x in
  let x = Map.add 15 1500 x in
  test_eq_range 1 15 x 

do test39342()

let test39343() = 
  let x = Map.empty in 
  let x = Map.add 15 1500 x in
  let x = Map.add 14 1400 x in
  let x = Map.add 13 1300 x in
  let x = Map.add 12 1200 x in
  let x = Map.add 11 1100 x in
  let x = Map.add 10 1000 x in
  let x = Map.add 9 900 x in
  let x = Map.add 8 800 x in
  let x = Map.add 7 700 x in
  let x = Map.add 6 600 x in
  let x = Map.add 5 500 x in
  let x = Map.add 4 400 x in
  let x = Map.add 3 300 x in
  let x = Map.add 2 200 x in
  let x = Map.add 1 100 x in
  test_eq_range 1 15 x 

do test39343()

let test39344() = 
  let x = Map.empty in 
  let x = Map.add 4 400 x in
  test_eq_range 4 4 x 

do test39344()

let test39345() = 
  let x = Map.empty in 
  let x = Map.add 4 400 x in
  let x = Map.add 4 400 x in
  test_eq_range 4 4 x 

do test39345()


let test39346() = 
  let x = Map.empty in 
  let x = Map.add 4 400 x in
  let x = Map.remove 4 x in
  test_eq_range 4 3 x 

do test39346()

let test39347() = 
  let x = Map.empty in 
  let x = Map.add 1 100 x in
  let x = Map.add 2 200 x in
  let x = Map.add 3 300 x in
  let x = Map.add 4 400 x in
  let x = Map.add 5 500 x in
  let x = Map.add 6 600 x in
  let x = Map.add 7 700 x in
  let x = Map.add 8 800 x in
  let x = Map.add 9 900 x in
  let x = Map.add 10 1000 x in
  let x = Map.add 11 1100 x in
  let x = Map.add 12 1200 x in
  let x = Map.add 13 1300 x in
  let x = Map.add 14 1400 x in
  let x = Map.add 15 1500 x in
  let x = Map.remove 3 x in
  let x = Map.remove 2 x in
  let x = Map.remove 1 x in
  let x = Map.remove 15 x in
  test_eq_range 4 14 x 

do test39347()

let test_fold() =
    let m = Map.ofList [for i in 1..20 -> i, i] in
    test "fold 1" (Map.fold (fun acc _ _ -> acc + 1) 0 m = 20);
    test "fold 2" (Map.foldBack (fun _ _ acc -> acc + 1) m 0 = 20);
    test "fold 3" (Map.fold (fun acc n _ -> acc + " " + string n) "0" m = String.concat " " [for i in 0..20 -> string i]);
    test "fold 4" (Map.foldBack (fun n _ acc -> acc + " " + string n) m "21" = String.concat " " [for i in 21..-1..1 -> string i]);
    test "fold 5" (Map.foldBack (fun _ -> max) m 0 = 20);
    test "fold 6" (Map.fold (fun acc n _ -> max acc n) 0 m = 20)

do test_fold()

let testEqNoComparison () = 
    let this = Map.ofArray [| 1, obj() |] 
    let that = Map.ofArray [| 1, obj() |] 
    let that2 = Map.ofArray [| 2, obj() |] 
    test "eq 1a" (this = this)
    test "eq 1b" (that = that)
    test "eq 1c" (that2 = that2)
    test "eq 2a" (this <> that2)
    for i in 0 .. 6 do 
        for j in 0 .. 6 do
            let m1 = Map.ofArray [| for x in 1 .. i -> x, obj() |] 
            let m2 = Map.ofArray [| for x in 1 .. j -> x, obj() |] 
            test "eq 3" ((m1 = m2) = (i = 0 && j = 0))
            test "eq 4" ((m1 = m1) = true)
            test "eq 5" ((m2 = m2) = true)

do testEqNoComparison()

// Adhoc test, bug 6307:
module Bug_FSharp_1_0_6307 = 
    // below works
    let i : global.System.Int32 = 0
    // below does not parse
    let t = typeof<global.System.Int32>

module TestSetHashCodeCase =
    let s = Set.singleton 2147483017
    s.GetHashCode()  // this was failing due to use of 'abs' in GetHashCode

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

