// #Conformance #Constants #Recursion #LetBindings #MemberDefinitions #Mutable 
#if TESTS_AS_APP
module Core_apporder
#endif

#light
let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test s b = if b then () else report_failure(s) 

(* TEST SUITE FOR Int32 *)

let out r (s:string) = r := !r @ [s]

let check s actual expected = 
    if actual = expected then printfn "%s: OK" s
    else report_failure (sprintf "%s: FAILED, expected %A, got %A" s expected actual)

let check2 s expected actual = check s actual expected 

module CheckMutationOfArgumentValuesInOtherArguments = 
    let test1232() = 
        let mutable cell1 = 1
        let f1 x = printfn "hello"; (fun y -> printfn "x = %A, y = %A" x y; (x,y))
        let f2 x y = printfn "x = %A, y = %A" x y; (x,y)
        cell1 <- 1 // reset
        let res = f1 (cell1 <- 11;  cell1) cell1
        check "test1232 - test1" res (11,11)
        cell1 <- 1 // reset
        let res = f1 cell1 (cell1 <- 21;  cell1) 
        check "test1232 - test2" res (1,21)
        cell1 <- 1 // reset
        let res = (f1 (cell1 <- 11;  cell1)) cell1
        check "test1232 - test3" res (11,11)
        cell1 <- 1 // reset
        let res = (f1 cell1) (cell1 <- 21;  cell1) 
        check "test1232 - test4" res (1,21)
        cell1 <- 1 // reset
        let res = f2 (cell1 <- 11;  cell1) cell1
        check "test1232 - test5" res (11,11)
        cell1 <- 1 // reset
        let res = f2 cell1 (cell1 <- 21;  cell1) 
        check "test1232 - test6" res (1,21)
        cell1 <- 1 // reset
        let res = (f2 cell1) (cell1 <- 21;  cell1) 
        check "test1232 - test7" res (1,21)
        cell1 <- 1 // reset
        let res = f2 (cell1 <- 11;  cell1) cell1
        check "test1232 - test8" res (11,11)
        cell1 <- 1 // reset
        let res = f2 cell1 (cell1 <- 21;  cell1) 
        check "test1232 - test9" res (1,21)

    test1232()    

    let test1233() = 
        let cell1 = ref 1
        let f1 x = cell1 := 4; (fun y -> printfn "x = %A, y = %A, !cell1 = %A" x y !cell1; (x,y,!cell1))
        let f2 x y = printfn "x = %A, y = %A, !cell1 = %A" x y !cell1; (x,y,!cell1)
        cell1 := 1
        let res = f1 (cell1 := 11;  !cell1) !cell1
        check "test1233 - test1" res (11,11,4)
        cell1 := 1
        let res = (f1 (cell1 := 11;  !cell1)) !cell1
        check "test1233 - test2" res (11,4,4)
        cell1 := 1
        let res = f1 !cell1 (cell1 := 21;  !cell1) 
        check "test1233 - test3" res (1,21,4)
        cell1 := 1
        let res = (f1 !cell1) (cell1 := 21;  !cell1) 
        check "test1233 - test4" res (1,21,21)
        cell1 := 1
        let res = f2 (cell1 := 11;  !cell1) !cell1
        check "test1233 - test5" res (11,11,11)
        cell1 := 1
        let res = (f2 (cell1 := 11;  !cell1)) !cell1
        check "test1233 - test6" res (11,11,11)
        cell1 := 1
        let res = f2 !cell1 (cell1 := 21;  !cell1) 
        check "test1233 - test7" res (1,21,21)
        cell1 := 1
        let res = (f2 !cell1) (cell1 := 21;  !cell1) 
        check "test1233 - test8" res (1,21,21)


    test1233()    

    let test1234() = 
        let mutable cell1 = 1
        let f1 x = (); (fun y -> (x,y))
        let f2 x y =  (x,y)
        cell1 <- 1 // reset
        let res = f1 (cell1 <- 11;  cell1) cell1
        check "test1234 - test1" res (11,11)
        cell1 <- 1 // reset
        let res = f1 cell1 (cell1 <- 21;  cell1) 
        check "test1234 - test2" res (1,21)
        cell1 <- 1 // reset
        let res = (f1 (cell1 <- 11;  cell1)) cell1
        check "test1234 - test3" res (11,11)
        cell1 <- 1 // reset
        let res = (f1 cell1) (cell1 <- 21;  cell1) 
        check "test1234 - test4" res (1,21)
        cell1 <- 1 // reset
        let res = f2 (cell1 <- 11;  cell1) cell1
        check "test1234 - test5" res (11,11)
        cell1 <- 1 // reset
        let res = f2 cell1 (cell1 <- 21;  cell1) 
        check "test1234 - test6" res (1,21)
        cell1 <- 1 // reset
        let res = (f2 cell1) (cell1 <- 21;  cell1) 
        check "test1234 - test7" res (1,21)
        cell1 <- 1 // reset
        let res = f2 (cell1 <- 11;  cell1) cell1
        check "test1234 - test8" res (11,11)
        cell1 <- 1 // reset
        let res = f2 cell1 (cell1 <- 21;  cell1) 
        check "test1234 - test9" res (1,21)

    test1234()    

    let test1235() = 
        let cell1 = ref 1
        let f1 x = cell1 := 4; (fun y -> (* printfn "x = %A, y = %A, !cell1 = %A" x y !cell1; *) (x,y,!cell1))
        let f2 x y = (* printfn "x = %A, y = %A, !cell1 = %A" x y !cell1; *) (x,y,!cell1)
        cell1 := 1
        let res = f1 (cell1 := 11;  !cell1) !cell1
        check "test1235 - test1" res (11,11,4)
        cell1 := 1
        let res = (f1 (cell1 := 11;  !cell1)) !cell1
        check "test1235 - test2" res (11,4,4)
        cell1 := 1
        let res = f1 !cell1 (cell1 := 21;  !cell1) 
        check "test1235 - test3" res (1,21,4)
        cell1 := 1
        let res = (f1 !cell1) (cell1 := 21;  !cell1) 
        check "test1235 - test4" res (1,21,21)
        cell1 := 1
        let res = f2 (cell1 := 11;  !cell1) !cell1
        check "test1235 - test5" res (11,11,11)
        cell1 := 1
        let res = (f2 (cell1 := 11;  !cell1)) !cell1
        check "test1235 - test6" res (11,11,11)
        cell1 := 1
        let res = f2 !cell1 (cell1 := 21;  !cell1) 
        check "test1235 - test7" res (1,21,21)
        cell1 := 1
        let res = (f2 !cell1) (cell1 := 21;  !cell1) 
        check "test1235 - test8" res (1,21,21)


    test1235()    


