// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:3508
// Note: the code here has nothing to do with the
// test itself. Any F# code would pretty much do.

module M
type x ()=  let mutable verificationX = false
            member this.X with set ((x:decimal,y:decimal)) = verificationX <- (x = 1M && y= -2M)
