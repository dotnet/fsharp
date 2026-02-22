// #Regression #Diagnostics #Async
// Regression for FSHARP1.0:6095 - wrong line number for incomplete match warning inside async block
// Regression for DevDiv:108999
//<Expects status="error" id="FS0025" span="(10,10-10,22)">Incomplete pattern matches on this expression. For example, the value '\[|\_; \_; \_;\]' may indicate a case not covered by the pattern(s)\.</Expects>
//<Expects status="error" id="FS0025" span="(17,10-17,22)">Incomplete pattern matches on this expression. For example, the value '\[|\_; \_; \_;\]' may indicate a case not covered by the pattern(s)\.</Expects>
//<Expects status="error" id="FS0025" span="(24,5-24,12)">Incomplete pattern matches on this expression. For example, the value '\[|\_; \_; \_;\]' may indicate a case not covered by the pattern(s)\. Unmatched elements will be ignored\.</Expects>
//<Expects status="error" id="FS0025" span="(27,9-27,17)">Incomplete pattern matches on this expression. For example, the value '\[|\_; \_; \_;\]' may indicate a case not covered by the pattern(s)\.</Expects>

let a = async {
    let! [| r1; r2 |] = Async.Parallel [| async.Return(1); async.Return(2) |]
    return r1,r2 // array incomplete match reported on this line, rather than lines above
    }



let a1 = async {
    let! [| r1; r2 |] = Async.Parallel [| async.Return(1); async.Return(2) |]
    let y = 4    // squiggle spans this line and next
    return r1,r2 // array incomplete match reported on this line, rather than lines above
    }


let ars = [| [|1;2|] |]
for [|a;b|] in ars do
        ()
// squiggle too big
let f = function | [|a;b|] -> 2
// squiggle too big