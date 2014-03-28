// #Regression #Libraries #Collections
// Dev11:19569 - this used to throw an ArgumentException saying Object didn't implement IComparable

let m = Map.ofArray [| 1, obj() |]
exit <| if (m = m) then 0 else 1
