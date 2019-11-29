// #Conformance #UnitsOfMeasure 
#light

[<Measure>] type kg
let mass = 2.0<kg>

[<Measure>] 
type lb =
  static member fromKilos (x:float<kg>) = 2.2<lb/kg> * x

let weight = lb.fromKilos(3.0<kg>)

[<Measure>] type s
[<Measure>] type m

[<Measure>] type N = kg m / s^2
[<Measure>] type rad
[<MeasureAttribute()>] type Pa = N / m^2
//[<Measure>] type ([<Measure>] 'u) squared = 'u^2

[<Measure>] type dollars
[<Measure>] type years

let money = 20000.0m<dollars>

let angle = 3.0<rad>
let pressure = 2.5<Pa>
let ap = angle/pressure
let p = 2.5<Pa * Pa>

type vector<[<Measure>] 'u> = { x:float<'u>; y:float<'u>; z:float<'u> }

// Checks for genericity
let div:float<'u 'v> -> float<'u> -> float<'v> = fun x y -> x/y
let div2:float<'u> -> float<1> -> float<'u> = fun x y -> x/y
let div3:float<'u> -> float<'v> -> float<'w> = fun x y -> x/y

let hodiv:(float<'u 'v> -> float<'u 'v>) -> float<'u> -> float<'v> = fun f -> fun x -> 0.0<_>

/// A Gaussian distribution based on float numbers (struct type for memory efficiency) 
/// in exponential parameterisation. 
type Gaussian<[<Measure>] 'a> = 
        struct
            /// Precision times the mean of the Gaussian
            val PrecisionMean   : float<'a>
            /// Precision of the Gaussian
            val Precision       : float<'a^2>


            /// Constructor using the precision times mean and the precision
            new (pm,p) = { PrecisionMean = pm; Precision = p } 

            /// Mean of the Gaussian
            member this.Mu = this.PrecisionMean / this.Precision

            /// Mean of the Gaussian
            member this.Mean = this.Mu
            /// Variance of the Gaussian
            member this.Variance = 1.0 / this.Precision
            /// Standard deviation of the Gaussian
            member this.StandardDeviation = sqrt (this.Variance)
            /// Standard deviation of the Gaussian
            member this.Sigma = this.StandardDeviation

            /// Multiplies two Gaussians  
            static member ( * ) (a:Gaussian<'a>,b:Gaussian<'a>) = 
                new Gaussian<'a> (a.PrecisionMean + b.PrecisionMean, a.Precision + b.Precision)
            /// Divides two Gaussians
            static member (/) (a:Gaussian<'a>,b:Gaussian<'a>) =
                new Gaussian<'a> (a.PrecisionMean - b.PrecisionMean, a.Precision - b.Precision)
            /// Computes the absolute difference between two Gaussians
            static member AbsoluteDifference (a:Gaussian<'a>,b:Gaussian<'a>) = 
                max (abs (a.PrecisionMean - b.PrecisionMean)) (sqrt (abs (a.Precision - b.Precision)))
            /// Computes the absolute difference between two Gaussians
            static member (-) (a:Gaussian<'a>,b:Gaussian<'a>) = Gaussian.AbsoluteDifference (a,b)
        end


// First-class polymorphism, by a generic virtual method
type IProd =
    abstract Prod : float<'u> * float<'v> -> float<'u 'v>

let polyadd (prod : #IProd) = prod.Prod (2.0, 1.0<kg>) + prod.Prod (3.0<kg>, 4.0)

type SimpleProd() =
    interface IProd with
      member z.Prod (x:float<'u>, y:float<'v>) = x*y
type ComplexProd() =
    interface IProd with
      member z.Prod (x:float<'u>, y:float<'v>) = x*y*2.0

let r = (polyadd (new SimpleProd()))
let r2 = (polyadd (new ComplexProd()))

let weird(x:float<'u 'w>) (y:float<'v 'w>) (z:float<'w/'u 'v>) = 1

if (r/1.0<kg> = 14.0) && (r2/1.0<kg> = 28.0) then exit 0 else 1
