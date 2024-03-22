// #Conformance #UnitsOfMeasure #Constants 
#if TESTS_AS_APP
module Core_measures
#endif
#light

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures.Value <- failures.Value @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

(* TEST SUITE FOR Operators on units-of-measure *)

[<Measure>] type kg
[<Measure>] type s
[<Measure>] type m

[<Measure>] type sqrm = m^2

// Now some measures with members

[<Measure>] 
type lb =
  static member fromKg (x:float<kg>) = x*2.2<lb/kg>
  
// Augmentation
type s with
  static member Name = "seconds"

type s with
  static member Symbol = "s"

type lb with
  static member Name = "pounds"

type area = float<sqrm>
type intarea = int<sqrm>

module GENERICS =

  let f<'a when 'a:struct>(x:'a) = 1
  let x1 = f<float<kg>>(3.0<kg>)
  let x2 = f<float32<s>>(4.0f<s>)
  let x3 = f(5.0M<sqrm>)

module FLOAT =

 // set up bindings
 let x1 = 2.0<kg> + 4.0<kg>
 let x2 = 2.0<s> - 4.0<s>
 let x3 = 2.0<m> / 4.0<s>
 let x3a = 2.0<m> / 4.0<m>
 let x3b = 1.0 / 4.0<s>
 let x3c = 1.0<m> / 4.0
 let x4 = 2.0<m> * 4.0<s>
 let x4a = 2.0<m> * 4.0<m>
 let x4b = 2.0 * 4.0<m>
 let x4c = 2.0<m> * 4.0
 let x5 = 5.0<m> % 3.0<m>
 let x6 = - (2.0<m>)
 let x7 = abs (-2.0<m>)
 let x8 = sqrt (4.0<sqrm>)
 let x8a = sqrt (4.0<m>)
 let x8b = sqrt (4.0</m>)
 let x9 = [ 1.0<m> .. 1.0<m> .. 4.0<m> ]
 let x10 = sign (3.0<m/s>)
 let x11 = atan2 4.4<s^3> 5.4<s^3>
 let x11a : float<1> = acos 4.4<1> 
 let x11b : float<1> = asin 4.4<1> 
 let x11c : float<1> = atan 4.4<1> 
 let x11d : float<1> = ceil 4.4<1> 
 let x11e : float<1> = cos 4.4<1> 
 let x11f : float<1> = cosh 4.4<1> 
 let x11g : float<1> = exp 4.4<1> 
 let x11h : float<1> = floor 4.4<1> 
 let x11i : float<1> = log 4.4<1> 
 let x11j : float<1> = log10 4.4<1> 
 let x11k : float<1> = 4.4<1> ** 3.0<1>
 let x11l : float<1> = pown 4.4<1> 3
 let x11m : float<1> = round 4.4<1> 
 let x11n : int = sign 4.4<1> 
 let x11o : float<1> = sin 4.4<1> 
 let x11p : float<1> = sinh 4.4<1> 
 let x11q : float<1> = sqrt 4.4<1> 
 let x11r : float<1> = tan 4.4<1> 
 let x11s : float<1> = tanh 4.4<1> 
 let x12 = Seq.sum [2.0<sqrm>; 3.0<m^2>]
 let x12a = Seq.sumBy (fun x -> x*x) [(2.0<sqrm> : area); 3.0<m^2>]
 let x13 = (Seq.average [2.0<sqrm>; 3.0<m^2>]) : area
 let x13a = Seq.averageBy (fun x -> x*x) [2.0<m^2/m>; 3.0<m>]
 let x14 = x13 + x13a
 let x15 = 5.0<m> < 3.0<m>
 let x16 = 5.0<m> <= 3.0<m>
 let x17 = 5.0<m> > 3.0<m>
 let x18 = 5.0<m> >= 3.0<m>
 let x19 = max 5.0<m> 3.0<m>
 let x20 = min 5.0<m> 3.0<m>
 let x21 = typeof<float<m>>
 let x22<[<Measure>] 'a>() = typeof<float<'a>>

 // Force trig functions etc to be dimensionless
 let x23a = acos (4.4<_>)
 let x23b = asin (4.4<_>)
 let x23c = atan (4.4<_>)
 let x23d = ceil (4.4<_>)
 let x23e = cos (4.4<_>)
 let x23f = cosh (4.4<_>)
 let x23g = exp (4.4<_>)
 let x23h = floor (4.4<_>)
 let x23i = log (4.4<_>)
 let x23j = log10 (4.4<_>)
 let x23k = 4.4<_> ** 3.0<_>
 let x23l = pown (4.4<_>) 3
 let x23m = round (4.4<_>)
 let x23o = sin (4.4<_>)
 let x23p = sinh (4.4<_>)
 let x23r = tan (4.4<_>)
 let x23s = tanh (4.4<_>)
#if !NETCOREAPP
 let x23t = truncate (4.5<_>)
#endif
 // check the types and values!
 test "x1" (x1 = 6.0<kg>)
 test "x2" (x2 = -2.0<s>)
 test "x3" (x3 = 0.5<m/s>)
 test "x3a" (x3a = 0.5)
 test "x3b" (x3b = 0.25<1/s>)
 test "x3c" (x3c = 0.25<m>)
 test "x4" (x4 = 8.0<m s>)
 test "x4a" (x4a = 8.0<m^2>)
 test "x4b" (x4b = 8.0<m>)
 test "x4c" (x4c = 8.0<m>)
 test "x5" (x5 = 2.0<m>)
 test "x6" (x6 = -2.0<m>)
 test "x7" (x7 = 2.0<m>)
 test "x8" (x8 = 2.0<m>)
 test "x8a" (x8a = 2.0<m^(1/2)>)
 test "x8b" (x8b = 2.0<m^(-1/2)>)
 test "x9" (x9 = [1.0<m>; 2.0<m>; 3.0<m>; 4.0<m>])
 test "x10" (x10 = 1)
 test "x12" (x12 = 5.0<m^2>)
 test "x12a" (x12a = 13.0<m^4>)
 test "x13" (x13 = 2.5<m^2>)
 test "x13a" (x13a = 6.5<m^2>)
 test "x14" (x14 = 9.0<m^2>)
 test "x15" (x15 = false)
 test "x16" (x16 = false)
 test "x17" (x17 = true)
 test "x18" (x18 = true)
 test "x19" (x19 = 5.0<m>)
 test "x20" (x20 = 3.0<m>)
 test "x21" (x21 = typeof<float>)
 test "x22" (x22<m>() = typeof<float>)
  

module INT =

 // set up bindings
 let x1 = 2<kg> + 4<kg>
 let x2 = 2<s> - 4<s>
 let x3 = 8<m> / 4<s>
 let x3a = 8<m> / 4<m>
 let x3b = 8 / 4<s>
 let x3c = 8<m> / 4
 let x4 = 2<m> * 4<s>
 let x4a = 2<m> * 4<m>
 let x4b = 2 * 4<m>
 let x4c = 2<m> * 4
 let x5 = 5<m> % 3<m>
 let x6 = - (2<m>)
 let x7 = abs (-2<m>)
 let x9 = [ 1<m> .. 1<m> .. 4<m> ]
 let x10 = sign (3<m/s>)
 let x11n : int = sign 4<1> 
 let x12 = Seq.sum [2<sqrm>; 3<m^2>]
 let x12a = Seq.sumBy (fun x -> x*x) [(2<sqrm> : intarea); 3<m^2>]
 let x15 = 5<m> < 3<m>
 let x16 = 5<m> <= 3<m>
 let x17 = 5<m> > 3<m>
 let x18 = 5<m> >= 3<m>
 let x19 = max 5<m> 3<m>
 let x20 = min 5<m> 3<m>
 let x21 = typeof<int<m>>
 let x22<[<Measure>] 'a>() = typeof<int<'a>>

 // Force bitwise functions etc to be dimensionless
 let x23a = 4<_> ||| 8<_>
 let x23b = 3<_> &&& 1<_>
 let x23c = ~~~ 1<_>
 let x23d = 2<_> >>> 1<_>
 let x23e = 2<_> <<< 1<_>
 let x23f = 3<_> ^^^ 3<_>

 // check the types and values!
 test "x1" (x1 = 6<kg>)
 test "x2" (x2 = -2<s>)
 test "x3" (x3 = 2<m/s>)
 test "x3a" (x3a = 2)
 test "x3b" (x3b = 2<1/s>)
 test "x3c" (x3c = 2<m>)
 test "x4" (x4 = 8<m s>)
 test "x4a" (x4a = 8<m^2>)
 test "x4b" (x4b = 8<m>)
 test "x4c" (x4c = 8<m>)
 test "x5" (x5 = 2<m>)
 test "x6" (x6 = -2<m>)
 test "x7" (x7 = 2<m>)
 test "x9" (x9 = [1<m>; 2<m>; 3<m>; 4<m>])
 test "x10" (x10 = 1)
 test "x12" (x12 = 5<m^2>)
 test "x12a" (x12a = 13<m^4>)
 test "x15" (x15 = false)
 test "x16" (x16 = false)
 test "x17" (x17 = true)
 test "x18" (x18 = true)
 test "x19" (x19 = 5<m>)
 test "x20" (x20 = 3<m>)
 test "x21" (x21 = typeof<int>)
 test "x22" (x22<m>() = typeof<int>)
  

module FLOAT32 =

 let y1 = 2.0f<kg> + 4.0f<kg>
 let y2 = 2.0f<s> - 4.0f<s>
 let y3 = 2.0f<m> / 4.0f<s>
 let y3a = 2.0f<m> / 4.0f<m>
 let y3b = 1.0f / 4.0f<s>
 let y3c = 1.0f<m> / 4.0f
 let y4 = 2.0f<m> * 4.0f<s>
 let y4a = 2.0f<m> * 4.0f<m>
 let y4b = 2.0f * 4.0f<m>
 let y4c = 2.0f<m> * 4.0f
 let y5 = 5.0f<m> % 3.0f<m>
 let y6 = - (2.0f<m>)
 let y7 = abs (2.0f<m>)
 let y8 = sqrt (4.0f<sqrm>)
 let y9 = [ 1.0f<m> .. 1.0f<m> .. 4.0f<m> ]
 let y10 = sign (3.0f<m/s>)
 let y11 = atan2 4.4f<s^3> 5.4f<s^3>
 let x11a : float32<1> = acos 4.4f<1> 
 let x11b : float32<1> = asin 4.4f<1> 
 let x11c : float32<1> = atan 4.4f<1> 
 let x11d : float32<1> = ceil 4.4f<1> 
 let x11e : float32<1> = cos 4.4f<1> 
 let x11f : float32<1> = cosh 4.4f<1> 
 let x11g : float32<1> = exp 4.4f<1> 
 let x11h : float32<1> = floor 4.4f<1> 
 let x11i : float32<1> = log 4.4f<1> 
 let x11j : float32<1> = log10 4.4f<1> 
 let x11k : float32<1> = 4.4f<1> ** 3.0f<1>
 let x11l : float32<1> = pown 4.4f<1> 3
 let x11m : float32<1> = round 4.4f<1> 
 let x11n : int = sign 4.4f<1> 
 let x11o : float32<1> = sin 4.4f<1> 
 let x11p : float32<1> = sinh 4.4f<1> 
 let x11q : float32<1> = sqrt 4.4f<1> 
 let x11r : float32<1> = tan 4.4f<1> 
 let x11s : float32<1> = tanh 4.4f<1> 
 let y12 = Seq.sum [2.0f<sqrm>; 3.0f<m^2>]
 let y12a = Seq.sumBy (fun y -> y*y) [2.0f<sqrm>; 3.0f<m^2>]
 let y13 = Seq.average [2.0f<sqrm>; 3.0f<m^2>]
 let y13a = Seq.averageBy (fun y -> y*y) [2.0f<sqrm>; 3.0f<m^2>]

 // check the types and values!
 test "y1" (y1 = 6.0f<kg>)
 test "y2" (y2 = -2.0f<s>)
 test "y3" (y3 = 0.5f<m/s>)
 test "y3a" (y3a = 0.5f)
 test "y3b" (y3b = 0.25f<1/s>)
 test "y3c" (y3c = 0.25f<m>)
 test "y4" (y4 = 8.0f<m s>)
 test "y4a" (y4a = 8.0f<m^2>)
 test "y4b" (y4b = 8.0f<m>)
 test "y4c" (y4c = 8.0f<m>)
 test "y5" (y5 = 2.0f<m>)
 test "y6" (y6 = -2.0f<m>)
 test "y7" (y7 = 2.0f<m>)
 test "y8" (y8 = 2.0f<m>)
 test "y9" (y9 = [1.0f<m>; 2.0f<m>; 3.0f<m>; 4.0f<m>])
 test "y10" (y10 = 1)
 test "y12" (y12 = 5.0f<m^2>)
 test "y12a" (y12a = 13.0f<m^4>)
 test "y13" (y13 = 2.5f<m^2>)
 test "y13a" (y13a = 6.5f<m^4>)
  

module DECIMAL =

 let z1 = 2.0M<kg> + 4.0M<kg>
 let z2 = 2.0M<s> - 4.0M<s>
 let z3 = 2.0M<m> / 4.0M<s>
 let z3a = 2.0M<m> / 4.0M<m>
 let z3b = 1.0M / 4.0M<s>
 let z3c = 1.0M<m> / 4.0M
 let z4 = 2.0M<m> * 4.0M<s>
 let z4a = 2.0M<m> * 4.0M<m>
 let z4b = 2.0M * 4.0M<m>
 let z4c = 2.0M<m> * 4.0M
 let z5 = 5.0M<m> % 3.0M<m>
 let z6 = - (2.0M<m>)
 let z7 = abs (2.0M<m>)
// let z9 = [ 1.0M<m> .. 4.0M<m> ]
 let z10 = sign (3.0M<m/s>)

 let x1d : decimal = ceil 4.4M 
 let x1h : decimal = floor 4.4M 
 let x1l : decimal = pown 4.4M 3
#if !NETCOREAPP
 let x1m : decimal = round 4.4M 
#endif
 let x1n : int = sign 4.4M 

 //let x11d : decimal<1> = ceil 4.4M<1> 
 //let x11h : decimal<1> = floor 4.4M<1> 
 //let x11m : decimal<1> = round 4.4M<1> 
 let x11l : decimal<1> = pown 4.4M<1> 3
 let x11n : int = sign 4.4M<1> 

 let z12 = Seq.sum [2.0M<sqrm>; 3.0M<m^2>]
 let z12a = Seq.sumBy (fun z -> z*z) [2.0M<sqrm>; 3.0M<m^2>]
 let z13 = Seq.average [2.0M<sqrm>; 3.0M<m^2>]
 let z13a = Seq.averageBy (fun z -> z*z) [2.0M<sqrm>; 3.0M<m^2>]


 // check the types and values!
 test "z1" (z1 = 6.0M<kg>)
 test "z2" (z2 = -2.0M<s>)
 test "z3" (z3 = 0.5M<m/s>)
 test "z3a" (z3a = 0.5M)
 test "z3b" (z3b = 0.25M<1/s>)
 test "z3c" (z3c = 0.25M<m>)
 test "z4" (z4 = 8.0M<m s>)
 test "z4a" (z4a = 8.0M<m^2>)
 test "z4b" (z4b = 8.0M<m>)
 test "z4c" (z4c = 8.0M<m>)
 test "z5" (z5 = 2.0M<m>)
 test "z6" (z6 = -2.0M<m>)
 test "z7" (z7 = 2.0M<m>)
 test "z10" (z10 = 1)
 test "z12" (z12 = 5.0M<m^2>)
 test "z12a" (z12a = 13.0M<m^4>)
 test "z13" (z13 = 2.5M<m^2>)
 test "z13a" (z13a = 6.5M<m^4>)
  

module FLOAT_CHECKED =
 open Microsoft.FSharp.Core.Operators.Checked

 // set up bindings
 let x1 = 2.0<kg> + 4.0<kg>
 let x2 = 2.0<s> - 4.0<s>
 let x3 = 2.0<m> / 4.0<s>
 let x3a = 2.0<m> / 4.0<m>
 let x3b = 1.0 / 4.0<s>
 let x3c = 1.0<m> / 4.0
 let x4 = 2.0<m> * 4.0<s>
 let x4a = 2.0<m> * 4.0<m>
 let x4b = 2.0 * 4.0<m>
 let x4c = 2.0<m> * 4.0
 let x5 = 5.0<m> % 3.0<m>
 let x6 = - (2.0<m>)
 let x7 = abs (-2.0<m>)
 let x8 = sqrt (4.0<sqrm>)
 let x9 = [ 1.0<m> .. 1.0<m> .. 4.0<m> ]
 let x10 = sign (3.0<m/s>)
 let x11 = atan2 4.4<s^3> 5.4<s^3>
 let x12 = Seq.sum [2.0<sqrm>; 3.0<m^2>]
 let x12a = Seq.sumBy (fun x -> x*x) [(2.0<sqrm> : area); 3.0<m^2>]
 let x13 = (Seq.average [2.0<sqrm>; 3.0<m^2>]) : area
 let x13a = Seq.averageBy (fun x -> x*x) [2.0<m^2/m>; 3.0<m>]
 let x14 = x13 + x13a

 // check the types and values!
 test "x1" (x1 = 6.0<kg>)
 test "x2" (x2 = -2.0<s>)
 test "x3" (x3 = 0.5<m/s>)
 test "x3a" (x3a = 0.5)
 test "x3b" (x3b = 0.25<1/s>)
 test "x3c" (x3c = 0.25<m>)
 test "x4" (x4 = 8.0<m s>)
 test "x4a" (x4a = 8.0<m^2>)
 test "x4b" (x4b = 8.0<m>)
 test "x4c" (x4c = 8.0<m>)
 test "x5" (x5 = 2.0<m>)
 test "x6" (x6 = -2.0<m>)
 test "x7" (x7 = 2.0<m>)
 test "x8" (x8 = 2.0<m>)
 test "x9" (x9 = [1.0<m>; 2.0<m>; 3.0<m>; 4.0<m>])
 test "x10" (x10 = 1)
 test "x12" (x12 = 5.0<m^2>)
 test "x12a" (x12a = 13.0<m^4>)
 test "x13" (x13 = 2.5<m^2>)
 test "x13a" (x13a = 6.5<m^2>)
  

module FLOAT32_CHECKED =
 open Microsoft.FSharp.Core.Operators.Checked

 let y1 = 2.0f<kg> + 4.0f<kg>
 let y2 = 2.0f<s> - 4.0f<s>
 let y3 = 2.0f<m> / 4.0f<s>
 let y3a = 2.0f<m> / 4.0f<m>
 let y3b = 1.0f / 4.0f<s>
 let y3c = 1.0f<m> / 4.0f
 let y4 = 2.0f<m> * 4.0f<s>
 let y4a = 2.0f<m> * 4.0f<m>
 let y4b = 2.0f * 4.0f<m>
 let y4c = 2.0f<m> * 4.0f
 let y5 = 5.0f<m> % 3.0f<m>
 let y6 = - (2.0f<m>)
 let y7 = abs (2.0f<m>)
 let y8 = sqrt (4.0f<sqrm>)
 let y9 = [ 1.0f<m> .. 1.0f<m> .. 4.0f<m> ]
 let y10 = sign (3.0f<m/s>)
 let y11 = atan2 4.4f<s^3> 5.4f<s^3>
 let y12 = Seq.sum [2.0f<sqrm>; 3.0f<m^2>]
 let y12a = Seq.sumBy (fun y -> y*y) [2.0f<sqrm>; 3.0f<m^2>]
 let y13 = Seq.average [2.0f<sqrm>; 3.0f<m^2>]
 let y13a = Seq.averageBy (fun y -> y*y) [2.0f<sqrm>; 3.0f<m^2>]

 // check the types and values!
 test "y1" (y1 = 6.0f<kg>)
 test "y2" (y2 = -2.0f<s>)
 test "y3" (y3 = 0.5f<m/s>)
 test "y3a" (y3a = 0.5f)
 test "y3b" (y3b = 0.25f<1/s>)
 test "y3c" (y3c = 0.25f<m>)
 test "y4" (y4 = 8.0f<m s>)
 test "y4a" (y4a = 8.0f<m^2>)
 test "y4b" (y4b = 8.0f<m>)
 test "y4c" (y4c = 8.0f<m>)
 test "y5" (y5 = 2.0f<m>)
 test "y6" (y6 = -2.0f<m>)
 test "y7" (y7 = 2.0f<m>)
 test "y8" (y8 = 2.0f<m>)
 test "y9" (y9 = [1.0f<m>; 2.0f<m>; 3.0f<m>; 4.0f<m>])
 test "y10" (y10 = 1)
 test "y12" (y12 = 5.0f<m^2>)
 test "y12a" (y12a = 13.0f<m^4>)
 test "y13" (y13 = 2.5f<m^2>)
 test "y13a" (y13a = 6.5f<m^4>)
  

module DECIMAL_CHECKED =
 open Microsoft.FSharp.Core.Operators.Checked

 let z1 = 2.0M<kg> + 4.0M<kg>
 let z2 = 2.0M<s> - 4.0M<s>
 let z3 = 2.0M<m> / 4.0M<s>
 let z3a = 2.0M<m> / 4.0M<m>
 let z3b = 1.0M / 4.0M<s>
 let z3c = 1.0M<m> / 4.0M
 let z4 = 2.0M<m> * 4.0M<s>
 let z4a = 2.0M<m> * 4.0M<m>
 let z4b = 2.0M * 4.0M<m>
 let z4c = 2.0M<m> * 4.0M
 let z5 = 5.0M<m> % 3.0M<m>
 let z6 = - (2.0M<m>)
 let z7 = abs (2.0M<m>)
// let z9 = [ 1.0M<m> .. 4.0M<m> ]
 let z10 = sign (3.0M<m/s>)
 let z12 = Seq.sum [2.0M<sqrm>; 3.0M<m^2>]
 let z12a = Seq.sumBy (fun z -> z*z) [2.0M<sqrm>; 3.0M<m^2>]
 let z13 = Seq.average [2.0M<sqrm>; 3.0M<m^2>]
 let z13a = Seq.averageBy (fun z -> z*z) [2.0M<sqrm>; 3.0M<m^2>]


 // check the types and values!
 test "z1" (z1 = 6.0M<kg>)
 test "z2" (z2 = -2.0M<s>)
 test "z3" (z3 = 0.5M<m/s>)
 test "z3a" (z3a = 0.5M)
 test "z3b" (z3b = 0.25M<1/s>)
 test "z3c" (z3c = 0.25M<m>)
 test "z4" (z4 = 8.0M<m s>)
 test "z4a" (z4a = 8.0M<m^2>)
 test "z4b" (z4b = 8.0M<m>)
 test "z4c" (z4c = 8.0M<m>)
 test "z5" (z5 = 2.0M<m>)
 test "z6" (z6 = -2.0M<m>)
 test "z7" (z7 = 2.0M<m>)
 test "z10" (z10 = 1)
 test "z12" (z12 = 5.0M<m^2>)
 test "z12a" (z12a = 13.0M<m^4>)
 test "z13" (z13 = 2.5M<m^2>)
 test "z13a" (z13a = 6.5M<m^4>)
  

module MembersTest =
    let f = 2.0<kg>
    let s = 2.0f<kg>
    let d = 2.0M<kg>

    let tmpCulture = System.Threading.Thread.CurrentThread.CurrentCulture
    System.Threading.Thread.CurrentThread.CurrentCulture <- System.Globalization.CultureInfo("en-US")
    test "f" (f.ToString().Equals("2"))
    test "s" (s.ToString().Equals("2"))
    test "d" (d.ToString().Equals("2.0"))
    System.Threading.Thread.CurrentThread.CurrentCulture <- tmpCulture

    let fc = (f :> System.IComparable<float<kg>>).CompareTo(f+f)
    let sc = (s :> System.IComparable<float32<kg>>).CompareTo(s+s)
    let dc = (d :> System.IComparable<decimal<kg>>).CompareTo(d+d)
    test "fc" (fc = -1)
    test "sc" (sc = -1)
    test "dc" (dc = -1)

    let f1 = (f :> System.IFormattable)
    let f2 = (f :> System.IComparable)
    let f3 = (f :> System.IEquatable<float<kg>>)
#if !NETCOREAPP
    let f4 = (f :> System.IConvertible)
#endif
  
module WrappedFloatTypeTest = 
    type C<[<Measure>] 'T> (v:float<'T>) = 
        member x.V : float<'T> = v // note, a type annotation is needed here to allow generic recursion
        static member (+) (c1:C<'T>,c2:C<'T>) = C<'T>(c1.V + c2.V)
        static member (*) (c1:C<'T>,c2:C<'U>) = C<'T 'U>(c1.V * c2.V)
        static member (/) (c1:C<'T>,c2:C<'U>) = C<'T / 'U>(c1.V / c2.V)
        static member (-) (c1:C<'T>,c2:C<'T>) = C<'T>(c1.V - c2.V)
        static member Sqrt (c1:C<_>) = C<_>(sqrt c1.V)
        static member Abs (c1:C<_>) = C<_>(abs c1.V)
        static member Acos (c1:C<1>) = C<1>(acos c1.V)
        static member Asin (c1:C<1>) = C<1>(asin c1.V)
        static member Atan (c1:C<1>) = C<1>(atan c1.V)
        static member Atan2 (c1:C<'u>,c2:C<'u>) = C<1>(atan2 c1.V c2.V)
        static member Ceiling (c1:C<1>) = C<1>(ceil c1.V)
        static member Floor (c1:C<1>) = C<1>(floor c1.V)
        member c1.Sign = sign c1.V
        static member Round (c1:C<1>) = C<1>(round c1.V)
