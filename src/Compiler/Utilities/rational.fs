// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Rational arithmetic, used for exponents on units-of-measure
module internal Internal.Utilities.Rational

open System.Numerics

type Rational =
    {
        numerator: BigInteger
        denominator: BigInteger
    }

let rec gcd a (b: BigInteger) =
    if b = BigInteger.Zero then a else gcd b (a % b)

let lcm a b = (a * b) / (gcd a b)

let mkRational p q =
    let p, q =
        if q = BigInteger.Zero then
            raise (System.DivideByZeroException())

        let g = gcd q p in
        p / g, q / g

    let p, q = if q > BigInteger.Zero then p, q else -p, -q

     in

    { numerator = p; denominator = q }

let intToRational (p: int) =
    mkRational (BigInteger(p)) BigInteger.One

let ZeroRational = mkRational BigInteger.Zero BigInteger.One
let OneRational = mkRational BigInteger.One BigInteger.One

let AddRational m n =
    let d = gcd m.denominator n.denominator
    let m' = m.denominator / d
    let n' = n.denominator / d
    mkRational (m.numerator * n' + n.numerator * m') (m.denominator * n')

let NegRational m = mkRational (-m.numerator) m.denominator

let MulRational m n =
    mkRational (m.numerator * n.numerator) (m.denominator * n.denominator)

let DivRational m n =
    mkRational (m.numerator * n.denominator) (m.denominator * n.numerator)

let AbsRational m =
    mkRational (abs m.numerator) m.denominator

let RationalToString m =
    if m.denominator = BigInteger.One then
        m.numerator.ToString()
    else
        sprintf "(%A/%A)" m.numerator m.denominator

let GcdRational m n =
    mkRational (gcd m.numerator n.numerator) (lcm m.denominator n.denominator)

let GetNumerator p = int p.numerator
let GetDenominator p = int p.denominator

let SignRational p =
    if p.numerator < BigInteger.Zero then -1
    else if p.numerator > BigInteger.Zero then 1
    else 0
