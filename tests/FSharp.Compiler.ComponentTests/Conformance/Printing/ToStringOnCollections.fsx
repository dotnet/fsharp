// #Regression #NoMT #Printing 
// Regression test on FSharp1.0:5129
// Debugger: provide a decent override of ToString on FSharpList

let emptyList = ([] : int list)
if  emptyList.ToString() <> "[]" then raise (new System.Exception("CustomExceptions01.fs failed line 6"))

let emptySeq = Seq.ofList emptyList
if  emptySeq.ToString() <> "[]" then raise (new System.Exception("CustomExceptions01.fs failed line 9"))

let emptySet = Set.ofList emptyList
if  emptySet.ToString() <> "set []" then raise (new System.Exception("CustomExceptions01.fs failed line 12"))

let emptyMap = List.empty<int * int> |> Map.ofList
if  emptyMap.ToString() <> "map []" then raise (new System.Exception("CustomExceptions01.fs failed line 15"))

let nonEmptyList = [1;2;3;4;5]
if nonEmptyList.ToString() <> "[1; 2; 3; ... ]" then raise (new System.Exception("CustomExceptions01.fs failed line 18"))

let nonEmptySeq = Seq.ofList nonEmptyList
if  nonEmptySeq.ToString() <> "[1; 2; 3; ... ]" then raise (new System.Exception("CustomExceptions01.fs failed line 21"))

let nonEmptySet = Set.ofList nonEmptyList
if  nonEmptySet.ToString() <> "set [1; 2; 3; ... ]" then raise (new System.Exception("CustomExceptions01.fs failed line 24"))

let nonEmptyMap = new Map<int,int>(seq { for i in 0..5 do yield (i,i)} )
if  nonEmptyMap.ToString() <> "map [(0, 0); (1, 1); (2, 2); ... ]" then raise (new System.Exception("CustomExceptions01.fs line 27 failed"))

()
