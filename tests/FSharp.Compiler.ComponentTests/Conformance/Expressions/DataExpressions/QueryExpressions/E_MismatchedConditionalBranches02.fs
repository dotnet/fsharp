// #Conformance #DataExpressions #Query #Regression
//<Expects status="error" span="(6,14-6,19)" id="FS3163">'match' expressions may not be used in queries$</Expects>

let q10 =
     query { for i in [1..10] do 
             match i with 
             | 8 -> 
                 for j in [1..10] do 
                     if i = j then 
                         yield i
             | _ -> () }

exit 1
