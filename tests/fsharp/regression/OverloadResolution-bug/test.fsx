(*****************  Repro for issue#5246 ---- https://github.com/Microsoft/visualfsharp/issues/5246         *****************)
(*
    When the bug was present we saw this error:

    Compile errors are raised:
    error FS0041: A unique overload for method 'ofObject' could not be determined based on type 
                  information prior to this program point. A type annotation may be needed. 
                      Candidates: static member Methods.ofObject : t0:'a -> Methods option when 'a : null, 
                      static member Methods.ofObject : t1:'a -> Methods option when 'a :> ITest1 and 'a : null

    Expected behavior
        The code should compile fine (possibly with a warning on :? obj always succeeding).

*)

module TestOfObj =

    type [<AllowNullLiteral>] ITest1 = interface end

    type Methods =
        | Test1 of ITest1
        | Other of obj

        static member ofObject t1 = Option.ofObj t1 |> Option.map Test1
        static member ofObject t0 = Option.ofObj t0 |> Option.map Other

        static member convert (x: obj) =
            match x with
            | :? ITest1 as one -> Methods.ofObject one
            | :? obj as one -> Methods.ofObject one
            | _ -> None


    printf "TEST PASSED OK" ;
    printfn "Succeeded"
