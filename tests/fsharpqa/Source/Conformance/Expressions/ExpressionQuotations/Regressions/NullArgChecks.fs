// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:5643
// Call all the Expr factories with null to any args that accept it from normal F# (ex Method/PropertyInfos, not Expr/Var)
open System
open Microsoft.FSharp.Quotations

// will swallow exceptions that represent graceful failures and spit back ones that don't (ex. ArgumentNullException, NullRef)
let throws f = 
    try f() with 
            | e -> let e2 = match e with | :? System.Reflection.TargetInvocationException -> e.InnerException | _ -> e
                   match e2 with
                   | :? ArgumentNullException | :? ArgumentException | :? InvalidOperationException -> () | _ -> raise e2

let etyp = typeof<Microsoft.FSharp.Quotations.Expr>
let exprMethods = etyp.GetMethods(System.Reflection.BindingFlags.Static ||| System.Reflection.BindingFlags.Public)
let result =
    try
        for m in exprMethods do
            let hasArgsThatCanBeNull = m.GetParameters() |> Array.exists(fun p -> not (p.ParameterType.Name.Contains("FSharp") || p.ParameterType.Name.Contains("Expr") || p.ParameterType.Name.Contains("Var")))
            if hasArgsThatCanBeNull then
                if m.Name <> "Deserialize" && m.Name <> "Cast" && m.Name <> "RegisterReflectedDefinitions" then // these aren't Expr factories like the others
                    let parameters = m.GetParameters()
                    // create an array of args to pass, using null whenever possible
                    let args = seq { for p in parameters -> 
                                        if p.ParameterType = typeof<Expr> then 
                                            Expr.Value(0, typeof<int>) :> Object
                                        else if p.ParameterType = typeof<Var> then
                                            Var("x", typeof<obj>) :> Object
                                        else if p.ParameterType = typeof<List<Expr>> then
                                            [Expr.Value(0, typeof<int>)] :> Object 
                                        else
                                            null  :> Object
                                    } |> Seq.toArray
                    throws (fun _ -> m.Invoke(null, args |> Seq.toArray) |> ignore)
        0
    with
    | e -> 1
    
exit <| result
