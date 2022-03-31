// #NoMono #NoMT #CodeGen #EmittedIL   
#light
              
let TestFunction14() =
    List.map (fun f -> f 2) [(fun x -> x + 1)] 
