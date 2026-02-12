// #Conformance #DataExpressions #Query #Regression
// Dev11:188932, used to be errors trying to use let and other expressions between a for and join and such

open System.Linq

let f1 db db2 =
    query {
        for i in db do
        let x = i
        where (true)
        join j in db2 on (i = j)
        let y = i,j
        sortBy y
        take 3
    }

let f2 db db2 =
    query {
        for i in db do
        let x = i,i
        for j in db do
        let y = 
            match (x,j) with
            | _,_ -> x,j
        join k in db2 on (i=k)
        select (i,j,k)
    }

let f3 db db2 =
    query {
        for i in db do
        let x = i,i
        groupJoin j in db2 on (i=j) into g
        let g' = g,g
        select (i,g')
    }

let f4 db db2 =
    query {
        for i in db do
        let j = i,i
        leftOuterJoin k in db2 on (j=(k,k)) into g
        select g
    }

let f5 db db2 =
    query {
        for i in db do
        join j in db2 on (i = j)
        sortBy j
        groupJoin k in db on (j=k) into g
        where (true)
        for z in g do
        select z
    }

// Dev11:311334
//let f5 db db2 =
//    query {
//        for i in db do
//        groupJoin j in db2 on (i=j) into g
//        let z = g
//        groupJoin x in z on (j = x) into gg
//        select z
//    }

let db1 = [1..10].AsQueryable()
let db2 = [1..2..10].AsQueryable()

type Customer(name) =
    member this.Name = name
    override this.Equals(obj) = match obj with | :? Customer as c1 -> this.Name = c1.Name | _ -> false
    interface System.IComparable with
        override this.CompareTo(obj) = 
            match obj with 
            | :? Customer as c1 -> if this.Name = c1.Name then 0 else 1 
            | _ -> -1
let cdb1 = [Customer("Jeff"); Customer("Annie"); Customer("Britta"); Customer("Troy"); Customer("Abed"); Customer("Pierce")].AsQueryable()
let cdb2 = [Customer("Jeff"); Customer("Annie"); Customer("Britta"); Customer("Troy"); Customer("Abed")].AsQueryable()

type RCustomer = { Name : string }
let rdb1 = [{ Name = "Jeff" }; { Name = "Annie" }; { Name = "Britta" }; { Name = "Troy" }; { Name = "Abed" }; { Name = "Pierce" }].AsQueryable()
let rdb2 = [{ Name = "Jeff" }; { Name = "Annie" }; { Name = "Britta" }; { Name = "Troy" }; { Name = "Abed" }].AsQueryable()

// just making sure they run, don't care about results
f1 db1 db2 |> ignore
f2 db1 db2 |> ignore
f3 db1 db2 |> ignore
f4 db1 db2 |> ignore
f5 db1 db2 |> ignore

f1 cdb1 cdb2 |> ignore
f2 cdb1 cdb2 |> ignore
f3 cdb1 cdb2 |> ignore
f4 cdb1 cdb2 |> ignore
f5 cdb1 cdb2 |> ignore

f1 rdb1 rdb2 |> ignore
f2 rdb1 rdb2 |> ignore
f3 rdb1 rdb2 |> ignore
f4 rdb1 rdb2 |> ignore
f5 rdb1 rdb2 |> ignore

exit 0