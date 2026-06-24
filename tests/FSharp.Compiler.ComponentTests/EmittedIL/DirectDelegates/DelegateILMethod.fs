module DelegateILMethod

open System
open System.Text

// IL (BCL) method targets are compiled as TOp.ILCall rather than an F# value application. They are made
// direct only when the eta-expanded forwarding call survives to codegen, i.e. in optimized builds; in
// unoptimized builds the eta form keeps a closure (matching the F# eta policy). See DelegateNonInlinable
// for the F#-value equivalent.

// Static IL method (System.Math.Max), eta-expanded.
let ilStaticEta () = Func<int, int, int>(fun a b -> Math.Max(a, b))

// Instance IL method (StringBuilder.Append(string)) on a reference type, eta-expanded. The receiver is a
// parameter, evaluated at the construction site and carried as the delegate's Target.
let ilInstanceEta (sb: StringBuilder) = Func<string, StringBuilder>(fun s -> sb.Append(s))
