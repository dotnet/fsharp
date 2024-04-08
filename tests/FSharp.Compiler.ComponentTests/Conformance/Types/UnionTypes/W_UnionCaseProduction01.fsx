// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// union-case :=
//  | id	-- nullary union case
//  | id of type * ... * type	-- n-ary union case
//  | id : sig-spec	-- n-ary union case
//<Expects id="FS0042" span="(11,12-11,24)" status="warning">This construct is deprecated: it is only for use in the F# library</Expects>
//<Expects id="FS0042" span="(11,12-11,24)" status="warning">This construct is deprecated: it is only for use in the F# library</Expects>
#light

type T = | D : int -> T
