// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Check thru reflection the set of methods
// Record
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
type R = { A : int }

// Notice there are 2 CompareTo's, 2 Equals's and 1 GetStructuralHashCode
let r = TestListOfMethods typeof<R> [|"CompareTo"; "CompareTo"; "GetHashCode"; "GetHashCode"; "CompareTo"; "get_A"; "Equals"; "Equals"; "Equals"; "ToString"; "GetType"|]

(if r then 0 else 1) |> exit
