// #Regression #Conformance #TypeInference #Recursion 
// FS1 952, VS crash due to bad recursive type definition



type bogusType = (int, bogusType) Map

let empty : bogusType = Map.empty

