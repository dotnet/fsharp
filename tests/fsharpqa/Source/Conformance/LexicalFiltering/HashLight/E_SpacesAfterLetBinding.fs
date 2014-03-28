// #Regression #Conformance #LexFilter 
// Verify you get a warning when you put spaces after a let-binding
// Regression from FSB 1829

//<Expects id="FS0010" status="error" span="(10,6)">Unexpected identifier in binding\. Expected incomplete structured construct at or before this point or other token\.$</Expects>
//<Expects status="error" span="(9,5)" id="FS3118">Incomplete value or function definition\. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword\.$</Expects>

let simpleFunc() =
    let mutable counter = 0 
     counter <- counter + 1
    counter

exit 0
