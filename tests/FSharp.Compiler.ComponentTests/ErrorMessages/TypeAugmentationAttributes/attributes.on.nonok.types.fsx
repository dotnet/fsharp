[<CustomEquality>]
type T1() =
    override _.Equals _ = true
[<CustomEquality;CustomComparison>]
type T2() =
    override _.Equals _ = true
[<CustomEquality>]
type S1(a:int) = struct
    override _.Equals _ = true
end
[<CustomEquality;CustomComparison>]
type S2(a:int) = struct
    override _.Equals _ = true
end
[<CustomEquality>]
type I1 = interface end
[<CustomEquality;CustomComparison>]
type I2 = interface end
[<CustomEquality>]
type E1 = A = 1 | B = 2
[<CustomEquality;CustomComparison>]
type E2 = A = 1 | B = 2
[<CustomEquality>]
type D1 = delegate of (int * int) -> int
[<CustomEquality;CustomComparison>]
type D2 = delegate of (int * int) -> int
[<CustomEquality>]
module M1 =
    let a = 1
[<CustomEquality;CustomComparison>]
module M2 =
    let a = 1
