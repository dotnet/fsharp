// #Conformance #Arrays #Stress #Structs #Mutable #ControlFlow #LetBindings 
#if TESTS_AS_APP
module Core_array
#endif

let mutable failures = []
let report_failure (s) = 
  stderr.WriteLine " NO"; failures <- s :: failures
let test s b = if not b then (stderr.Write(s:string);   report_failure(s) )
let check s b1 b2 = test s (b1 = b2)


(* TEST SUITE FOR Array *)

module Array2Tests = 

  let test_make_get_set_length () = 
    let arr = Array2D.create 3 4 0 in 
    test "fewoih1" (Array2D.get arr 0 0 = 0);
    test "fewoih2" (Array2D.get arr 0 1 = 0);
    test "vvrew03" (Array2D.get arr 2 2 = 0);
    test "vvrew04" (Array2D.get arr 2 3 = 0);
    ignore (Array2D.set arr 0 2 4);
    test "vsdiuvs5" (Array2D.get arr 0 2 = 4);
    arr[0,2] <- 2;
    test "vsdiuvs6" (arr[0,2] = 2);
    test "vropivrwe7" (Array2D.length1 arr = 3);
    test "vropivrwe8" (Array2D.length2 arr = 4)

    let a = Array2D.init 10 10 (fun i j -> i,j)
    let b = Array2D.init 2 2 (fun i j -> i+1,j+1)
    //test "a2_sub"
    //    (Array2D.sub a 1 1 2 2 = b)


    Array2D.blit b 0 0 a 0 0 2 2
    //test "a2_blit"
    //      (Array2D.sub a 0 0 2 2 = b)

  let _ = test_make_get_set_length ()

module ArrayNonZeroBasedTestsSlice = 
  let runTest () = 
    let arr = (Array2D.initBased 5 4 3 2 (fun i j -> (i,j)))
    test "fewoih1" (arr[6,*] = [|(6, 4); (6, 5)|])
    test "fewoih2" (arr[*,*][1,*] = [|(6, 4); (6, 5)|])
    test "fewoih3" (arr[*,5] =  [|(5, 5); (6, 5); (7, 5)|])
    test "fewoih4" (arr[*,*][*,1] =  [|(5, 5); (6, 5); (7, 5)|])
    test "fewoih5" (arr.GetLowerBound(0) = 5)
    test "fewoih6" (arr.GetLowerBound(1) = 4)
    test "fewoih7" (arr[*,*].GetLowerBound(0) = 0)
    test "fewoih8" (arr[*,*].GetLowerBound(1) = 0)
    test "fewoih9" (arr[*,*][0..,1] =  [|(5, 5); (6, 5); (7, 5)|])
    test "fewoih10" (arr[*,*][1..,1] =  [|(6, 5); (7, 5)|])
    let arr2d = 
        let arr = Array2D.zeroCreateBased 5 4 3 2 
        for i in 5..7 do for j in 4..5 do arr[i,j] <- (i,j)
        arr
    let arr2d2 = 
        let arr = Array2D.zeroCreate 3 2 
        for i in 0..2 do for j in 0..1 do arr[i,j] <- (j,i)
        arr
    test "fewoih11" (arr2d[6..6,5] =  [|(6, 5)|])
    test "fewoih11" (arr2d[..6,5] =  [|(5, 5); (6, 5)|])
    test "fewoih11" (arr2d[6..,5] =  [|(6, 5); (7, 5)|])
    test "fewoih12" (arr2d[*,*][1..,1] =  [|(6, 5); (7, 5)|])
    arr2d[*,*] <- arr2d2
    test "fewoih13" (arr2d[*,*][0..0,1] =  [|(1, 0)|])
    test "fewoih13" (arr2d[*,*][1..,1] =  [|(1, 1); (1, 2)|])
    test "fewoih13" (arr2d[*,*][1,1..] =  [|(1, 1)|])
    test "fewoih13" (arr2d[*,*][1,0..0] =  [|(0, 1)|])
    let arr3d = 
        let arr = System.Array.CreateInstance(typeof<int*int*int>, [| 3;2;1 |], [|5;4;3|]) :?> (int*int*int)[,,]
        for i in 5..7 do for j in 4..5 do for k in 3..3 do arr[i,j,k] <- (i,j,k)
        arr
    let arr3d2 = 
        let arr = System.Array.CreateInstance(typeof<int*int*int>, [| 3;2;1 |]) :?> (int*int*int)[,,]
        for i in 0..2 do for j in 0..1 do for k in 0..0 do arr[i,j,k] <- (k,j,i)
        arr

    test "fewoih14" (arr3d[5,4,3] = (5,4,3))
    test "fewoih15" (arr3d[*,*,*][0,0,0] =  (5,4,3))
    arr3d[*,*,*] <- arr3d2
    test "fewoih16" (arr3d[5,4,3] =  (0,0,0))
    test "fewoih16" (arr3d[5,5,3] =  (0,1,0))
    test "fewoih16" (arr3d[6,5,3] =  (0,1,1))
  let _ = runTest()

