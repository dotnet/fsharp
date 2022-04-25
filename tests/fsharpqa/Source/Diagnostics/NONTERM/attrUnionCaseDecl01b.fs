// #Regression #Diagnostics 
// Regression test for FSharp1.0:3702
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0010" status="error" span="(12,5-12,9)">Incomplete structured construct at or before this point in union case\. Expected identifier, '\(' or other token\.$</Expects>


           
    type Stuff = | AnonymousVariableType of string
                 | TypeVariable of string
                 | StaticHeadTypeTypeVariable of string
                 |  
    type Typar () = 
                    member x.Random () = match rnd.Next( 3 ) with 
                                         | 0 -> AnonymousVariableType("_")
                                         | 1 -> TypeVariable("'" + Ident.Random())
                                         | _ -> StaticHeadTypeTypeVariable("^" + Ident.Random())

