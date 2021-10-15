// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties
// Regression test for FSHARP1.0:966
// Verify that we can have properties that takes tuples for arguments
// Note: the non-curried syntax ((x:decimal, y:decimal)) is expected
// Run thru fsi
//<Expects status="success">  type x =</Expects>
//<Expects status="success">    new: unit -> x</Expects>
//<Expects status="success">    member Verify: int</Expects>
//<Expects status="success">    member X: decimal \* decimal with set</Expects>
//<Expects status="success">    member Y: decimal \* decimal with set</Expects>
#light

type x ()= class
            let mutable verificationX = false
            let mutable verificationY = false
            
            member this.X
             with set ((x:decimal,y:decimal)) = verificationX <- (x = 1M && y= -2M)
                         
            member this.Y
             with set (x : decimal*decimal) = verificationY <- (-(fst x) = 1M) && ( -(snd x) = -2M)
             
            member this.Verify = if verificationX && verificationY then 0 else 1
           end;;

0 |> exit;;
#q;;
