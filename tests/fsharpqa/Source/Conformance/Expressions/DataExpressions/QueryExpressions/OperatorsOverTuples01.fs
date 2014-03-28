// #Conformance #DataExpressions #Query
// Sanity check operators over a sequence of tuples.

open System
open System.Linq

let CustomersTuples1 = [
        ("Jeff", 34)
        ("Annie", 19) 
        ("Abed", 25)
        ("Troy", 25)
        ("Pierce", 62)
        ("Britta", 26) ]
let CustomersTuples2 = CustomersTuples1 |> Array.ofList
let CustomersTuples3 = CustomersTuples1 |> Seq.ofList
let CustomersTuples4 = CustomersTuples3.AsParallel()

let queries (dataSource : seq<_>) =
    // For/Yield
    let q1 = 
        query {
            for c in dataSource do
            yield c
        }
    if (q1 |> Seq.length <> 6) then printfn "Failed on for/yield"; exit 1

    // Where
    let q2 =
        query {
            for (n,a) in dataSource do
            where (a > 25)
            yield n
        }
    if (q2 |> Seq.length <> 3) then printfn "Failed on where"; exit 1

    // sumBy
    let q3 =
        query {
            for (n,a) in dataSource do
            sumBy a
        }
    if q3 <> 191 then printfn "Failed on sumBy"; exit 1

    // maxBy
    let q4 =
        query {
            for (n,a) in dataSource do
            maxBy a
        }
    if q4 <> 62 then printfn "Failed on maxBy"; exit 1

    // averageBy
    let q5 =
        query {
            for (n,a) in dataSource do
            averageBy (float a)
        } |> int
    if q5 <> 31 then printfn "Failed on averageBy"; exit 1

    // sortBy
    let q6 =
        query {
            for (n,a) in dataSource do
            sortBy (a)
            select n
        }
    if (q6 |> Seq.head) <> "Annie" then printfn "Failed on sortBy"; exit 1

    // thenBy
    let q7 =
        query {
            for (n,a) in dataSource do
            sortBy (a)
            thenBy (n)
        }
    if (q7 |> Seq.head) <> ("Annie",19) then printfn "Failed on theBy"; exit 1

    // sortByDescending
    let q8 =
        query {
            for (n,a) in dataSource do
            sortByDescending (a)
            select n
        }
    if (q8 |> Seq.head) <> "Pierce" then printfn "Failed on sortByDescending"; exit 1

    // thenByDescending
    let q9 =
        query {
            for (n,a) in dataSource do
            sortByDescending (a)
            thenByDescending (n)
            select n
        }
    if (q9 |> Seq.head) <> "Pierce" then printfn "Failed on theByDescending"; exit 1

    // exists
    let q10 =
        query {
            for (n,a) in dataSource do
            exists (n = "Pierce")
        }
    if not q10 then printfn "Failed on exists"; exit 1

    // all
    let q11 =
        query {
            for (n,a) in dataSource do
            all (a > 18)
        }
    if not q11 then  printfn "Failed on all"; exit 1

    // nested query
    let q12 =
        query {
            for (n,a) in dataSource do
            let q =
                query {
                    for (n1,a1) in dataSource do
                    where (a1 > 26)
                    select n1
                }      
            yield (n,q)      
        }
    if (q12 |> Seq.length) <> 6 then printfn "Failed on nested query"; exit 1

    // yield!
    let q13 =
        query {
            for (n,a) in dataSource do
            let q =
                query {
                    for (n1,a1) in dataSource do
                    where (a1 > 26)
                    select n1
                }      
            yield! q    
        }
    if (q13 |> Seq.length) <> 12 then printfn "Failed on yield!"; exit 1

    // distinct
    let q14 = 
        query {
            for (n,a) in dataSource do
            let q =
                query {
                    for d in [1;1;2;2;3;3;3] do
                    distinct
                }    
            select (n,q)
        }
    let c,q = (q14 |> Seq.head)
    if (q |> Seq.length <> 3) then  printfn "Failed on distinct"; exit 1

    // groupBy
    let q15 = 
        query {
            for c in dataSource do
            groupBy c
        }
    
    if (q15 |> Seq.head).Key <> ("Jeff",34) then printfn "Failed on groupBy"; exit 1
    if q15 |> Seq.length <> 6 then exit 1

    // last
    let q16 = 
        query {
            for c in dataSource do
            last
        }

    if q16 <> ("Britta",26) then printfn "Failed on last"; exit 1

    // lastOrDefault
    let q17 = 
        query {
            for c in ([] : int list) do
            lastOrDefault
        }

    if q17 <> 0 then printfn "Failed on lastOrDefault"; exit 1

    // head
    let q18 = 
        query {
            for c in dataSource do
            head
        }

    if q18 <> ("Jeff", 34) then printfn "Failed on head"; exit 1

    // headOrDefault
    let q19 = 
        query {
            for c in ([] : int list) do
            headOrDefault
        }

    if q19 <> 0 then printfn "Failed on headOfDefault"; exit 1

    // exactlyOne
    let q20 = 
        query {
            for c in [(dataSource |> Seq.head)] do
            exactlyOne
        }

    if q20 <> ("Jeff", 34) then printfn "Failed on exactlyOne";  exit 1
    
    try
        let q21 = 
            query {
                for c in dataSource do
                exactlyOne
            }
        printfn "Failed on exactlyOne"
        exit 1
    with
        :? InvalidOperationException -> ()        

    try
        let q22 = 
            query {
                for c in ([] : int list) do
                exactlyOne
            }
        printfn "Failed on exactlyOne"
        exit 1
    with
        :? InvalidOperationException -> ()        

    // exactlyOneOrDefault
    try
        let q23 = 
            query {
                for c in dataSource do
                exactlyOneOrDefault
            }
        printfn "Failed on exactlyOneOrDefault"
        exit 1
    with
        :? InvalidOperationException -> ()        


    let q24 = 
        query {
            for c in ([] : int list) do
            exactlyOneOrDefault
        }

    if q24 <> 0 then printfn "Failed on exactlyOneOrDefault";  exit 1

    let q25 = 
        query {
            for c in [(dataSource |> Seq.head)] do
            exactlyOneOrDefault
        }

    if q25 <> ("Jeff", 34) then printfn "Failed on exactlyOneOrDefault";   exit 1

    // count
    let q26 = 
        query {
            for c in dataSource do
            count
        }

    if q26 <> 6 then printfn "Failed on count";   exit 1

    // nth
    let q27 = 
        query {
            for c in dataSource do
            nth 3
        }

    if q27 <> ("Troy", 25) then printfn "Failed on nth";   exit 1

    // skip
    let q28 = 
        query {
            for c in dataSource do
            skip 2
        }

    if q28 |> Seq.length <> 4 then printfn "Failed on skip";   exit 1

    // skipWhile
    let q29 = 
        query {
            for (n,a) in dataSource do
            skipWhile (a > 26)
        }
    
    if q29 |> Seq.length <> 5 then printfn "Failed on skipWhile";   exit 1

    // take
    let q30 = 
        query {
            for c in dataSource do
            take 2
        }

    if q30 |> Seq.length <> 2 then printfn "Failed on take";   exit 1

    // takeWhile
    let q31 = 
        query {
            for (n,a) in dataSource do
            takeWhile (a > 20)
        }

    if q31 |> Seq.length <> 1 then printfn "Failed on takeWhile";   exit 1

    // find
    let q33 = 
        query {
            for (n,a) in dataSource do
            find (a < 26 && a > 20)
        }

    if q33 <> ("Abed", 25) then printfn "Failed on find";   exit 1

    // minBy
    let q34 = 
        query {
            for (n,a) in dataSource do
            minBy a
        }

    if q34 <> 19 then printfn "Failed on minBy";   exit 1

    // maxBy
    let q35 = 
        query {
            for (n,a) in dataSource do
            maxBy a
        }

    if q35 <> 62 then printfn "Failed on maxBy";   exit 1

    // groupValBy
    let q35 = 
        query {
            for c in dataSource do
            let n,a = c
            groupValBy c a
        } |> Seq.toArray
    let r = q35.[2] |> Seq.toArray
    if q35.Length <> 5 || r.Length <> 2 then printfn "Failed on groupValBy";   exit 1

queries CustomersTuples1
queries CustomersTuples2
queries CustomersTuples3
queries CustomersTuples4