module CheckMutationOfArgumentValuesInOtherArgumentsWithIdFunction = 
    let test1232() = 
        let mutable cell1 = 1
        let f1 x = printfn "hello"; (fun y -> printfn "x = %A, y = %A" x y; (x,y))
        let f2 x y = printfn "x = %A, y = %A" x y; (x,y)
        cell1 <- 1 // reset
        let res = id f1 (cell1 <- 11;  cell1) cell1
        check "test1232 - test1" res (11,11)
        cell1 <- 1 // reset
        let res = id f1 cell1 (cell1 <- 21;  cell1) 
        check "test1232 - test2" res (1,21)
        cell1 <- 1 // reset
        let res = id (f1 (cell1 <- 11;  cell1)) cell1
        check "test1232 - test3" res (11,11)
        cell1 <- 1 // reset
        let res = id (f1 cell1) (cell1 <- 21;  cell1) 
        check "test1232 - test4" res (1,21)
        cell1 <- 1 // reset
        let res = id f2 (cell1 <- 11;  cell1) cell1
        check "test1232 - test5" res (11,11)
        cell1 <- 1 // reset
        let res = id f2 cell1 (cell1 <- 21;  cell1) 
        check "test1232 - test6" res (1,21)
        cell1 <- 1 // reset
        let res = id (f2 cell1) (cell1 <- 21;  cell1) 
        check "test1232 - test7" res (1,21)
        cell1 <- 1 // reset
        let res = id f2 (cell1 <- 11;  cell1) cell1
        check "test1232 - test8" res (11,11)
        cell1 <- 1 // reset
        let res = id f2 cell1 (cell1 <- 21;  cell1) 
        check "test1232 - test9" res (1,21)

    test1232()    

    let test1233() = 
        let cell1 = ref 1
        let f1 x = cell1 := 4; (fun y -> printfn "x = %A, y = %A, !cell1 = %A" x y !cell1; (x,y,!cell1))
        let f2 x y = printfn "x = %A, y = %A, !cell1 = %A" x y !cell1; (x,y,!cell1)
        cell1 := 1
        let res = id f1 (cell1 := 11;  !cell1) !cell1
        check "test1233 - test1" res (11,11,4)
        cell1 := 1
        let res = id (f1 (cell1 := 11;  !cell1)) !cell1
        check "test1233 - test2" res (11,4,4)
        cell1 := 1
        let res = id f1 !cell1 (cell1 := 21;  !cell1) 
        check "test1233 - test3" res (1,21,4)
        cell1 := 1
        let res = id (f1 !cell1) (cell1 := 21;  !cell1) 
        check "test1233 - test4" res (1,21,21)
        cell1 := 1
        let res = id f2 (cell1 := 11;  !cell1) !cell1
        check "test1233 - test5" res (11,11,11)
        cell1 := 1
        let res = id (f2 (cell1 := 11;  !cell1)) !cell1
        check "test1233 - test6" res (11,11,11)
        cell1 := 1
        let res = id f2 !cell1 (cell1 := 21;  !cell1) 
        check "test1233 - test7" res (1,21,21)
        cell1 := 1
        let res = id (f2 !cell1) (cell1 := 21;  !cell1) 
        check "test1233 - test8" res (1,21,21)


    test1233()    

    let test1234() = 
        let mutable cell1 = 1
        let f1 x = (); (fun y -> (x,y))
        let f2 x y =  (x,y)
        cell1 <- 1 // reset
        let res = id f1 (cell1 <- 11;  cell1) cell1
        check "test1234 - test1" res (11,11)
        cell1 <- 1 // reset
        let res = id f1 cell1 (cell1 <- 21;  cell1) 
        check "test1234 - test2" res (1,21)
        cell1 <- 1 // reset
        let res = id (f1 (cell1 <- 11;  cell1)) cell1
        check "test1234 - test3" res (11,11)
        cell1 <- 1 // reset
        let res = id (f1 cell1) (cell1 <- 21;  cell1) 
        check "test1234 - test4" res (1,21)
        cell1 <- 1 // reset
        let res = id f2 (cell1 <- 11;  cell1) cell1
        check "test1234 - test5" res (11,11)
        cell1 <- 1 // reset
        let res = id f2 cell1 (cell1 <- 21;  cell1) 
        check "test1234 - test6" res (1,21)
        cell1 <- 1 // reset
        let res = id (f2 cell1) (cell1 <- 21;  cell1) 
        check "test1234 - test7" res (1,21)
        cell1 <- 1 // reset
        let res = id f2 (cell1 <- 11;  cell1) cell1
        check "test1234 - test8" res (11,11)
        cell1 <- 1 // reset
        let res = id f2 cell1 (cell1 <- 21;  cell1) 
        check "test1234 - test9" res (1,21)

    test1234()    

    let test1235() = 
        let cell1 = ref 1
        let f1 x = cell1 := 4; (fun y -> (* printfn "x = %A, y = %A, !cell1 = %A" x y !cell1; *) (x,y,!cell1))
        let f2 x y = (* printfn "x = %A, y = %A, !cell1 = %A" x y !cell1; *) (x,y,!cell1)
        cell1 := 1
        let res = id f1 (cell1 := 11;  !cell1) !cell1
        check "test1235 - test1" res (11,11,4)
        cell1 := 1
        let res = id (f1 (cell1 := 11;  !cell1)) !cell1
        check "test1235 - test2" res (11,4,4)
        cell1 := 1
        let res = id f1 !cell1 (cell1 := 21;  !cell1) 
        check "test1235 - test3" res (1,21,4)
        cell1 := 1
        let res = id (f1 !cell1) (cell1 := 21;  !cell1) 
        check "test1235 - test4" res (1,21,21)
        cell1 := 1
        let res = id f2 (cell1 := 11;  !cell1) !cell1
        check "test1235 - test5" res (11,11,11)
        cell1 := 1
        let res = id (f2 (cell1 := 11;  !cell1)) !cell1
        check "test1235 - test6" res (11,11,11)
        cell1 := 1
        let res = id f2 !cell1 (cell1 := 21;  !cell1) 
        check "test1235 - test7" res (1,21,21)
        cell1 := 1
        let res = id (f2 !cell1) (cell1 := 21;  !cell1) 
        check "test1235 - test8" res (1,21,21)


    test1235()    

