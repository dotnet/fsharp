// #Regression #Diagnostics 
// Regression test for FSharp1.0:3702
//<Expects id="FS0010" span="(10,5-10,9)" status="error">Incomplete structured construct at or before this point in union case\. Expected identifier, '\(' or other token\.$</Expects>
#light
           
    type Stuff = | AnonymousVariableType of string
                 | TypeVariable of string
                 | StaticHeadTypeTypeVariable of string
                 |  
    type Typar () = 
                    member x.Random () = match rnd.Next( 3 ) with 
                                         | 0 -> AnonymousVariableType("_")
                                         | 1 -> TypeVariable("'" + Ident.Random())
                                         | _ -> StaticHeadTypeTypeVariable("^" + Ident.Random())

