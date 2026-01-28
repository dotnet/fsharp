// #Regression #Diagnostics 
//<Expects status="warning" span="(10,5-10,14)" id="FS3190">Lowercase literal 'lowerCase' is being shadowed by a new pattern with the same name\. Only uppercase and module-prefixed literals can be used as named patterns\.$</Expects>
module M

let [<Literal>] lowerCase = "lowerCase"
let [<Literal>] UpperCase = "UpperCase"

let f = function
  | UpperCase -> "UpperCase"
  | lowerCase -> "LowerCase"

f "A" |> ignore

