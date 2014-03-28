module Neg59


let dbOne = [[1]]

let joinMustUseASimplePattern = 
    query { for i in dbOne do 
            // Join must use a simple pattern
            join [i] in dbOne on (i = j)
            select (i, j) } |> Seq.toList

let groupJoinMustUseASimplePattern = 
    query { for i in dbOne do 
            // Join must use a simple pattern
            groupJoin [i] in dbOne on (i = j) into group
            select (i, group) } |> Seq.toList

let functionDefinitionInQuery = 
    query { for i in dbOne do 
              let f (x:int) (y:int) = x 
              select i }

let recFunctionDefinitionInQueryWithCustomOperator = 
    query { for i in dbOne do 
              let rec f (x:int) (y:int) = f y x
              select i }


let recValueInQueryWithCustomOperator = 
    query { for i in dbOne do 
              let rec x = ((); (fun () -> x() + 1))
              select i }

let recFunctionDefinitionInQueryNoCustomOperator = 
    query { for i in dbOne do 
              let rec f (x:int) (y:int) = f y x
              yield i }


let recValueInQueryNoCustomOperator = 
    query { for i in dbOne do 
              let rec x = ((); (fun () -> x() + 1))
              yield i }



let ifThenElseInQueryWithCustomOperator = 
    query { for i in dbOne do 
              if true then
                select i 
              else 
                select (i+1) }

let ifThenElseInQueryNoCustomOperator = 
    query { for i in dbOne do 
              if true then
                yield i 
              else 
                yield (i+1) }

let matchInQueryWithCustomOperator = 
    query { for i in dbOne do 
              match 1 with 
              | 1 -> select i 
              | _ -> select (i+1) }

let matchInQueryNoCustomOperator = 
    query { for i in dbOne do 
              match 1 with 
              | 1 -> yield i 
              | _ -> yield (i+1) }


let tryWithInQueryWithCustomOperator = 
    query { for i in dbOne do 
              try 
                select i 
              with _ -> () }

let tryWithInQueryNoCustomOperator = 
    query { for i in dbOne do 
              try 
                yield i 
              with _ -> () }


let tryFinallyInQueryWithCustomOperator = 
    query { for i in dbOne do 
              try 
                select i 
              finally () }

let tryFinallyInQueryNoCustomOperator = 
    query { for i in dbOne do 
              try 
                yield i 
              finally () }


let useInQueryWithCustomOperator = 
    query { for i in dbOne do 
              use x = { new IDisposable with x.Dispose() = () }
              select i  }


let useInQueryNoCustomOperator = 
    query { for i in dbOne do 
              use x = { new IDisposable with x.Dispose() = () }
              yield i }

let whileInQueryWithCustomOperator = 
    query { for i in dbOne do 
              while true do 
                select i }

let whileInQueryNoCustomOperator = 
    query { for i in dbOne do 
              while true do 
                yield i }


let returnInQuery1 = 
    query { for i in dbOne do 
                return i }

let returnFromInQuery1 = 
    query { for i in dbOne do 
                return! i }




