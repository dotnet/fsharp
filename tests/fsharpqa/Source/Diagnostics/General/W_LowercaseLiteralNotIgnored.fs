// #Regression #Diagnostics 
//<Expects status="warning" span="(11,7-11,16)" id="FS0026">This rule will never be matched\.$</Expects>
module M0

module M1 =
  let [<Literal>] lowerCase = "lowerCase"
  let [<Literal>] UpperCase = "UpperCase"

module M2 =
  let f = function
    | M1.lowerCase -> "LowerCase"
    | lowerCase2 -> "LowerCase2"
    | _ -> "Don't know"

printfn "%A" (M2.f "B")
