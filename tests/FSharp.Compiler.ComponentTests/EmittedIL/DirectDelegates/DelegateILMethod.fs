module DelegateILMethod

open System
open System.Text

// IL (BCL) method targets are compiled as TOp.ILCall rather than an F# value application. They are made
// direct only in optimized builds; in unoptimized builds the eta form keeps a closure (matching the F#
// eta policy). See DelegateKnownFunction for the F#-value equivalent.

// 12. eta IL/BCL static method (System.Math.Max).
let ilStaticEta () = Func<int, int, int>(fun a b -> Math.Max(a, b))

// 13. eta IL/BCL instance method (StringBuilder.Append(string)) on a reference type. The receiver is a
// parameter, evaluated at the construction site and carried as the delegate's Target.
let ilInstanceEta (sb: StringBuilder) = Func<string, StringBuilder>(fun s -> sb.Append(s))
