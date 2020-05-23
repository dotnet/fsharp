// #Globalization 
#light

namespace हताहत 

module पिछले =
  // Function
  let फ़ैसला बस = बस + 1

  // Enum
  type ख़तरनाक  = 
                | अलगाववादी = 1 
                | मिलती = 2
  
  // DU
  type ख़तरxनाक = 
               | अलगाववादी     // There's no uppercase/lowercase in Hindi, ensure that a Hindi character will suffice to start the DU case name
               | मिलती of ख़तरनाक
               | X
  
  // Record
  type घोषणा = {  अलगाववादी : int; मिलती : ख़तरनाक }
  let r = {  अलगाववादी = 4; मिलती = ख़तरनाक.मिलती }

  // Class...
  type तत्कालिन() =
                class
                   member त.त्का() = 42
                   static member त्का(में) = में * में
                end

  // ... and corresponding object with method (static/instance) invocation        
  let o = new तत्कालिन()
  तत्कालिन.त्का(3) |> ignore
  o.त्का() |> ignore
  
  // Units of measure
  [<Measure>] type मिलती
  [<Measure>] type k2 = मिलती ^ 2
  
  
  // In a string and in a comment
  let p = (* पिछले *) "पिछले"             // पिछले is a comment
  let p2 = (* पिछले *) """पिछले"""         // पिछले is a comment

  
