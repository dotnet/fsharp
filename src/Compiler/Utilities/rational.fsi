// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Rational arithmetic, used for exponents on units-of-measure
module internal Internal.Utilities.Rational

type Rational

val intToRational: int -> Rational
val AbsRational: Rational -> Rational
val AddRational: Rational -> Rational -> Rational
val MulRational: Rational -> Rational -> Rational
val DivRational: Rational -> Rational -> Rational
val NegRational: Rational -> Rational
val SignRational: Rational -> int
val ZeroRational: Rational
val OneRational: Rational

// Can be negative
val GetNumerator: Rational -> int

// Always positive
val GetDenominator: Rational -> int

// Greatest rational that divides both exactly
val GcdRational: Rational -> Rational -> Rational
val RationalToString: Rational -> string
