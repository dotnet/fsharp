// #Conformance #Reflection #Unions #Tuples 
module Test2
#nowarn "44"

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

open System.Reflection

(* TEST SUITE FOR Microsoft.FSharp.Reflection *)

open Test

open Microsoft.FSharp.Reflection

module NewTests = 

    let (|C|) (c:UnionCaseInfo) = c.Name, c.GetFields()
    let (|M|) (c:System.Reflection.MethodInfo) = c.Name, c.MemberType
    let (|P|) (c:System.Reflection.PropertyInfo) = c.Name, c.MemberType
    let (|String|_|) (v:obj) = match v with :? string as s -> Some(s) | _ -> None
    let (|Int|_|) (v:obj) = match v with :? int as s -> Some(s) | _ -> None
    let showAll = 
#if NETCOREAPP
        true
#else
        System.Reflection.BindingFlags.Public ||| System.Reflection.BindingFlags.NonPublic 
#endif
    do test "ncwowe932aq" (FSharpType.IsUnion (typeof<PublicUnionType1>)) 
    do test "ncwowe932aw" (FSharpType.IsUnion (XX("1","2").GetType())) 
    do test "ncwowe932ae" (FSharpType.IsUnion ((XX2 "1").GetType()))
    do test "ncwowe932ar" (FSharpType.IsRecord (typeof<PublicRecordType1>)) 
    do test "ncwowe932at" (FSharpType.IsRecord (typeof<PublicRecordType2<int>>)) 
    do test "ncwowe932at" (FSharpType.IsRecord (typeof<PublicRecordType3WithCLIMutable<int>>)) 
    do test "ncwowe932at" (not (FSharpType.IsFunction (typeof<PublicRecordType3WithCLIMutable<int>>)) )
    do test "ncwowe932at" (not (FSharpType.IsExceptionRepresentation (typeof<PublicRecordType3WithCLIMutable<int>>)) )
    do test "ncwowe932at" (not (FSharpType.IsUnion (typeof<PublicRecordType3WithCLIMutable<int>>)) )

    
    do test "ncwowe932ay" (FSharpType.IsFunction (typeof<(int -> int)>)) 
    do test "ncwowe932au" (FSharpType.IsFunction ( (fun x -> x).GetType()))
    do test "ncwowe932ai" (FSharpType.IsExceptionRepresentation (typeof< MatchFailureException>))

    do test "ncwowe932a1" (not (FSharpType.IsExceptionRepresentation (typeof<int * int>))) 
    do test "ncwowe932a2" (not (FSharpType.IsFunction (typeof<int * int>))) 
    do test "ncwowe932a3" (not (FSharpType.IsUnion (typeof<int * int>))) 
    do test "ncwowe932a4" (not (FSharpType.IsUnion (typeof<PublicRecordType1>))) 
    do test "ncwowe932a5" (not (FSharpType.IsRecord (typeof<PublicUnionType1>))) 
    do test "ncwowe932a6" (not (FSharpType.IsRecord (typeof<int * int>))) 
    do test "ncwowe932a7" (not (FSharpType.IsFunction (typeof<int * int>))) 
    do test "ncwowe932a8" (not (FSharpType.IsUnion (typeof<int * int>))) 
    

    do test "ncwowe932a" (match FSharpType.GetUnionCases (typeof<PublicUnionType1>) with [| C("X",[| _ |]); C("XX",[|_; _|]) |] -> true | _ -> false)
    do test "ncwowe932gb" (match FSharpType.GetUnionCases (typeof<PublicUnionType2>) with [| C("X2",[||]); C("XX2",[|_|]) |]-> true | _ -> false)
    do test "ncwowe932f" (match FSharpType.GetUnionCases (typeof<int option>) with [| C("None",[||]); C("Some", [| _ |]) |] -> true | _ -> false)
    do test "ncwowe932e" (match FSharpType.GetUnionCases (typeof<int list>) with [| C("Empty",[||]); C("Cons", [|_;_|]) |] -> true | _ -> false)
    do test "ncwowe932w" (match FSharpType.GetUnionCases (typeof<PublicUnionType3<int>>) with [| C("X3",[||]); C("XX3", [|_|]) |] -> true | _ -> false)
    do test "ncwowe932ew" (match FSharpType.GetRecordFields (typeof<PublicRecordType1>) with [| P("r1a",_) |] -> true | _ -> false)
    do test "ncwowe932q" (match FSharpType.GetRecordFields (typeof<PublicRecordType2<int>>) with  [| P("r2b",_); P("r2a",_) |] -> true | _ -> false)
    do test "ncwowe932q" (match FSharpType.GetRecordFields (typeof<PublicRecordType3WithCLIMutable<int>>) with  [| P("r3b",_); P("r3a",_) |] -> true | _ -> false)
    //do test "ncwowe932" (match Value.GetTypeInfo( () ) with UnitType _ -> true | _ -> false)
    do test "ncqmkee32al1" (match FSharpValue.GetUnionFields(XX ("1","2"),null) with C("XX", [|_;_|]), [| String("1"); String("2") |] -> true | _ -> false)
    do test "ncqmkee32al2" (match FSharpValue.GetUnionFields(XX2 "1",null) with C("XX2", [|_|]), [| String("1") |] -> true | _ -> false)
    do test "ncqmkee32al3" (match FSharpValue.GetUnionFields([1],null) with C("Cons", [|_;_|]), [| Int(1); _ |] -> true | _ -> false)
    do test "ncqmkee32al4" (match FSharpValue.GetUnionFields([1],typeof<list<int>>) with C("Cons", [|_;_|]), [| Int(1); _ |] -> true | _ -> false)
    do test "ncqmkee32al5" (match FSharpValue.GetUnionFields([],null) with C("Empty", [||]), [| |] -> true | _ -> false)
    do test "ncqmkee32al6" (match FSharpValue.GetUnionFields(([]:list<int>),typeof<list<int>>) with C("Empty", [||]), [| |] -> true | _ -> false)

    do test "ncqmkee32al7" (match FSharpValue.GetUnionFields(Some(1),null) with C("Some", [|_|]), [| Int(1) |] -> true | _ -> false)
    do test "ncqmkee32al8" (match FSharpValue.GetUnionFields(Some(1),typeof<option<int>>) with C("Some", [|_|]), [| Int(1) |] -> true | _ -> false)
    do test "ncqmkee32al9" (match FSharpValue.GetUnionFields(None,typeof<option<int>>) with C("None", [||]), [| |] -> true | _ -> false)

    do test "ncqmkee32ala" (match FSharpValue.GetUnionFields(Some(Some(1)),null) with C("Some", [|_|]), [| _ |] -> true | _ -> false)
    do test "ncqmkee32alb" (match FSharpValue.GetUnionFields(Some("abc"),null) with C("Some", [|_|]), [| String("abc") |] -> true | _ -> false)

    do test "ncqmkee32alc" (match FSharpValue.GetRecordFields(ref 1) with [| Int(1) |] -> true | _ -> false)

    do test "ncqmkee32g1" (try ignore(FSharpValue.GetRecordFields(1)); false with :? System.ArgumentException -> true)
    do test "ncqmkee32g2" (try ignore(FSharpValue.GetRecordFields([1])); false with :? System.ArgumentException -> true)
    do test "ncqmkee32g3" (try ignore(FSharpValue.GetRecordFields(None)); false with :? System.ArgumentException -> true)
    do test "ncqmkee32g4" (try ignore(FSharpValue.GetRecordFields(Some(1))); false with :? System.ArgumentException -> true)
    do test "ncqmkee32g5" (try ignore(FSharpValue.GetRecordFields(1M)); false with :? System.ArgumentException -> true)

    do test "ncqmkee32j" (match FSharpValue.GetTupleFields((1,2)) with  [|Int(1);Int(2)|] -> true | _ -> false)
    do test "ncqmkee32k" (match FSharpValue.GetTupleFields((1,2,3)) with [|Int(1);Int(2);Int(3)|] -> true | _ -> false)
    do test "ncqmkee32l" (match FSharpValue.GetTupleFields((1,2,3,4)) with [|Int(1);Int(2);Int(3);Int(4)|] -> true | _ -> false)
    do test "ncqmkee32m" (match FSharpValue.GetTupleFields((1,2,3,4,5)) with [|Int(1);Int(2);Int(3);Int(4);Int(5)|] -> true | _ -> false)
    do test "ncqmkee32n" (match FSharpValue.GetTupleFields((1,2,3,4,5,6)) with [|Int(1);Int(2);Int(3);Int(4);Int(5);Int(6)|] -> true | _ -> false)
    do test "ncqmkee32o" (match FSharpValue.GetTupleFields((1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)) with [|Int(1);Int(2);Int(3);Int(4);Int(5);Int(6);Int(7);Int(8);Int(9);Int(10);Int(11);Int(12);Int(13);Int(14);Int(15);Int(16)|] -> true | _ -> false)

    do test "ncqmkee32ao" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<PublicUnionType1>).[1]) (box (XX ("1","2"))) with [| String("1");String("2") |] -> true | _ -> false)
    do test "ncqmkee32ap" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<PublicUnionType1>).[0]) (box (X "1")) with [| String("1") |] -> true | _ -> false)
    do test "ncqmkee32aq" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<PublicUnionType2>).[1]) (box (XX2 "1")) with [| String("1") |] -> true | _ -> false)
    do test "ncqmkee32ar" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<PublicUnionType2>).[0]) (box X2) with [| |] -> true | _ -> false)
    do test "ncqmkee32bs" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<list<int>>).[1]) (box [100]) with [| Int(100);_ |] -> true | _ -> false)
    do test "ncqmkee32bt" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<list<int>>).[0]) (box ([]:list<int>)) with [| |] -> true | _ -> false)
    do test "ncqmkee32bu" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<option<int>>).[1]) (box (Some(1))) with [| Int(1) |] -> true | _ -> false)
    do test "ncqmkee32bu" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<option<int>>).[0]) (box (None:int option)) with [| |] -> true | _ -> false)
    do test "ncqmkee32f" (match FSharpValue.PreComputeRecordReader (typeof<int ref>) (box (ref 1)) with [| Int(1) |] -> true | _ -> false)
    do test "ncqmkee33h" (match  FSharpValue.PreComputeTupleReader (typeof<int * int>) (box (1,2)) with [| Int(1);Int(2) |] -> true | _ -> false)
    do test "ncqmkee33h" (match  FSharpValue.PreComputeTupleReader (typeof<int * int * int>) (box (1,2,3)) with [| Int(1);Int(2);Int(3) |] -> true | _ -> false)
    do test "ncqmkee33h" (match  FSharpValue.PreComputeTupleReader (typeof<int * int * int * int>) (box (1,2,3,4)) with [| Int(1);Int(2);Int(3);Int(4) |] -> true | _ -> false)
    do test "ncqmkee33h" (match  FSharpValue.PreComputeTupleReader (typeof<int * int * int * int * int>) (box (1,2,3,4,5)) with [| Int(1);Int(2);Int(3);Int(4);Int(5) |] -> true | _ -> false)
    do test "ncqmkee33h" (match  FSharpValue.PreComputeTupleReader (typeof<int * int * int * int * int * int>) (box (1,2,3,4,5,6)) with [| Int(1);Int(2);Int(3);Int(4);Int(5);Int(6) |] -> true | _ -> false)
    do test "ncqmkee33i" (match  FSharpValue.PreComputeTupleReader (typeof<int * int * int * int * int * int * int * int * int * int * int * int * int * int * int * int>) (box (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)) with [|Int(1);Int(2);Int(3);Int(4);Int(5);Int(6);Int(7);Int(8);Int(9);Int(10);Int(11);Int(12);Int(13);Int(14);Int(15);Int(16)|] -> true | _ -> false)


    let UCI (ty,i) = FSharpType.GetUnionCases(ty).[i]
    
    do test "ncqmkee32ao1" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<PublicUnionType1>, 0)) [| box "1" |] = box (X "1"))
    do test "ncqmkee32ao2" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<PublicUnionType1>, 1)) [| box "1"; box "2" |] = box (XX ("1","2")))
    do test "ncqmkee32ao3" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<PublicUnionType2>, 0)) [|  |] = box X2)
    do test "ncqmkee32ao4" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<PublicUnionType2>, 1)) [| box "1"; |] = box (XX2 "1"))
    do test "ncqmkee32ao5" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<int PublicUnionType3>, 0)) [|  |] = box (X3 : int PublicUnionType3))
    do test "ncqmkee32ao6" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<string PublicUnionType3>, 1)) [| box "1"; |] = box (XX3 "1"))
    do test "ncqmkee32ao7" (FSharpValue.PreComputeRecordConstructor(typeof<PublicRecordType1>) [| box 1; |] = box {r1a = 1 })
    do test "ncqmkee32ao8" (FSharpValue.PreComputeRecordConstructor(typeof<PublicRecordType2<string>>) [| box "1"; box 1 |] = box {r2b = "1"; r2a = 1 })
    do test "ncqmkee32ao8" (FSharpValue.PreComputeRecordConstructor(typeof<PublicRecordType3WithCLIMutable<string>>) [| box "1"; box 1 |] = box {r3b = "1"; r3a = 1 })

    do test "ncqmkee33h1" (FSharpValue.PreComputeTupleConstructor(typeof<int * int>) [| box 1; box 2 |] = box (1,2))
    do test "ncqmkee33h2" (FSharpValue.PreComputeTupleConstructor(typeof<int * int * int>) [| box 1; box 2; box 3 |] = box (1,2,3))
    do test "ncqmkee33h3" (FSharpValue.PreComputeTupleConstructor(typeof<int * int * int * int>) [| box 1; box 2; box 3; box 4 |] = box (1,2,3,4))
    do test "ncqmkee33h4" (FSharpValue.PreComputeTupleConstructor(typeof<int * int * int * int * int>) [| box 1; box 2; box 3; box 4; box 5 |] = box (1,2,3,4,5))
    do test "ncqmkee33h5" (FSharpValue.PreComputeTupleConstructor(typeof<int * int * int * int * int * int>) [| box 1; box 2; box 3; box 4; box 5; box 6 |] = box (1,2,3,4,5,6))

    do test "ncqmkee33h6" (FSharpValue.PreComputeTupleConstructor(typeof<string * string>) [| box "1"; box "2" |] = box ("1","2"))
    do test "ncqmkee33h7" (FSharpValue.PreComputeTupleConstructor(typeof<string * string * string>) [| box "1"; box "2"; box "3" |] = box ("1","2","3"))
    do test "ncqmkee33h8" (FSharpValue.PreComputeTupleConstructor(typeof<string * string * string * string>) [| box "1"; box "2"; box "3"; box "4" |] = box ("1","2","3","4"))
    do test "ncqmkee33h9" (FSharpValue.PreComputeTupleConstructor(typeof<string * string * string * string * string * string * string * string * string * string * string * string * string * string * string * string>) [| box "1"; box "2"; box "3"; box "4"; box "5"; box "6"; box "7"; box "8"; box "9"; box "10"; box "11"; box "12"; box "13"; box "14"; box "15"; box "16"|] = box ("1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16"))

    let badarg f = try ignore(f()); false with :? System.ArgumentException -> true
    
    do test "mcwowe932a" (badarg (fun () -> FSharpType.GetUnionCases (typeof<InternalUnionType1>)))
    do test "mcwowe932gb" (badarg (fun () -> FSharpType.GetUnionCases (typeof<InternalUnionType2>)))
    do test "mcwowe932w" (badarg (fun () -> FSharpType.GetUnionCases (typeof<InternalUnionType3<int>>)))
    do test "mcwowe932ew" (badarg (fun () -> FSharpType.GetRecordFields (typeof<InternalRecordType1>)))
    do test "mcwowe932q" (badarg (fun () -> FSharpType.GetRecordFields (typeof<InternalRecordType2<int>>)))
    do test "mcqmkee32al1" (badarg (fun () -> FSharpValue.GetUnionFields(InternalXX ("1","2"),null)))
    do test "mcqmkee32al2" (badarg (fun () -> FSharpValue.GetUnionFields(InternalXX2 "1",null)))


    do test "mcqmkee32ao" (badarg (fun () -> FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<InternalUnionType1>).[1])))
    do test "mcqmkee32ap" (badarg (fun () ->  FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<InternalUnionType1>).[0])))
    do test "mcqmkee32aq" (badarg (fun () ->  FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<InternalUnionType2>).[1])))
    do test "mcqmkee32ar" (badarg (fun () ->  FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<InternalUnionType2>).[0])))
    
    do test "mcqmkee32ao1" (badarg (fun () -> FSharpValue.PreComputeUnionReader(UCI(typeof<InternalUnionType1>, 0))))
    do test "mcqmkee32ao2" (badarg (fun () -> FSharpValue.PreComputeUnionReader(UCI(typeof<InternalUnionType1>, 1))))
    do test "mcqmkee32ao3" (badarg (fun () -> FSharpValue.PreComputeUnionReader(UCI(typeof<InternalUnionType2>, 0))))
    do test "mcqmkee32ao4" (badarg (fun () -> FSharpValue.PreComputeUnionReader(UCI(typeof<InternalUnionType2>, 1))))
    do test "mcqmkee32ao5" (badarg (fun () -> FSharpValue.PreComputeUnionReader(UCI(typeof<int InternalUnionType3>, 0))))
    do test "mcqmkee32ao6" (badarg (fun () -> FSharpValue.PreComputeUnionReader(UCI(typeof<string InternalUnionType3>, 1))))
    do test "mcqmkee32ao7" (badarg (fun () -> FSharpValue.PreComputeRecordReader(typeof<InternalRecordType1>)))
    do test "ncqmkee32ao8" (badarg (fun () -> FSharpValue.PreComputeRecordReader(typeof<string InternalRecordType2>)))

    module RepeatTestsWithShowAll  =
        do test "ncwowe932a" (match FSharpType.GetUnionCases (typeof<PublicUnionType1>,showAll) with [| C("X",[| _ |]); C("XX",[|_; _|]) |] -> true | _ -> false)
        do test "ncwowe932gb" (match FSharpType.GetUnionCases (typeof<PublicUnionType2>,showAll) with [| C("X2",[||]); C("XX2",[|_|]) |]-> true | _ -> false)
        do test "ncwowe932f" (match FSharpType.GetUnionCases (typeof<int option>,showAll) with [| C("None",[||]); C("Some", [| _ |]) |] -> true | _ -> false)
        do test "ncwowe932e" (match FSharpType.GetUnionCases (typeof<int list>,showAll) with [| C("Empty",[||]); C("Cons", [|_;_|]) |] -> true | _ -> false)
        do test "ncwowe932w" (match FSharpType.GetUnionCases (typeof<PublicUnionType3<int>>,showAll) with [| C("X3",[||]); C("XX3", [|_|]) |] -> true | _ -> false)
        do test "ncwowe932ew" (match FSharpType.GetRecordFields (typeof<PublicRecordType1>,showAll) with [| P("r1a",_) |] -> true | _ -> false)
        do test "ncwowe932q" (match FSharpType.GetRecordFields (typeof<PublicRecordType2<int>>,showAll) with  [| P("r2b",_); P("r2a",_) |] -> true | _ -> false)
        do test "ncwowe932q" (match FSharpType.GetRecordFields (typeof<PublicRecordType3WithCLIMutable<int>>,showAll) with  [| (P("r3b",_) as p1); (P("r3a",_) as p2) |] -> p1.CanWrite && p2.CanWrite | _ -> false)
        do test "ncqmkee32al1" (match FSharpValue.GetUnionFields(XX ("1","2"),null,showAll) with C("XX", [|_;_|]), [| String("1"); String("2") |] -> true | _ -> false)
        do test "ncqmkee32al2" (match FSharpValue.GetUnionFields(XX2 "1",null,showAll) with C("XX2", [|_|]), [| String("1") |] -> true | _ -> false)
        do test "ncqmkee32al3" (match FSharpValue.GetUnionFields([1],null,showAll) with C("Cons", [|_;_|]), [| Int(1); _ |] -> true | _ -> false)
        do test "ncqmkee32al4" (match FSharpValue.GetUnionFields([1],typeof<list<int>>,showAll) with C("Cons", [|_;_|]), [| Int(1); _ |] -> true | _ -> false)
        do test "ncqmkee32al5" (match FSharpValue.GetUnionFields([],null,showAll) with C("Empty", [||]), [| |] -> true | _ -> false)
        do test "ncqmkee32al6" (match FSharpValue.GetUnionFields(([]:list<int>),typeof<list<int>>,showAll) with C("Empty", [||]), [| |] -> true | _ -> false)

        do test "ncqmkee32al7" (match FSharpValue.GetUnionFields(Some(1),null,showAll) with C("Some", [|_|]), [| Int(1) |] -> true | _ -> false)
        do test "ncqmkee32al8" (match FSharpValue.GetUnionFields(Some(1),typeof<option<int>>,showAll) with C("Some", [|_|]), [| Int(1) |] -> true | _ -> false)
        do test "ncqmkee32al9" (match FSharpValue.GetUnionFields(None,typeof<option<int>>,showAll) with C("None", [||]), [| |] -> true | _ -> false)

        do test "ncqmkee32ala" (match FSharpValue.GetUnionFields(Some(Some(1)),null,showAll) with C("Some", [|_|]), [| _ |] -> true | _ -> false)
        do test "ncqmkee32alb" (match FSharpValue.GetUnionFields(Some("abc"),null,showAll) with C("Some", [|_|]), [| String("abc") |] -> true | _ -> false)

        do test "ncqmkee32alc" (match FSharpValue.GetRecordFields(ref 1,showAll) with [| Int(1) |] -> true | _ -> false)

        do test "ncqmkee32g1" (try ignore(FSharpValue.GetRecordFields(1,showAll)); false with :? System.ArgumentException -> true)
        do test "ncqmkee32g2" (try ignore(FSharpValue.GetRecordFields([1],showAll)); false with :? System.ArgumentException -> true)
        do test "ncqmkee32g3" (try ignore(FSharpValue.GetRecordFields(None,showAll)); false with :? System.ArgumentException -> true)
        do test "ncqmkee32g4" (try ignore(FSharpValue.GetRecordFields(Some(1),showAll)); false with :? System.ArgumentException -> true)
        do test "ncqmkee32g5" (try ignore(FSharpValue.GetRecordFields(1M,showAll)); false with :? System.ArgumentException -> true)

        do test "ncqmkee32ao" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<PublicUnionType1>,showAll).[1]) (box (XX ("1","2"))) with [| String("1");String("2") |] -> true | _ -> false)
        do test "ncqmkee32ap" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<PublicUnionType1>,showAll).[0]) (box (X "1")) with [| String("1") |] -> true | _ -> false)
        do test "ncqmkee32aq" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<PublicUnionType2>,showAll).[1]) (box (XX2 "1")) with [| String("1") |] -> true | _ -> false)
        do test "ncqmkee32ar" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<PublicUnionType2>,showAll).[0]) (box X2) with [| |] -> true | _ -> false)
        do test "ncqmkee32bs" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<list<int>>,showAll).[1]) (box [100]) with [| Int(100);_ |] -> true | _ -> false)
        do test "ncqmkee32bt" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<list<int>>,showAll).[0]) (box ([]:list<int>)) with [| |] -> true | _ -> false)
        do test "ncqmkee32bu" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<option<int>>,showAll).[1]) (box (Some(1))) with [| Int(1) |] -> true | _ -> false)
        do test "ncqmkee32bu" (match FSharpValue.PreComputeUnionReader (FSharpType.GetUnionCases(typeof<option<int>>,showAll).[0]) (box (None:int option)) with [| |] -> true | _ -> false)
        do test "ncqmkee32f" (match FSharpValue.PreComputeRecordReader (typeof<int ref>,showAll) (box (ref 1)) with [| Int(1) |] -> true | _ -> false)


        let UCI (ty,i) = FSharpType.GetUnionCases(ty).[i]
        
        do test "ncqmkee32ao1" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<PublicUnionType1>, 0),showAll) [| box "1" |] = box (X "1"))
        do test "ncqmkee32ao2" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<PublicUnionType1>, 1),showAll) [| box "1"; box "2" |] = box (XX ("1","2")))
        do test "ncqmkee32ao3" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<PublicUnionType2>, 0),showAll) [|  |] = box X2)
        do test "ncqmkee32ao4" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<PublicUnionType2>, 1),showAll) [| box "1"; |] = box (XX2 "1"))
        do test "ncqmkee32ao5" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<int PublicUnionType3>, 0),showAll) [|  |] = box (X3 : int PublicUnionType3))
        do test "ncqmkee32ao6" (FSharpValue.PreComputeUnionConstructor(UCI(typeof<string PublicUnionType3>, 1),showAll) [| box "1"; |] = box (XX3 "1"))
        do test "ncqmkee32ao7" (FSharpValue.PreComputeRecordConstructor(typeof<PublicRecordType1>,showAll) [| box 1; |] = box {r1a = 1 })
        do test "ncqmkee32ao8a" (FSharpValue.PreComputeRecordConstructor(typeof<string PublicRecordType2>,showAll) [| box "1"; box 1 |] = box {r2b = "1"; r2a = 1 })
        do test "ncqmkee32ao8b" (FSharpValue.PreComputeRecordConstructor(typeof<PublicRecordType3WithCLIMutable<string>>,showAll) [| box "1"; box 1 |] = box {r3b = "1"; r3a = 1 })
        do test "ncqmkee32ao8c" (typeof<PublicRecordType3WithCLIMutable<string>>.GetConstructor([| |]) <> null) 

    open System.Reflection
    do printfn "%A" (FSharpType.GetUnionCases (typeof<InternalUnionType1>, showAll))
    do test "qcwowe932a" (match FSharpType.GetUnionCases (typeof<InternalUnionType1>, showAll) with [| C("InternalX",[| _ |]); C("InternalXX",[|_; _|]) |] -> true | _ -> false)
    do test "qcwowe932gb" (match FSharpType.GetUnionCases (typeof<InternalUnionType2>, showAll) with [| C("InternalX2",[||]); C("InternalXX2",[|_|]) |]-> true | _ -> false)
    do test "qcwowe932w" (match FSharpType.GetUnionCases (typeof<InternalUnionType3<int>>, showAll) with [| C("InternalX3",[||]); C("InternalXX3", [|_|]) |] -> true | _ -> false)
    do test "qcwowe932ew" (match FSharpType.GetRecordFields (typeof<InternalRecordType1>, showAll) with [| P("internal_r1a",_) |] -> true | _ -> false)
    do printfn "%A" (FSharpType.GetRecordFields (typeof<InternalRecordType2<int>>, showAll))
    do test "qcwowe932q" (match FSharpType.GetRecordFields (typeof<InternalRecordType2<int>>, showAll) with  [| P("internal_r2b",_); P("internal_r2a",_) |] -> true | _ -> false)
    do test "qcqmkee32al1" (match FSharpValue.GetUnionFields(InternalXX ("1","2"),null, showAll) with C("InternalXX", [|_;_|]), [| String("1"); String("2") |] -> true | _ -> false)
    do test "qcqmkee32al2" (match FSharpValue.GetUnionFields(InternalXX2 "1",null, showAll) with C("InternalXX2", [|_|]), [| String("1") |] -> true | _ -> false)


    let UCI2 (ty,i) = FSharpType.GetUnionCases(ty, showAll).[i]
    
    do test "qcqmkee32ao" (match FSharpValue.PreComputeUnionReader (UCI2(typeof<InternalUnionType1>, 1),showAll) (box (InternalXX ("1","2"))) with [| String("1");String("2") |] -> true | _ -> false)
    do test "qcqmkee32ap" (match FSharpValue.PreComputeUnionReader (UCI2(typeof<InternalUnionType1>, 0),showAll) (box (InternalX "1")) with [| String("1") |] -> true | _ -> false)
    do test "qcqmkee32aq" (match FSharpValue.PreComputeUnionReader (UCI2(typeof<InternalUnionType2>, 1),showAll) (box (InternalXX2 "1")) with [| String("1") |] -> true | _ -> false)
    do test "qcqmkee32ar" (match FSharpValue.PreComputeUnionReader (UCI2(typeof<InternalUnionType2>, 0),showAll) (box InternalX2) with [| |] -> true | _ -> false)

    do test "qcqmkee32ao1" (FSharpValue.PreComputeUnionConstructor(UCI2(typeof<InternalUnionType1>, 0), showAll) [| box "1" |] = box (InternalX "1"))
    do test "qcqmkee32ao2" (FSharpValue.PreComputeUnionConstructor(UCI2(typeof<InternalUnionType1>, 1), showAll) [| box "1"; box "2" |] = box (InternalXX ("1","2")))
    do test "qcqmkee32ao3" (FSharpValue.PreComputeUnionConstructor(UCI2(typeof<InternalUnionType2>, 0), showAll) [|  |] = box InternalX2)
    do test "qcqmkee32ao4" (FSharpValue.PreComputeUnionConstructor(UCI2(typeof<InternalUnionType2>, 1), showAll) [| box "1"; |] = box (InternalXX2 "1"))
    do test "qcqmkee32ao5" (FSharpValue.PreComputeUnionConstructor(UCI2(typeof<int InternalUnionType3>, 0), showAll) [|  |] = box (InternalX3 : int InternalUnionType3))
    do test "qcqmkee32ao6" (FSharpValue.PreComputeUnionConstructor(UCI2(typeof<string InternalUnionType3>, 1), showAll) [| box "1"; |] = box (InternalXX3 "1"))
    do test "qcqmkee32ao7" (FSharpValue.PreComputeRecordConstructor(typeof<InternalRecordType1>, showAll) [| box 1; |] = box {internal_r1a = 1 })
    do test "qcqmkee32ao8" (FSharpValue.PreComputeRecordConstructor(typeof<string InternalRecordType2>, showAll) [| box "1"; box 1 |] = box {internal_r2b = "1"; internal_r2a = 1 })

