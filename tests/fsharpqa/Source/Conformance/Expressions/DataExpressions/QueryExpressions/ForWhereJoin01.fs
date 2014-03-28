// #Conformance #DataExpressions #Query #Regression
// DevDiv:179139, we used to give a bad error on intervening expressions between join/groupJoin and a for loop
// Note: as of 10/26/2011, it is ok to have where before join/groupJoin!
//<Expects status="success"></Expects>

let q1 = 
    query {
        for i in [1..10] do
        where (i > 2)
        join f in [1..10] on (i = f)
        yield (i,f)
    } |> Seq.toArray

let q2 =
    query {
        for i in [1..10] do
        where (i > 3)
        groupJoin f in [1..10] on (i = f) into prods
        yield prods
    } |> Seq.toArray
