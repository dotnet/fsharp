// #NoMono #NoMT #CodeGen #EmittedIL 

              
let TestFunction15(inp) =
    let x = inp+1
    [1;2;3] |> List.map (fun x -> x + 1)
