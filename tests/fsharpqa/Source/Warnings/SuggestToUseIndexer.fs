// #Warnings
//<Expects status="Error" span="(6,9)" id="FS3217">This value is not a function and cannot be applied. But the given value has an indexer. Did you intend to call obj.[index] instead of obj[index]?</Expects>
//<Expects status="Error" span="(8,9)" id="FS3217">This value is not a function and cannot be applied.$</Expects>

let d = [1,1] |> dict
let y = d[1]

let z = d[|1|]

exit 0