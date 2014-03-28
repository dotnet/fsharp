// #Conformance #Quotations 
#light

// FSB 959, TOp_asm in pattern match

open Microsoft.FSharp

let quotation = 
    <@@ match ("a", "b") with
        | "a", sth -> sth
        | sth, "b" -> sth
        | _ ->        "nada" @@>
  
// Originally this code would cause a compile error.
exit 0