module AppTwo = 
    let test1() = 
        let r = ref []
        let f x = out r "app1"; (fun y -> out r "app2")
        f (out r "1") (out r "2")
        check "(two args at a time) test1" !r ["1"; "2"; "app1"; "app2"]

    let test2() = 
        let r = ref []
        let f x y = out r "app1"; out r "app2"
        f (out r "1") (out r "2")
        check "(two args at a time) test2" !r ["1"; "2"; "app1"; "app2"]

    let test3() = 
        let r = ref []
        let f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        f (out r "1") (out r "2")
        check "(two args at a time) test3" !r ["f0"; "1"; "2"; "app1"; "app2"]

    let mutable obj = obj()

    let test1captured() = 
        let r = ref []
        let f x = out r "app1"; (fun y -> out r "app2")
        f (out r "1") (out r "2")
        obj <- f
        check "(two args at a time) test1captured" !r ["1"; "2"; "app1"; "app2"]

    let test2captured() = 
        let r = ref []
        let f x y = out r "app1"; out r "app2"
        f (out r "1") (out r "2")
        obj <- f
        check "(two args at a time) test2captured" !r ["1"; "2"; "app1"; "app2"]

    let test3captured() = 
        let r = ref []
        let f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        f (out r "1") (out r "2")
        obj <- f
        check "(two args at a time) test3captured" !r ["f0"; "1"; "2"; "app1"; "app2"]

    let test1top = 
        let r = ref []
        let f x = out r "app1"; (fun y -> out r "app2")
        f (out r "1") (out r "2")
        fun () -> check "(two args at a time) test1top" !r ["1"; "2"; "app1"; "app2"]

    let test2top = 
        let r = ref []
        let f x y = out r "app1"; out r "app2"
        f (out r "1") (out r "2")
        fun () -> check "(two args at a time) test2top" !r ["1"; "2"; "app1"; "app2"]

    let test3top = 
        let r = ref []
        let f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        f (out r "1") (out r "2")
        fun () -> check "(two args at a time) test3top" !r ["f0"; "1"; "2"; "app1"; "app2"]

    let r1 = ref []
    let f1 x = out r1 "app1"; (fun y -> out r1 "app2")
    let test1mod() = 
        f1 (out r1 "1") (out r1 "2")
        check "(two args at a time) test1mod" !r1 ["1"; "2"; "app1"; "app2"]

    let r2 = ref []
    let f2 x y = out r2 "app1"; out r2 "app2"
    let test2mod() = 
        f2 (out r2 "1") (out r2 "2")
        check "(two args at a time) test2mod" !r2 ["1"; "2"; "app1"; "app2"]

    let r3 = ref []
    let f3 = out r3 "f0"; (fun x -> out r3 "app1"; (fun y -> out r3 "app2"))
    let test3mod() = 
        f3 (out r3 "1") (out r3 "2")
        check "(two args at a time) test3mod" !r3 ["f0"; "1"; "2"; "app1"; "app2"]

    test1()
    test2()
    test3()
        
    test1captured()
    test2captured()
    test3captured()

    test1top()
    test2top()
    test3top()
        
    test1mod()
    test2mod()
    test3mod()
    
