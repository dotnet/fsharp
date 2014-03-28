// I can say typeof<provided interface>
//<Expects status="success"></Expects>

let t = typeof<N.I1>

exit <| if (t.GetMethods() |> Array.map (fun s -> s.Name)) = [|"ToString"; "Equals"; "Equals"; "ReferenceEquals"; "GetHashCode"; "GetType"|] then 0 else 1
