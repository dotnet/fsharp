// #Quotations #Query
#if TESTS_AS_APP
module Core_queriesOverIQueryable
#endif

#nowarn "57"

open System
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape
open Microsoft.FSharp.Linq.RuntimeHelpers

[<AutoOpen>]
module Infrastructure =
    let failures = ref []

    let report_failure (s : string) = 
        stderr.Write" NO: "
        stderr.WriteLine s
        failures := !failures @ [s]

    let check  s v1 v2 = 
       if v1 = v2 then 
           printfn "test %s...passed " s 
       else 
           report_failure (sprintf "test %s...failed, expected \n\t%A\ngot\n\t%A" s v2 v1)

    let test s b = check s b true
    let qmap f (x:System.Linq.IQueryable<_>) = x |> Seq.map f |> System.Linq.Queryable.AsQueryable

    let checkCommuteSeq s (q1: System.Linq.IQueryable<'T>) q2 =
        check s (q1 |> Seq.toList) (q2 |> Seq.toList)

    let checkCommuteVal s q1 q2 =
        check s q1 q2

module QueryExecutionOverIQueryable =
    open System
    open Microsoft.FSharp.Linq    
    type Customer(name:string, data: int, cost:float, sizes: int list, quantity:Nullable<int>) = 
        member x.Name = name
        member x.Data = data
        member x.Cost = cost
        member x.Sizes = sizes
        member x.Quantity = quantity
        member x.SomeInt16Value = int16 (quantity.GetValueOrDefault())
        member x.SomeNullableInt16Value = Nullable<int16>(int16 (quantity.GetValueOrDefault()))
        member x.AlwaysNull = Nullable<int>()
    let c1 = Customer( name="Don", data=6, cost=6.2, sizes=[1;2;3;4], quantity=Nullable())
    let c2 = Customer( name="Peter", data=7, cost=4.2, sizes=[10;20;30;40], quantity=Nullable(10))
    let c3 = Customer( name="Freddy", data=8, cost=9.2, sizes=[11;12;13;14], quantity=Nullable())
    let c4 = Customer( name="Freddi", data=10, cost=1.0, sizes=[21;22;23;24], quantity=Nullable(32))
    let c5 = Customer( name="Don", data=9, cost=1.0, sizes=[21;22;23;24], quantity=Nullable())
    // Not in the database
    let c6 = Customer( name="Bob", data=9, cost=1.0, sizes=[21;22;23;24],quantity=Nullable())
    
    let data = [c1;c2;c3;c4;c5]
    let db = System.Linq.Queryable.AsQueryable<Customer>(data |> List.toSeq)

    let dbEmpty = System.Linq.Queryable.AsQueryable<int>([] |> List.toSeq)
    let dbOne = System.Linq.Queryable.AsQueryable<int>([1] |> List.toSeq)

    let checkLinqQueryText s (q1: System.Linq.IQueryable<'T>) text =
        check s (try q1.Expression.ToString().Replace(db.ToString(),"db").Replace("QueryExecutionOverIQueryable.db","db") with e -> "Unexpected error: " + e.ToString()) text


    checkCommuteSeq "cnewnc01" 
        (query { yield! db }) 
        db

    checkCommuteSeq "cnewnc01nested" 
        (query { for v in query { yield! db } do yield v }) 
        db


    checkCommuteSeq "cnewnc02" 
        (query { for i in db -> i }) 
        db

    checkCommuteSeq "cnewnc02nested" 
        (query { for v in query { for i in db -> i } do yield v }) 
        db

    checkCommuteSeq "cnewnc02nested2" 
        (query { let q = query { for i in db -> i } in for v in q do yield v }) 
        db

    checkCommuteSeq "cnewnc03" 
        (query { for i in db -> i.Name }) 
        (seq { for i in db -> i.Name })

    checkLinqQueryText "ltcjhnwec2" 
        (query { for i in db -> i })  
        "db.Select(_arg1 => _arg1)"

    checkLinqQueryText "ltcjhnwec5" 
        (query { for i in db -> i.Name })  
        "db.Select(_arg1 => _arg1.Name)"

    checkLinqQueryText "ltcjhnwec7" 
        (query { for i in db do for j in db do yield i.Name  })   
        "db.SelectMany(_arg1 => db, (_arg1, _arg2) => _arg1.Name)"

    checkLinqQueryText "ltcjhnwec0" 
        (query { for i in db do for j in db do if i.Data = j.Data then yield i.Name })  
        "db.SelectMany(_arg1 => db.Where(_arg2 => (_arg1.Data == _arg2.Data)), (_arg1, _arg2) => _arg1.Name)"

    checkLinqQueryText "ltcjhnwecq"  
        (query { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield i.Name })  
        "db.Where(_arg1 => (_arg1.Data == 8)).SelectMany(_arg1 => db.Where(_arg2 => (_arg1.Data == _arg2.Data)), (_arg1, _arg2) => _arg1.Name)"
         




    type QB2() = 
        inherit Microsoft.FSharp.Linq.QueryBuilder()
        member x.Run(e:Expr<'T>) = e
   
        
    checkLinqQueryText "ltcjhnwecw" 
        (query { let q = query { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield i.Name } in for v in q do yield v })  
        "db.Where(_arg1 => (_arg1.Data == 8)).SelectMany(_arg1 => db.Where(_arg2 => (_arg1.Data == _arg2.Data)), (_arg1, _arg2) => _arg1.Name).Select(_arg3 => _arg3)"

    checkLinqQueryText "ltcjhnwec8" 
        (query { let q = query { for i in db do for j in db do yield i.Name } in for v in q do yield v })   
        "db.SelectMany(_arg1 => db, (_arg1, _arg2) => _arg1.Name).Select(_arg3 => _arg3)"

    checkLinqQueryText "ltcjhnwec9" 
        (query { let q = query { for i in db do for j in db do if i.Data = j.Data then yield i.Name } in for v in q do yield v })  
        "db.SelectMany(_arg1 => db.Where(_arg2 => (_arg1.Data == _arg2.Data)), (_arg1, _arg2) => _arg1.Name).Select(_arg3 => _arg3)"

    checkLinqQueryText "ltcjhnwec3" 
        (query { for v in query { for i in db -> i } do yield v })  
        "db.Select(_arg1 => _arg1).Select(_arg2 => _arg2)"

    checkLinqQueryText "ltcjhnwec4" 
        (query { let q = query { for i in db -> i } in for v in q do yield v })  
        "db.Select(_arg1 => _arg1).Select(_arg2 => _arg2)"

    checkLinqQueryText "ltcjhnwec6" 
        (query { let q = query { for i in db -> i.Name } in for v in q do yield v } )  
        "db.Select(_arg1 => _arg1.Name).Select(_arg2 => _arg2)"

    open FSharp.Reflection

    let t = typeof< {| Name1: string; Name2: string |} >
    check "wkcwe09" (FSharpType.IsRecord t) true
    check "wkcwe09" (FSharpType.GetRecordFields t |> Array.forall (fun f -> f.CanWrite)) false

//    checkLinqQueryText "ltcjhnwec6" 
//        (query { for i in db -> {| Name1 = i.Name; Name2 = i.Name |} } )  
//        "db.Select(_arg1 => _arg1.Name).Select(_arg2 => _arg2)"
//        "System.Linq.Enumerable+WhereSelectEnumerableIterator`2[Microsoft.FSharp.Linq.RuntimeHelpers.AnonymousObject`2[System.String,System.String],<>f__AnonymousType3691853213`2'[System.String,System.String]].Select(_arg2 => _arg2)"

    checkCommuteSeq "cnewnc03nested" 
        (query { let q = query { for i in db -> i.Name } in for v in q do yield v } ) 
        (seq { for i in db -> i.Name })

    checkCommuteSeq "cnewnc06y" 
        (query { for i in db do for j in db do yield (i.Name,j.Name)  })  
        (seq   { for i in db do for j in db do yield (i.Name,j.Name)  })

    checkCommuteSeq "cnewnc06y" 
        (query { for i in db do for j in db do yield {| Name1 = i.Name; Name2 = j.Name |}  })  
        (seq   { for i in db do for j in db do yield {| Name1 = i.Name; Name2 = j.Name |}  })

    checkCommuteSeq "cnewnc06ynested" 
        (query { let q = query { for i in db do for j in db do yield (i.Name,j.Name) } in for v in q do yield v })  
        (seq   { for i in db do for j in db do yield (i.Name,j.Name)  })

    checkCommuteSeq "cnewnc06bnested" 
        (query { let q = query { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) } in for v in q do yield v }) 
        (seq   { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "cnewnc06b" 
        (query { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) }) 
        (seq   { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "cnewnc06w" 
        (query { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) }) 
        (seq   { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "cnewnc06wnested2" 
        (query { let q = query { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) } in for v in q do yield v }) 
        (seq   { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "cnewnc06ys" 
        (query { for i in db do for j in db do select (i.Name,j.Name)  })  
        (seq   { for i in db do for j in db do yield (i.Name,j.Name)  })

    checkCommuteSeq "cnewnc06ynesteds" 
        (query { let q = query { for i in db do for j in db do select (i.Name,j.Name) } in for v in q do select v })  
        (seq   { for i in db do for j in db do yield (i.Name,j.Name)  })

    checkCommuteSeq "cnewnc06bnesteds" 
        (query { let q = query { for i in db do for j in db do if i.Data = j.Data then select (i.Data,i.Name,j.Name) } in for v in q do select v }) 
        (seq   { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "cnewnc06bs" 
        (query { for i in db do for j in db do if i.Data = j.Data then select (i.Data,i.Name,j.Name) }) 
        (seq   { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "cnewnc06ws" 
        (query { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then select (i.Data,i.Name,j.Name) }) 
        (seq   { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "cnewnc06wnested2s" 
        (query { let q = query { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then select (i.Data,i.Name,j.Name) } in for v in q do select v }) 
        (seq   { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkLinqQueryText "ltcjhnwecs" 
        (query { for i in db do take 3 })  
        "db.Take(3)"

    checkLinqQueryText "ltcjhnwecd" 
        (query { for i in db do where true; take 3 })  
        "db.Where(i => True).Take(3)"

    checkLinqQueryText "ltcjhnwecf" 
        (query { for i in db do for j in db do where true; take 3; select i })  
        "db.SelectMany(_arg1 => db, (_arg1, _arg2) => new AnonymousObject`2(Item1 = _arg1, Item2 = _arg2)).Where(tupledArg => True).Take(3).Select(tupledArg => tupledArg.Item1)"

    checkLinqQueryText "ltcjhnwecg" 
        (query { let q = query { for i in db do for j in db do where true; take 3; select i } in for v in q do yield v } )  
        "db.SelectMany(_arg1 => db, (_arg1, _arg2) => new AnonymousObject`2(Item1 = _arg1, Item2 = _arg2)).Where(tupledArg => True).Take(3).Select(tupledArg => tupledArg.Item1).Select(_arg3 => _arg3)"



    checkCommuteSeq "cnewnc06z" 
        (query { for i in db do take 3 }) 
        (seq   { for i in db do yield i } |> Seq.take 3)

    checkCommuteSeq "cnewnc06x" 
        (query { for i in db do where true; take 3 }) 
        (seq   { for i in db do yield i } |> Seq.take 3)

    
    checkCommuteSeq "cnewnc06xb" 
        (query { for i in db do for j in db do where true; take 3 }) 
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.take 3)

    checkCommuteSeq "cnewnc06xbnested" 
        (query { let q = query { for i in db do for j in db do where true; take 3 } in for v in q do yield v } ) 
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.take 3)

    checkLinqQueryText "ltcjhnwech" 
        (query { for i in db do select i })  
        "db.Select(i => i)"

    checkLinqQueryText "ltcjhnweck" 
        (query { for i in db do for j in db do select i })  
        "db.SelectMany(_arg1 => db, (_arg1, _arg2) => new AnonymousObject`2(Item1 = _arg1, Item2 = _arg2)).Select(tupledArg => tupledArg.Item1)"

    checkLinqQueryText "ltcjhnwecl" 
        (query { for i in db do select i.Name into n; distinct }) 
        "db.Select(i => i.Name).Distinct()"

    checkLinqQueryText "ltcjhnwecz" 
        (query { let q = query { for i in db do select i.Name into n; distinct } in for v in q do yield v }) 
        "db.Select(i => i.Name).Distinct().Select(_arg3 => _arg3)"

    checkCommuteSeq "cnewnc06ya" 
        (query { for i in db do select i }) 

        (seq   { for i in db do yield i })

    checkCommuteSeq "cnewnc06yab" 
        (query { for i in db do for j in db do select i }) 
        (seq   { for i in db do for j in db do yield i })

    checkCommuteSeq "cnewnc06ya3" 
        (query { for i in db do 
                 select i.Name into n 
                 distinct }) 
        ["Don"; "Peter"; "Freddy"; "Freddi"]


    checkCommuteSeq "cnewnc06ya3nested" 
        (query { let q = query { for i in db do 
                                 select i.Name into n 
                                 distinct } 
                 for v in q do yield v }) 
        ["Don"; "Peter"; "Freddy"; "Freddi"]

    checkCommuteSeq "cnewnc06ya3b" 
        (query { for i in db do 
                  select i.Name
                  distinct }) 
        ["Don"; "Peter"; "Freddy"; "Freddi"]

    checkCommuteSeq "cnewnc06ya3b" 
        (query { for i in db do 
                  select i.Name
                  distinct 
                  take 2 }) 
        ["Don"; "Peter"]

    checkLinqQueryText "ltcjhnwecd" 
        (query { for i in db do where true; take 3 })  
        "db.Where(i => True).Take(3)"

    checkCommuteSeq "cnewnc06yb" 
        (query { for i in db do groupBy i.Name } |> Seq.map (fun g -> (g.Key,Seq.toList g)) |> System.Linq.Queryable.AsQueryable)
        (seq   { for i in db do yield i } |> Seq.groupBy (fun i -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))


    checkCommuteSeq "cnewnc06ybx" 
        (query { for i in db do groupValBy i i.Name } |> Seq.map (fun g -> (g.Key,Seq.toList g)) |> System.Linq.Queryable.AsQueryable)
        (seq   { for i in db do yield i } |> Seq.groupBy (fun i -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "cnewnc06yb2" 
        (query { for i in db do for j in db do groupBy i.Name } |> Seq.map (fun g -> (g.Key,Seq.toList g)) |> System.Linq.Queryable.AsQueryable)
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.groupBy (fun (i,j) -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))
    checkCommuteSeq "cnewnc06yb2xff" 
        (query { for i in db do groupValBy (i,i) i.Name } |> Seq.map (fun g -> (g.Key,Seq.toList g)) |> System.Linq.Queryable.AsQueryable)
        (seq   { for i in db do yield (i,i) } |> Seq.groupBy (fun (i,j) -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))


    checkCommuteSeq "cnewnc06yb2x" 
        (query { for i in db do for j in db do groupValBy (i,j) i.Name } |> Seq.map (fun g -> (g.Key,Seq.toList g)) |> System.Linq.Queryable.AsQueryable)
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.groupBy (fun (i,j) -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "cnewnc06yc" 
        (query { for i in db do sortBy i.Name } )
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.toList)

    checkCommuteSeq "cnewnc06yd" 
        (query { for i in db do sortBy i.Name; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.toList)

    checkCommuteSeq "cnewnc06yds" 
        (query { for i in db do sortBy i.Name; select i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.map (fun i -> i) |> Seq.toList)

    checkCommuteSeq "cnewnc06ydx" 
        (query { for i in db do sortBy i.Name; yield (i,i) } )
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.map (fun i -> i,i) |> Seq.toList)

    checkCommuteSeq "cnewnc06ydxs" 
        (query { for i in db do sortBy i.Name; select (i,i) } )
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.map (fun i -> i,i) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq" 
        (query { for i in db do sortByDescending i.Data; yield i } )
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Data) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yqs" 
        (query { for i in db do sortByDescending i.Data; select i } )
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Data) |> Seq.toList |> List.rev)
                
    checkCommuteSeq "cnewnc06yqx" 
        (query { for i in db do sortByDescending i.Data; yield (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Data) |> Seq.map (fun i -> (i,i)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yqxs" 
        (query { for i in db do sortByDescending i.Data; select (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Data) |> Seq.map (fun i -> (i,i)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq3" 
        (query { for i in db do sortBy i.Name; thenBy i.Data; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq3s" 
        (query { for i in db do sortBy i.Name; thenBy i.Data; select i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq3x" 
        (query { for i in db do sortBy i.Name; thenBy i.Data; yield (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.map (fun i -> (i,i)) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq3xs" 
        (query { for i in db do sortBy i.Name; thenBy i.Data; select (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.map (fun i -> (i,i)) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq4" 
        (query { for i in db do sortByDescending i.Name; thenBy i.Data; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq4s" 
        (query { for i in db do sortByDescending i.Name; thenBy i.Data; select i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq4x" 
        (query { for i in db do sortByDescending i.Name; thenBy i.Data; yield (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.map (fun i-> (i,i)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq4xs" 
        (query { for i in db do sortByDescending i.Name; thenBy i.Data; select (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.map (fun i-> (i,i)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq5" 
        (query { for i in db do sortByDescending i.Name; thenByDescending i.Data; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq5s" 
        (query { for i in db do sortByDescending i.Name; thenByDescending i.Data; select i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq5x" 
        (query { for i in db do sortByDescending i.Name; thenByDescending i.Data; yield (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.map (fun i -> (i,i)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq5xs" 
        (query { for i in db do sortByDescending i.Name; thenByDescending i.Data; select (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.map (fun i -> (i,i)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq6" 
        (query { for i in db do sortBy i.Name; thenByDescending i.Data; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq6s" 
        (query { for i in db do sortBy i.Name; thenByDescending i.Data; select i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq6b" 
        (query { for i in db do sortByNullable i.Quantity; yield i } |> Seq.map (fun x -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6bs" 
        (query { for i in db do sortByNullable i.Quantity; select i } |> Seq.map (fun x -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6b" 
        (query { for i in db do sortByNullable i.Quantity; yield (i,i) } |> Seq.map (fun (x,_) -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6bs" 
        (query { for i in db do sortByNullable i.Quantity; select (i,i) } |> Seq.map (fun (x,_) -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6c" 
        (query { for i in db do sortByNullable i.Quantity; thenByNullable i.AlwaysNull; yield i } |> Seq.map (fun x -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6cs" 
        (query { for i in db do sortByNullable i.Quantity; thenByNullable i.AlwaysNull; select i } |> Seq.map (fun x -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6cx" 
        (query { for i in db do sortByNullable i.Quantity; thenByNullable i.AlwaysNull; yield (i,i) } |> Seq.map (fun (x,_) -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6cxs" 
        (query { for i in db do sortByNullable i.Quantity; thenByNullable i.AlwaysNull; select (i,i) } |> Seq.map (fun (x,_) -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6c" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullable i.AlwaysNull; yield i } |> Seq.map (fun x -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "cnewnc06yq6cs" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullable i.AlwaysNull; select i } |> Seq.map (fun x -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "cnewnc06yq6cx" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullable i.AlwaysNull; yield (i,i) } |> Seq.map (fun (x,_) -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "cnewnc06yq6cxs" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullable i.AlwaysNull; select (i,i) } |> Seq.map (fun (x,_) -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "cnewnc06yq6c" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullableDescending i.AlwaysNull; yield i } |> Seq.map (fun x -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "cnewnc06yq6cs" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullableDescending i.AlwaysNull; select i } |> Seq.map (fun x -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "cnewnc06yq6cx" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullableDescending i.AlwaysNull; yield (i,i) } |> Seq.map (fun (x,_) -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "cnewnc06yq6cxs" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullableDescending i.AlwaysNull; select (i,i) } |> Seq.map (fun (x,_) -> x.Name )|> System.Linq.Queryable.AsQueryable)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    check "cnewnc06ye"  
        (query { for i in db do contains c1 }) 
        true

    check "cnewnc06yex"  
        (query { for i in db do for j in db do contains (c1,c1) }) 
        true

    check "cnewnc06ye"  
        (query { for i in dbEmpty do contains 1 }) 
        false

    check "cnewnc06yeg"  
        (query { for i in dbOne do contains 1 }) 
        true

    check "cnewnc06yf" 
        (query { for i in db do contains c6 }) 
        false

    check "cnewnc06yg" 
        (query { for i in db do count }) 
        5

    check "cnewnc06yh" 
        (query { for i in db do last }) 
        c5

    check "cnewnc06yh" 
        (query { for i in db do for j in db do last }) 
        (c5,c5)

    check "cnewnc06yh2a" 
        (query { for i in db do lastOrDefault }) 
        c5

    check "cnewnc06yh2a" 
        (query { for i in db do for j in db do lastOrDefault }) 
        (c5,c5)

    check "cnewnc06yh2b" 
        (query { for i in db do headOrDefault }) 
        c1

    check "cnewnc06yh2bx" 
        (query { for i in db do for j in db do headOrDefault }) 
        (c1,c1)

    check "cnewnc06yh3a" 
        (query { for i in dbEmpty do lastOrDefault }) 
        0

    check "cnewnc06yh3b" 
        (query { for i in dbEmpty do headOrDefault }) 
        0

    check "cnewnc06yh4" 
        (query { for i in dbOne do exactlyOne }) 
        1

    check "cnewnc06yh4x" 
        (query { for i in dbOne do for j in dbOne do exactlyOne }) 
        (1,1)

    check "cnewnc06yh5" 
        (query { for i in dbOne do exactlyOneOrDefault }) 
        1

    check "cnewnc06yh5x" 
        (query { for i in dbOne do for j in dbOne do exactlyOneOrDefault }) 
        (1,1)

    
    check "cnewnc06yh6" 
        (try 
            query { for i in dbEmpty do exactlyOne } |> ignore; false 
         with :? System.InvalidOperationException -> true) 
        true

    check "cnewnc06yh7" 
        (query { for i in dbEmpty do exactlyOneOrDefault }) 
        0

    check "cnewnc06yh8" 
        (query { for i in dbOne do minBy i }) 
        1

    check "cnewnc06yh8x" 
        (query { for i in dbOne do for j in dbOne do minBy i }) 
        1

    check "cnewnc06yh9" 
        (query { for i in db do minBy i.Data }) 
        c1.Data


    check "cnewnc06yh9x" 
        (query { for i in db do for j in db do minBy i.Data }) 
        c1.Data

    
    check "cnewnc06yh9" 
        (try query { for i in dbEmpty do minBy i } with :? System.InvalidOperationException -> -10) 
        -10

    check "cnewnc06yh9xff" 
        (try query { for i in dbEmpty do nth 3 } with :? System.ArgumentOutOfRangeException -> -10) 
        -10

    check "cnewnc06yh9xx" 
        (query { for i in dbEmpty do skip 1 } |> Seq.length |> ignore; 10)
        10


    check "cnewnc06yh10" 
        (query { for i in dbOne do maxBy i }) 
        1

    check "cnewnc06yh11" 
        (query { for i in db do maxBy i.Data }) 
        c4.Data
    
    check "cnewnc06yh11" 
        (query { for i in db do for j in db do maxBy i.Data }) 
        c4.Data
    
    check "cnewnc06yh12" 
        (try query { for i in dbEmpty do maxBy i } with :? System.InvalidOperationException -> -10) 
        -10

    check "cnewnc06yh81" 
        (query { for i in dbOne do sumBy i }) 
        1

    check "cnewnc06yh81x" 
        (query { for i in dbOne do for j in dbOne do sumBy i }) 
        1

    check "cnewnc06yh82" 
        (query { for i in dbEmpty do sumBy i }) 
        0

    check "cnewnc06yh82x" 
        (query { for i in dbEmpty do for j in dbEmpty do sumBy i }) 
        0

    check "cnewnc06yh81b" 
        (query { for i in dbOne do averageBy (float i) }) 
        1.0
    
    check "cnewnc06yh81bx" 
        (query { for i in dbOne do for j in dbOne do averageBy (float i) }) 
        1.0
    

    check "cnewnc06yh81c" 
        (query { for i in dbOne do averageBy (float32 i) }) 
        1.0f


    check "cnewnc06yh81cx" 
        (query { for i in dbOne do for j in dbOne do averageBy (float32 i) }) 
        1.0f

    check "cnewnc06yh81d" 
        (query { for i in dbOne do averageBy (decimal i) }) 
        1.0M

    check "cnewnc06yh81e" 
        (query { for i in dbOne do sumBy (int64 i) }) 
        1L

    check "cnewnc06yh81f" 
        (query { for i in dbOne do sumBy (decimal i) }) 
        1.0M

    check "cnewnc06yh81g" 
        (query { for i in dbOne do sumBy (int32 i) }) 
        1

    check "cnewnc06yh81h" 
        (query { for i in dbOne do sumBy (float i) }) 
        1.0

    check "cnewnc06yh81h" 
        (query { for i in dbOne do sumBy (float32 i) }) 
        1.0f

    
    check "cnewnc06yh9" 
        (try query { for i in dbEmpty do averageBy (float i) } with :? System.InvalidOperationException -> -10.0) 
        -10.0

    check "cnewnc06yh91" 
        (query { for i in db do sumByNullable i.Quantity }) 
        (Nullable 42)

    check "cnewnc06yh92" 
        (query { for i in db do averageByNullable (Nullable(10.0)) }) 
        (Nullable 10.0)

    check "cnewnc06yh92" 
        (query { for i in db do maxByNullable i.Quantity }) 
        (Nullable 32)

    check "cnewnc06yh93" 
        (query { for i in db do minByNullable i.Quantity }) 
        (Nullable 10)

    check "cnewnc06yh94" 
        (query { for i in dbEmpty do sumByNullable (Nullable(i)) }) 
        (Nullable 0)

    check "cnewnc06yh95" 
        (query { for i in dbEmpty do averageByNullable (Nullable(float i)) }) 
        (Nullable())

    check "cnewnc06yh96" 
        (query { for i in dbEmpty do maxByNullable (Nullable i) }) 
        (Nullable())

    check "cnewnc06yh97" 
        (query { for i in dbEmpty do minByNullable (Nullable i) }) 
        (Nullable())

    check "cnewnc06yh91" 
        (query { for i in db do sumByNullable i.Quantity }) 
        (Nullable 42)

    check "cnewnc06yh94" 
        (query { for i in dbEmpty do sumByNullable (Nullable(i)) }) 
        (Nullable 0)

    check "cnewnc06yh94b" 
        (query { for i in dbEmpty do sumByNullable (Nullable(float32 i)) }) 
        (Nullable 0.0f)

    check "cnewnc06yh94c" 
        (query { for i in dbEmpty do sumByNullable (Nullable(decimal i)) }) 
        (Nullable 0.0M)

    check "cnewnc06yh94d" 
        (query { for i in dbEmpty do sumByNullable (Nullable(int64 i)) }) 
        (Nullable 0L)

    check "cnewnc06yh94e" 
        (query { for i in dbEmpty do sumByNullable (Nullable(float i)) }) 
        (Nullable 0.0)

    check "cnewnc06yh98" 
        (query { for i in db do sumByNullable i.AlwaysNull }) 
        (Nullable(0))

    check "cnewnc06yh96" 
        (query { for i in dbEmpty do maxByNullable (Nullable i) }) 
        (Nullable())

    check "cnewnc06yh96b" 
        (query { for i in dbEmpty do maxByNullable (Nullable (float i)) }) 
        (Nullable())

    check "cnewnc06yh96c" 
        (query { for i in dbEmpty do maxByNullable (Nullable (decimal i)) }) 
        (Nullable())

    check "cnewnc06yh96d" 
        (query { for i in dbEmpty do maxByNullable (Nullable (float32 i)) }) 
        (Nullable())

    check "cnewnc06yh96e" 
        (query { for i in dbEmpty do maxByNullable (Nullable (int64 i)) }) 
        (Nullable())

    check "cnewnc06yh991" 
        (query { for i in db do maxByNullable i.AlwaysNull }) 
        (Nullable())

    check "cnewnc06yh9Q" 
        (query { for i in db do minByNullable i.AlwaysNull }) 
        (Nullable())

    check "cnewnc06yh992" 
        (query { for i in db do all (i.Name = "Don") }) 
        false

    check "cnewnc06yh993" 
        (query { for i in db do exists (i.Name = "Don") }) 
        true

    check "cnewnc06yh994" 
        (query { for i in dbEmpty do all (i = 1) }) 
        true

    check "cnewnc06yh995" 
        (query { for i in dbEmpty do exists (i = 1) }) 
        false

    check "cnewnc06yh996" 
        (query { for i in db do find (i.Name = "Peter") }) 
        c2

    check "cnewnc06yh997" 
        (query { for i in db do skip 1; select i.Name} |> Seq.toList) 
        ["Peter"; "Freddy"; "Freddi"; "Don"]

    check "cnewnc06yh998" 
        (query { for i in db do skipWhile (i.Name = "Don"); select i.Name} |> Seq.toList) 
        ["Peter"; "Freddy"; "Freddi"; "Don"]

    check "cnewnc06yh999" 
        (query { for i in db do take 2; select i.Name} |> Seq.toList) 
        ["Don"; "Peter"]

    check "cnewnc06yh990" 
        (query { for i in db do takeWhile (i.Name = "Don" || i.Name = "Peter"); select i.Name} |> Seq.toList) 
        ["Don"; "Peter"]

    check "cnewnc06yh99q" 
        (query { for i in db do nth 0 }) 
        c1

    check "cnewnc06yh9Q1" 
        (query { for i in db do 
                 join j in db on (i.Name = j.Name)
                 select (i.Cost, j.Cost) } |> Seq.toList) 
        [(6.2, 6.2); (6.2, 1.0); (4.2, 4.2); (9.2, 9.2); (1.0, 1.0); (1.0, 6.2); (1.0, 1.0)]


    check "cnewnc06yh9Q1b" 
        (query { for i in db do 
                 let icost = i.Cost + 1.0 - 1.0
                 join j in db on (i.Name = j.Name)
                 select (icost, j.Cost) } |> Seq.toList) 
        [(6.2, 6.2); (6.2, 1.0); (4.2, 4.2); (9.2, 9.2); (1.0, 1.0); (1.0, 6.2); (1.0, 1.0)]

    check "cnewnc06yh9Q1c" 
        (query { for i in db do 
                 let icost1 = i.Cost + 1.0 
                 let icost = icost1 - 1.0
                 join j in db on (i.Name = j.Name)
                 select (icost, j.Cost) } |> Seq.toList) 
        [(6.2, 6.2); (6.2, 1.0); (4.2, 4.2); (9.2, 9.2); (1.0, 1.0); (1.0, 6.2); (1.0, 1.0)]

    check "cnewnc06yh9Q2" 
        (query { for i in db do 
                 join j in db on (i.Name = j.Name)
                 yield (i.Cost, j.Cost) } |> Seq.toList) 
        [(6.2, 6.2); (6.2, 1.0); (4.2, 4.2); (9.2, 9.2); (1.0, 1.0); (1.0, 6.2); (1.0, 1.0)]

    check "cnewnc06yh9Q2s" 
        (query { for i in db do 
                 join j in db on (i.Name = j.Name)
                 select (i.Cost, j.Cost) } |> Seq.toList) 
        [(6.2, 6.2); (6.2, 1.0); (4.2, 4.2); (9.2, 9.2); (1.0, 1.0); (1.0, 6.2); (1.0, 1.0)]

    check "cnewnc06yh9Q3" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?= j.Quantity.GetValueOrDefault())
                 yield (i.Cost, j.Cost) } |> Seq.toList) 
        [(4.2, 4.2); (1.0, 1.0)]

    check "cnewnc06yh9Q3s" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?= j.Quantity.GetValueOrDefault())
                 select (i.Cost, j.Cost) } |> Seq.toList) 
        [(4.2, 4.2); (1.0, 1.0)]

    check "cnewnc06yh9Q4" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?=? j.Quantity)
                 yield (i.Cost, j.Cost) } |> Seq.toList) 
        [(4.2, 4.2); (1.0, 1.0)]

    check "cnewnc06yh9Q4" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?=? j.Quantity)
                 select (i.Cost, j.Cost) } |> Seq.toList) 
        [(4.2, 4.2); (1.0, 1.0)]



    checkLinqQueryText "ltcjhnwecd23" 
        (query { for i in db do where true; take 3 })  
        "db.Where(i => True).Take(3)"

    checkLinqQueryText "ltcnewnc06yb" 
        (query { for i in db do groupBy i.Name })
        "db.GroupBy(i => i.Name)"

    checkLinqQueryText "ltcnewnc06ybx" 
        (query { for i in db do groupValBy i i.Name })
        "db.GroupBy(i => i.Name, i => i)"

    checkLinqQueryText "ltcnewnc06yb2x" 
        (query { for i in db do for j in db do groupValBy j i.Name })
        "db.SelectMany(_arg1 => db, (_arg1, _arg2) => new AnonymousObject`2(Item1 = _arg1, Item2 = _arg2)).GroupBy(tupledArg => tupledArg.Item1.Name, tupledArg => tupledArg.Item2)"

    checkLinqQueryText "ltcnewnc06yc" 
        (query { for i in db do sortBy i.Name })
        "db.OrderBy(i => i.Name)"

    checkLinqQueryText "ltcnewnc06yd" 
        (query { for i in db do sortBy i.Name; yield i })
        "db.OrderBy(i => i.Name)"

    checkLinqQueryText "ltcnewnc06yq" 
        (query { for i in db do sortByDescending i.Data; yield i })
        "db.OrderByDescending(i => i.Data)"

    checkLinqQueryText "ltcnewnc06yq3" 
        (query { for i in db do sortBy i.Name; thenBy i.Data; yield i })
        "db.OrderBy(i => i.Name).ThenBy(i => i.Data)"

    checkLinqQueryText "ltcnewnc06yq4" 
        (query { for i in db do sortByDescending i.Name; thenBy i.Data; yield i })
        "db.OrderByDescending(i => i.Name).ThenBy(i => i.Data)"

    checkLinqQueryText "ltcnewnc06yq5" 
        (query { for i in db do sortByDescending i.Name; thenByDescending i.Data; yield i })
        "db.OrderByDescending(i => i.Name).ThenByDescending(i => i.Data)"

    checkLinqQueryText "ltcnewnc06yq6" 
        (query { for i in db do sortBy i.Name; thenByDescending i.Data; yield i })
        "db.OrderBy(i => i.Name).ThenByDescending(i => i.Data)"

    checkLinqQueryText "ltcnewnc06yq6b" 
        (query { for i in db do sortByNullable i.Quantity; yield i })
        "db.OrderBy(i => i.Quantity)"

    checkLinqQueryText "ltcnewnc06yq6c" 
        (query { for i in db do sortByNullable i.Quantity; thenByNullable i.AlwaysNull; yield i })
        "db.OrderBy(i => i.Quantity).ThenBy(i => i.AlwaysNull)"

    checkLinqQueryText "ltcnewnc06yq6c" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullable i.AlwaysNull; yield i })
        "db.OrderByDescending(i => i.Quantity).ThenBy(i => i.AlwaysNull)"

    checkLinqQueryText "ltcnewnc06yq6c2" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullableDescending i.AlwaysNull; yield i })
        "db.OrderByDescending(i => i.Quantity).ThenByDescending(i => i.AlwaysNull)"


    checkLinqQueryText "lqtcnewnc06yh9Q1" 
        (query { for i in db do 
                 join j in db on (i.Name = j.Name)
                 select (i.Cost + j.Cost) }) 
        "db.Join(db, i => i.Name, j => j.Name, (i, j) => new AnonymousObject`2(Item1 = i, Item2 = j)).Select(_arg1 => new AnonymousObject`2(Item1 = _arg1.Item1, Item2 = _arg1.Item2)).Select(tupledArg => (tupledArg.Item1.Cost + tupledArg.Item2.Cost))"


    checkLinqQueryText "lqtcnewnc06yh9Q1b" 
        (query { for i in db do 
                 let icost = i.Cost
                 join j in db on (i.Name = j.Name)
                 select (icost + j.Cost) }) 
          "db.Select(_arg1 => new AnonymousObject`2(Item1 = _arg1, Item2 = _arg1.Cost)).Join(db, tupledArg => tupledArg.Item1.Name, j => j.Name, (tupledArg, j) => new AnonymousObject`3(Item1 = tupledArg.Item1, Item2 = tupledArg.Item2, Item3 = j)).Select(_arg2 => new AnonymousObject`3(Item1 = _arg2.Item1, Item2 = _arg2.Item2, Item3 = _arg2.Item3)).Select(tupledArg => (tupledArg.Item2 + tupledArg.Item3.Cost))",
  
//"db.Join(db, i => i.Name, j => j.Name, (i, j) => new MutableTuple`2() {Item1 = i, Item2 = j}).Select(_arg => new MutableTuple`2() {Item1 = _arg.Item1, Item2 = _arg.Item2}).Select(tupledArg => (tupledArg.Item1.Cost + tupledArg.Item2.Cost))"


    checkLinqQueryText "lqtcnewnc06yh9Q2" 
        (query { for i in db do 
                 join j in db on (i.Name = j.Name)
                 yield (i.Cost + j.Cost) }) 
        "db.Join(db, i => i.Name, j => j.Name, (i, j) => new AnonymousObject`2(Item1 = i, Item2 = j)).Select(_arg3 => (_arg3.Item1.Cost + _arg3.Item2.Cost))"

    checkLinqQueryText "lqtcnewnc06yh9Q3" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?= j.Quantity.GetValueOrDefault())
                 yield (i.Cost + j.Cost) }) 
        "db.Join(db, i => i.Quantity, j => Convert(j.Quantity.GetValueOrDefault()), (i, j) => new AnonymousObject`2(Item1 = i, Item2 = j)).Select(_arg1 => (_arg1.Item1.Cost + _arg1.Item2.Cost))"

    checkLinqQueryText "lqtcnewnc06yh9Q4" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?=? j.Quantity)
                 yield (i.Cost + j.Cost) }) 
        "db.Join(db, i => i.Quantity, j => j.Quantity, (i, j) => new AnonymousObject`2(Item1 = i, Item2 = j)).Select(_arg1 => (_arg1.Item1.Cost + _arg1.Item2.Cost))"

    checkLinqQueryText "ltcnewnc06yh9Q5" 
        (query { for i in db do 
                 groupJoin j in db on (i.Name = j.Name) into group
                 yield group } ) 
        "db.GroupJoin(db, i => i.Name, j => j.Name, (i, group) => new AnonymousObject`2(Item1 = i, Item2 = group)).Select(_arg1 => _arg1.Item2)"

    checkLinqQueryText "ltcnewnc06yh9Q5b" 
        (query { for i in db do 
                 let iname = i.Name
                 groupJoin j in db on (iname = j.Name) into group
                 yield group } ) 
          "db.Select(_arg1 => new AnonymousObject`2(Item1 = _arg1, Item2 = _arg1.Name)).GroupJoin(db, tupledArg => tupledArg.Item2, j => j.Name, (tupledArg, group) => new AnonymousObject`3(Item1 = tupledArg.Item1, Item2 = tupledArg.Item2, Item3 = group)).Select(_arg2 => _arg2.Item3)"

    checkLinqQueryText "ltcnewnc06yh9Q6" 
        (query { for i in db do 
                 groupJoin j in db on (i.Quantity ?= j.Quantity.GetValueOrDefault()) into group
                 yield group  } ) 
        "db.GroupJoin(db, i => i.Quantity, j => Convert(j.Quantity.GetValueOrDefault()), (i, group) => new AnonymousObject`2(Item1 = i, Item2 = group)).Select(_arg1 => _arg1.Item2)"

    checkLinqQueryText "ltcnewnc06yh9Q7" 
        (query { for i in db do 
                 groupJoin j in db on (i.Quantity.GetValueOrDefault() =? j.Quantity) into group
                 yield group} ) 
        "db.GroupJoin(db, i => Convert(i.Quantity.GetValueOrDefault()), j => j.Quantity, (i, group) => new AnonymousObject`2(Item1 = i, Item2 = group)).Select(_arg1 => _arg1.Item2)"

    checkLinqQueryText "ltcnewnc06yh9Q8" 
        (query { for i in db do groupJoin j in db on (i.Quantity ?=? j.Quantity) into group; yield group } ) 
        "db.GroupJoin(db, i => i.Quantity, j => j.Quantity, (i, group) => new AnonymousObject`2(Item1 = i, Item2 = group)).Select(_arg1 => _arg1.Item2)"

    checkLinqQueryText "ltcnewnc06yh9Q5left1" 
        (query { for i in db do 
                 leftOuterJoin j in db on (i.Name = j.Name) into group
                 yield group } ) 
        "db.GroupJoin(db, i => i.Name, j => j.Name, (i, group) => new AnonymousObject`2(Item1 = i, Item2 = group.DefaultIfEmpty())).Select(_arg1 => _arg1.Item2)"

    checkLinqQueryText "ltcnewnc06yh9Q5left1b" 
        (query { for i in db do 
                 let iname = i.Name
                 leftOuterJoin j in db on (iname = j.Name) into group
                 yield group } ) 
        "db.Select(_arg1 => new AnonymousObject`2(Item1 = _arg1, Item2 = _arg1.Name)).GroupJoin(db, tupledArg => tupledArg.Item2, j => j.Name, (tupledArg, group) => new AnonymousObject`3(Item1 = tupledArg.Item1, Item2 = tupledArg.Item2, Item3 = group.DefaultIfEmpty())).Select(_arg2 => _arg2.Item3)"

    check "cnewnc06yh9Q5" 
        (query { for i in db do 
                 groupJoin j in db on (i.Name = j.Name) into group
                 yield group |> Seq.map (fun x -> x.Name) |> Seq.toList } |> Seq.toList) 
        [["Don"; "Don"]; ["Peter"]; ["Freddy"]; ["Freddi"]; ["Don"; "Don"]]

    check "cnewnc06yh9Q6" 
        (query { for i in db do 
                 groupJoin j in db on (i.Quantity ?= j.Quantity.GetValueOrDefault()) into group
                 yield (group |> Seq.map (fun x -> x.Name) |> Seq.toList) } 
            |> Seq.toList) 
        [[]; ["Peter"]; []; ["Freddi"]; []]

    check "cnewnc06yh9Q7" 
        (query { for i in db do 
                 groupJoin j in db on (i.Quantity.GetValueOrDefault() =? j.Quantity) into group
                 yield group |> Seq.map (fun x -> x.Name) |> Seq.toList } |> Seq.toList) 
        [[]; ["Peter"]; []; ["Freddi"]; []]

    check "cnewnc06yh9Q8" 
        (query { for i in db do groupJoin j in db on (i.Quantity ?=? j.Quantity) into group; yield group |> Seq.map (fun x -> x.Name) |> Seq.toList } |> Seq.toList) 
        [[]; ["Peter"]; []; ["Freddi"]; []]

    check "cnewnc06yh9Q5left1" 
        (query { for i in db do 
                 leftOuterJoin j in db on (i.Name = j.Name) into group
                 yield group |> Seq.map (fun x -> x.Name) |> Seq.toList } |> Seq.toList) 
        [["Don"; "Don"]; ["Peter"]; ["Freddy"]; ["Freddi"]; ["Don"; "Don"]]

    check "cnewnc06yh9Q5left2" 
        (query { for i in ["1";"2"] do 
                 leftOuterJoin j in ["1";"12"] on (i.[0] = j.[0]) into group
                 yield (i, group |> Seq.toList) } |> Seq.toList) 
        [("1", ["1";"12"]); ("2", [null]) ]

    // Smoke test for returning a tuple
    checkCommuteSeq "smcnewnc01a" 
        (query { yield (1,1) } ) 
        (seq { yield (1,1) } |> Seq.toList)

    // Smoke test for returning a nested tuple
    checkCommuteSeq "smcnewnc01nested" 
        (query { yield (1,(2,3)) } ) 
        [ (1,(2,3)) ]

    // Smoke test for returning a tuple, size = 7
    checkCommuteSeq "smcnewnc07" 
        (query { yield (1,2,3,4,5,6,7) }) 
        [ (1,2,3,4,5,6,7) ]

    // Smoke test for returning a tuple, size = 8
    checkCommuteSeq "smcnewnc08" 
        (query { yield (1,2,3,4,5,6,7,8) }) 
        [ (1,2,3,4,5,6,7,8) ]

    // Smoke test for returning a tuple, size = 9
    checkCommuteSeq "smcnewnc09" 
        (query { yield (1,2,3,4,5,6,7,8,9) }) 
        (seq { yield (1,2,3,4,5,6,7,8,9) } |> Seq.toList)

    // Smoke test for returning a tuple, size = 16
    checkCommuteSeq "smcnewnc09" 
        (query { yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) }) 
        [ (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) ]

    // Smoke test for returning a tuple
    checkCommuteSeq "smcnewnc01f" 
        (query { yield (1,1) }) 
        (seq { yield (1,1) })

    // Smoke test for returning a nested tuple
    checkCommuteSeq "smcnewnc01nested" 
        (query { yield (1,(2,3)) }) 
        [ (1,(2,3)) ]

    // Smoke test for returning a tuple, size = 7
    checkCommuteSeq "smcnewnc07" 
        (query { yield (1,2,3,4,5,6,7) }) 
        [ (1,2,3,4,5,6,7) ]

    // Smoke test for returning a tuple, size = 8
    checkCommuteSeq "smcnewnc08" 
        (query { yield (1,2,3,4,5,6,7,8) }) 
        [ (1,2,3,4,5,6,7,8) ]

    // Smoke test for returning a tuple, size = 9
    checkCommuteSeq "smcnewnc09" 
        (query { yield (1,2,3,4,5,6,7,8,9) }) 
        (seq { yield (1,2,3,4,5,6,7,8,9) })

    // Smoke test for returning a tuple, size = 16
    checkCommuteSeq "smcnewnc09" 
        (query { yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) }) 
        [ (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) ]


    // Smoke test for returning a tuple
    checkCommuteSeq "smcnewnc01x" 
        (query { for x in db do yield (1,1) }) 
        (seq { for x in db do yield (1,1) })

    // Smoke test for returning a nested tuple
    checkCommuteSeq "smcnewnc01nestedx" 
        (query { for x in db do yield (1,(2,3)) }) 
        (seq { for x in db do yield (1,(2,3)) }) 

    // Smoke test for returning a tuple, size = 7
    checkCommuteSeq "smcnewnc07x" 
        (query { for x in db do yield (1,2,3,4,5,6,7) }) 
        (seq { for x in db do yield (1,2,3,4,5,6,7) }) 

    // Smoke test for returning a tuple, size = 8
    checkCommuteSeq "smcnewnc08x" 
        (query { for x in db do yield (1,2,3,4,5,6,7,8) }) 
        (seq { for x in db do yield (1,2,3,4,5,6,7,8) }) 

    // Smoke test for returning a tuple, size = 9
    checkCommuteSeq "smcnewnc09x" 
        (query { for x in db do yield (1,2,3,4,5,6,7,8,9) }) 
        (seq { for x in db do yield (1,2,3,4,5,6,7,8,9) })

    // Smoke test for returning a tuple, size = 16
    checkCommuteSeq "smcnewnc09x" 
        (query { for x in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) }) 
        (seq { for x in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) }) 

    // Smoke test for returning a tuple, nested for loops
    checkCommuteSeq "smcnewnc01xx" 
        (query { for x in db do for y in db do yield (1,1) }) 
        (seq { for x in db do for y in db do yield (1,1) })

    // Smoke test for returning a nested tuple, nested for loops
    checkCommuteSeq "smcnewnc01nestedxx" 
        (query { for x in db do for y in db do yield (1,(2,3)) }) 
        (seq { for x in db do for y in db do yield (1,(2,3)) }) 

    // Smoke test for returning a tuple, size = 7, nested for loops
    checkCommuteSeq "smcnewnc07xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7) }) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7) }) 

    // Smoke test for returning a tuple, size = 8, nested for loops
    checkCommuteSeq "smcnewnc08xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7,8) }) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7,8) }) 

    // Smoke test for returning a tuple, size = 9, nested for loops
    checkCommuteSeq "smcnewnc09xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9) }) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9) })

    // Smoke test for returning a tuple, size = 16, nested for loops
    checkCommuteSeq "smcnewnc09xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) }) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) }) 

    type R1 =  { V1 : int }
    type R7 =  { V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int }
    type R8 =  { V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int; V8 : int }

    // Smoke test for returning an immutable record object, size = 1
    checkCommuteSeq "rsmcnewnc01" 
        (query { yield { R1.V1=1 } } |> Seq.map (fun r -> r.V1) |> System.Linq.Queryable.AsQueryable) 
        [1;]

    // Smoke test for returning an immutable record object, size = 7
    checkCommuteSeq "rsmcnewnc07" 
        (query { yield { R7.V1=1; R7.V2=2; R7.V3=3; R7.V4=4; R7.V5=5; R7.V6=6; R7.V7=7 } } |> qmap (fun r -> r.V1, r.V2) |> System.Linq.Queryable.AsQueryable) 
        [1,2;]

   // Smoke test for returning an immutable record object, size = 8
    checkCommuteSeq "rsmcnewnc08" 
        (query { yield { R8.V1=1; R8.V2=2; R8.V3=3; R8.V4=4; R8.V5=5; R8.V6=6; R8.V7=7; R8.V8=8 } } |> qmap (fun r -> r.V1, r.V2) |> System.Linq.Queryable.AsQueryable) 
        [1,2;]


    // Smoke test for returning an immutable record object, size = 1
    checkCommuteSeq "rsmcnewnc01x" 
        (query { for x in db do yield { R1.V1=1 } } |> qmap (fun r -> r.V1)) 
        (seq { for x in db do yield { R1.V1=1 } } |> Seq.map (fun r -> r.V1)) 

    // Smoke test for returning an immutable record object, size = 7
    checkCommuteSeq "rsmcnewnc07x" 
        (query { for x in db do yield { R7.V1=1; R7.V2=2; R7.V3=3; R7.V4=4; R7.V5=5; R7.V6=6; R7.V7=7 } } |> qmap (fun r -> r.V1, r.V2) |> System.Linq.Queryable.AsQueryable) 
        (seq { for x in db do yield { R7.V1=1; R7.V2=2; R7.V3=3; R7.V4=4; R7.V5=5; R7.V6=6; R7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2)) 

    // Smoke test for returning an immutable record object, size = 8
    checkCommuteSeq "rsmcnewnc08x" 
        (query { for x in db do yield { R8.V1=1; R8.V2=2; R8.V3=3; R8.V4=4; R8.V5=5; R8.V6=6; R8.V7=7; R8.V8=8 } } |> qmap (fun r -> r.V1, r.V2)) 
        (seq { for x in db do yield { R8.V1=1; R8.V2=2; R8.V3=3; R8.V4=4; R8.V5=5; R8.V6=6; R8.V7=7; R8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2)) 

    type MR1 =  { mutable V1 : int }
    type PMR7 =  { mutable V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int }
    type PMR8 =  { mutable V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int; V8 : int }
    type MR7 =  { mutable V1 : int; mutable V2 : int; mutable V3 : int; mutable V4 : int; mutable V5 : int; mutable V6 : int; mutable V7 : int }
    type MR8 =  { mutable V1 : int; mutable V2 : int; mutable V3 : int; mutable V4 : int; mutable V5 : int; mutable V6 : int; mutable V7 : int; mutable V8 : int }

    // Smoke test for returning a mutable record object, size = 1
    checkCommuteSeq "mrsmcnewnc01" 
        (query { yield { MR1.V1=1 } } |> qmap (fun r -> r.V1)) 
        [1;]

    // Smoke test for returning a partially immutable record object, size = 7
    checkCommuteSeq "pmrsmcnewnc07" 
        (query { yield { PMR7.V1=1; PMR7.V2=2; PMR7.V3=3; PMR7.V4=4; PMR7.V5=5; PMR7.V6=6; PMR7.V7=7 } } |> qmap (fun r -> r.V1, r.V2)) 
        [1,2;]

    // Smoke test for returning a mutable record object, size = 7
    checkCommuteSeq "mrsmcnewnc07" 
        (query { yield { MR7.V1=1; MR7.V2=2; MR7.V3=3; MR7.V4=4; MR7.V5=5; MR7.V6=6; MR7.V7=7 } } |> qmap (fun r -> r.V1, r.V2)) 
        [1,2;]

    // Smoke test for returning a partially immutable record object, size = 8
    checkCommuteSeq "pmrsmcnewnc08" 
        (query { yield { PMR8.V1=1; PMR8.V2=2; PMR8.V3=3; PMR8.V4=4; PMR8.V5=5; PMR8.V6=6; PMR8.V7=7; PMR8.V8=8 } } |> qmap (fun r -> r.V1, r.V2)) 
        [1,2;]

    // Smoke test for returning a mutable record object, size = 8
    checkCommuteSeq "mrsmcnewnc08" 
        (query { yield { MR8.V1=1; MR8.V2=2; MR8.V3=3; MR8.V4=4; MR8.V5=5; MR8.V6=6; MR8.V7=7; MR8.V8=8 } } |> qmap (fun r -> r.V1, r.V2)) 
        [1,2;]

    // Smoke test for returning a mutable record object, size = 1
    checkCommuteSeq "mrsmcnewnc01x" 
        (query { for x in db do yield { MR1.V1=1 } } |> qmap (fun r -> r.V1)) 
        (seq   { for x in db do yield { MR1.V1=1 } } |> Seq.map (fun r -> r.V1)) 

    // Smoke test for returning a partially immutable record object, size = 7
    checkCommuteSeq "pmrsmcnewnc07x" 
        (query { for x in db do yield { PMR7.V1=1; PMR7.V2=2; PMR7.V3=3; PMR7.V4=4; PMR7.V5=5; PMR7.V6=6; PMR7.V7=7 } } |> qmap (fun r -> r.V1, r.V2)) 
        (seq   { for x in db do yield { PMR7.V1=1; PMR7.V2=2; PMR7.V3=3; PMR7.V4=4; PMR7.V5=5; PMR7.V6=6; PMR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2)) 

    // Smoke test for returning a mutable record object, size = 7
    checkCommuteSeq "mrsmcnewnc07x" 
        (query { for x in db do yield { MR7.V1=1; MR7.V2=2; MR7.V3=3; MR7.V4=4; MR7.V5=5; MR7.V6=6; MR7.V7=7 } } |> qmap (fun r -> r.V1, r.V2)) 
        (seq   { for x in db do yield { MR7.V1=1; MR7.V2=2; MR7.V3=3; MR7.V4=4; MR7.V5=5; MR7.V6=6; MR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2)) 

    // Smoke test for returning a partially immutable record object, size = 8
    checkCommuteSeq "pmrsmcnewnc08x" 
        (query { for x in db do yield { PMR8.V1=1; PMR8.V2=2; PMR8.V3=3; PMR8.V4=4; PMR8.V5=5; PMR8.V6=6; PMR8.V7=7; PMR8.V8=8 } } |> qmap (fun r -> r.V1, r.V2)) 
        (seq   { for x in db do yield { PMR8.V1=1; PMR8.V2=2; PMR8.V3=3; PMR8.V4=4; PMR8.V5=5; PMR8.V6=6; PMR8.V7=7; PMR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2)) 

    // Smoke test for returning a mutable record object, size = 8
    checkCommuteSeq "mrsmcnewnc08x" 
        (query { for x in db do yield { MR8.V1=1; MR8.V2=2; MR8.V3=3; MR8.V4=4; MR8.V5=5; MR8.V6=6; MR8.V7=7; MR8.V8=8 } } |> qmap (fun r -> r.V1, r.V2)) 
        (seq   { for x in db do yield { MR8.V1=1; MR8.V2=2; MR8.V3=3; MR8.V4=4; MR8.V5=5; MR8.V6=6; MR8.V7=7; MR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2)) 


    // Smoke test for returning an immutable anonymous record object, size = 1
    checkCommuteSeq "rsmcnewnc01" 
        (query { yield {| V1=1 |} } |> Seq.map (fun r -> r.V1) |> System.Linq.Queryable.AsQueryable) 
        [1;]

    // Smoke test for returning an immutable anonymous record object, size = 7
    checkCommuteSeq "rsmcnewnc07" 
        (query { yield {| V1=1; V2=2; V3=3; V4=4; V5=5; V6=6; V7=7 |} } |> qmap (fun r -> r.V1, r.V2) |> System.Linq.Queryable.AsQueryable) 
        [1,2;]

   // Smoke test for returning an immutable anonymous record object, size = 8
    checkCommuteSeq "rsmcnewnc08" 
        (query { yield {| V1=1; V2=2; V3=3; V4=4; V5=5; V6=6; V7=7; V8=8 |} } |> qmap (fun r -> r.V1, r.V2) |> System.Linq.Queryable.AsQueryable) 
        [1,2;]


    // Smoke test for returning an immutable anonymous record object, size = 1
    checkCommuteSeq "rsmcnewnc01x" 
        (query { for x in db do yield {| V1=1 |} } |> qmap (fun r -> r.V1)) 
        (seq { for x in db do yield {| V1=1 |} } |> Seq.map (fun r -> r.V1)) 

    // Smoke test for returning an immutable anonymous record object, size = 7
    checkCommuteSeq "rsmcnewnc07x" 
        (query { for x in db do yield {| V1=1; V2=2; V3=3; V4=4; V5=5; V6=6; V7=7 |} } |> qmap (fun r -> r.V1, r.V2) |> System.Linq.Queryable.AsQueryable) 
        (seq { for x in db do yield {| V1=1; V2=2; V3=3; V4=4; V5=5; V6=6; V7=7 |} } |> Seq.map (fun r -> r.V1, r.V2)) 

    // Smoke test for returning an immutable anonymous record object, size = 8
    checkCommuteSeq "rsmcnewnc08x" 
        (query { for x in db do yield {| V1=1; V2=2; V3=3; V4=4; V5=5; V6=6; V7=7; V8=8 |} } |> qmap (fun r -> r.V1, r.V2)) 
        (seq { for x in db do yield {| V1=1; V2=2; V3=3; V4=4; V5=5; V6=6; V7=7; V8=8 |} } |> Seq.map (fun r -> r.V1, r.V2)) 

    // Smoke test for returning an object using property-set notation for member init, size = 8
    type C1() = 
        let mutable v1 = 0
        member __.V1 with get() = v1 and set v = v1 <- v

    checkCommuteSeq "smcnewnc01v" 
        (query { yield C1(V1=1) } |> qmap (fun r -> r.V1)) 
        [1;]

    checkCommuteSeq "smcnewnc01x" 
        (query { for x in db do yield C1(V1=1) } |> qmap (fun r -> r.V1)) 
        (seq { for x in db do yield C1(V1=1) } |> Seq.map (fun r -> r.V1)) 

    // Smoke test for returning an object using property-set notation for member init
    type C2() = 
        let mutable v1 = 0
        let mutable v2 = 0
        member __.V1 with get() = v1 and set v = v1 <- v
        member __.V2 with get() = v2 and set v = v2 <- v

    checkCommuteSeq "smcnewnc01b" 
        (query { yield C2(V1=1, V2=2) } |> qmap (fun r -> r.V1, r.V2)) 
        [1,2;]

    checkCommuteSeq "smcnewnc01x" 
        (query { for x in db do yield C2(V1=1, V2=2) } |> qmap (fun r -> r.V1, r.V2)) 
        (seq  { for x in db do yield C2(V1=1, V2=2) } |> Seq.map (fun r -> r.V1, r.V2)) 

    // Smoke test for returning an object using property-set notation for member init
    type C8() = 
        let mutable v1 = 0
        let mutable v2 = 0
        let mutable v3 = 0
        let mutable v4 = 0
        let mutable v5 = 0
        let mutable v6 = 0
        let mutable v7 = 0
        let mutable v8 = 0
        member __.V1 with get() = v1 and set v = v1 <- v
        member __.V2 with get() = v2 and set v = v2 <- v
        member __.V3 with get() = v3 and set v = v3 <- v
        member __.V4 with get() = v4 and set v = v4 <- v
        member __.V5 with get() = v5 and set v = v5 <- v
        member __.V6 with get() = v6 and set v = v6 <- v
        member __.V7 with get() = v7 and set v = v7 <- v
        member __.V8 with get() = v8 and set v = v8 <- v

    checkCommuteSeq "smcnewnc08" 
        (query { yield C8(V1=1, V2=2, V3=3,V4=4, V5=5, V6=6, V7=7, V8=8) } |> qmap (fun r -> r.V1, r.V2, r.V3, r.V4, r.V5, r.V6, r.V7, r.V8)) 
        [(1,2,3,4,5,6,7,8)]

    checkCommuteSeq "smcnewnc08x" 
        (query { for x in db do yield C8(V1=1, V2=2, V3=3,V4=4, V5=5, V6=6, V7=7, V8=8) } |> qmap (fun r -> r.V1, r.V2, r.V3, r.V4, r.V5, r.V6, r.V7, r.V8)) 
        (seq   { for x in db do yield C8(V1=1, V2=2, V3=3,V4=4, V5=5, V6=6, V7=7, V8=8) } |> Seq.map (fun r -> r.V1, r.V2, r.V3, r.V4, r.V5, r.V6, r.V7, r.V8)) 

    // Smoke test for returning a tuple
    checkCommuteSeq "smcnewnc02" 
        (query { for i in db -> (i, i) }) 
        (seq { for i in db -> (i,i) })

    checkCommuteSeq "smcnewnc022df1" 
       (query { for p in db do
                groupBy p.Name into g
                let s = query { for a in g do sumByNullable a.SomeNullableInt16Value }
                select (g.Key, s) })
        [("Don", System.Nullable 0s); ("Peter", System.Nullable 10s); ("Freddy", System.Nullable 0s); ("Freddi", System.Nullable 32s)]

    checkCommuteSeq "smcnewnc022df1" 
       (query { for p in db do
                groupBy p.Name into g
                let s = query { for a in g do sumBy a.SomeInt16Value }
                select (g.Key, s) })
        [("Don", 0s); ("Peter", 10s); ("Freddy", 0s); ("Freddi", 32s)]

module QueryExecutionOverIQueryableWhereDataIsRecord =
    open System
    open Microsoft.FSharp.Linq    
    type Customer = 
       { Name:string; Data: int; Cost:float; Sizes: int list; Quantity:Nullable<int> }
       member x.AlwaysNull = Nullable<int>()
    let c1 = { Name="Don"; Data=6; Cost=6.2; Sizes=[1;2;3;4]; Quantity=Nullable() }
    let c2 = { Name="Peter"; Data=7; Cost=4.2; Sizes=[10;20;30;40]; Quantity=Nullable(10) }
    let c3 = { Name="Freddy"; Data=8; Cost=9.2; Sizes=[11;12;13;14]; Quantity=Nullable() }
    let c4 = { Name="Freddi"; Data=10; Cost=1.0; Sizes=[21;22;23;24]; Quantity=Nullable(32) }
    let c5 = { Name="Don"; Data=9; Cost=1.0; Sizes=[21;22;23;24]; Quantity=Nullable() }
    // Not in the database
    let c6 = { Name="Bob"; Data=9; Cost=1.0; Sizes=[21;22;23;24]; Quantity=Nullable() }
    
    let data = [c1;c2;c3;c4;c5]
    let db = System.Linq.Queryable.AsQueryable<Customer>(data |> List.toSeq)

    let dbEmpty = System.Linq.Queryable.AsQueryable<int>([] |> List.toSeq)
    let dbOne = System.Linq.Queryable.AsQueryable<int>([1] |> List.toSeq)

    checkCommuteSeq "rrcnewnc01" 
        (query { yield! db }) 
        db

    checkCommuteSeq "rrcnewnc01nested" 
        (query { for v in query { yield! db } do yield v }) 
        db

    checkCommuteSeq "rrcnewnc02" 
        (query { for i in db -> i }) 
        db

    checkCommuteSeq "rrcnewnc02nested" 
        (query { for v in query { for i in db -> i } do yield v }) 
        db

    checkCommuteSeq "rrcnewnc02nested2" 
        (query { let q = query { for i in db -> i } in for v in q do yield v }) 
        db

    checkCommuteSeq "rrcnewnc03" 
        (query { for i in db -> i.Name }) 
        (seq { for i in db -> i.Name })

    checkCommuteSeq "rrcnewnc03nested" 
        (query { let q = query { for i in db -> i.Name } in for v in q do yield v } ) 
        (seq { for i in db -> i.Name })

    checkCommuteSeq "rrcnewnc06y" 
        (query { for i in db do for j in db do yield (i.Name,j.Name)  })  
        (seq   { for i in db do for j in db do yield (i.Name,j.Name)  })

    checkCommuteSeq "rrcnewnc06ynested" 
        (query { let q = query { for i in db do for j in db do yield (i.Name,j.Name) } in for v in q do yield v })  
        (seq   { for i in db do for j in db do yield (i.Name,j.Name)  })

    checkCommuteSeq "rrcnewnc06bnested" 
        (query { let q = query { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) } in for v in q do yield v }) 
        (seq   { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "rrcnewnc06b" 
        (query { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) }) 
        (seq   { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "rrcnewnc06w" 
        (query { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) }) 
        (seq   { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "rrcnewnc06wnested2" 
        (query { let q = query { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) } in for v in q do yield v }) 
        (seq   { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "rrcnewnc06z" 
        (query { for i in db do take 3 }) 
        (seq   { for i in db do yield i } |> Seq.take 3)

    checkCommuteSeq "rrcnewnc06x" 
        (query { for i in db do where true; take 3 }) 
        (seq   { for i in db do yield i } |> Seq.take 3)

    
    checkCommuteSeq "rrcnewnc06xb" 
        (query { for i in db do for j in db do where true; take 3 }) 
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.take 3)

    checkCommuteSeq "rrcnewnc06xbnested" 
        (query { let q = query { for i in db do for j in db do where true; take 3 } in for v in q do yield v } ) 
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.take 3)

    checkCommuteSeq "rrcnewnc06ya" 
        (query { for i in db do select i }) 
        (seq   { for i in db do yield i })

    checkCommuteSeq "rrcnewnc06yab" 
        (query { for i in db do for j in db do select i }) 
        (seq   { for i in db do for j in db do yield i })

    checkCommuteSeq "rrcnewnc06ya3" 
        (query { for i in db do 
                 select i.Name into n 
                 distinct }) 
        ["Don"; "Peter"; "Freddy"; "Freddi"]

    checkCommuteSeq "rrcnewnc06ya3nested" 
        (query { let q = query { for i in db do 
                                 select i.Name into n 
                                 distinct } 
                 for v in q do yield v }) 
        ["Don"; "Peter"; "Freddy"; "Freddi"]

    checkCommuteSeq "rrcnewnc06ya3b" 
        (query { for i in db do 
                  select i.Name
                  distinct }) 
        ["Don"; "Peter"; "Freddy"; "Freddi"]

    checkCommuteSeq "rrcnewnc06ya3b" 
        (query { for i in db do 
                  select i.Name
                  distinct 
                  take 2 }) 
        ["Don"; "Peter"]

    checkCommuteSeq "rrcnewnc06yb" 
        (query { for i in db do groupBy i.Name } |> qmap (fun g -> (g.Key,Seq.toList g)))
        (seq   { for i in db do yield i } |> Seq.groupBy (fun i -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "rrcnewnc06ybx" 
        (query { for i in db do groupValBy i i.Name } |> qmap (fun g -> (g.Key,Seq.toList g)))
        (seq   { for i in db do yield i } |> Seq.groupBy (fun i -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "rrcnewnc06yb2" 
        (query { for i in db do for j in db do groupBy i.Name } |> qmap (fun g -> (g.Key,Seq.toList g)))
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.groupBy (fun (i,j) -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "rrcnewnc06yb2x" 
        (query { for i in db do for j in db do groupValBy (i,j) i.Name } |> qmap (fun g -> (g.Key,Seq.toList g)))
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.groupBy (fun (i,j) -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "rrcnewnc06yc" 
        (query { for i in db do sortBy i.Name })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.toList)

    checkCommuteSeq "rrcnewnc06yd" 
        (query { for i in db do sortBy i.Name; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.toList)

    checkCommuteSeq "rrcnewnc06ydx" 
        (query { for i in db do sortBy i.Name; yield (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.map (fun i -> i,i) |> Seq.toList)

    checkCommuteSeq "rrcnewnc06yq" 
        (query { for i in db do sortByDescending i.Data; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Data) |> Seq.toList |> List.rev)

    checkCommuteSeq "rrcnewnc06yqx" 
        (query { for i in db do sortByDescending i.Data; yield (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Data) |> Seq.map (fun i -> (i,i)) |> Seq.toList |> List.rev)

    checkCommuteSeq "rrcnewnc06yq3" 
        (query { for i in db do sortBy i.Name; thenBy i.Data; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.toList)

    checkCommuteSeq "rrcnewnc06yq3x" 
        (query { for i in db do sortBy i.Name; thenBy i.Data; yield (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.map (fun i -> (i,i)) |> Seq.toList)

    checkCommuteSeq "rrcnewnc06yq4" 
        (query { for i in db do sortByDescending i.Name; thenBy i.Data; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.toList |> List.rev)

    checkCommuteSeq "rrcnewnc06yq4x" 
        (query { for i in db do sortByDescending i.Name; thenBy i.Data; yield (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.map (fun i-> (i,i)) |> Seq.toList |> List.rev)

    checkCommuteSeq "rrcnewnc06yq5" 
        (query { for i in db do sortByDescending i.Name; thenByDescending i.Data; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.toList |> List.rev)

    checkCommuteSeq "rrcnewnc06yq5x" 
        (query { for i in db do sortByDescending i.Name; thenByDescending i.Data; yield (i,i) })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.map (fun i -> (i,i)) |> Seq.toList |> List.rev)

    checkCommuteSeq "rrcnewnc06yq6" 
        (query { for i in db do sortBy i.Name; thenByDescending i.Data; yield i })
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.toList)

    checkCommuteSeq "rrcnewnc06yq6b" 
        (query { for i in db do sortByNullable i.Quantity; yield i } |> qmap (fun x -> x.Name ))
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "rrcnewnc06yq6b" 
        (query { for i in db do sortByNullable i.Quantity; yield (i,i) } |> qmap (fun (x,_) -> x.Name ))
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "rrcnewnc06yq6c" 
        (query { for i in db do sortByNullable i.Quantity; thenByNullable i.AlwaysNull; yield i } |> qmap (fun x -> x.Name ))
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "rrcnewnc06yq6cx" 
        (query { for i in db do sortByNullable i.Quantity; thenByNullable i.AlwaysNull; yield (i,i) } |> qmap (fun (x,_) -> x.Name ))
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "rrcnewnc06yq6c" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullable i.AlwaysNull; yield i } |> qmap (fun x -> x.Name ))
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "rrcnewnc06yq6cx" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullable i.AlwaysNull; yield (i,i) } |> qmap (fun (x,_) -> x.Name ))
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "rrcnewnc06yq6c" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullableDescending i.AlwaysNull; yield i } |> qmap (fun x -> x.Name ))
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]


    checkCommuteSeq "rrcnewnc06yq6cx" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullableDescending i.AlwaysNull; yield (i,i) } |> qmap (fun (x,_) -> x.Name ) )
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    check "rrcnewnc06ye"  
        (query { for i in db do contains c1 }) 
        true

    check "rrcnewnc06yex"  
        (query { for i in db do for j in db do contains (c1,c1) }) 
        true

    check "rrcnewnc06ye"  
        (query { for i in dbEmpty do contains 1 }) 
        false

    check "rrcnewnc06yeg"  
        (query { for i in dbOne do contains 1 }) 
        true

    check "rrcnewnc06yf" 
        (query { for i in db do contains c6 }) 
        false

    check "rrcnewnc06yg" 
        (query { for i in db do count }) 
        5

    check "rrcnewnc06yh" 
        (query { for i in db do last }) 
        c5

    check "rrcnewnc06yh" 
        (query { for i in db do for j in db do last }) 
        (c5,c5)

    check "rrcnewnc06yh2a" 
        (query { for i in db do lastOrDefault }) 
        c5

    check "rrcnewnc06yh2a" 
        (query { for i in db do for j in db do lastOrDefault }) 
        (c5,c5)

    check "rrcnewnc06yh2b" 
        (query { for i in db do headOrDefault }) 
        c1

    check "rrcnewnc06yh2bx" 
        (query { for i in db do for j in db do headOrDefault }) 
        (c1,c1)

    check "rrcnewnc06yh3a" 
        (query { for i in dbEmpty do lastOrDefault }) 
        0

    check "rrcnewnc06yh3b" 
        (query { for i in dbEmpty do headOrDefault }) 
        0

    check "rrcnewnc06yh4" 
        (query { for i in dbOne do exactlyOne }) 
        1

    check "rrcnewnc06yh4x" 
        (query { for i in dbOne do for j in dbOne do exactlyOne }) 
        (1,1)

    check "rrcnewnc06yh5" 
        (query { for i in dbOne do exactlyOneOrDefault }) 
        1

    check "rrcnewnc06yh5x" 
        (query { for i in dbOne do for j in dbOne do exactlyOneOrDefault }) 
        (1,1)

    
    check "rrcnewnc06yh6" 
        (try 
            query { for i in dbEmpty do exactlyOne } |> ignore; false 
         with :? System.InvalidOperationException -> true) 
        true

    check "rrcnewnc06yh7" 
        (query { for i in dbEmpty do exactlyOneOrDefault }) 
        0

    check "rrcnewnc06yh8" 
        (query { for i in dbOne do minBy i }) 
        1

    check "rrcnewnc06yh8x" 
        (query { for i in dbOne do for j in dbOne do minBy i }) 
        1

    check "rrcnewnc06yh9" 
        (query { for i in db do minBy i.Data }) 
        c1.Data


    check "rrcnewnc06yh9x" 
        (query { for i in db do for j in db do minBy i.Data }) 
        c1.Data

    
    check "rrcnewnc06yh9" 
        (try query { for i in dbEmpty do minBy i } with :? System.InvalidOperationException -> -10) 
        -10

    check "rrcnewnc06yh9xff" 
        (try query { for i in dbEmpty do nth 3 } with :? System.ArgumentOutOfRangeException -> -10) 
        -10

    check "rrcnewnc06yh9xx" 
        (query { for i in dbEmpty do skip 1 } |> Seq.length |> ignore; 10)
        10


    check "rrcnewnc06yh10" 
        (query { for i in dbOne do maxBy i }) 
        1

    check "rrcnewnc06yh11" 
        (query { for i in db do maxBy i.Data }) 
        c4.Data
    
    check "rrcnewnc06yh11" 
        (query { for i in db do for j in db do maxBy i.Data }) 
        c4.Data
    
    check "rrcnewnc06yh12" 
        (try query { for i in dbEmpty do maxBy i } with :? System.InvalidOperationException -> -10) 
        -10

    check "rrcnewnc06yh81" 
        (query { for i in dbOne do sumBy i }) 
        1

    check "rrcnewnc06yh81x" 
        (query { for i in dbOne do for j in dbOne do sumBy i }) 
        1

    check "rrcnewnc06yh82" 
        (query { for i in dbEmpty do sumBy i }) 
        0

    check "rrcnewnc06yh82x" 
        (query { for i in dbEmpty do for j in dbEmpty do sumBy i }) 
        0

    check "rrcnewnc06yh81b" 
        (query { for i in dbOne do averageBy (float i) }) 
        1.0
    
    check "rrcnewnc06yh81bx" 
        (query { for i in dbOne do for j in dbOne do averageBy (float i) }) 
        1.0
    

    check "rrcnewnc06yh81c" 
        (query { for i in dbOne do averageBy (float32 i) }) 
        1.0f


    check "rrcnewnc06yh81cx" 
        (query { for i in dbOne do for j in dbOne do averageBy (float32 i) }) 
        1.0f

    check "rrcnewnc06yh81d" 
        (query { for i in dbOne do averageBy (decimal i) }) 
        1.0M

    check "rrcnewnc06yh81e" 
        (query { for i in dbOne do sumBy (int64 i) }) 
        1L

    check "rrcnewnc06yh81f" 
        (query { for i in dbOne do sumBy (decimal i) }) 
        1.0M

    check "rrcnewnc06yh81g" 
        (query { for i in dbOne do sumBy (int32 i) }) 
        1

    check "rrcnewnc06yh81h" 
        (query { for i in dbOne do sumBy (float i) }) 
        1.0

    check "rrcnewnc06yh81h" 
        (query { for i in dbOne do sumBy (float32 i) }) 
        1.0f

    
    check "rrcnewnc06yh9" 
        (try query { for i in dbEmpty do averageBy (float i) } with :? System.InvalidOperationException -> -10.0) 
        -10.0

    check "rrcnewnc06yh91" 
        (query { for i in db do sumByNullable i.Quantity }) 
        (Nullable 42)

    check "rrcnewnc06yh92" 
        (query { for i in db do averageByNullable (Nullable(10.0)) }) 
        (Nullable 10.0)

    check "rrcnewnc06yh92" 
        (query { for i in db do maxByNullable i.Quantity }) 
        (Nullable 32)

    check "rrcnewnc06yh93" 
        (query { for i in db do minByNullable i.Quantity }) 
        (Nullable 10)

    check "rrcnewnc06yh94" 
        (query { for i in dbEmpty do sumByNullable (Nullable(i)) }) 
        (Nullable 0)

    check "rrcnewnc06yh95" 
        (query { for i in dbEmpty do averageByNullable (Nullable(float i)) }) 
        (Nullable())

    check "rrcnewnc06yh96" 
        (query { for i in dbEmpty do maxByNullable (Nullable i) }) 
        (Nullable())

    check "rrcnewnc06yh97" 
        (query { for i in dbEmpty do minByNullable (Nullable i) }) 
        (Nullable())

    check "rrcnewnc06yh91" 
        (query { for i in db do sumByNullable i.Quantity }) 
        (Nullable 42)

    check "rrcnewnc06yh94" 
        (query { for i in dbEmpty do sumByNullable (Nullable(i)) }) 
        (Nullable 0)

    check "rrcnewnc06yh94b" 
        (query { for i in dbEmpty do sumByNullable (Nullable(float32 i)) }) 
        (Nullable 0.0f)

    check "rrcnewnc06yh94c" 
        (query { for i in dbEmpty do sumByNullable (Nullable(decimal i)) }) 
        (Nullable 0.0M)

    check "rrcnewnc06yh94d" 
        (query { for i in dbEmpty do sumByNullable (Nullable(int64 i)) }) 
        (Nullable 0L)

    check "rrcnewnc06yh94e" 
        (query { for i in dbEmpty do sumByNullable (Nullable(float i)) }) 
        (Nullable 0.0)

    check "rrcnewnc06yh98" 
        (query { for i in db do sumByNullable i.AlwaysNull }) 
        (Nullable(0))

    check "rrcnewnc06yh96" 
        (query { for i in dbEmpty do maxByNullable (Nullable i) }) 
        (Nullable())

    check "rrcnewnc06yh96b" 
        (query { for i in dbEmpty do maxByNullable (Nullable (float i)) }) 
        (Nullable())

    check "rrcnewnc06yh96c" 
        (query { for i in dbEmpty do maxByNullable (Nullable (decimal i)) }) 
        (Nullable())

    check "rrcnewnc06yh96d" 
        (query { for i in dbEmpty do maxByNullable (Nullable (float32 i)) }) 
        (Nullable())

    check "rrcnewnc06yh96e" 
        (query { for i in dbEmpty do maxByNullable (Nullable (int64 i)) }) 
        (Nullable())

    check "rrcnewnc06yh99" 
        (query { for i in db do maxByNullable i.AlwaysNull }) 
        (Nullable())

    check "rrcnewnc06yh9Q" 
        (query { for i in db do minByNullable i.AlwaysNull }) 
        (Nullable())

    check "rrcnewnc06yh99" 
        (query { for i in db do all (i.Name = "Don") }) 
        false

    check "rrcnewnc06yh99" 
        (query { for i in db do exists (i.Name = "Don") }) 
        true

    check "rrcnewnc06yh99" 
        (query { for i in dbEmpty do all (i = 1) }) 
        true

    check "rrcnewnc06yh99" 
        (query { for i in dbEmpty do exists (i = 1) }) 
        false

    check "rrcnewnc06yh99" 
        (query { for i in db do find (i.Name = "Peter") }) 
        c2

    check "rrcnewnc06yh99" 
        (query { for i in db do skip 1; select i.Name} |> Seq.toList) 
        ["Peter"; "Freddy"; "Freddi"; "Don"]

    check "rrcnewnc06yh99" 
        (query { for i in db do skipWhile (i.Name = "Don"); select i.Name} |> Seq.toList) 
        ["Peter"; "Freddy"; "Freddi"; "Don"]

    check "rrcnewnc06yh99" 
        (query { for i in db do take 2; select i.Name} |> Seq.toList) 
        ["Don"; "Peter"]

    check "rrcnewnc06yh99" 
        (query { for i in db do takeWhile (i.Name = "Don" || i.Name = "Peter"); select i.Name} |> Seq.toList) 
        ["Don"; "Peter"]

    check "rrcnewnc06yh99" 
        (query { for i in db do nth 0 }) 
        c1

    check "rrcnewnc06yh9Q1" 
        (query { for i in db do 
                 join j in db on (i.Name = j.Name)
                 select (i.Cost, j.Cost) } |> Seq.toList) 
        [(6.2, 6.2); (6.2, 1.0); (4.2, 4.2); (9.2, 9.2); (1.0, 1.0); (1.0, 6.2); (1.0, 1.0)]


    check "rrcnewnc06yh9Q2" 
        (query { for i in db do 
                 join j in db on (i.Name = j.Name)
                 yield (i.Cost, j.Cost) } |> Seq.toList) 
        [(6.2, 6.2); (6.2, 1.0); (4.2, 4.2); (9.2, 9.2); (1.0, 1.0); (1.0, 6.2); (1.0, 1.0)]

    check "rrcnewnc06yh9Q3" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?= j.Quantity.GetValueOrDefault())
                 yield (i.Cost, j.Cost) } |> Seq.toList) 
        [(4.2, 4.2); (1.0, 1.0)]

    check "rrcnewnc06yh9Q4" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?=? j.Quantity)
                 yield (i.Cost, j.Cost) } |> Seq.toList) 
        [(4.2, 4.2); (1.0, 1.0)]

    check "rrcnewnc06yh9Q5" 
        (query { for i in db do 
                 groupJoin j in db on (i.Name = j.Name) into group
                 yield group |> Seq.map (fun x -> x.Name) |> Seq.toList } |> Seq.toList) 
        [["Don"; "Don"]; ["Peter"]; ["Freddy"]; ["Freddi"]; ["Don"; "Don"]]

    check "rrcnewnc06yh9Q6" 
        (query { for i in db do 
                 groupJoin j in db on (i.Quantity ?= j.Quantity.GetValueOrDefault()) into group
                 yield (group |> Seq.map (fun x -> x.Name) |> Seq.toList) } 
            |> Seq.toList) 
        [[]; ["Peter"]; []; ["Freddi"]; []]

    check "rrcnewnc06yh9Q7" 
        (query { for i in db do 
                 groupJoin j in db on (i.Quantity.GetValueOrDefault() =? j.Quantity) into group
                 yield group |> Seq.map (fun x -> x.Name) |> Seq.toList } |> Seq.toList) 
        [[]; ["Peter"]; []; ["Freddi"]; []]

    check "rrcnewnc06yh9Q8" 
        (query { for i in db do groupJoin j in db on (i.Quantity ?=? j.Quantity) into group; yield group |> Seq.map (fun x -> x.Name) |> Seq.toList } |> Seq.toList) 
        [[]; ["Peter"]; []; ["Freddi"]; []]





    check "rrcnewnc06yh9Q5left1" 
        (query { for i in db do 
                 leftOuterJoin j in db on (i.Name = j.Name) into group
                 yield group |> Seq.map (fun x -> x.Name) |> Seq.toList } |> Seq.toList) 
        [["Don"; "Don"]; ["Peter"]; ["Freddy"]; ["Freddi"]; ["Don"; "Don"]]

    check "rrcnewnc06yh9Q5left2" 
        (query { for i in ["1";"2"] do 
                 leftOuterJoin j in ["1";"12"] on (i.[0] = j.[0]) into group
                 yield (i, group |> Seq.toList) } |> Seq.toList) 
        [("1", ["1";"12"]); ("2", [null]) ]



    // Smoke test for returning a tuple
    checkCommuteSeq "rrsmcnewnc01a" 
        (query { yield (1,1) } ) 
        (seq { yield (1,1) } |> Seq.toList)

    // Smoke test for returning a nested tuple
    checkCommuteSeq "rrsmcnewnc01nested" 
        (query { yield (1,(2,3)) } ) 
        [ (1,(2,3)) ]

    // Smoke test for returning a tuple, size = 7
    checkCommuteSeq "rrsmcnewnc07" 
        (query { yield (1,2,3,4,5,6,7) } ) 
        [ (1,2,3,4,5,6,7) ]

    // Smoke test for returning a tuple, size = 8
    checkCommuteSeq "rrsmcnewnc08" 
        (query { yield (1,2,3,4,5,6,7,8) } ) 
        [ (1,2,3,4,5,6,7,8) ]

    // Smoke test for returning a tuple, size = 9
    checkCommuteSeq "rrsmcnewnc09" 
        (query { yield (1,2,3,4,5,6,7,8,9) } ) 
        (seq { yield (1,2,3,4,5,6,7,8,9) } )

    // Smoke test for returning a tuple, size = 16
    checkCommuteSeq "rrsmcnewnc09" 
        (query { yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } ) 
        [ (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) ]

    // Smoke test for returning a tuple
    checkCommuteSeq "rrsmcnewnc01f" 
        (query { yield (1,1) } ) 
        (seq { yield (1,1) } )

    // Smoke test for returning a nested tuple
    checkCommuteSeq "rrsmcnewnc01nested" 
        (query { yield (1,(2,3)) } ) 
        [ (1,(2,3)) ]

    // Smoke test for returning a tuple, size = 7
    checkCommuteSeq "rrsmcnewnc07" 
        (query { yield (1,2,3,4,5,6,7) } ) 
        [ (1,2,3,4,5,6,7) ]

    // Smoke test for returning a tuple, size = 8
    checkCommuteSeq "rrsmcnewnc08" 
        (query { yield (1,2,3,4,5,6,7,8) } ) 
        [ (1,2,3,4,5,6,7,8) ]

    // Smoke test for returning a tuple, size = 9
    checkCommuteSeq "rrsmcnewnc09" 
        (query { yield (1,2,3,4,5,6,7,8,9) } ) 
        (seq { yield (1,2,3,4,5,6,7,8,9) } )

    // Smoke test for returning a tuple, size = 16
    checkCommuteSeq "rrsmcnewnc09" 
        (query { yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } ) 
        [ (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) ]

    // Smoke test for returning a tuple
    checkCommuteSeq "rrsmcnewnc01x" 
        (query { for x in db do yield (1,1) } ) 
        (seq { for x in db do yield (1,1) } )

    // Smoke test for returning a nested tuple
    checkCommuteSeq "rrsmcnewnc01nestedx" 
        (query { for x in db do yield (1,(2,3)) } ) 
        (seq { for x in db do yield (1,(2,3)) } ) 

    // Smoke test for returning a tuple, size = 7
    checkCommuteSeq "rrsmcnewnc07x" 
        (query { for x in db do yield (1,2,3,4,5,6,7) } ) 
        (seq { for x in db do yield (1,2,3,4,5,6,7) } ) 

    // Smoke test for returning a tuple, size = 8
    checkCommuteSeq "rrsmcnewnc08x" 
        (query { for x in db do yield (1,2,3,4,5,6,7,8) } ) 
        (seq { for x in db do yield (1,2,3,4,5,6,7,8) } ) 

    // Smoke test for returning a tuple, size = 9
    checkCommuteSeq "rrsmcnewnc09x" 
        (query { for x in db do yield (1,2,3,4,5,6,7,8,9) } ) 
        (seq { for x in db do yield (1,2,3,4,5,6,7,8,9) } )

    // Smoke test for returning a tuple, size = 16
    checkCommuteSeq "rrsmcnewnc09x" 
        (query { for x in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } ) 
        (seq { for x in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } ) 



    // Smoke test for returning a tuple, nested for loops
    checkCommuteSeq "rrsmcnewnc01xx" 
        (query { for x in db do for y in db do yield (1,1) } ) 
        (seq { for x in db do for y in db do yield (1,1) } )

    // Smoke test for returning a nested tuple, nested for loops
    checkCommuteSeq "rrsmcnewnc01nestedxx" 
        (query { for x in db do for y in db do yield (1,(2,3)) } ) 
        (seq { for x in db do for y in db do yield (1,(2,3)) } ) 

    // Smoke test for returning a tuple, size = 7, nested for loops
    checkCommuteSeq "rrsmcnewnc07xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7) } ) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7) } ) 

    // Smoke test for returning a tuple, size = 8, nested for loops
    checkCommuteSeq "rrsmcnewnc08xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7,8) } ) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7,8) } ) 

    // Smoke test for returning a tuple, size = 9, nested for loops
    checkCommuteSeq "rrsmcnewnc09xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9) } ) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9) } )

    // Smoke test for returning a tuple, size = 16, nested for loops
    checkCommuteSeq "rrsmcnewnc09xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } ) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } ) 


    type R1 =  { V1 : int }
    type R7 =  { V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int }
    type R8 =  { V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int; V8 : int }


    // Smoke test for returning an immutable record object, size = 1
    checkCommuteSeq "rrrsmcnewnc01" 
        (query { yield { R1.V1=1 } } |> qmap (fun r -> r.V1) ) 
        [1;]

    // Smoke test for returning an immutable record object, size = 7
    checkCommuteSeq "rrrsmcnewnc07" 
        (query { yield { R7.V1=1; R7.V2=2; R7.V3=3; R7.V4=4; R7.V5=5; R7.V6=6; R7.V7=7 } } |> qmap (fun r -> r.V1, r.V2) ) 
        [1,2;]

   // Smoke test for returning an immutable record object, size = 8
    checkCommuteSeq "rrrsmcnewnc08" 
        (query { yield { R8.V1=1; R8.V2=2; R8.V3=3; R8.V4=4; R8.V5=5; R8.V6=6; R8.V7=7; R8.V8=8 } } |> qmap (fun r -> r.V1, r.V2) ) 
        [1,2;]

    // Smoke test for returning an immutable record object, size = 1
    checkCommuteSeq "rrrsmcnewnc01x" 
        (query { for x in db do yield { R1.V1=1 } } |> qmap (fun r -> r.V1) ) 
        (seq { for x in db do yield { R1.V1=1 } } |> Seq.map (fun r -> r.V1) ) 

    // Smoke test for returning an immutable record object, size = 7
    checkCommuteSeq "rrrsmcnewnc07x" 
        (query { for x in db do yield { R7.V1=1; R7.V2=2; R7.V3=3; R7.V4=4; R7.V5=5; R7.V6=6; R7.V7=7 } } |> qmap (fun r -> r.V1, r.V2) ) 
        (seq { for x in db do yield { R7.V1=1; R7.V2=2; R7.V3=3; R7.V4=4; R7.V5=5; R7.V6=6; R7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) ) 

    // Smoke test for returning an immutable record object, size = 8
    checkCommuteSeq "rrrsmcnewnc08x" 
        (query { for x in db do yield { R8.V1=1; R8.V2=2; R8.V3=3; R8.V4=4; R8.V5=5; R8.V6=6; R8.V7=7; R8.V8=8 } } |> qmap (fun r -> r.V1, r.V2) ) 
        (seq { for x in db do yield { R8.V1=1; R8.V2=2; R8.V3=3; R8.V4=4; R8.V5=5; R8.V6=6; R8.V7=7; R8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) ) 

    type MR1 =  { mutable V1 : int }
    type PMR7 =  { mutable V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int }
    type PMR8 =  { mutable V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int; V8 : int }
    type MR7 =  { mutable V1 : int; mutable V2 : int; mutable V3 : int; mutable V4 : int; mutable V5 : int; mutable V6 : int; mutable V7 : int }
    type MR8 =  { mutable V1 : int; mutable V2 : int; mutable V3 : int; mutable V4 : int; mutable V5 : int; mutable V6 : int; mutable V7 : int; mutable V8 : int }

    // Smoke test for returning a mutable record object, size = 1
    checkCommuteSeq "rrmrsmcnewnc01" 
        (query { yield { MR1.V1=1 } } |> qmap (fun r -> r.V1) ) 
        [1;]

    // Smoke test for returning a partially immutable record object, size = 7
    checkCommuteSeq "rrpmrsmcnewnc07" 
        (query { yield { PMR7.V1=1; PMR7.V2=2; PMR7.V3=3; PMR7.V4=4; PMR7.V5=5; PMR7.V6=6; PMR7.V7=7 } } |> qmap (fun r -> r.V1, r.V2) ) 
        [1,2;]

    // Smoke test for returning a mutable record object, size = 7
    checkCommuteSeq "rrmrsmcnewnc07" 
        (query { yield { MR7.V1=1; MR7.V2=2; MR7.V3=3; MR7.V4=4; MR7.V5=5; MR7.V6=6; MR7.V7=7 } } |> qmap (fun r -> r.V1, r.V2) ) 
        [1,2;]

    // Smoke test for returning a partially immutable record object, size = 8
    checkCommuteSeq "rrpmrsmcnewnc08" 
        (query { yield { PMR8.V1=1; PMR8.V2=2; PMR8.V3=3; PMR8.V4=4; PMR8.V5=5; PMR8.V6=6; PMR8.V7=7; PMR8.V8=8 } } |> qmap (fun r -> r.V1, r.V2) ) 
        [1,2;]

    // Smoke test for returning a mutable record object, size = 8
    checkCommuteSeq "rrmrsmcnewnc08" 
        (query { yield { MR8.V1=1; MR8.V2=2; MR8.V3=3; MR8.V4=4; MR8.V5=5; MR8.V6=6; MR8.V7=7; MR8.V8=8 } } |> qmap (fun r -> r.V1, r.V2) ) 
        [1,2;]


    // Smoke test for returning a mutable record object, size = 1
    checkCommuteSeq "rrmrsmcnewnc01x" 
        (query { for x in db do yield { MR1.V1=1 } } |> qmap (fun r -> r.V1) ) 
        (seq   { for x in db do yield { MR1.V1=1 } } |> Seq.map (fun r -> r.V1) ) 

    // Smoke test for returning a partially immutable record object, size = 7
    checkCommuteSeq "rrpmrsmcnewnc07x" 
        (query { for x in db do yield { PMR7.V1=1; PMR7.V2=2; PMR7.V3=3; PMR7.V4=4; PMR7.V5=5; PMR7.V6=6; PMR7.V7=7 } } |> qmap (fun r -> r.V1, r.V2) ) 
        (seq   { for x in db do yield { PMR7.V1=1; PMR7.V2=2; PMR7.V3=3; PMR7.V4=4; PMR7.V5=5; PMR7.V6=6; PMR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) ) 

    // Smoke test for returning a mutable record object, size = 7
    checkCommuteSeq "rrmrsmcnewnc07x" 
        (query { for x in db do yield { MR7.V1=1; MR7.V2=2; MR7.V3=3; MR7.V4=4; MR7.V5=5; MR7.V6=6; MR7.V7=7 } } |> qmap (fun r -> r.V1, r.V2) ) 
        (seq   { for x in db do yield { MR7.V1=1; MR7.V2=2; MR7.V3=3; MR7.V4=4; MR7.V5=5; MR7.V6=6; MR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) ) 

    // Smoke test for returning a partially immutable record object, size = 8
    checkCommuteSeq "rrpmrsmcnewnc08x" 
        (query { for x in db do yield { PMR8.V1=1; PMR8.V2=2; PMR8.V3=3; PMR8.V4=4; PMR8.V5=5; PMR8.V6=6; PMR8.V7=7; PMR8.V8=8 } } |> qmap (fun r -> r.V1, r.V2) ) 
        (seq   { for x in db do yield { PMR8.V1=1; PMR8.V2=2; PMR8.V3=3; PMR8.V4=4; PMR8.V5=5; PMR8.V6=6; PMR8.V7=7; PMR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) ) 

    // Smoke test for returning a mutable record object, size = 8
    checkCommuteSeq "rrmrsmcnewnc08x" 
        (query { for x in db do yield { MR8.V1=1; MR8.V2=2; MR8.V3=3; MR8.V4=4; MR8.V5=5; MR8.V6=6; MR8.V7=7; MR8.V8=8 } } |> qmap (fun r -> r.V1, r.V2) ) 
        (seq   { for x in db do yield { MR8.V1=1; MR8.V2=2; MR8.V3=3; MR8.V4=4; MR8.V5=5; MR8.V6=6; MR8.V7=7; MR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) ) 


    // Smoke test for returning an object using property-set notation for member init, size = 8
    type C1() = 
        let mutable v1 = 0
        member __.V1 with get() = v1 and set v = v1 <- v

    checkCommuteSeq "rrsmcnewnc01v" 
        (query { yield C1(V1=1) } |> qmap (fun r -> r.V1) ) 
        [1;]

    checkCommuteSeq "rrsmcnewnc01x" 
        (query { for x in db do yield C1(V1=1) } |> qmap (fun r -> r.V1) ) 
        (seq { for x in db do yield C1(V1=1) } |> Seq.map (fun r -> r.V1) ) 

        //<@ C1(V1=1) @>

    // Smoke test for returning an object using property-set notation for member init
    type C2() = 
        let mutable v1 = 0
        let mutable v2 = 0
        member __.V1 with get() = v1 and set v = v1 <- v
        member __.V2 with get() = v2 and set v = v2 <- v

    checkCommuteSeq "rrsmcnewnc01b" 
        (query { yield C2(V1=1, V2=2) } |> qmap (fun r -> r.V1, r.V2) ) 
        [1,2;]

    checkCommuteSeq "rrsmcnewnc01x" 
        (query { for x in db do yield C2(V1=1, V2=2) } |> qmap (fun r -> r.V1, r.V2) ) 
        (seq  { for x in db do yield C2(V1=1, V2=2) } |> Seq.map (fun r -> r.V1, r.V2) ) 

    // Smoke test for returning an object using property-set notation for member init
    type C8() = 
        let mutable v1 = 0
        let mutable v2 = 0
        let mutable v3 = 0
        let mutable v4 = 0
        let mutable v5 = 0
        let mutable v6 = 0
        let mutable v7 = 0
        let mutable v8 = 0
        member __.V1 with get() = v1 and set v = v1 <- v
        member __.V2 with get() = v2 and set v = v2 <- v
        member __.V3 with get() = v3 and set v = v3 <- v
        member __.V4 with get() = v4 and set v = v4 <- v
        member __.V5 with get() = v5 and set v = v5 <- v
        member __.V6 with get() = v6 and set v = v6 <- v
        member __.V7 with get() = v7 and set v = v7 <- v
        member __.V8 with get() = v8 and set v = v8 <- v

    checkCommuteSeq "rrsmcnewnc08" 
        (query { yield C8(V1=1, V2=2, V3=3,V4=4, V5=5, V6=6, V7=7, V8=8) } |> qmap (fun r -> r.V1, r.V2, r.V3, r.V4, r.V5, r.V6, r.V7, r.V8) ) 
        [(1,2,3,4,5,6,7,8)]

    checkCommuteSeq "rrsmcnewnc08x" 
        (query { for x in db do yield C8(V1=1, V2=2, V3=3,V4=4, V5=5, V6=6, V7=7, V8=8) } |> qmap (fun r -> r.V1, r.V2, r.V3, r.V4, r.V5, r.V6, r.V7, r.V8) ) 
        (seq   { for x in db do yield C8(V1=1, V2=2, V3=3,V4=4, V5=5, V6=6, V7=7, V8=8) } |> Seq.map (fun r -> r.V1, r.V2, r.V3, r.V4, r.V5, r.V6, r.V7, r.V8) ) 

    // Smoke test for returning a tuple
    checkCommuteSeq "rrsmcnewnc02" 
        (query { for i in db -> (i, i) } ) 
        (seq { for i in db -> (i,i) } )


/// Check some of the conversions of leaf expressions when calling the API directly
module LeafExpressionConversionTests = 
    open Microsoft.FSharp.Linq.RuntimeHelpers
    open System.Linq.Expressions

    module ExpressionPatterns = 
        let (|Constant|_|) (c:Expression) = match c with :? ConstantExpression as c -> Some (c.Value, c.Type) | _ -> None
        let (|BinExpr|_|) (c:Expression) = match c with :? BinaryExpression as c -> Some (c.Left, c.NodeType, c.Right) | _ -> None
        let (|Int32Obj|_|) (c:obj) = match c with :? int32 as c -> Some c | _ -> None
        let (|Int32|_|) = function Constant(Int32Obj n, _) -> Some n | _ -> None
        let (|Add|_|) = function BinExpr(l,ExpressionType.Add,r) -> Some (l,r) | _ -> None
        let (|Modulo|_|) = function BinExpr(l,ExpressionType.Modulo,r) -> Some (l,r) | _ -> None
        let (|GreaterThan|_|) = function BinExpr(l,ExpressionType.GreaterThan,r) -> Some (l,r) | _ -> None
        let (|AddChecked|_|) = function BinExpr(l,ExpressionType.AddChecked,r) -> Some (l,r) | _ -> None

    let E x = LeafExpressionConverter.QuotationToExpression x
    open ExpressionPatterns
    check "rrckjwnew" (match E <@ 1 @> with | Int32 1 -> true | _ -> false) true
    check "rrckjwnew" (match E <@ 1 + 1 @> with | Add(Int32 1, Int32 1) -> true | _ -> false) true
    check "rrckjwnew" (match E <@ Operators.Checked.(+) 1 1 @> with | AddChecked(Int32 1, Int32 1) -> true | _ -> false) true
    check "rrckjwnew" (match E <@ 1 % 1 @> with | Modulo(Int32 1, Int32 1) -> true | _ -> false) true

module QueryConversionIssue = 
    check "QueryConversionIssue"
        (
            try
                let a = System.Linq.Queryable.AsQueryable [| 1 |]
                let r = 
                    query {
                    for v in a do
                    for (x, y) in (query { for z in [ 1 ] do select(z, z)}) do
                    select x
                }
                ignore(r.Expression);
                true
            with _ -> false
        ) true
    
/// Some smoke tests that implicit expression conversions compile correctly
module MiscTestsForImplicitExpressionConversion = 
    open Microsoft.FSharp.Linq

    module QueryOperators = 
        let mapReduce ([<ProjectionParameter>] mapper: 'T -> seq<'U>) (keySelector: 'U -> 'Key) (reducer: 'Key -> seq<'U> -> 'Result) (source: System.Linq.IQueryable<_>) =
            query { for v in source do
                    for x in mapper v do
                    groupValBy x (keySelector x) into group
                    yield reducer group.Key group }

    module TechnicalReportExamplesOption1 = 
        open System.Linq
        open System.Linq.Expressions

        module Histogram = 
            let histogram k (input: System.Linq.IQueryable<string>) =
                // Problem - type annotation required on input variable 
                // Problem - upcast required of return result of function (no covariance for functions)
                let words = input.SelectMany(fun x -> x.Split(' ') :> seq<_>)
                let groups = words.GroupBy(fun x -> x)
                let counts = groups.Select(fun x -> x.Key, x.Count())
                let ordered = counts.OrderByDescending(fun (key,count) -> count)
                let top = ordered.Take k
                top 

        check "cwnwe09" 
            (Histogram.histogram 3 (Queryable.AsQueryable ["Hello world"; "world hello"]) |> Seq.toList)
            [("world", 2); ("Hello", 1); ("hello", 1)]

        module PageRank1 = 
            type Edge = { source:int; target:int }
            type Rank = { source:int; value:int }
            let pageRank(edges: IQueryable<Edge>, ranks: IQueryable<Rank>) = 
                  edges.Join(ranks, 
                             (fun edge -> edge.source), 
                             (fun rank -> rank.source), 
                             (fun edge rank -> { source = edge.target; value = rank.value }))
                       . GroupBy(fun rank -> rank.source)
                       . Select(fun group -> { source = group.Key; value = group.Select(fun rank -> rank.value).Sum() })

            let results = 
              pageRank 
                ( Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; target = i % 7 } ], 
                  Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; value = i+100 } ]) |> Seq.toList |> List.map (fun c -> c.value)

            check "lceknwe90" results [2235; 2250; 2265; 2079; 2093; 2107; 2121]



    module TechnicalReportExamplesOption2 = 
        open System.Linq
        open System.Linq.Expressions

        type QSeq = 
            static member collect (f:Expression<System.Func<_,_>>)  = (fun (source:#IQueryable<_>) -> System.Linq.Queryable.SelectMany(source,f))
            static member groupBy (f:Expression<System.Func<_,_>>) = (fun (source:#IQueryable<_>) -> System.Linq.Queryable.GroupBy(source,f))
            // Note: no Expression conversion on curried members, reduces fluency w.r.t. F# combinator style
            static member join (source2, keySelector1:Expression<System.Func<_,_>>, keySelector2:Expression<System.Func<_,_>>, resultSelector:Expression<System.Func<_,_,_>>) = (fun (source:#IQueryable<_>) -> System.Linq.Queryable.Join(source,source2,keySelector1,keySelector2,resultSelector))
            static member map (f:Expression<System.Func<_,_>>) = (fun (source:#IQueryable<_>) -> System.Linq.Queryable.Select(source,f))
            static member orderBy (f:Expression<System.Func<_,_>>) = (fun (source:#IQueryable<_>) -> System.Linq.Queryable.OrderBy(source,f))
            static member orderByDescending (f:Expression<System.Func<_,_>>) = (fun (source:#IQueryable<_>) -> System.Linq.Queryable.OrderByDescending(source,f))
            static member take n = (fun (source:#IQueryable<_>) -> System.Linq.Queryable.Take(source,n))


        module Histogram = 
            let histogram k (input: System.Linq.IQueryable<string>) =
                input 
                 |> QSeq.collect (fun x -> x.Split(' ') :> seq<_>)
                 |> QSeq.groupBy (fun x -> x)
                 |> QSeq.map (fun x -> x.Key, x.Count())
                 |> QSeq.orderByDescending (fun (key,count) -> count)
                 |> QSeq.take k

        module PageRank1 = 
            type Edge = { source:int; target:int }
            type Rank = { source:int; value:int }
            let pageRank(edges: IQueryable<Edge>, ranks: IQueryable<Rank>) = 
                  edges
                  // Note, in this model join is neither pleasant (less pleasant than C#) nor curried (though no expectation it would be)
                  |> QSeq.join (ranks, (fun edge -> edge.source), (fun rank -> rank.source), (fun edge rank -> { source = edge.target; value = rank.value }))
                  |> QSeq.groupBy(fun rank -> rank.source)
                  // Note: in this model, nested queries use 'Seq' combinators. Very non-fluent
                  |> QSeq.map (fun group -> { source = group.Key; value = group |> Seq.sumBy (fun rank -> rank.value) })

            let results = 

                pageRank ( Queryable.AsQueryable [ for i in 0 .. 100 -> ({ source = i; target = i % 7 } : Edge) ], 
                           Queryable.AsQueryable [ for i in 0 .. 100 -> ({ source = i; value = i+100 } : Rank) ])
                |> Seq.toList |> List.map (fun c -> c.value)

            check "lceknwe91" results [2235; 2250; 2265; 2079; 2093; 2107; 2121]



        module PageRank2 = 
            type Edge = { source:int; target:int }
            type Rank = { source:int; value:int }
            let pageRank(edges: IQueryable<Edge>, ranks: IQueryable<Rank>) = 
                query { for edge in edges do
                        join rank in ranks on (edge.source = rank.source)
                        let newRank = { source = edge.target; value = rank.value }
                        groupValBy newRank newRank.source into group
                        yield    { source = group.Key; value = query { for rank in group do sumBy rank.value } } }

            pageRank ( Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; target = i % 7 }  : Edge], 
                       Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; value = i+100 }  : Rank ])


        module PageRank1b = 
            type Edge = { mutable source:int; mutable target:int }
            type Rank = { mutable source:int; mutable value:int }
            let pageRank(edges: IQueryable<Edge>, ranks: IQueryable<Rank>) = 
                query { for edge in edges do
                        join rank in ranks on (edge.source = rank.source)
                        let newRank = { source = edge.target; value = rank.value }

                        groupValBy newRank newRank.source into group

                        select { source = group.Key; value = query { for rank in group do sumBy rank.value } } }

            pageRank ( Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; target = i % 7 } : Edge ], 
                       Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; value = i+100 } : Rank ])

        module PageRank1c = 
            type Edge(source:int, target:int) = 
                member x.Source = source 
                member x.Target = target
            type Rank(source:int, value:int) = 
                member x.Source = source 
                member x.Value = value

            let pageRank(edges: IQueryable<Edge>, ranks: IQueryable<Rank>) = 
                query { for edge in edges do
                        join rank in ranks on (edge.Source = rank.Source)
                        let newRank = Rank(source = edge.Target, value = rank.Value)
                        groupValBy newRank newRank.Source into group
                        yield (Rank(source = group.Key, value = query { for rank in group do sumBy rank.Value } )) }

            pageRank ( Queryable.AsQueryable [ for i in 0 .. 100 -> Edge(source = i, target = i % 7) ], 
                       Queryable.AsQueryable [ for i in 0 .. 100 -> Rank(source = i, value = i+100) ])



module Problem2 = 
    open System.Linq

    type Item = { Name : string } with override x.ToString() = x.Name
    let item = {Name = "1"}
    let l = [box item]
    let items = l.AsQueryable()

    QueryExecutionOverIQueryable.checkLinqQueryText "ltcjhnwec7eweww2" 
       (query { for item in items do
                where (item :? Item)
                select (item :?> Item) })
       "[1].Where(item => (item Is Item)).Select(item => Convert(item))"

    checkCommuteSeq "ltcjhnwec7eweww2b" 
       (query { for item in items do
                where (item :? Item)
                select (item :?> Item) })
       [item]


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