module AppOne = 
    let test1() = 
        let r = ref []
        let f x = out r "app1"; (fun y -> out r "app2")
        (f (out r "1")) (out r "2")
        check "(one arg at a time) test1" !r ["1"; "app1"; "2"; "app2"]

    let test2() = 
        let r = ref []
        let f x y = out r "app1"; out r "app2"
        (f (out r "1")) (out r "2")
        check "(one arg at a time) test2" !r ["1"; "2"; "app1"; "app2"]

    let test3() = 
        let r = ref []
        let f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        (f (out r "1")) (out r "2")
        check "(one arg at a time) test3" !r ["f0"; "1"; "app1"; "2"; "app2"]

    let mutable obj = obj()

    let test1captured() = 
        let r = ref []
        let f x = out r "app1"; (fun y -> out r "app2")
        (f (out r "1")) (out r "2")
        obj <- f
        check "(one arg at a time) test1captured" !r ["1"; "app1"; "2"; "app2"]

    let test2captured() = 
        let r = ref []
        let f x y = out r "app1"; out r "app2"
        (f (out r "1")) (out r "2")
        obj <- f
        check "(one arg at a time) test2captured" !r ["1"; "2"; "app1"; "app2"]

    let test3captured() = 
        let r = ref []
        let f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        (f (out r "1")) (out r "2")
        obj <- f
        check "(one arg at a time) test3captured" !r ["f0"; "1"; "app1"; "2"; "app2"]

    let test1top = 
        let r = ref []
        let f x = out r "app1"; (fun y -> out r "app2")
        (f (out r "1")) (out r "2")
        fun () -> check "(one arg at a time) test1top" !r ["1"; "app1"; "2"; "app2"]

    let test2top = 
        let r = ref []
        let f x y = out r "app1"; out r "app2"
        (f (out r "1")) (out r "2")
        fun () -> check "(one arg at a time) test2top" !r ["1"; "2"; "app1"; "app2"]

    let test3top = 
        let r = ref []
        let f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        (f (out r "1")) (out r "2")
        fun () -> check "(one arg at a time) test3top" !r ["f0"; "1"; "app1"; "2"; "app2"]

    let r1 = ref []
    let f1 x = out r1 "app1"; (fun y -> out r1 "app2")
    let test1mod() = 
        (f1 (out r1 "1")) (out r1 "2")
        check "(one arg at a time) test1mod" !r1 ["1"; "app1"; "2"; "app2"]

    let r2 = ref []
    let f2 x y = out r2 "app1"; out r2 "app2"
    let test2mod() = 
        (f2 (out r2 "1")) (out r2 "2")
        check "(one arg at a time) test2mod" !r2 ["1"; "2"; "app1"; "app2"]

    let r3 = ref []
    let f3 = out r3 "f0"; (fun x -> out r3 "app1"; (fun y -> out r3 "app2"))
    let test3mod() = 
        (f3 (out r3 "1")) (out r3 "2")
        check "(one arg at a time) test3mod" !r3 ["f0"; "1"; "app1"; "2"; "app2"]

    test1()
    test2()
    test3()
        
    test1captured()
    test2captured()
    test3captured()

    test1top()
    test2top()
    test3top()
        
    test1mod()
    test2mod()
    test3mod()
    
