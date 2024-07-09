// <testmetadata>
// { "optimization": { "reported_in": "#6984", "reported_by": "@teo-tsirpanis", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>
let factorialOk1 x =
  let rec impl x acc =
    if x = 0UL then
      acc
    else
      impl (x - 1UL) (x * acc)
  impl x 1UL

let factorialOk2 x =
  let rec impl x acc =
    if x = 0UL then
      acc
    else
      impl <|| (x - 1UL, x * acc)
  impl x 1UL
  
let factorialNotOk1 x =
    let rec impl x acc =
        if x = 0UL then
            acc
        else
            impl (x - 1UL) <| x * acc
    impl x 1UL
    
let factorialNotOk2 x =
  let rec impl x acc =
    if x = 0UL then
      acc
    else
      (impl (x-1UL)) (x * acc)
  impl x 1UL

System.Console.WriteLine()