// #Regression #Conformance #LexFilter #Exceptions 
// Regression test for FSHARP1.0:5205
// Indentation rules (this should have worked, but it has been made by design)
//<Expects span="(10,20-10,21)" status="error" id="FS0010">Unexpected symbol '\[' in binding\. Expected incomplete structured construct at or before this point or other token\.$</Expects>
//<Expects status="error" span="(8,1-8,4)" id="FS3118">Incomplete value or function definition\. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword\.$</Expects>
//<Expects span="(11,1-11,1)" status="error" id="FS0010">Incomplete structured construct at or before this point in implementation file$</Expects>

let fccccccc xx = List.iter (fun z -> 
        match 1 with
        | _ -> ()) [1;2]
