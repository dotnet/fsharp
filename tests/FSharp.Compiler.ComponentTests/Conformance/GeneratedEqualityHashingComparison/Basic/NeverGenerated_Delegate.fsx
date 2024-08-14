// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Check thru reflection the set of methods
// Delegate
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
type D<'a> = delegate of obj * 'a -> unit 

let r = TestListOfMethods typeof<D<_>> [|"BeginInvoke"; "Clone"; "DynamicInvoke"; "EndInvoke"; "Equals"; "get_Method"; "get_Target"; "GetHashCode"; "GetInvocationList"; "GetObjectData"; "GetType"; "Invoke"; "ToString"|]

(if r then 0 else 1) |> exit
