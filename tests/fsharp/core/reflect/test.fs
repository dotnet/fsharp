// #Conformance #Reflection #Unions #Tuples 
module Test

#nowarn "44"

type PublicUnionType1 = X of string | XX of string * string
type PublicUnionType2 = X2 | XX2 of string
type PublicUnionType3<'T> = X3 | XX3 of 'T
type PublicRecordType1 = { r1a : int }
type PublicRecordType2<'T> = { r2b : 'T; r2a : int }
[<CLIMutable>]
type PublicRecordType3WithCLIMutable<'T> = { r3b : 'T; r3a : int }


type internal InternalUnionType1 = InternalX of string | InternalXX of string * string
type internal InternalUnionType2 = InternalX2 | InternalXX2 of string
type internal InternalUnionType3<'T> = InternalX3 | InternalXX3 of 'T
type internal InternalRecordType1 = { internal_r1a : int }
type internal InternalRecordType2<'T> = { internal_r2b : 'T; internal_r2a : int }
