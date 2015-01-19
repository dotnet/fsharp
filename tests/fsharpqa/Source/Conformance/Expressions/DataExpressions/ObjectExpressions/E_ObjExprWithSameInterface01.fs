// #Regression #Conformance #DataExpressions #ObjectConstructors 
// This was Dev10:854519 and Dev11:5525. The fix was to make this a compile error to avoid a runtime exception.
//<Expects status="error" span="(13,5-16,31)" id="FS0443">This type implements the same interface at different generic instantiations 'IQueue<'T>' and 'IQueue<obj>'\. This is not permitted in this version of F#\.$</Expects>

type IQueue<'a> =
    abstract Addd: 'a -> IQueue<'a>
    
type IQueueEx<'a> =
    inherit IQueue<'a>
    abstract Add: 'a -> IQueueEx<'a>
  
let makeQueueEx2() = 
    {new IQueueEx<'T> with
        member q.Add(x) = q
     interface IQueue<obj> with
        member q.Addd(x) = q }
 
makeQueueEx2() |> ignore

exit 1