// #Conformance #DataExpressions #Query
// Sanity check operators over a sequence of classes

open System
open System.Linq

type Customer(name : string, age : int) =
    member this.Name = name
    member this.Age = age

let CustomerClass1 = [ Customer("Jeff", 34); Customer("Annie", 19); Customer("Abed", 25); Customer("Troy", 25); Customer("Pierce", 62); Customer("Britta",26) ]
let CustomerClass2 = [| Customer("Jeff", 34); Customer("Annie", 19); Customer("Abed", 25); Customer("Troy", 25); Customer("Pierce", 62); Customer("Britta",26) |]
let CustomerClass3 = CustomerClass2 |> Seq.ofArray
let CustomerClass4 = CustomerClass1.AsParallel()

let queries (dataSource : seq<Customer>) =
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
            for c in dataSource do
            where (c.Age > 25)
            yield c
        }
    if (q2 |> Seq.length <> 3) then printfn "Failed on where"; exit 1

    // sumBy
    let q3 =
        query {
            for c in dataSource do
            sumBy c.Age
        }
    if q3 <> 191 then printfn "Failed on sumBy"; exit 1

    // maxBy
    let q4 =
        query {
            for c in dataSource do
            maxBy c.Age
        }
    if q4 <> 62 then printfn "Failed on maxBy"; exit 1

    // averageBy
    let q5 =
        query {
            for c in dataSource do
            averageBy (float c.Age)
        } |> int
    if q5 <> 31 then printfn "Failed on averageBy"; exit 1

    // sortBy
    let q6 =
        query {
            for c in dataSource do
            sortBy (c.Age)
        }
    if (q6 |> Seq.head).Name <> "Annie" then printfn "Failed on sortBy"; exit 1

    // thenBy
    let q7 =
        query {
            for c in dataSource do
            sortBy (c.Age)
            thenBy (c.Name)
        }
    if (q7 |> Seq.head).Name <> "Annie" then printfn "Failed on theBy"; exit 1

    // sortByDescending
    let q8 =
        query {
            for c in dataSource do
            sortByDescending (c.Age)
            select c
        }
    if (q8 |> Seq.head).Name <> "Pierce" then printfn "Failed on sortByDescending"; exit 1

    // thenByDescending
    let q9 =
        query {
            for c in dataSource do
            sortByDescending (c.Age)
            thenByDescending (c.Name)
            select c
        }
    if (q9 |> Seq.head).Name <> "Pierce" then printfn "Failed on theByDescending"; exit 1

    // exists
    let q10 =
        query {
            for c in dataSource do
            exists (c.Name = "Pierce")
        }
    if not q10 then printfn "Failed on exists"; exit 1

    // all
    let q11 =
        query {
            for c in dataSource do
            all (c.Age > 18)
        }
    if not q11 then  printfn "Failed on all"; exit 1

    // nested query
    let q12 =
        query {
            for c in dataSource do
            let q =
                query {
                    for d in dataSource do
                    where (d.Age > 26)
                    select d
                }      
            yield (c,q)      
        }
    if (q12 |> Seq.length) <> 6 then printfn "Failed on nested query"; exit 1

    // yield!
    let q13 =
        query {
            for c in dataSource do
            let q =
                query {
                    for d in dataSource do
                    where (d.Age > 26)
                    select d
                }      
            yield! q    
        }
    if (q13 |> Seq.length) <> 12 then printfn "Failed on yield!"; exit 1

    // distinct
    let q14 = 
        query {
            for c in dataSource do
            let q =
                query {
                    for d in [1;1;2;2;3;3;3] do
                    distinct
                }    
            select (c,q)
        }
    let c,q = (q14 |> Seq.head)
    if (q |> Seq.length <> 3) then  printfn "Failed on distinct"; exit 1

    // groupBy
    let q15 = 
        query {
            for c in dataSource do
            groupBy c
        }
    
    if (q15 |> Seq.head).Key.Name <> "Jeff" then printfn "Failed on groupBy"; exit 1
    if q15 |> Seq.length <> 6 then exit 1

    // last
    let q16 = 
        query {
            for c in dataSource do
            last
        }

    if q16.Name <> "Britta" then printfn "Failed on last"; exit 1

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

    if q18.Name <> "Jeff" then printfn "Failed on head"; exit 1

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

    if q20.Name <> "Jeff" then printfn "Failed on exactlyOne";  exit 1
    
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

    if q25.Name <> "Jeff" then printfn "Failed on exactlyOneOrDefault";   exit 1

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

    if q27.Name <> "Troy" then printfn "Failed on nth";   exit 1

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
            for c in dataSource do
            skipWhile (c.Age > 26)
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
            for c in dataSource do
            takeWhile (c.Age > 20)
        }

    if q31 |> Seq.length <> 1 then printfn "Failed on takeWhile";   exit 1

    // find
    let q33 = 
        query {
            for c in dataSource do
            find (c.Age < 26 && c.Age > 20)
        }

    if q33.Name <> "Abed" then printfn "Failed on find";   exit 1

    // minBy
    let q34 = 
        query {
            for c in dataSource do
            minBy c.Age
        }

    if q34 <> 19 then printfn "Failed on minBy";   exit 1

    // maxBy
    let q35 = 
        query {
            for c in dataSource do
            maxBy c.Age
        }

    if q35 <> 62 then printfn "Failed on maxBy";   exit 1

    // groupValBy
    let q35 = 
        query {
            for c in dataSource do
            groupValBy c c.Age
        } |> Seq.toArray
    let r = q35.[2] |> Seq.toArray
    if q35.Length <> 5 || r.Length <> 2 then printfn "Failed on groupValBy";   exit 1

queries CustomerClass1
queries CustomerClass2
queries CustomerClass3
queries CustomerClass4