// #Quotations #Query
#if TESTS_AS_APP
module Core_queriesOverIEnumerable
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
           report_failure (sprintf "test %s...failed, expected %A got %A" s v2 v1)

    let test s b = check s b true

module QueryExecutionOverIEnumerable =
    open System
    open Microsoft.FSharp.Linq    
    type Customer(name:string, data: int, cost:float, sizes: int list, quantity:Nullable<int>) = 
        member x.Name = name
        member x.Data = data
        member x.Cost = cost
        member x.Sizes = sizes
        member x.Quantity = quantity
        member x.AlwaysNull = Nullable<int>()
    let c1 = Customer( name="Don", data=6, cost=6.2, sizes=[1;2;3;4], quantity=Nullable())
    let c2 = Customer( name="Peter", data=7, cost=4.2, sizes=[10;20;30;40], quantity=Nullable(10))
    let c3 = Customer( name="Freddy", data=8, cost=9.2, sizes=[11;12;13;14], quantity=Nullable())
    let c4 = Customer( name="Freddi", data=10, cost=1.0, sizes=[21;22;23;24], quantity=Nullable(32))
    let c5 = Customer( name="Don", data=9, cost=1.0, sizes=[21;22;23;24], quantity=Nullable())
    // Not in the database
    let c6 = Customer( name="Bob", data=9, cost=1.0, sizes=[21;22;23;24],quantity=Nullable())
    
    let data = [c1;c2;c3;c4;c5]
    let db = (data |> List.toSeq)

    let dbEmpty : seq<int> = ([] |> List.toSeq)
    let dbOne = ([1] |> List.toSeq)

    let checkCommuteSeq s q1 q2 =
        check s (q1 |> Seq.toList) (q2 |> Seq.toList)

    let checkCommuteVal s q1 q2 =
        check s q1 q2

    checkCommuteSeq "cnewnc01" 
        (query { yield! db }) 
        db

    checkCommuteSeq "cnewnc02" 
        (query { for i in db -> i }) 
        db

    checkCommuteSeq "cnewnc03" 
        (query { for i in db -> i.Name }) 
        (seq { for i in db -> i.Name })

    checkCommuteSeq "cnewnc06y" 
        (query { for i in db do for j in db do yield (i.Name,j.Name)  })  
        (seq   { for i in db do for j in db do yield (i.Name,j.Name)  })

    checkCommuteSeq "cnewnc06b" 
        (query { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) }) 
        (seq   { for i in db do for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    checkCommuteSeq "cnewnc06w" 
        (query { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) }) 
        (seq   { for i in db do if i.Data = 8 then for j in db do if i.Data = j.Data then yield (i.Data,i.Name,j.Name) })

    // TODO: ERROR: System.ArgumentException: Type mismatch when building 'cond': types of true and false branches differ. Expected 'System.Linq.IQueryable`1[Microsoft.FSharp.Linq.RuntimeHelpers.MutableTuple`3[System.Int32,System.String,System.String]]', but received type 'System.Collections.Generic.IEnumerable`1[Microsoft.FSharp.Linq.RuntimeHelpers.MutableTuple`3[System.Int32,System.String,System.String]]'.
    (*
    checkCommuteSeq "cnewnc08q" 
         (query { for i in db do 
                    match i.Data with 
                    | 8 -> 
                        for j in db do 
                            if i.Data = j.Data then 
                               yield (i.Data,i.Name,j.Name) 
                    | _ -> () })
         (seq   { for i in db do 
                    match i.Data with 
                    | 8 -> 
                        for j in db do 
                            if i.Data = j.Data then 
                                yield (i.Data,i.Name,j.Name) 
                    | _ -> () })

                    *)

    let t() = 
        checkCommuteSeq "cnewnc06z" 
            (query { for i in db do take 3 }) 
            (seq   { for i in db do yield i } |> Seq.take 3)

    t()

    checkCommuteSeq "cnewnc06x" 
        (query { for i in db do where true; take 3 }) 
        (seq   { for i in db do yield i } |> Seq.take 3)

    checkCommuteSeq "cnewnc06xb" 
        (query { for i in db do for j in db do where true; take 3 }) 
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.take 3)

    checkCommuteSeq "cnewnc06ya" 
        (query { for i in db do select i }) 
        (seq   { for i in db do yield i })

    checkCommuteSeq "cnewnc06yab" 
        (query { for i in db do for j in db do select i }) 
        (seq   { for i in db do for j in db do yield i })

    checkCommuteSeq "cnewnc06ya3" 
        (query { for i in db do 
                 select i.Name into n 
                 distinct } |> Seq.toList) 
        ["Don"; "Peter"; "Freddy"; "Freddi"]

