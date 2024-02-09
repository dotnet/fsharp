// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Lists   
// Regression test for FSHARP1.0:4058
module ListExpressionSteppingTest2
module ListExpressionSteppingTest2 = 
    let f1 () = 
        [ printfn "hello"
          yield 1
          printfn "goodbye"
          yield 2]

    let _ = f1()

    // Test debug point generation for ||> and |||>
    let f2 x =
        let xs1 =
           ([x;x;x], [0..2])
           ||> List.zip
           |> List.map (fun (a,b) -> a, b+1)
           |> List.map (fun (a,b) -> a, b+1)

        let xs2 =
           ([x;x;x], [0..2], [0..2])
           |||> List.zip3
           |> List.map (fun (a,b,c) -> a, b+1, c)
           |> List.map (fun (a,b,c) -> a, b+1, c)

        xs1, xs2

    let _ = f2 5

