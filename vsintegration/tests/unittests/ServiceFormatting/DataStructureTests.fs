// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.ListTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``array indices``() =
    formatSourceString false """
let array1 = [| 1; 2; 3 |]
array1.[0..2] 
array2.[2.., 0..]
array2.[..3, ..1] 
array1.[1] <- 3
    """ config
    |> prepend newline
    |> should equal """
let array1 = [| 1; 2; 3 |]

array1.[0..2]
array2.[2.., 0..]
array2.[..3, ..1]
array1.[1] <- 3
"""

[<Test>]
let ``array values``() =
    formatSourceString false """
let arr = [|(1, 1, 1); (1, 2, 2); (1, 3, 3); (2, 1, 2); (2, 2, 4); (2, 3, 6); (3, 1, 3);
  (3, 2, 6); (3, 3, 9)|]
    """ { config with SemicolonAtEndOfLine = true }
    |> prepend newline
    |> should equal """
let arr =
    [| (1, 1, 1);
       (1, 2, 2);
       (1, 3, 3);
       (2, 1, 2);
       (2, 2, 4);
       (2, 3, 6);
       (3, 1, 3);
       (3, 2, 6);
       (3, 3, 9) |]
"""

[<Test>]
let ``cons and list patterns``() =
    formatSourceString false """
let rec printList l =
    match l with
    | head :: tail -> printf "%d " head; printList tail
    | [] -> printfn ""

let listLength list =
    match list with
    | [] -> 0
    | [ _ ] -> 1
    | [ _; _ ] -> 2
    | [ _; _; _ ] -> 3
    | _ -> List.length list"""  config
    |> prepend newline
    |> should equal """
let rec printList l =
    match l with
    | head :: tail -> 
        printf "%d " head
        printList tail
    | [] -> printfn ""

let listLength list =
    match list with
    | [] -> 0
    | [ _ ] -> 1
    | [ _; _ ] -> 2
    | [ _; _; _ ] -> 3
    | _ -> List.length list
"""

[<Test>]
let ``array patterns``() =
    formatSourceString false """
let vectorLength vec =
    match vec with
    | [| var1 |] -> var1
    | [| var1; var2 |] -> sqrt (var1*var1 + var2*var2)
    | [| var1; var2; var3 |] -> sqrt (var1*var1 + var2*var2 + var3*var3)
    | _ -> failwith "vectorLength called with an unsupported array size of %d." (vec.Length)""" config
    |> prepend newline
    |> should equal """
let vectorLength vec =
    match vec with
    | [| var1 |] -> var1
    | [| var1; var2 |] -> sqrt (var1 * var1 + var2 * var2)
    | [| var1; var2; var3 |] -> sqrt (var1 * var1 + var2 * var2 + var3 * var3)
    | _ -> 
        failwith "vectorLength called with an unsupported array size of %d." 
            (vec.Length)
"""

[<Test>]
let ``should keep -> notation``() =
    formatSourceString false """let environVars target =
    [for e in Environment.GetEnvironmentVariables target ->
        let e1 = e :?> Collections.DictionaryEntry
        e1.Key, e1.Value]
    """ config
    |> prepend newline
    |> should equal """
let environVars target =
    [ for e in Environment.GetEnvironmentVariables target -> 
          let e1 = e :?> Collections.DictionaryEntry
          e1.Key, e1.Value ]
"""

[<Test>]
let ``list comprehensions``() =
    formatSourceString false """
let listOfSquares = [ for i in 1 .. 10 -> i*i ]
let list0to3 = [0 .. 3]""" config
    |> prepend newline
    |> should equal """
let listOfSquares =
    [ for i in 1..10 -> i * i ]

let list0to3 = [ 0..3 ]
"""

[<Test>]
let ``array comprehensions``() =
    formatSourceString false """
let a1 = [| for i in 1 .. 10 -> i * i |]
let a2 = [| 0 .. 99 |]  
let a3 = [| for n in 1 .. 100 do if isPrime n then yield n |]""" config
    |> prepend newline
    |> should equal """
let a1 =
    [| for i in 1..10 -> i * i |]

let a2 = [| 0..99 |]

let a3 =
    [| for n in 1..100 do
           if isPrime n then yield n |]
"""

[<Test>]
let ``should keep Array2D``() =
    formatSourceString false """
let cast<'a> (A:obj[,]):'a[,] = A |> Array2D.map unbox
let flatten (A:'a[,]) = A |> Seq.cast<'a>
let getColumn c (A:_[,]) = flatten A.[*,c..c] |> Seq.toArray""" config
    |> prepend newline
    |> should equal """
let cast<'a> (A : obj [,]) : 'a [,] = A |> Array2D.map unbox
let flatten (A : 'a [,]) = A |> Seq.cast<'a>
let getColumn c (A : _ [,]) = flatten A.[*, c..c] |> Seq.toArray
"""

[<Test>]
let ``should be able to support F# 3.1 slicing``() =
    formatSourceString false """
let x = matrix.[*, 3]
let y = matrix.[3, *]""" config
    |> prepend newline
    |> should equal """
let x = matrix.[*, 3]
let y = matrix.[3, *]
"""
