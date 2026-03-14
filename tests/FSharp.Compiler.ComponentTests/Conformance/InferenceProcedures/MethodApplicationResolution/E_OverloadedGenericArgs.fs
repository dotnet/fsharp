// #Regression #Conformance #TypeInference 

// bug 1947 - the error message returned here should not be an overloading error message.
//<Expects id="FS0504" span="(13,9-13,24)" status="error">Incorrect generic instantiation\. No accessible member named 'M' takes 2 generic arguments\.</Expects>

type T<'K>(n: int) = 
    new (n:int) = new T<'K>()
    new () = new T<'K>()

    static member M (n : int) =  new T<'K>()
    static member M () = new T<'K>()

let h = T.M<int,int> 10
