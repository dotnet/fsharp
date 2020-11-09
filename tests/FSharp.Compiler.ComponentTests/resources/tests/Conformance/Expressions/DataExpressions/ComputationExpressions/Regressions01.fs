// #Regression #Conformance #DataExpressions #ComputationExpressions 
#light

// Regression test for FSB, 1831
// Finally block called twice for nested sequence comprehensions

open System.Collections.Generic

type Location =
    | InTryBlock
    | InFinallyBlock
    | InFirstForLoop
    | InSecondForLoop

let orderOfExecution = new List<Location>()

let seqTest1 valueRef value =
    seq { 
        try
            do orderOfExecution.Add(InTryBlock)
            yield (valueRef := value; !valueRef)
        finally
            do orderOfExecution.Add(InFinallyBlock)
            valueRef := -1
    }

let seqTest2 valueRef =
    seq { 
        for i in seqTest1 valueRef 1 do
            do orderOfExecution.Add(InFirstForLoop)
            yield i
        for i in seqTest1 valueRef 2 do
            // On yield, the value reference should still be set.
            // the finally clause should not have set it to -1 by now
            do orderOfExecution.Add(InSecondForLoop)
            yield i
    }

let valueRef = ref 0 in
let results = Seq.toList (seqTest2 valueRef) @ [!valueRef]

let order = List.ofSeq orderOfExecution

if order   <> [InTryBlock; InFirstForLoop; InFinallyBlock; InTryBlock; InSecondForLoop; InFinallyBlock] then exit 1
if results <> [1; 2; -1] then exit 1

exit 0
