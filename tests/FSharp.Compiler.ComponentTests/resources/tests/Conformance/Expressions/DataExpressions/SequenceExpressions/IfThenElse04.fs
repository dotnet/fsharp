// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:4527
//<Expects status="success"></Expects>

let p = [ (if true then
                    [for i = 1 to 10 do yield i]        // yield is not immediately under the 'then' branch,
                                                        // so this is not a candidate for a seq expr
                  else
                    [for i = 1 to 10 do yield i]        // yield is not immediately under the 'else' branch,
                                                        // so this is not a candidate for a seq expr
           )
        ]

(if List.length p = 1 then 0 else 1) |> exit
