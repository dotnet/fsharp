// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1698
// Make sure we don't emit the laconic "syntax error" message. 
//<Expects id="FS0010" span="(14,27-14,29)" status="error">Unexpected keyword 'as' in expression\. Expected '}' or other token</Expects>
//<Expects id="FS0604" span="(14,9-14,10)" status="error">Unmatched '{'</Expects>



type Explicit =
    inherit Base 
    val y : int
    new(x) as self =
        let initialDummyCodeInConstructor = 3
        { inherit Base(x) as base
          y = x 
        }
        then
        printfn "in the constructor"
        base.BaseMember()
        self.ExplicitMember()
    member this.ExplicitMember() = ()
