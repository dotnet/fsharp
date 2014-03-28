// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5695
// Quotations don't allow pattern matching on arrays inside quotations
//<Expects status="error" span="(9,23-9,24)" id="FS0452">Quotations cannot contain inline assembly code or pattern matching on arrays$</Expects>
//<Expects status="error" span="(20,23-20,24)" id="FS0452">Quotations cannot contain inline assembly code or pattern matching on arrays$</Expects>

let q = 
     <@ let x = [|1;2;3;4|]
        let y = match x with
                | [|x1;x2;x3|] -> Some(x3)
                | _ -> None
        x
     @>


let (|AP|) (x : 'a array) = x.[0..2]

let q2 = 
     <@ let x = [|1;2;3;4|]
        let y = match x with
                | AP [|x1;x2;x3|] -> Some(x3)
                | _ -> None
        x
     @>

exit 0
