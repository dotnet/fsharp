// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Check thru reflection the set of methods
// Classes
//<Expects status="success"></Expects>
#light
         
let TestListOfMethods (typ : System.Type) (exp : string array) = 
                                                            Array.sortInPlaceWith (fun a b -> System.String.Compare(a,b)) exp
                                                            let actual = typ.GetMethods() |> Array.map (fun a -> a.Name)
                                                            Array.sortInPlaceWith (fun a b -> System.String.Compare(a,b)) actual
                                                            let r = actual = exp
                                                            if not r then 
                                                                            printfn "Expected: %A" exp
                                                                            printfn "Actual: %A" actual
                                                            r

type C = class end

let r = TestListOfMethods typeof<C> [|"Equals"; "GetHashCode"; "GetType"; "ToString"|]

(if r then 0 else 1) |> exit