module Array3Tests = 

  let test_make_get_set_length () = 
    let arr = Array3D.create 3 4 5 0 in 
    test "fewoih1" (Array3D.get arr 0 0 0 = 0);
    test "fewoih2" (Array3D.get arr 0 1 0 = 0);
    test "vvrew03" (Array3D.get arr 2 2 2 = 0);
    test "vvrew04" (Array3D.get arr 2 3 4 = 0);
    ignore (Array3D.set arr 0 2 3 4);
    test "vsdiuvs5" (Array3D.get arr 0 2 3 = 4);
    arr[0,2,3] <- 2;
    test "vsdiuvs6" (arr[0,2,3] = 2);
    arr[0,2,3] <- 3;
    test "vsdiuvs" (arr[0,2,3] = 3);
    test "vropivrwe7" (Array3D.length1 arr = 3);
    test "vropivrwe8" (Array3D.length2 arr = 4);
    test "vropivrwe9" (Array3D.length3 arr = 5)

  let _ = test_make_get_set_length ()

module Array4Tests =

  let test_make_get_set_length () = 
    let arr = Array4D.create 3 4 5 6 0 in 
    arr[0,2,3,4] <- 2;
    test "vsdiuvsq" (arr[0,2,3,4] = 2);
    arr[0,2,3,4] <- 3;
    test "vsdiuvsw" (arr[0,2,3,4] = 3);
    test "vsdiuvsw" (Array4D.get arr 0 2 3 4 = 3);
    Array4D.set arr 0 2 3 4 5;
    test "vsdiuvsw" (Array4D.get arr 0 2 3 4 = 5);
    test "vropivrwee" (Array4D.length1 arr = 3);
    test "vropivrwer" (Array4D.length2 arr = 4);
    test "vropivrwet" (Array4D.length3 arr = 5)
    test "vropivrwey" (Array4D.length4 arr = 6)

  let test_init () = 
    let arr = Array4D.init 3 4 5 6 (fun i j k m -> i+j+k+m) in 
    test "vsdiuvs1" (arr[0,2,3,4] = 9);
    test "vsdiuvs2" (arr[0,2,3,3] = 8);
    test "vsdiuvs3" (arr[0,0,0,0] = 0);
    arr[0,2,3,4] <- 2;
    test "vsdiuvs4" (arr[0,2,3,4] = 2);
    arr[0,2,3,4] <- 3;
    test "vsdiuvs5" (arr[0,2,3,4] = 3);
    test "vropivrwe1" (Array4D.length1 arr = 3);
    test "vropivrwe2" (Array4D.length2 arr = 4);
    test "vropivrwe3" (Array4D.length3 arr = 5)
    test "vropivrwe4" (Array4D.length4 arr = 6)

  let _ = test_make_get_set_length ()
  let _ = test_init ()

