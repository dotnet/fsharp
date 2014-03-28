module Test

// Literal tests
module DecimalLiteralTests = 
    let literalTooBig = 7.922816252e+28M
    let literalTooSmall = -7.922816252e+28M

module CheckMisDefinedProperties = begin 
    module Test1 = begin

        type T() = 
          class
            member this.X5 with get (idx1 : int) (idx2 : int) = 4 // should be rejected by parsing
            member this.X6 with get (idx1 : int) (idx2 : int) (idx3 : int) = 4 // should be rejected by parsing
            member this.S5 with set (idx1 : int) (idx2 : int) v = 4 // should be rejected by parsing
            static member SX5 with get (idx1 : int) (idx2 : int) = 4 // should be rejected by parsing
            static member SX6 with get (idx1 : int) (idx2 : int) (idx3 : int) = 4 // should be rejected by parsing
            static member SS5 with set (idx1 : int) (idx2 : int) v = 4 // should be rejected by parsing
            
          end

    end
end
