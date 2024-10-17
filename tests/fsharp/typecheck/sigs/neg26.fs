module Test

module ObjectExpressionNegativeTests = 

    module Test1 = 
        type ITest =
            abstract member Meth1: string -> string

        type ITestSub =
            inherit ITest  
            abstract member Meth2: int -> int


        let ErroneousComplete () =    
            { new ITestSub with member x.Meth2(y) = 2 } // This should give an error  - Meth1 is not implemented


    module Test2 = 
        type ITest =
            abstract member Meth1: string -> string

        type ITestSub =
            inherit ITest
            abstract member Meth2: int -> int

        let ErroneousComplete () =
            { new ITestSub with override x.Meth1(s:string) = s  }  // Expect an error here 
            
            
module AnotherObjectExpressionTest = 
    type ITest =
        abstract member Meth1: int -> int

    type ITestSub =
        inherit ITest
        abstract member Meth1: int -> int

    let Partial() =
        { new ITestSub with                   // Expect an error here 
            override this.Meth1 (x:int) = x }

            
module YetAnotherObjectExpressionTest = 
    type ITest =
        abstract member Meth1: int -> int

    type ITestSub =
        inherit ITest
        abstract member Meth1: int -> int

    let Partial() =
        { new ITestSub with                   // Expect an error here - ambiguous slot, so no slot inference, incorrect type
            override this.Meth1 (x:'a) = x }
