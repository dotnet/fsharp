// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:1168
// Make sure we can reference both .dll and .exe
// from both .exe and .dll
// using both fsc and fsi
// Note: the code here has nothing to do with the
// test itself. Any F# code would pretty much do.
#light

namespace Name.Space

module M =
    type x ()= class
                  let mutable verificationX = false
                  member this.X
                   with set ((x:decimal,y:decimal)) = verificationX <- (x = 1M && y= -2M)
                end

    exit 0
