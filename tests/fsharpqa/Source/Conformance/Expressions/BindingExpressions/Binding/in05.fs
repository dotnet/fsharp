// #in #BindingExpressions
//
// These are pretty pathological cases related to ";;" and "in"
// I'm adding these cases to make sure we do not accidentally change the behavior from version to version
// Eventually, we will deprecated them - and the specs will be updated.
//<Expects status="error" span="(12,13-12,14)" id="FS0001">The type 'int' does not match the type 'unit'$</Expects>
//<Expects status="error" span="(12,18-12,24)" id="FS0001">Type mismatch\. Expecting a.    ''a -> 'b'    .but given a.    ''a -> unit'    .The type 'int' does not match the type 'unit'$</Expects>
//<Expects status="error" span="(10,5-13,10)" id="FS0020">This expression has a value of type 'bool' that is implicitly ignored\. Use the 'ignore' function to discard this value explicitly, e\.g\. 'expr \|> ignore', or bind it to a name to refer to it later, e\.g\. 'let result = expr'\.$</Expects>
module E = 
    let a = 3 in
        a + 1 |> ignore
        a + 1 |> ignore
    a > 4
