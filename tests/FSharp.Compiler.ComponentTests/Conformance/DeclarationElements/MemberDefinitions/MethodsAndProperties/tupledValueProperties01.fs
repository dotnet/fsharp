// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:966
// Verify that we can have properties that takes tuples for arguments
// Note: the non-curried syntax ((x:decimal, y:decimal)) is expected
#light

type x ()= class
            let mutable verificationX = false
            let mutable verificationY = false
            
            member this.X
             with set ((x:decimal,y:decimal)) = verificationX <- (x = 1M && y= -2M)
                         
            member this.Y
             with set (x : decimal*decimal) = verificationY <- (-(fst x) = 1M) && ( -(snd x) = -2M)
             
            member this.Verify = if verificationX && verificationY then 0 else 1
           end
 

let v = new x()

v.X <- (1M,-2M)
v.Y <- (-1M,2M)

if v.Verify <> 0 then failwith $"Failed: {v.Verify}"
