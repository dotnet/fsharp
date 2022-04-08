// #NoMono #NoMT #CodeGen #EmittedIL 
#light

let q = Seq.forall  (fun s -> 
                              if not true then ()
                              true
                    ) [ 1 ]
