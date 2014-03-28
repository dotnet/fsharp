// #NoMono #NoMT #CodeGen #EmittedIL #NETFX20Only #NETFX40Only 
#light
              
let TestFunction14() =
    List.map (fun f -> f 2) [(fun x -> x + 1)] 
