// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Check thru reflection the set of methods
// Enum
//<Expects status="success"></Expects>

         
let TestListOfMethods (typ : System.Type) (exp : string array) = 
                                                            Array.sortInPlaceWith (fun a b -> System.String.Compare(a,b)) exp
                                                            let actual = typ.GetMethods() |> Array.map (fun a -> a.Name)
                                                            Array.sortInPlaceWith (fun a b -> System.String.Compare(a,b)) actual
                                                            let r = actual = exp
                                                            if not r then 
                                                                            printfn "Expected: %A" exp
                                                                            printfn "Actual: %A" actual
                                                            r
type E = | E1 = 1 | E2 = 2

// Determine baseline based on the runtime
let netfx40 = [|"CompareTo"; "Equals"; "GetHashCode"; "GetType"; "GetTypeCode"; "HasFlag"; "ToString"; "ToString"; "ToString"; "ToString"|]
let netfx20 = [|"CompareTo"; "Equals"; "GetHashCode"; "GetType"; "GetTypeCode";            "ToString"; "ToString"; "ToString"; "ToString"|]
let baseline = if (System.Environment.Version.Major, System.Environment.Version.Minor) >= (4,0) then netfx40 else netfx20

let r = TestListOfMethods typeof<E> baseline

(if r then 0 else 1) |> exit
