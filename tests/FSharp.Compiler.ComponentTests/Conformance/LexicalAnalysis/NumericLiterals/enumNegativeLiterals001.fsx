// #Regression #Conformance #LexicalAnalysis #Constants 
// Regression test for FSHARP1.0:1284
// Enum type definitions do not support negative literals
// Negative literal with no space 
// See FSHARP1.0:3714
#light

type EnumInt8       = | A1 = -10y
type EnumInt16      = | A1 = -10s
type EnumInt32      = | A1 = -10
type EnumInt64      = | A1 = -10L
// type EnumDouble     = | A1 = -1.2   -- moved to a negative test since they are now illegal
// type EnumSingle     = | A1 = -1.2f  -- moved to a negative test since they are now illegal
