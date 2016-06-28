// #Regression #Conformance #TypeInference #Recursion 
// FS1 952, VS crash due to bad recursive type definition
//<Expects id="FS0953" span="(6,6-6,15)" status="error">This type definition involves an immediate cyclic reference through an abbreviation$</Expects>
//<Expects id="FS0001" span="(8,25-8,34)" status="error">This expression was expected to have type.    'bogusType'    .but here has type.    'Map<'a,'b>'</Expects>

type bogusType = (int, bogusType) Map

let empty : bogusType = Map.empty

