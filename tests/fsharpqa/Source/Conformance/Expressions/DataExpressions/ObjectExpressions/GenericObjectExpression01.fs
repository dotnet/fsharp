// #Regression #Conformance #DataExpressions #ObjectConstructors 
#light  

// FSB 1112, Bug in definition of generic interface

// Regression for using a generic type variable in a construction expression

open System.Collections.Generic

type ITest<'T> = interface
                   abstract member IListLength : #IList<'T> -> int
                 end

let impl = {new ITest<'t> with
                override this.IListLength orgList = Seq.length orgList};

if impl.IListLength [|1..10|] <> 10 then exit 1
exit 0
