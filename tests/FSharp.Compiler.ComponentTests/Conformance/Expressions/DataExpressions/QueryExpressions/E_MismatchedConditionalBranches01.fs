// #Conformance #DataExpressions #Query #Regression
// DevDiv:196007, this used to throw
//<Expects status="error" span="(8,13-8,18)" id="FS3163">'match' expressions may not be used in queries$</Expects>

let x =
    query { 
        for i in [1..10] do 
            match i with 
            | 1 ->
                for j in [1..10] do
                    if i = 0 then yield i else yield i + 1
            | _ -> ()
    } |> Seq.toArray

exit 1


