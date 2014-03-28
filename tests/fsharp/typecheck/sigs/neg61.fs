
module Neg61

let b2 = 
    query { for x in [1] do
            join y in [2] on (x = y) 
            select x }
let b3 = 
    query { for x in [1] do
            groupJoin y in [2] on ( x < y) into g
            select x }
let b4 = 
    query { for x in [1] do
            groupJoin y in [2] on ( x = y)
            select x }
let a0 = 
    query { for x in [1] do
            groupJoin y in [2] on ( x = y)
            select x }
let a1 = 
    query { for x in [1] do
            zip [2] 
            select x }
let a2 = 
    query { for x in [1] do
            select }

let a3 = 
    query { for x in [1] do
            zip }

let a4 = 
    query { for x in [1] do
            groupJoin }

let x0 = 
    query { for x in [1] do
            join }

let x1 = 
    query { for x in [1] do
            id select }


let x2 = 
    query { for x in [1] do
            id join }


let x3 = 
    query { for x in [1] do
            id groupJoin }

let x4 = 
    query { for x in [1] do
            id zip }

let x5 = 
    query { for c in [1..10] do
            truncate }   

let x6 = 
    query { for c in [1..10] do
            printfn "hello"
            yield 1 }   

let x7 = 
    query { for c in [1..10] do
            while true do 
              yield 1 }   

let x8 = 
    query { for c in [1..10] do
            for i = 1 to 100 do 
              yield 1 }   

let x9 = 
    query { for c in [1..10] do
            try 
               yield 1
            with _ -> 
               yield 2  }   

let x10 = 
    query { for c in [1..10] do
            try 
               yield 1
            finally ()  }   

let x11 = 
    query { for c in [1..10] do
            use x = { new System.IDisposable with __.Dispose() = () }
            yield 1  }   

let x12 = 
    query { for c in [1..10] do
            let! x = failwith ""
            yield 1  }   

let x13 = 
    query { for c in [1..10] do
            do! failwith ""
            yield 1  }   

let x14 = 
    query { for c in [1..10] do
            return 1 }   

let x15 = 
    query { for c in [1..10] do
            return! [1] }   
let x16 = 
    query { for c in [1..10] do
            truncate 3 }   

let x17ok = 
    query {
        for d in [1..10] do
        let f x = x + 1 // no error expected here
        select (f d)
    }

let x18ok = 
    query {
        for d in [1..10] do
        let f x = x + 1 // no error expected here
        select (f d)
    }

let x18rec = 
    query {
        for d in [1..10] do
        let rec f x = x + 1 // error expected here - no recursive functions
        select (f d)
    }

let x18rec2 = 
    query {
        for d in [1..10] do
        let rec f x = x + 1 // error expected here - no recursive functions
        and g x = f x + 2
        select (f d)
    }

let x18inline = 
    query {
        for d in [1..10] do
        let inline f x = x + 1 // error expected here - no inline functions
        select (f d)
    }


let x18mutable = 
    query {
        for d in [1..10] do
        let mutable v = 1 // error expected here - no mutable values
        select (f d)
    }

let x19 = 
    query {
        for d in [1..10] do
        let r =   // error expected here - no generic functions in quotations
            let add (x : 'a list) y = y :: x
            add
        select (r [] d)
    }

let x20 = 
    query { for c in [1..10] do
            sumBy }   

let x21 = 
    query { for c in 1 do 
            sumBy c }

// check misuse of binary operator in sequence position
let x23a = 
    query { for x in ["1"] do
            where x.Length > x
            select x }

// check misuse of binary operator in last position
let x23b = 
    query { for x in ["1"] do
            where x.Length > x }

// check misuse of binary operator in sequence position
let x24a = 
    query { for x in ["1"] do
            groupBy x.Length + 1
            select x }

// check misuse of binary operator in last position
let x24b = 
    query { for x in ["1"] do
            groupBy x.Length + 1 }

// check misuse of tuple in select position
let x24c = 
    query { for x in ["1"] do
            select x.Length, x }

