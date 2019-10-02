// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:4527
//<Expects id="FS0035" span="(10,9-10,45)" status="error">.+'if ... then ... else'</Expects>

// warning FS0035: This construct is deprecated: This list or array
// expression includes an element of the form 'if ... then ... else'. Parenthesize
// this expression to indicate it is an individual element of the list or array, to
// disambiguate this from a list generated using a sequence expression.

let p = [ if true then printfn "hello"; () ];;
(if p = [ () ] then 0 else 1) |> exit