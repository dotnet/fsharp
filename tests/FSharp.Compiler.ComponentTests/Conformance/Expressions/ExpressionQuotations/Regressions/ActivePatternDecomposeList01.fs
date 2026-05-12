// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:4918
// Ensure we can decompose a list returned from an active pattern inside a quotation

let (|RefCell|) x = !x 
let q = 
    <@@ let xs = ref [1.;2.] 
        match xs with 
        | RefCell [v1_b; v2_b] -> v1_b + v2_b 
        | _ -> failwith "?" 
     @@> 

let xs = ref [1.0; 2.0] 
let ys = (|RefCell|) xs
let result =
    if (match ys with _::_ -> true | _ -> false) then 
        if (match ys.Tail with _::_ -> true | _ -> false) then 
            if (match (ys.Tail).Tail with [] -> true | _ -> false) then 
                (fun v2_b v1_b -> v2_b + v1_b) ((ys.Tail).Head) (ys.Head) 
            else 
                -1.0 
        else 
            -1.0
    else 
        -1.0
        
exit <| match result with
        | 3.0 -> 0
        | _ -> 1
