// #Regression #TypeInference 
// Verify we emit an error when we use the deprecated OverloadID attribute
//<Expects status="error" span="(10,7-10,17)" id="FS0039">The type 'OverloadID' is not defined</Expects>
//<Expects status="error" span="(11,7-11,17)" id="FS0039">The type 'OverloadID' is not defined</Expects>

type One = | One
type Two = | Two

type O =
    [<OverloadID "1">] static member M1(x:int) = One
    [<OverloadID "2">] static member M1(x:char) = Two

(if (O.M1(1) = One && O.M1('a') = Two) then 0 else 1) |> exit