#if LOGC
        static member Log (c1:C<'u>) = LogC<'u>(log (float c1.V))
#else
        static member Exp (c1:C<1>) = C<1>(exp (float c1.V))
        static member Log (c1:C<1>) = C<1>(log (float c1.V))
#endif
        static member Log10 (c1:C<1>) = C<1>(log10 (float c1.V))
        static member Cos (c1:C<1>) = C<1>(cos c1.V)
        static member Cosh (c1:C<1>) = C<1>(cosh c1.V)
        static member Sin (c1:C<1>) = C<1>(sin c1.V)
        static member Sinh (c1:C<1>) = C<1>(sinh c1.V)
        static member Tanh (c1:C<1>) = C<1>(tan c1.V)
#if !NETCOREAPP
        static member Truncate (c1:C<1>) = C<1>(truncate c1.V)
#endif
        static member Pow (c1:C<1>,c2:C<1>) = C<1>( c1.V ** c2.V)
        static member Mul (c1:C<'T>,c2:C<'U>) = C<'T 'U>(c1.V * c2.V)
#if LOGC
    and LogC<[<Measure>] 'T> (v:float) = 
        member x.UnsafeV = v
        static member Exp (c1:LogC<'U>) = C<'U>(exp c1.UnsafeV |> box |> unbox)
#endif
   
    [<Measure>] 
    type kg

    //let v1 = pown 3.0<kg> 2 
   // let v2 = pown 3.0<kg> 1
   // let x = acos (3.0<_>)
    //acosr 3.0  : 3.0<radians>
    
    let c1 = C<kg>(3.0<kg>)
    let c2 = C<kg>(4.0<kg>)

    let c3 = c1 + c2
    let c5 = c1 * c2         
    let c6 = c1 / c2         
    let c7 = c1 - c2  
    let c8a : C<kg^2> = c1 * c1        
    let c8b = C<kg>.Sqrt c8a
    let c8 = sqrt c8a
    let c9 = acos (C<1>(0.5))
    let c11 = abs (C<1>(0.5))
    let c12 = asin (C<1>(0.5))
    let c13 = atan (C<1>(0.5))
    let c14 = atan2 (C<1>(0.5)) (C<1>(0.5))
    let c15 = atan2 (C<kg>(0.5<kg>)) (C<kg>(0.5<kg>))
    let c16 = ceil (C<1>(0.5))
    let c17 = exp (C<1>(0.5))
    let c18 = floor (C<1>(0.5))
    let c19 = sign (C<1>(0.5))
    let c20 = sign (C<1>(0.5))
    let c21 = round (C<1>(0.5))
    let c22 = log (C<1>(0.5))
    let c23 = log10 (C<1>(0.5))
    let c24 = cos (C<1>(0.5))
    let c25 = cosh (C<1>(0.5))
    let c26 = sin (C<1>(0.5))
    let c27 = sinh (C<1>(0.5))
    let c28 = tanh (C<1>(0.5))
#if !NETCOREAPP
    let c29 = truncate (C<1>(0.5))
#endif
    let c30 =  C<1>(0.5) ** C<1>(2.0)
    let c31 =  C<1>.Mul (C<1>(0.5),C<1>(2.0))
    let c32 =  C<kg>.Mul (C<kg>(0.5<kg>),C<kg>(2.0<kg>))