module Array2TestsNoDot = 

  let test_make_get_set_length () = 
    let arr = Array2D.create 3 4 0 in 
    test "fewoih1" (Array2D.get arr 0 0 = 0);
    test "fewoih2" (Array2D.get arr 0 1 = 0);
    test "vvrew03" (Array2D.get arr 2 2 = 0);
    test "vvrew04" (Array2D.get arr 2 3 = 0);
    ignore (Array2D.set arr 0 2 4);
    test "vsdiuvs5" (Array2D.get arr 0 2 = 4);
    arr[0,2] <- 2;
    test "vsdiuvs6" (arr[0,2] = 2);
    test "vropivrwe7" (Array2D.length1 arr = 3);
    test "vropivrwe8" (Array2D.length2 arr = 4)

    let a = Array2D.init 10 10 (fun i j -> i,j)
    let b = Array2D.init 2 2 (fun i j -> i+1,j+1)
    //test "a2_sub"
    //    (Array2D.sub a 1 1 2 2 = b)


    Array2D.blit b 0 0 a 0 0 2 2
    //test "a2_blit"
    //      (Array2D.sub a 0 0 2 2 = b)

  let _ = test_make_get_set_length ()


module ArrayNonZeroBasedTestsSliceNoDot = 
  let runTest () = 
    let arr = (Array2D.initBased 5 4 3 2 (fun i j -> (i,j)))
    test "fewoih1" (arr[6,*] = [|(6, 4); (6, 5)|])
    test "fewoih2" (arr[*,*][1,*] = [|(6, 4); (6, 5)|])
    test "fewoih3" (arr[*,5] =  [|(5, 5); (6, 5); (7, 5)|])
    test "fewoih4" (arr[*,*][*,1] =  [|(5, 5); (6, 5); (7, 5)|])
    test "fewoih5" (arr.GetLowerBound(0) = 5)
    test "fewoih6" (arr.GetLowerBound(1) = 4)
    test "fewoih7" (arr[*,*].GetLowerBound(0) = 0)
    test "fewoih8" (arr[*,*].GetLowerBound(1) = 0)
    test "fewoih9" (arr[*,*][0..,1] =  [|(5, 5); (6, 5); (7, 5)|])
    test "fewoih10" (arr[*,*][1..,1] =  [|(6, 5); (7, 5)|])
    let arr2d = 
        let arr = Array2D.zeroCreateBased 5 4 3 2 
        for i in 5..7 do for j in 4..5 do arr[i,j] <- (i,j)
        arr
    let arr2d2 = 
        let arr = Array2D.zeroCreate 3 2 
        for i in 0..2 do for j in 0..1 do arr[i,j] <- (j,i)
        arr
    test "fewoih11" (arr2d[6..6,5] =  [|(6, 5)|])
    test "fewoih11" (arr2d[..6,5] =  [|(5, 5); (6, 5)|])
    test "fewoih11" (arr2d[6..,5] =  [|(6, 5); (7, 5)|])
    test "fewoih12" (arr2d[*,*][1..,1] =  [|(6, 5); (7, 5)|])
    arr2d[*,*] <- arr2d2
    test "fewoih13" (arr2d[*,*][0..0,1] =  [|(1, 0)|])
    test "fewoih13" (arr2d[*,*][1..,1] =  [|(1, 1); (1, 2)|])
    test "fewoih13" (arr2d[*,*][1,1..] =  [|(1, 1)|])
    test "fewoih13" (arr2d[*,*][1,0..0] =  [|(0, 1)|])
    let arr3d = 
        let arr = System.Array.CreateInstance(typeof<int*int*int>, [| 3;2;1 |], [|5;4;3|]) :?> (int*int*int)[,,]
        for i in 5..7 do for j in 4..5 do for k in 3..3 do arr[i,j,k] <- (i,j,k)
        arr
    let arr3d2 = 
        let arr = System.Array.CreateInstance(typeof<int*int*int>, [| 3;2;1 |]) :?> (int*int*int)[,,]
        for i in 0..2 do for j in 0..1 do for k in 0..0 do arr[i,j,k] <- (k,j,i)
        arr

    test "fewoih14" (arr3d[5,4,3] = (5,4,3))
    test "fewoih15" (arr3d[*,*,*][0,0,0] =  (5,4,3))
    arr3d[*,*,*] <- arr3d2
    test "fewoih16" (arr3d[5,4,3] =  (0,0,0))
    test "fewoih16" (arr3d[5,5,3] =  (0,1,0))
    test "fewoih16" (arr3d[6,5,3] =  (0,1,1))
  let _ = runTest()

