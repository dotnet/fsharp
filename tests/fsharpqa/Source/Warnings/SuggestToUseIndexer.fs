// #Warnings
//<Expects status="Error" span="(7,9)" id="FS3217">This value is of type 'System.Collections.Generic.IDictionary<int,int>' which is not a function type. A value is being passed to it as an argument as if it were a function. Did you intend to access the indexer via d.\[index\] instead?</Expects>
//<Expects status="Error" span="(9,9)" id="FS0003">This value is of type 'System.Collections.Generic.IDictionary<int,int>', which is not a function type. A value is being passed to it as an argument as if it were a function.$</Expects>
//<Expects status="Error" span="(12,10)" id="FS3217">This value is of type 'System.Collections.Generic.IDictionary<int,int>' which is not a function type. A value is being passed to it as an argument as if it were a function. Did you intend to access the indexer via expr.\[index\] instead?</Expects>

let d = [1,1] |> dict
let y = d[1]

let z = d[|1|]

let f() = d
let a = (f())[1]

exit 0