module AppTwoRec = 
    let test1() = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2")
        and g() = f (out r "1") (out r "2")
        g ()
        check "(two args at a time, rec) test1" !r ["1"; "2"; "app1"; "app2"]

    let test2() = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"
        and g() = f (out r "1") (out r "2")
        g ()
        check "(two args at a time, rec) test2" !r ["1"; "2"; "app1"; "app2"]

    let test3() = 
        let r = ref []
        let rec f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        and g() = f (out r "1") (out r "2")
        g ()
        check "(two args at a time, rec) test3" !r ["f0"; "1"; "2"; "app1"; "app2"]

    let mutable obj = obj()

    let test1captured() = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2")
        and g() = f (out r "1") (out r "2")
        g ()
        obj <- f
        check "(two args at a time, rec) test1captured" !r ["1"; "2"; "app1"; "app2"]

    let test2captured() = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"
        and g() = f (out r "1") (out r "2")
        g ()
        obj <- f
        check "(two args at a time, rec) test2captured" !r ["1"; "2"; "app1"; "app2"]

    let test3captured() = 
        let r = ref []
        let rec f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        and g() = f (out r "1") (out r "2")
        g ()
        obj <- f
        check "(two args at a time, rec) test3captured" !r ["f0"; "1"; "2"; "app1"; "app2"]

    let test1top = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2")
        and g() = f (out r "1") (out r "2")
        g ()
        fun () -> check "(two args at a time, rec) test1top" !r ["1"; "2"; "app1"; "app2"]

    let test2top = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"
        and g() = f (out r "1") (out r "2")
        g ()
        fun () -> check "(two args at a time, rec) test2top" !r ["1"; "2"; "app1"; "app2"]

    let test3top = 
        let r = ref []
        let rec f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        and g() = f (out r "1") (out r "2")
        g ()
        fun () -> check "(two args at a time, rec) test3top" !r ["f0"; "1"; "2"; "app1"; "app2"]

    let r1 = ref []
    let rec f1 x = out r1 "app1"; (fun y -> out r1 "app2")
    and g1() = f1 (out r1 "1") (out r1 "2")
    let test1mod() = 
        g1()
        check "(two args at a time, rec) test1mod" !r1 ["1"; "2"; "app1"; "app2"]

    let r2 = ref []
    let rec f2 x y = out r2 "app1"; out r2 "app2"
    and g2() = f2 (out r2 "1") (out r2 "2")
    let test2mod() = 
        g2()
        check "(two args at a time, rec) test2mod" !r2 ["1"; "2"; "app1"; "app2"]

    let r3 = ref []
    let rec f3 = out r3 "f0"; (fun x -> out r3 "app1"; (fun y -> out r3 "app2"))
    and g3() = f3 (out r3 "1") (out r3 "2")
    let test3mod() = 
        g3()
        check "(two args at a time, rec) test3mod" !r3 ["f0"; "1"; "2"; "app1"; "app2"]

    test1()
    test2()
    test3()
        
    test1captured()
    test2captured()
    test3captured()

    test1top()
    test2top()
    test3top()
        
    test1mod()
    test2mod()
    test3mod()
    
module AppOneRec = 
    let test1() = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2")
        and g() = (f (out r "1")) (out r "2")
        g()
        check "(one arg at a time, rec) test1" !r ["1"; "app1"; "2"; "app2"]

    let test2() = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"
        and g() = (f (out r "1")) (out r "2")
        g()
        check "(one arg at a time, rec) test2" !r ["1"; "2"; "app1"; "app2"]

    let test3() = 
        let r = ref []
        let rec f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        and g() = (f (out r "1")) (out r "2")
        g()
        check "(one arg at a time, rec) test3" !r ["f0"; "1"; "app1"; "2"; "app2"]

    let mutable obj = obj()

    let test1captured() = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2")
        and g() = (f (out r "1")) (out r "2")
        g()
        obj <- f
        check "(one arg at a time, rec) test1captured" !r ["1"; "app1"; "2"; "app2"]

    let test2captured() = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"
        and g() = (f (out r "1")) (out r "2")
        g()
        obj <- f
        check "(one arg at a time, rec) test2captured" !r ["1"; "2"; "app1"; "app2"]

    let test3captured() = 
        let r = ref []
        let rec f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        and g() = (f (out r "1")) (out r "2")
        g()
        obj <- f
        check "(one arg at a time, rec) test3captured" !r ["f0"; "1"; "app1"; "2"; "app2"]

    let test1top = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2")
        and g() = (f (out r "1")) (out r "2")
        g()
        fun () -> check "(one arg at a time, rec) test1top" !r ["1"; "app1"; "2"; "app2"]

    let test2top = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"
        and g() = (f (out r "1")) (out r "2")
        g()
        fun () -> check "(one arg at a time, rec) test2top" !r ["1"; "2"; "app1"; "app2"]

    let test3top = 
        let r = ref []
        let rec f = out r "f0"; (fun x -> out r "app1"; (fun y -> out r "app2"))
        and g() = (f (out r "1")) (out r "2")
        g()
        fun () -> check "(one arg at a time, rec) test3top" !r ["f0"; "1"; "app1"; "2"; "app2"]

    let r1 = ref []
    let rec f1 x = out r1 "app1"; (fun y -> out r1 "app2")
    and g1() = (f1 (out r1 "1")) (out r1 "2")
    let test1mod() = 
        g1()
        check "(one arg at a time, rec) test1mod" !r1 ["1"; "app1"; "2"; "app2"]

    let r2 = ref []
    let rec f2 x y = out r2 "app1"; out r2 "app2"
    and g2() = (f2 (out r2 "1")) (out r2 "2")
    let test2mod() = 
        g2()
        check "(one arg at a time, rec) test2mod" !r2 ["1"; "2"; "app1"; "app2"]

    let r3 = ref []
    let rec f3 = out r3 "f0"; (fun x -> out r3 "app1"; (fun y -> out r3 "app2"))
    and g3() = (f3 (out r3 "1")) (out r3 "2")
    let test3mod() = 
        g3()
        check "(one arg at a time, rec) test3mod" !r3 ["f0"; "1"; "app1"; "2"; "app2"]

    test1()
    test2()
    test3()
        
    test1captured()
    test2captured()
    test3captured()

    test1top()
    test2top()
    test3top()
        
    test1mod()
    test2mod()
    test3mod()
    
