module Test
#nowarn "57"

[<Measure>] type kg
[<Measure>] type s
[<Measure>] type m

[<Measure>] type sqrm = m^2

type area = float<sqrm>

let x1 = 2.0<kg> + 4.0
let x2 = 2.0<s> - 4.0<sqrm>
let x3 = 2.0<m> / 4.0f
let x8 = sqrt (4.0<m>)
let x10 = sign (3.0<m/s>)
let x11 = atan2 4.4<s^3> 5.4<s^4>
let x12 = Seq.sum [2.0<sqrm>; 3.0<m^2>]
let x12a = Seq.sumBy  (fun x -> x*x) [(2.0<sqrm> : area); 3.0<m^3>]
let x13 = (Seq.average [2.0<sqrm>; 3.0<m^3>]) : area
let x13a = Seq.averageBy  (fun x -> x*x) [2.0<s^2>; 3.0<m>]
let x14 = x13 + x13a
let x15 = 3.0<kg> ** 2.5




