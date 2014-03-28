// #TypeProvider #Regression
// Inheritance from a syntetic erased interface is not allowed, yet this weird thing may happen
// Somehow related to DevDiv:202021
//<Expects status="success"></Expects>

exit <| if (null :> N.I1).M() = [|1;2;3|] then 0 else 1

