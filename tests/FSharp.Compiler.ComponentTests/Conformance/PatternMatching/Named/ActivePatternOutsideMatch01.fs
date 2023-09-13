// #Conformance #PatternMatching #ActivePatterns 
// Verify active patterns can be used outside of a match statement

// This is really needed to make sure the test runs fine on non-ENU boxes!
// ToString() may use , or . or whatever symbols is defined for the floating point
System.Threading.Thread.CurrentThread.CurrentCulture <- System.Globalization.CultureInfo.InvariantCulture

let (|ToString|) (x : decimal) = x.ToString()

let (ToString test1) = 1234.56789M
if test1 <> "1234.56789" then exit 1

// Test nesting outside of match statements
let (|ToFloat|) (x : string) = System.Double.Parse(x)

// decimal -> string -> float
let (ToString (ToFloat test2)) = 1234.56789M
if test2 <> 1234.56789 then exit 1

exit 0
