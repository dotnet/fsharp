// #Regression #Conformance #LexicalAnalysis #Constants 
// Regression test for FSHARP1.0:1284
// Enum type definitions do not support negative literals
// Negative literal with no space 
// As per FSHARP1.0:3714, enums can't be based on floats (pos/neg)


#light

type EnumDouble     = | A1 = -1.2   // err
type EnumSingle     = | A1 = -1.2f  // err