module AppTwoRecGeneric = 
    let test1() = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2"; Unchecked.defaultof<'a>)
        and g() = f (out r "1") (out r "2")
        (g () : int) |> ignore
        check "(two args at a time, rec, generic) test1" !r ["1"; "2"; "app1"; "app2"]
        r := []
        (g () : string) |> ignore
        check "(two args at a time, rec, generic) test1" !r ["1"; "2"; "app1"; "app2"]

    let test2() = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"; Unchecked.defaultof<'a>
        and g() = f (out r "1") (out r "2")
        (g () : int) |> ignore
        check "(two args at a time, rec, generic) test2" !r ["1"; "2"; "app1"; "app2"]
        r := []
        (g () : string) |> ignore
        check "(two args at a time, rec, generic) test2" !r ["1"; "2"; "app1"; "app2"]

    let mutable obj = obj()

    let test1captured() = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2"; Unchecked.defaultof<'a>)
        and g() = f (out r "1") (out r "2")
        obj <- f
        (g () : int) |> ignore
        check "(two args at a time, rec, generic) test1captured" !r ["1"; "2"; "app1"; "app2"]
        r := []
        (g () : string) |> ignore
        check "(two args at a time, rec, generic) test1captured" !r ["1"; "2"; "app1"; "app2"]

    let test2captured() = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"; Unchecked.defaultof<'a>
        and g() = f (out r "1") (out r "2")
        g ()
        obj <- f
        check "(two args at a time, rec, generic) test2captured" !r ["1"; "2"; "app1"; "app2"]

    let test1top = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2"; Unchecked.defaultof<'a>)
        and g() = f (out r "1") (out r "2")
        g ()
        fun () -> check "(two args at a time, rec, generic) test1top" !r ["1"; "2"; "app1"; "app2"]

    let test2top = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"; Unchecked.defaultof<'a>
        and g() = f (out r "1") (out r "2")
        g ()
        fun () -> check "(two args at a time, rec, generic) test2top" !r ["1"; "2"; "app1"; "app2"]


    let r1 = ref []
    let rec f1 x = out r1 "app1"; (fun y -> out r1 "app2"; Unchecked.defaultof<'a>)
    and g1() = f1 (out r1 "1") (out r1 "2")
    let test1mod() = 
        g1()
        check "(two args at a time, rec, generic) test1mod" !r1 ["1"; "2"; "app1"; "app2"]

    let r2 = ref []
    let rec f2 x y = out r2 "app1"; out r2 "app2"; Unchecked.defaultof<'a>
    and g2() = f2 (out r2 "1") (out r2 "2")
    let test2mod() = 
        g2()
        check "(two args at a time, rec, generic) test2mod" !r2 ["1"; "2"; "app1"; "app2"]

    test1()
    test2()
        
    test1captured()
    test2captured()

    test1top()
    test2top()
        
    test1mod()
    test2mod()
    
module AppOneRecGeneric = 
    let test1() = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2"; Unchecked.defaultof<'a>)
        and g() = (f (out r "1")) (out r "2")
        g()
        check "(one arg at a time, rec, generic) test1" !r ["1"; "app1"; "2"; "app2"]

    let test2() = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"; Unchecked.defaultof<'a>
        and g() = (f (out r "1")) (out r "2")
        g()
        check "(one arg at a time, rec) test2" !r ["1"; "2"; "app1"; "app2"]


    let mutable obj = obj()

    let test1captured() = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2"; Unchecked.defaultof<'a>)
        and g() = (f (out r "1")) (out r "2")
        g()
        obj <- f
        check "(one arg at a time, rec) test1captured" !r ["1"; "app1"; "2"; "app2"]

    let test2captured() = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"; Unchecked.defaultof<'a>
        and g() = (f (out r "1")) (out r "2")
        g()
        obj <- f
        check "(one arg at a time, rec, generic) test2captured" !r ["1"; "2"; "app1"; "app2"]


    let test1top = 
        let r = ref []
        let rec f x = out r "app1"; (fun y -> out r "app2"; Unchecked.defaultof<'a>)
        and g() = (f (out r "1")) (out r "2")
        g()
        fun () -> check "(one arg at a time, rec, generic) test1top" !r ["1"; "app1"; "2"; "app2"]

    let test2top = 
        let r = ref []
        let rec f x y = out r "app1"; out r "app2"; Unchecked.defaultof<'a>
        and g() = (f (out r "1")) (out r "2")
        g()
        fun () -> check "(one arg at a time, rec, generic) test2top" !r ["1"; "2"; "app1"; "app2"]

    let r1 = ref []
    let rec f1 x = out r1 "app1"; (fun y -> out r1 "app2"; Unchecked.defaultof<'a>)
    and g1() = (f1 (out r1 "1")) (out r1 "2")
    let test1mod() = 
        g1()
        check "(one arg at a time, rec, generic) test1mod" !r1 ["1"; "app1"; "2"; "app2"]

    let r2 = ref []
    let rec f2 x y = out r2 "app1"; out r2 "app2"; Unchecked.defaultof<'a>
    and g2() = (f2 (out r2 "1")) (out r2 "2")
    let test2mod() = 
        g2()
        check "(one arg at a time, rec, generic) test2mod" !r2 ["1"; "2"; "app1"; "app2"]

    test1()
    test2()
        
    test1captured()
    test2captured()

    test1top()
    test2top()
        
    test1mod()
    test2mod()

    
module DuplicateTestsWithCondensedArgs = 
    module CheckMutationOfArgumentValuesInOtherArguments = 
        let test1232() = 
            let mutable cell1 = 1
            let f1 (x:System.IComparable) = printfn "hello"; (fun (y:System.IComparable) -> printfn "x = %A, y = %A" x y; ((x :?> int),(y :?> int) ))
            let f2 (x:System.IComparable) (y:System.IComparable) = printfn "x = %A, y = %A" x y; ((x :?> int),(y :?> int) )
            cell1 <- 1 // reset
            let res = f1 (cell1 <- 11;  cell1) cell1
            check "test1232 - test1" res (11,11)
            cell1 <- 1 // reset
            let res = f1 cell1 (cell1 <- 21;  cell1) 
            check "test1232 - test2" res (1,21)
            cell1 <- 1 // reset
            let res = (f1 (cell1 <- 11;  cell1)) cell1
            check "test1232 - test3" res (11,11)
            cell1 <- 1 // reset
            let res = (f1 cell1) (cell1 <- 21;  cell1) 
            check "test1232 - test4" res (1,21)
            cell1 <- 1 // reset
            let res = f2 (cell1 <- 11;  cell1) cell1
            check "test1232 - test5" res (11,11)
            cell1 <- 1 // reset
            let res = f2 cell1 (cell1 <- 21;  cell1) 
            check "test1232 - test6" res (1,21)
            cell1 <- 1 // reset
            let res = (f2 cell1) (cell1 <- 21;  cell1) 
            check "test1232 - test7" res (1,21)
            cell1 <- 1 // reset
            let res = f2 (cell1 <- 11;  cell1) cell1
            check "test1232 - test8" res (11,11)
            cell1 <- 1 // reset
            let res = f2 cell1 (cell1 <- 21;  cell1) 
            check "test1232 - test9" res (1,21)

        test1232()    

        let test1233() = 
            let cell1 = ref 1
            let f1 (x:System.IComparable) = cell1 := 4; printfn "hello"; (fun (y:System.IComparable) -> printfn "x = %A, y = %A" x y; ((x :?> int),(y :?> int),!cell1 ))
            let f2 (x:System.IComparable) (y:System.IComparable) = printfn "x = %A, y = %A" x y; ((x :?> int),(y :?> int), !cell1 )
            cell1 := 1
            let res = f1 (cell1 := 11;  !cell1) !cell1
            check "test1233 - test1" res (11,11,4)
            cell1 := 1
            let res = (f1 (cell1 := 11;  !cell1)) !cell1
            check "test1233 - test2" res (11,4,4)
            cell1 := 1
            let res = f1 !cell1 (cell1 := 21;  !cell1) 
            check "test1233 - test3" res (1,21,4)
            cell1 := 1
            let res = (f1 !cell1) (cell1 := 21;  !cell1) 
            check "test1233 - test4" res (1,21,21)
            cell1 := 1
            let res = f2 (cell1 := 11;  !cell1) !cell1
            check "test1233 - test5" res (11,11,11)
            cell1 := 1
            let res = (f2 (cell1 := 11;  !cell1)) !cell1
            check "test1233 - test6" res (11,11,11)
            cell1 := 1
            let res = f2 !cell1 (cell1 := 21;  !cell1) 
            check "test1233 - test7" res (1,21,21)
            cell1 := 1
            let res = (f2 !cell1) (cell1 := 21;  !cell1) 
            check "test1233 - test8" res (1,21,21)


        test1233()    

module MemberAppOrder = 
    type Foo(x:int,y:int,?z:int) =
        let mutable a = 0
        let mutable b = 0
        member this.A with get() = a
                      and set(x) = a <- x
        member this.B with get() = b
                      and set(x) = b <- x
        member this.X = x
        member this.Y = y
        member this.Z = defaultArg z 99
        
    let mutable state = []
    let out i =
        state <- state @ [i]
        i
    let foo = new Foo(B=out 5, A=out 4, y=out 2, x=out 3)
    check "cwkneccewi" state [3;2;5;4]
    check "nvroirv" (sprintf "%d %d %d %d %d" foo.A foo.B foo.X foo.Y foo.Z) "4 5 3 2 99"

type RecordWithInts = 
    { A : int
      B : int
      C : int }

module OrderOfRecordInitialisation = 

    let expected =  
        { A = 1
          B = 2
          C = 3 }

    let ShouldInitializeInGivenOrder1 = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1
            B = let _ = order := !order + "2" in 2
            C = let _ = order := !order + "3" in 3 }

        check "cnclewlecp2" expected actual
        check "ceiewoi" "123" !order

    let ShouldInitializeInGivenOrder2 = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1
            C = let _ = order := !order + "2" in 3
            B = let _ = order := !order + "3" in 2 }

        check "cd33289e0ewn1" expected actual
        check "ewlknewv90we2" "123" !order

    let ShouldInitializeInGivenOrder3 = 
        let order = ref ""
        let actual =
          { B = let _ = order := !order + "1" in 2
            A = let _ = order := !order + "2" in 1
            C = let _ = order := !order + "3" in 3 }

        check "cewekcjnwe3" expected actual
        check "cewekcjnwe4" "123" !order


    let ShouldInitializeInGivenOrder4 = 
        let order = ref ""
        let actual =
          { B = let _ = order := !order + "1" in 2
            C = let _ = order := !order + "2" in 3
            A = let _ = order := !order + "3" in 1 }

        check "cewekcjnwe5" expected actual
        check "cewekcjnwe6" "123" !order


    let ShouldInitializeInGivenOrder5 = 
        let order = ref ""
        let actual =
          { C = let _ = order := !order + "1" in 3
            A = let _ = order := !order + "2" in 1
            B = let _ = order := !order + "3" in 2 }

        check "cewekcjnwe7" expected actual
        check "cewekcjnwe8" "123" !order


    let ShouldInitializeInGivenOrder6 = 
        let order = ref ""
        let actual =
          { C = let _ = order := !order + "1" in 3
            B = let _ = order := !order + "2" in 2
            A = let _ = order := !order + "3" in 1 }

        check "cewekcjnwe9" expected actual
        check "cewekcjnwe10" "123" !order


type RecordWithDifferentTypes = 
    { A : int
      B : string
      C : float
      D : RecordWithInts }


module RecordInitialisationWithDifferentTypes = 

    let expected =  
        { A = 1
          B = "2"
          C = 3.0
          D = 
            { A = 4
              B = 5
              C = 6 }}


    let ShouldInitializeInGivenOrder1 = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1
            B = let _ = order := !order + "2" in "2"
            C = let _ = order := !order + "3" in 3.0
            D = let _ = order := !order + "4" in 
                              { A = let _ = order := !order + "5" in 4
                                B = let _ = order := !order + "6" in 5
                                C = let _ = order := !order + "7" in 6 } }

        check "cewekcjnwe11" expected actual
        check "cewekcjnwe12" "1234567" !order


    let ShouldInitializeInGivenOrder2 = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1            
            C = let _ = order := !order + "2" in 3.0
            D = let _ = order := !order + "3" in 
                              { A = let _ = order := !order + "4" in 4
                                B = let _ = order := !order + "5" in 5
                                C = let _ = order := !order + "6" in 6 }
                                
            B = let _ = order := !order + "7" in "2" }

        check "cewekcjnwe13" expected actual
        check "cewekcjnwe14" "1234567" !order


    let ShouldInitializeInGivenOrder3 = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1            
            C = let _ = order := !order + "2" in 3.0
            B = let _ = order := !order + "3" in "2"
            D = let _ = order := !order + "4" in 
                              { A = let _ = order := !order + "5" in 4
                                B = let _ = order := !order + "6" in 5
                                C = let _ = order := !order + "7" in 6 } }

        check "cewekcjnwe15" expected actual
        check "cewekcjnwe16" "1234567" !order



    let ShouldInitializeInGivenOrder4 = 
        let order = ref ""
        let actual =
          { B = let _ = order := !order + "1" in "2"
            A = let _ = order := !order + "2" in 1            
            C = let _ = order := !order + "3" in 3.0            
            D = let _ = order := !order + "4" in 
                              { A = let _ = order := !order + "5" in 4
                                B = let _ = order := !order + "6" in 5
                                C = let _ = order := !order + "7" in 6 } }

        check "cewekcjnwe17" expected actual
        check "cewekcjnwe18" "1234567" !order


    let ShouldInitializeInGivenOrder5 = 
        let order = ref ""
        let actual =
          { D = let _ = order := !order + "1" in 
                              { A = let _ = order := !order + "2" in 4
                                B = let _ = order := !order + "3" in 5
                                C = let _ = order := !order + "4" in 6 } 
            B = let _ = order := !order + "5" in "2"
            C = let _ = order := !order + "6" in 3.0
            A = let _ = order := !order + "7" in 1 }

        check "cewekcjnwe19" expected actual
        check "cewekcjnwe20" "1234567" !order


    let ShouldInitializeInGivenOrder6 = 
        let order = ref ""
        let actual =
          { D = let _ = order := !order + "1" in 
                              { A = let _ = order := !order + "2" in 4
                                B = let _ = order := !order + "3" in 5
                                C = let _ = order := !order + "4" in 6 } 
            A = let _ = order := !order + "5" in 1
            B = let _ = order := !order + "6" in "2"
            C = let _ = order := !order + "7" in 3.0 }

        check "cewekcjnwe21" expected actual
        check "cewekcjnwe22" "1234567" !order

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