module Array3TestsNoDot = 

  let test_make_get_set_length () = 
    let arr = Array3D.create 3 4 5 0 in 
    test "fewoih1" (Array3D.get arr 0 0 0 = 0);
    test "fewoih2" (Array3D.get arr 0 1 0 = 0);
    test "vvrew03" (Array3D.get arr 2 2 2 = 0);
    test "vvrew04" (Array3D.get arr 2 3 4 = 0);
    ignore (Array3D.set arr 0 2 3 4);
    test "vsdiuvs5" (Array3D.get arr 0 2 3 = 4);
    arr[0,2,3] <- 2;
    test "vsdiuvs6" (arr[0,2,3] = 2);
    arr[0,2,3] <- 3;
    test "vsdiuvs" (arr[0,2,3] = 3);
    test "vropivrwe7" (Array3D.length1 arr = 3);
    test "vropivrwe8" (Array3D.length2 arr = 4);
    test "vropivrwe9" (Array3D.length3 arr = 5)

  let _ = test_make_get_set_length ()

module Array4TestsNoDot = 

  let test_make_get_set_length () = 
    let arr = Array4D.create 3 4 5 6 0 in 
    arr[0,2,3,4] <- 2;
    test "vsdiuvsq" (arr[0,2,3,4] = 2);
    arr[0,2,3,4] <- 3;
    test "vsdiuvsw" (arr[0,2,3,4] = 3);
    test "vsdiuvsw" (Array4D.get arr 0 2 3 4 = 3);
    Array4D.set arr 0 2 3 4 5;
    test "vsdiuvsw" (Array4D.get arr 0 2 3 4 = 5);
    test "vropivrwee" (Array4D.length1 arr = 3);
    test "vropivrwer" (Array4D.length2 arr = 4);
    test "vropivrwet" (Array4D.length3 arr = 5)
    test "vropivrwey" (Array4D.length4 arr = 6)

  let test_init () = 
    let arr = Array4D.init 3 4 5 6 (fun i j k m -> i+j+k+m) in 
    test "vsdiuvs1" (arr[0,2,3,4] = 9);
    test "vsdiuvs2" (arr[0,2,3,3] = 8);
    test "vsdiuvs3" (arr[0,0,0,0] = 0);
    arr[0,2,3,4] <- 2;
    test "vsdiuvs4" (arr[0,2,3,4] = 2);
    arr[0,2,3,4] <- 3;
    test "vsdiuvs5" (arr[0,2,3,4] = 3);
    test "vropivrwe1" (Array4D.length1 arr = 3);
    test "vropivrwe2" (Array4D.length2 arr = 4);
    test "vropivrwe3" (Array4D.length3 arr = 5)
    test "vropivrwe4" (Array4D.length4 arr = 6)

  let _ = test_make_get_set_length ()
  let _ = test_init ()

module MiscIndexNotationTests =
    [<Measure>]
    type foo
    let m, n = 1<foo>, 10<foo>
    let vs1 = seq { 1 .. 2 .. 3 }         
    let vs2 = [ 1 .. 2 .. 4 ]         
    let vs3 = [m .. 1<foo> .. n]         
    let vs4 = [| 1 .. 2 .. 4 |]         
    let vs5 = [| m .. 1<foo> .. n |]         

    let arr = [| 1;2;3;4;5 |]
    let arr2 : int[,] = Array2D.zeroCreate 5 5

    let v1 = arr[1] 
    let v2 = arr2[1,1] 
    let v3 = arr[1..3] 

    let v4 = arr[..3]
    let v5 = arr[1..]
    let v6 = arr[*]

    let v7  = arr2[1..3,1..3]
    let v8  = arr2[..3,1..3]
    let v9  = arr2[1..,1..3]
    let v10 = arr2[*,1..3]
    let v11 = arr2[1..3,1..]
    let v12 = arr2[..3,1..]
    let v13 = arr2[1..,1..]
    let v14 = arr2[*,1..]
    let v15 = arr2[1..3,..3]
    let v16 = arr2[..3,..3]
    let v17 = arr2[1..,..3]
    let v18 = arr2[*,..3]
    let v19 = arr2[1..3,*]
    let v20 = arr2[..3,*]
    let v21 = arr2[1..,*]
    let v22 = arr2[*,*]

    module Apps =
        let arrf () = arr
        let arrf2 () = arr2
        let v1 = arrf()[1] 
        let v2 = arrf2()[1,1] 
        let v3 = arrf()[1..3] 

        let v4 = arrf()[..3]
        let v5 = arrf()[1..]
        let v6 = arrf()[*]

        let v7  = arrf2()[1..3,1..3]
        let v8  = arrf2()[..3,1..3]
        let v9  = arrf2()[1..,1..3]
        let v10 = arrf2()[*,1..3]
        let v11 = arrf2()[1..3,1..]
        let v12 = arrf2()[..3,1..]
        let v13 = arrf2()[1..,1..]
        let v14 = arrf2()[*,1..]
        let v15 = arrf2()[1..3,..3]
        let v16 = arrf2()[..3,..3]
        let v17 = arrf2()[1..,..3]
        let v18 = arrf2()[*,..3]
        let v19 = arrf2()[1..3,*]
        let v20 = arrf2()[..3,*]
        let v21 = arrf2()[1..,*]
        let v22 = arrf2()[*,*]

