// #Regression #Conformance #TypeInference 
// Regression test for FSharp1.0:2419
// Title: Give each module or class binding an "empty" type parameter environment, apart from enclosing declared type parameters
// Descr: Verify type constraint on the first <'a> doesn't propagate to other let's in chain of 'let recs'

let spColl = System.Configuration.SettingsPropertyCollection()

let rec f1 (x:'a :> System.ICloneable) = 3
and f2 (y:'a) = 2

if f1 (spColl) <> 3 then exit 1
if f2 ( 100 ) <> 2 || f2 (spColl) <> 2 then exit 1