module TwoCasedUnionWithNullAsTrueValueAnnotation =
    [<Core.CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    type T =
        | A
        | B of string * bool
    let result =
        try
            let cases = Reflection.FSharpType.GetUnionCases(typeof<T>)
            let mkA = Reflection.FSharpValue.PreComputeUnionConstructor(cases.[0])
            let mkB = Reflection.FSharpValue.PreComputeUnionConstructor(cases.[1])
            (mkA [||] :?> _ = A) && (mkB [|"a"; true|] :?> _ = B("a", true))
        with _ -> false
    test "TwoCasedUnionWithNullAsTrueValueAnnotation" result

module TEst = 

  type token = 
   | X
   | DOUBLELITERAL  of (System.Double)
   | DECIMALLITERAL of (System.Decimal)
   | Y              of (int)
   | INTEGERLITERAL
   | VARNAME       
   | QNAME          of (string)   
  let tok2 = (X:token)
  let _   = printfn "%A" tok2


  let printany x = (printf "%A" x;stderr.Write "\n")
    
  let _ = printany (1    : int)
  let _ = printany (true : bool)
  let _ = printany (27.3 : float)
  let _ = printany "IAmAString with some spaces and quotes \"'"

  type rr = { rr_int : int; rr_strs : string list; rr_thunk : unit -> unit}
  let rrv =  {rr_int = 99;
      rr_strs = ["skdf";"kshkshfskhf"];
      rr_thunk = fun () -> ()}
  let _ = printany rrv
    
  let _ = printany ["Lists";"have";"special";"treatment";"hence";"this";"test";"case"]
  let _ = printany []
  let _ = printany (Some "options likewise")
  let _ = printany None

  let poly (x:'a) = printany (None : 'a option)
  let _ = poly 12
  let _ = poly true

  let _ = printany (1,true,2.4,"a tuple",("nested",(fun () -> ()),[2;3],rrv))
  let _ = printany printany (* =) *)


module DynamicCall = 
    open System
    open System.Reflection

    module Test =
        type Marker = class end

        let inline call (a : ^a) =
            (^a : (member Stuff : Unit -> Unit) a)


    let genericCallMethod = typeof<Test.Marker>.DeclaringType.GetMethod "call"
    let callMethod = genericCallMethod.MakeGenericMethod [| typeof<Object> |]

    try 
       callMethod.Invoke (null, [| Object () |]) |> ignore
       failwith "expected an exception"
    with :? TargetInvocationException as ex ->
        // The test should cause a NotSupportedException with the Message:
        //              "Dynamic invocation of %s is not supported"
        //              %s Will be Stuff Because thats the member that is not supported.
        test "wcnr0vj" (ex.InnerException.Message.Contains("Stuff") && ex.InnerException.GetType() = typeof<System.NotSupportedException>)

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