module ArrayStructMutation = 
    module Array1D = 
        module Test1 = 
            [<Struct>]
            type T =
               val mutable i : int
            let a = Array.create 10 Unchecked.defaultof<T>
            a[0].i <- 27
            check "wekvw0301" 27 a[0].i


        module Test2 = 

            [<Struct>]
            type T =
               val mutable public i  : int
               member public this.Set i = this.i <- i
            let a  = Array.create 10 Unchecked.defaultof<T>
            a[0].Set 27
            a[2].Set 27
            check "wekvw0302" 27 a[0].i
            check "wekvw0303" 27 a[2].i
            
    module Array2D = 
        module Test1 = 
            [<Struct>]
            type T =
               val mutable i : int
            let a = Array2D.create 10 10 Unchecked.defaultof<T>
            a[0,0].i <- 27
            check "wekvw0304" 27 a[0,0].i


        module Test2 = 

            [<Struct>]
            type T =
               val mutable public i  : int
               member public this.Set i = this.i <- i
            let a  = Array2D.create 10 10 Unchecked.defaultof<T>
            a[0,0].Set 27
            a[0,2].Set 27
            check "wekvw0305" 27 a[0,0].i
            check "wekvw0306" 27 a[0,2].i
            

    module Array3D = 
        module Test1 = 
            [<Struct>]
            type T =
               val mutable i : int
            let a = Array3D.create 10 10 10 Unchecked.defaultof<T>
            a[0,0,0].i <- 27
            a[0,2,3].i <- 27
            check "wekvw0307" 27 a[0,0,0].i
            check "wekvw0308" 27 a[0,2,3].i


        module Test2 = 

            [<Struct>]
            type T =
               val mutable public i  : int
               member public this.Set i = this.i <- i
            let a  = Array3D.create 10 10 10 Unchecked.defaultof<T>
            a[0,0,0].Set 27
            a[0,2,3].Set 27
            check "wekvw0309" 27 a[0,0,0].i
            check "wekvw030q" 27 a[0,2,3].i
            
    module Array4D = 
        module Test1 = 
            [<Struct>]
            type T =
               val mutable i : int
            let a = Array4D.create 10 10 10 10 Unchecked.defaultof<T>
            a[0,0,0,0].i <- 27
            a[0,2,3,4].i <- 27
            check "wekvw030w" 27 a[0,0,0,0].i
            check "wekvw030e" 27 a[0,2,3,4].i


        module Test2 = 

            [<Struct>]
            type T =
               val mutable public i  : int
               member public this.Set i = this.i <- i
            let a  = Array4D.create 10 10 10 10 Unchecked.defaultof<T>
            a[0,0,0,0].Set 27
            a[0,2,3,4].Set 27
            check "wekvw030r" 27 a[0,0,0,0].i 
            check "wekvw030t" 27 a[0,2,3,4].i

    
#if TESTS_AS_APP
let RUN() = failures
#else
let aa =
  match failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

