// #Warnings
//<Expects status="Error" span="(6,9)" id="FS3217">This value is not a function and cannot be applied. Did you intend to access the indexer via d.[index] instead?</Expects>
//<Expects status="Error" span="(8,9)" id="FS3217">This value is not a function and cannot be applied.$</Expects>
//<Expects status="Error" span="(12,9)" id="FS3217">This value is not a function and cannot be applied. Did you intend to access the indexer via expr.[index] instead?"</Expects>

let d = [1,1] |> dict
let y = d[1]

let z = d[|1|]

let f() = d
let y = (f())[1]

exit 0