#if ZIP
    checkCommuteSeq "cnewnc06ya3" 
        (query { for i in [1;2;3;4] do 
                 zip  [4;3;2;1] into j
                 yield (i,j) } |> Seq.toList) 
        [(1, 4); (2, 3); (3, 2); (4, 1)]

    checkCommuteSeq "cnewnc06y43" 
        (query { for i in [1;2;3;4] do 
                 zip  [4;3;2;1;0] into j
                 yield (i,j) } |> Seq.toList) 
        [(1, 4); (2, 3); (3, 2); (4, 1)]

    checkCommuteSeq "cnewnc06y43b" 
        (query { for i in [1;2;3;4] do 
                 zip  [4;3;2] into j
                 yield (i,j) } |> Seq.toList) 
        [(1, 4); (2, 3); (3, 2)]

    checkCommuteSeq "cnewnc06y43c" 
        (query { for i in db do 
                 zip db into j
                 yield (i.Name,j.Name.Length) } |> Seq.toList) 
        [("Don", 3); ("Peter", 5); ("Freddy", 6); ("Freddi", 6); ("Don", 3)]

#endif

    checkCommuteSeq "cnewnc06yb" 
        (query { for i in db do groupBy i.Name } |> Seq.map (fun g -> (g.Key,Seq.toList g)))
        (seq   { for i in db do yield i } |> Seq.groupBy (fun i -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "cnewnc06ybx" 
        (query { for i in db do groupValBy i i.Name } |> Seq.map (fun g -> (g.Key,Seq.toList g)))
        (seq   { for i in db do yield i } |> Seq.groupBy (fun i -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "cnewnc06yb2" 
        (query { for i in db do for j in db do groupBy i.Name } |> Seq.map (fun g -> (g.Key,Seq.toList g)))
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.groupBy (fun (i,j) -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "cnewnc06yb2x" 
        (query { for i in db do for j in db do groupValBy (i,j) i.Name } |> Seq.map (fun g -> (g.Key,Seq.toList g)))
        (seq   { for i in db do for j in db do yield (i,j) } |> Seq.groupBy (fun (i,j) -> i.Name) |> Seq.map (fun (key,g) -> (key, Seq.toList g)))

    checkCommuteSeq "cnewnc06yc" 
        (query { for i in db do sortBy i.Name } |> Seq.toList)
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.toList)

    checkCommuteSeq "cnewnc06yd" 
        (query { for i in db do sortBy i.Name; yield i } |> Seq.toList)
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Name) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq" 
        (query { for i in db do sortByDescending i.Data; yield i } |> Seq.toList)
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> i.Data) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq3" 
        (query { for i in db do sortBy i.Name; thenBy i.Data; yield i } |> Seq.toList)
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq4" 
        (query { for i in db do sortByDescending i.Name; thenBy i.Data; yield i } |> Seq.toList)
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq5" 
        (query { for i in db do sortByDescending i.Name; thenByDescending i.Data; yield i } |> Seq.toList)
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, i.Data)) |> Seq.toList |> List.rev)

    checkCommuteSeq "cnewnc06yq6" 
        (query { for i in db do sortBy i.Name; thenByDescending i.Data; yield i } |> Seq.toList)
        (seq   { for i in db do yield i } |> Seq.sortBy (fun i -> (i.Name, -i.Data)) |> Seq.toList)

    checkCommuteSeq "cnewnc06yq6b" 
        (query { for i in db do sortByNullable i.Quantity; yield i } |> Seq.map (fun x -> x.Name ) |> Seq.toList)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6c1" 
        (query { for i in db do sortByNullable i.Quantity; thenByNullable i.AlwaysNull; yield i } |> Seq.map (fun x -> x.Name ) |> Seq.toList)
        ["Don"; "Freddy"; "Don"; "Peter"; "Freddi"]

    checkCommuteSeq "cnewnc06yq6c2" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullable i.AlwaysNull; yield i } |> Seq.map (fun x -> x.Name ) |> Seq.toList)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    checkCommuteSeq "cnewnc06yq6c3" 
        (query { for i in db do sortByNullableDescending i.Quantity; thenByNullableDescending i.AlwaysNull; yield i } |> Seq.map (fun x -> x.Name ) |> Seq.toList)
        ["Freddi"; "Peter"; "Don"; "Freddy"; "Don"]

    check "cnewnc06ye"  
        (query { for i in db do contains c1 }) 
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

    check "cnewnc06yh2a" 
        (query { for i in db do lastOrDefault }) 
        c5

    check "cnewnc06yh2b" 
        (query { for i in db do headOrDefault }) 
        c1

    check "cnewnc06yh3a" 
        (query { for i in dbEmpty do lastOrDefault }) 
        0

    check "cnewnc06yh3b" 
        (query { for i in dbEmpty do headOrDefault }) 
        0

    check "cnewnc06yh4" 
        (query { for i in dbOne do exactlyOne }) 
        1

    check "cnewnc06yh5" 
        (query { for i in dbOne do exactlyOneOrDefault }) 
        1

    
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

    check "cnewnc06yh9" 
        (query { for i in db do minBy i.Data }) 
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
    
    check "cnewnc06yh12" 
        (try query { for i in dbEmpty do maxBy i } with :? System.InvalidOperationException -> -10) 
        -10

    check "cnewnc06yh81" 
        (query { for i in dbOne do sumBy i }) 
        1

    check "cnewnc06yh82" 
        (query { for i in dbEmpty do sumBy i }) 
        0

    check "cnewnc06yh81b" 
        (query { for i in dbOne do averageBy (float i) }) 
        1.0
    

    check "cnewnc06yh81c" 
        (query { for i in dbOne do averageBy (float32 i) }) 
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

    check "cnewnc06yh92" 
        (query { for i in db do averageByNullable (Nullable(10.0)) }) 
        (Nullable 10.0)

    check "cnewnc06yh92b" 
        (query { for i in db do averageByNullable (Nullable(10.0M)) }) 
        (Nullable 10.0M)

    check "cnewnc06yh92c" 
        (query { for i in db do averageByNullable (Nullable(10.0f)) }) 
        (Nullable 10.0f)

    check "cnewnc06yh95" 
        (query { for i in dbEmpty do averageByNullable (Nullable(float i)) }) 
        (Nullable())

    check "cnewnc06yh92d" 
        (query { for i in db do maxByNullable i.Quantity }) 
        (Nullable 32)

    check "cnewnc06yh93" 
        (query { for i in db do minByNullable i.Quantity }) 
        (Nullable 10)

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

    check "cnewnc06yh97" 
        (query { for i in dbEmpty do minByNullable (Nullable i) }) 
        (Nullable())

    check "cnewnc06yh98" 
        (query { for i in db do sumByNullable i.AlwaysNull }) 
        (Nullable(0))

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

    check "cnewnc06yh99q" 
        (query { for i in db do takeWhile (i.Name = "Don" || i.Name = "Peter"); select i.Name} |> Seq.toList) 
        ["Don"; "Peter"]

    check "cnewnc06yh99w" 
        (query { for i in db do nth 0 }) 
        c1

    check "cnewnc06yh9Q1" 
        (query { for i in db do 
                 join j in db on (i.Name = j.Name)
                 select (i.Cost, j.Cost) } |> Seq.toList) 
        [(6.2, 6.2); (6.2, 1.0); (4.2, 4.2); (9.2, 9.2); (1.0, 1.0); (1.0, 6.2); (1.0, 1.0)]


    check "cnewnc06yh9Q2" 
        (query { for i in db do 
                 join j in db on (i.Name = j.Name)
                 yield (i.Cost, j.Cost) } |> Seq.toList) 
        [(6.2, 6.2); (6.2, 1.0); (4.2, 4.2); (9.2, 9.2); (1.0, 1.0); (1.0, 6.2); (1.0, 1.0)]

    check "cnewnc06yh9Q3" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?= j.Quantity.GetValueOrDefault())
                 yield (i.Cost, j.Cost) } |> Seq.toList) 
        [(4.2, 4.2); (1.0, 1.0)]

    check "cnewnc06yh9Q4" 
        (query { for i in db do 
                 join j in db on (i.Quantity ?=? j.Quantity)
                 yield (i.Cost, j.Cost) } |> Seq.toList) 
        [(4.2, 4.2); (1.0, 1.0)]

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
    checkCommuteSeq "smcnewnc01" 
        (query { yield (1,1) } |> Seq.toList) 
        (seq { yield (1,1) } |> Seq.toList)


    // Smoke test for returning a nested tuple
    checkCommuteSeq "smcnewnc01nested" 
        (query { yield (1,(2,3)) } |> Seq.toList) 
        [ (1,(2,3)) ]

    // Smoke test for returning a tuple, size = 7
    checkCommuteSeq "smcnewnc07" 
        (query { yield (1,2,3,4,5,6,7) } |> Seq.toList) 
        [ (1,2,3,4,5,6,7) ]

    // Smoke test for returning a tuple, size = 8
    checkCommuteSeq "smcnewnc08" 
        (query { yield (1,2,3,4,5,6,7,8) } |> Seq.toList) 
        [ (1,2,3,4,5,6,7,8) ]

    // Smoke test for returning a tuple, size = 9
    checkCommuteSeq "smcnewnc09c" 
        (query { yield (1,2,3,4,5,6,7,8,9) } |> Seq.toList) 
        (seq { yield (1,2,3,4,5,6,7,8,9) } |> Seq.toList)

    // Smoke test for returning a tuple, size = 16
    checkCommuteSeq "smcnewnc09f" 
        (query { yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } |> Seq.toList) 
        [ (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) ]



    // Smoke test for returning a tuple
    checkCommuteSeq "smcnewnc01x" 
        (query { for x in db do yield (1,1) } |> Seq.toList) 
        (seq { for x in db do yield (1,1) } |> Seq.toList)

    // Smoke test for returning a nested tuple
    checkCommuteSeq "smcnewnc01nestedx" 
        (query { for x in db do yield (1,(2,3)) } |> Seq.toList) 
        (seq { for x in db do yield (1,(2,3)) } |> Seq.toList) 

    // Smoke test for returning a tuple, size = 7
    checkCommuteSeq "smcnewnc07x" 
        (query { for x in db do yield (1,2,3,4,5,6,7) } |> Seq.toList) 
        (seq { for x in db do yield (1,2,3,4,5,6,7) } |> Seq.toList) 

    // Smoke test for returning a tuple, size = 8
    checkCommuteSeq "smcnewnc08x" 
        (query { for x in db do yield (1,2,3,4,5,6,7,8) } |> Seq.toList) 
        (seq { for x in db do yield (1,2,3,4,5,6,7,8) } |> Seq.toList) 

    // Smoke test for returning a tuple, size = 9
    checkCommuteSeq "smcnewnc09x" 
        (query { for x in db do yield (1,2,3,4,5,6,7,8,9) } |> Seq.toList) 
        (seq { for x in db do yield (1,2,3,4,5,6,7,8,9) } |> Seq.toList)

    // Smoke test for returning a tuple, size = 16
    checkCommuteSeq "smcnewnc09xd" 
        (query { for x in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } |> Seq.toList) 
        (seq { for x in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } |> Seq.toList) 




    // Smoke test for returning a tuple, nested for loops
    checkCommuteSeq "smcnewnc01xx" 
        (query { for x in db do for y in db do yield (1,1) } |> Seq.toList) 
        (seq { for x in db do for y in db do yield (1,1) } |> Seq.toList)

    // Smoke test for returning a nested tuple, nested for loops
    checkCommuteSeq "smcnewnc01nestedxx" 
        (query { for x in db do for y in db do yield (1,(2,3)) } |> Seq.toList) 
        (seq { for x in db do for y in db do yield (1,(2,3)) } |> Seq.toList) 

    // Smoke test for returning a tuple, size = 7, nested for loops
    checkCommuteSeq "smcnewnc07xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7) } |> Seq.toList) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7) } |> Seq.toList) 

    // Smoke test for returning a tuple, size = 8, nested for loops
    checkCommuteSeq "smcnewnc08xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7,8) } |> Seq.toList) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7,8) } |> Seq.toList) 

    // Smoke test for returning a tuple, size = 9, nested for loops
    checkCommuteSeq "smcnewnc09xx" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9) } |> Seq.toList) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9) } |> Seq.toList)

    // Smoke test for returning a tuple, size = 16, nested for loops
    checkCommuteSeq "smcnewnc09xxf" 
        (query { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } |> Seq.toList) 
        (seq { for x in db do for y in db do yield (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16) } |> Seq.toList) 




    type R1 =  { V1 : int }
    type R7 =  { V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int }
    type R8 =  { V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int; V8 : int }

    // Smoke test for returning an immutable record object, size = 1
    checkCommuteSeq "rsmcnewnc01" 
        (query { yield { R1.V1=1 } } |> Seq.map (fun r -> r.V1) |> Seq.toList) 
        [1;]

    // Smoke test for returning an immutable record object, size = 7
    checkCommuteSeq "rsmcnewnc07" 
        (query { yield { R7.V1=1; R7.V2=2; R7.V3=3; R7.V4=4; R7.V5=5; R7.V6=6; R7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        [1,2;]

    // Smoke test for returning an immutable record object, size = 8
    checkCommuteSeq "rsmcnewnc08" 
        (query { yield { R8.V1=1; R8.V2=2; R8.V3=3; R8.V4=4; R8.V5=5; R8.V6=6; R8.V7=7; R8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        [1,2;]




    // Smoke test for returning an immutable record object, size = 1
    checkCommuteSeq "rsmcnewnc01x" 
        (query { for x in db do yield { R1.V1=1 } } |> Seq.map (fun r -> r.V1) |> Seq.toList) 
        (seq { for x in db do yield { R1.V1=1 } } |> Seq.map (fun r -> r.V1) |> Seq.toList) 

    // Smoke test for returning an immutable record object, size = 7
    checkCommuteSeq "rsmcnewnc07x" 
        (query { for x in db do yield { R7.V1=1; R7.V2=2; R7.V3=3; R7.V4=4; R7.V5=5; R7.V6=6; R7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        (seq { for x in db do yield { R7.V1=1; R7.V2=2; R7.V3=3; R7.V4=4; R7.V5=5; R7.V6=6; R7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 

    // Smoke test for returning an immutable record object, size = 8
    checkCommuteSeq "rsmcnewnc08x" 
        (query { for x in db do yield { R8.V1=1; R8.V2=2; R8.V3=3; R8.V4=4; R8.V5=5; R8.V6=6; R8.V7=7; R8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        (seq { for x in db do yield { R8.V1=1; R8.V2=2; R8.V3=3; R8.V4=4; R8.V5=5; R8.V6=6; R8.V7=7; R8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 




    type MR1 =  { mutable V1 : int }
    type PMR7 =  { mutable V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int }
    type PMR8 =  { mutable V1 : int; V2 : int; V3 : int; V4 : int; V5 : int; V6 : int; V7 : int; V8 : int }
    type MR7 =  { mutable V1 : int; mutable V2 : int; mutable V3 : int; mutable V4 : int; mutable V5 : int; mutable V6 : int; mutable V7 : int }
    type MR8 =  { mutable V1 : int; mutable V2 : int; mutable V3 : int; mutable V4 : int; mutable V5 : int; mutable V6 : int; mutable V7 : int; mutable V8 : int }

    // Smoke test for returning a mutable record object, size = 1
    checkCommuteSeq "mrsmcnewnc01" 
        (query { yield { MR1.V1=1 } } |> Seq.map (fun r -> r.V1) |> Seq.toList) 
        [1;]

    // Smoke test for returning a partially immutable record object, size = 7
    checkCommuteSeq "pmrsmcnewnc07" 
        (query { yield { PMR7.V1=1; PMR7.V2=2; PMR7.V3=3; PMR7.V4=4; PMR7.V5=5; PMR7.V6=6; PMR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        [1,2;]

    // Smoke test for returning a mutable record object, size = 7
    checkCommuteSeq "mrsmcnewnc07" 
        (query { yield { MR7.V1=1; MR7.V2=2; MR7.V3=3; MR7.V4=4; MR7.V5=5; MR7.V6=6; MR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        [1,2;]

    // Smoke test for returning a partially immutable record object, size = 8
    checkCommuteSeq "pmrsmcnewnc08" 
        (query { yield { PMR8.V1=1; PMR8.V2=2; PMR8.V3=3; PMR8.V4=4; PMR8.V5=5; PMR8.V6=6; PMR8.V7=7; PMR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        [1,2;]

    // Smoke test for returning a mutable record object, size = 8
    checkCommuteSeq "mrsmcnewnc08" 
        (query { yield { MR8.V1=1; MR8.V2=2; MR8.V3=3; MR8.V4=4; MR8.V5=5; MR8.V6=6; MR8.V7=7; MR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        [1,2;]



    // Smoke test for returning a mutable record object, size = 1
    checkCommuteSeq "mrsmcnewnc01x" 
        (query { for x in db do yield { MR1.V1=1 } } |> Seq.map (fun r -> r.V1) |> Seq.toList) 
        (seq   { for x in db do yield { MR1.V1=1 } } |> Seq.map (fun r -> r.V1) |> Seq.toList) 

    // Smoke test for returning a partially immutable record object, size = 7
    checkCommuteSeq "pmrsmcnewnc07x" 
        (query { for x in db do yield { PMR7.V1=1; PMR7.V2=2; PMR7.V3=3; PMR7.V4=4; PMR7.V5=5; PMR7.V6=6; PMR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        (seq   { for x in db do yield { PMR7.V1=1; PMR7.V2=2; PMR7.V3=3; PMR7.V4=4; PMR7.V5=5; PMR7.V6=6; PMR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 

    // Smoke test for returning a mutable record object, size = 7
    checkCommuteSeq "mrsmcnewnc07x" 
        (query { for x in db do yield { MR7.V1=1; MR7.V2=2; MR7.V3=3; MR7.V4=4; MR7.V5=5; MR7.V6=6; MR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        (seq   { for x in db do yield { MR7.V1=1; MR7.V2=2; MR7.V3=3; MR7.V4=4; MR7.V5=5; MR7.V6=6; MR7.V7=7 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 

    // Smoke test for returning a partially immutable record object, size = 8
    checkCommuteSeq "pmrsmcnewnc08x" 
        (query { for x in db do yield { PMR8.V1=1; PMR8.V2=2; PMR8.V3=3; PMR8.V4=4; PMR8.V5=5; PMR8.V6=6; PMR8.V7=7; PMR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        (seq   { for x in db do yield { PMR8.V1=1; PMR8.V2=2; PMR8.V3=3; PMR8.V4=4; PMR8.V5=5; PMR8.V6=6; PMR8.V7=7; PMR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 

    // Smoke test for returning a mutable record object, size = 8
    checkCommuteSeq "mrsmcnewnc08x" 
        (query { for x in db do yield { MR8.V1=1; MR8.V2=2; MR8.V3=3; MR8.V4=4; MR8.V5=5; MR8.V6=6; MR8.V7=7; MR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        (seq   { for x in db do yield { MR8.V1=1; MR8.V2=2; MR8.V3=3; MR8.V4=4; MR8.V5=5; MR8.V6=6; MR8.V7=7; MR8.V8=8 } } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 



    // Smoke test for returning an object using property-set notation for member init, size = 1
    type C1() = 
        let mutable v1 = 0
        member __.V1 with get() = v1 and set v = v1 <- v

    checkCommuteSeq "smcnewnc0122" 
        (query { yield C1(V1=1) } |> Seq.map (fun r -> r.V1) |> Seq.toList) 
        [1;]

    checkCommuteSeq "smcnewnc01x" 
        (query { for x in db do yield C1(V1=1) } |> Seq.map (fun r -> r.V1) |> Seq.toList) 
        (seq { for x in db do yield C1(V1=1) } |> Seq.map (fun r -> r.V1) |> Seq.toList) 




    // Smoke test for returning an object using property-set notation for member init, size = 2
    type C2() = 
        let mutable v1 = 0
        let mutable v2 = 0
        member __.V1 with get() = v1 and set v = v1 <- v
        member __.V2 with get() = v2 and set v = v2 <- v

    checkCommuteSeq "smcnewnc0199" 
        (query { yield C2(V1=1, V2=2) } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        [1,2;]

    checkCommuteSeq "smcnewnc01x" 
        (query { for x in db do yield C2(V1=1, V2=2) } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 
        (seq  { for x in db do yield C2(V1=1, V2=2) } |> Seq.map (fun r -> r.V1, r.V2) |> Seq.toList) 



    // Smoke test for returning an object using property-set notation for member init, size = 8
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
        (query { yield C8(V1=1, V2=2, V3=3,V4=4, V5=5, V6=6, V7=7, V8=8) } |> Seq.map (fun r -> r.V1, r.V2, r.V3, r.V4, r.V5, r.V6, r.V7, r.V8) |> Seq.toList) 
        [(1,2,3,4,5,6,7,8)]

    checkCommuteSeq "smcnewnc08x" 
        (query { for x in db do yield C8(V1=1, V2=2, V3=3,V4=4, V5=5, V6=6, V7=7, V8=8) } |> Seq.map (fun r -> r.V1, r.V2, r.V3, r.V4, r.V5, r.V6, r.V7, r.V8) |> Seq.toList) 
        (seq   { for x in db do yield C8(V1=1, V2=2, V3=3,V4=4, V5=5, V6=6, V7=7, V8=8) } |> Seq.map (fun r -> r.V1, r.V2, r.V3, r.V4, r.V5, r.V6, r.V7, r.V8) |> Seq.toList) 


    // Smoke test for returning a tuple
    checkCommuteSeq "smcnewnc02" 
        (query { for i in db -> (i, i) } |> Seq.toList) 
        (seq { for i in db -> (i,i) } |> Seq.toList)


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
    check "ckjwnew" (match E <@ 1 @> with | Int32 1 -> true | _ -> false) true
    check "ckjwnew" (match E <@ 1 + 1 @> with | Add(Int32 1, Int32 1) -> true | _ -> false) true
    check "ckjwnew" (match E <@ Operators.Checked.(+) 1 1 @> with | AddChecked(Int32 1, Int32 1) -> true | _ -> false) true
    check "ckjwnew" (match E <@ 1 % 1 @> with | Modulo(Int32 1, Int32 1) -> true | _ -> false) true


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
            // Problem: no Expression conversion on curried members, reduces fluency w.r.t. F# combinator style
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
                  // Note, in this model nested queries use 'Seq' combinators. Very non-fluent
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
                        join rank in  ranks on (edge.source = rank.source)
                        let newRank = { source = edge.target; value = rank.value }
                        groupValBy newRank newRank.source into group
                        yield    { source = group.Key; value = query { for rank in group do sumBy rank.value } } }

            pageRank ( Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; target = i % 7 }  : Edge], 
                       Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; value = i+100 }  : Rank ])


        module PageRank1b = 
            open System.Linq
            open System.Linq.Expressions
            type Edge = { mutable source:int; mutable target:int }
            type Rank = { mutable source:int; mutable value:int }
            let pageRank(edges: IQueryable<Edge>, ranks: IQueryable<Rank>) = 
                query { for edge in edges do
                        join rank in  ranks on (edge.source = rank.source)
                        let newRank = { source = edge.target; value = rank.value }

                        groupValBy newRank newRank.source into group

                        select { source = group.Key; value = query { for rank in group do sumBy rank.value } } }

            pageRank ( Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; target = i % 7 } : Edge ], 
                       Queryable.AsQueryable [ for i in 0 .. 100 -> { source = i; value = i+100 } : Rank ])


        module PageRank1b2 = 
            open System.Linq
            open System.Linq.Expressions
            type Edge = { mutable source:int; mutable target:int }
            type Rank = { mutable source:int; mutable value:int }
            let pageRank(edges: IQueryable<Edge>, ranks: IQueryable<Rank>) = 
                query { for edge in edges do
                        join rank in  ranks on (edge.source = rank.source)
                        let newRank = { source = edge.target; value = rank.value }

                        groupValBy newRank newRank.source into group

                        select { source = group.Key; value = group.Sum(fun rank -> rank.value) } } 

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
                        join rank in  ranks on (edge.Source = rank.Source)
                        let newRank = Rank(source = edge.Target, value = rank.Value)
                        groupValBy newRank newRank.Source into group
                        yield (Rank(source = group.Key, value = query { for rank in group do sumBy rank.Value } )) }

            pageRank ( Queryable.AsQueryable [ for i in 0 .. 100 -> Edge(source = i, target = i % 7) ], 
                       Queryable.AsQueryable [ for i in 0 .. 100 -> Rank(source = i, value = i+100) ])


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

