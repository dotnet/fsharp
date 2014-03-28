module Neg13
// check that a decent error is given when a setter is too polymorphic
type Gaussian1D =
   class 
      member x.Feet with set(v) = failwith ""
   end


[<ReferenceEquality; StructuralEquality; StructuralComparison>]
type R1 = A | B

[<ReferenceEquality; StructuralComparison>]
type R2 = A | B

[<ReferenceEquality; StructuralEquality>]
type R3 = A | B

[<StructuralComparison>]
type R4 = A | B

[<StructuralEquality>]
type R5 = A | B

[<ReferenceEquality; StructuralEquality; StructuralComparison>]
type R6 = A | B

[<ReferenceEquality; StructuralComparison>]
type R7 = A | B

[<ReferenceEquality; StructuralEquality>]
type R8 = A | B

[<StructuralComparison>]
type R9 = A | B

[<StructuralEquality>]
type Rq = A | B

[<NoComparison>]
type Rt = A | B

[<NoEquality>]
type Ry = A | B


[<StructuralEquality; NoComparison>]
type Ru = 
    | A 
    | B
    with 
        override x.Equals(y) = false
    end


[<StructuralEquality; NoComparison>]
type Rv = 
    | A 
    | B
    with 
        override x.Equals(y) = false
        interface System.IComparable with 
            member x.CompareTo(y) = 0
        end
    end

