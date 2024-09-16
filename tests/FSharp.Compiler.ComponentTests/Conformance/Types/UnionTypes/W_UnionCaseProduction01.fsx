// #Regression #Conformance #TypesAndModules #Unions 
// Union Types
// union-case :=
//  | id	-- nullary union case
//  | id of type * ... * type	-- n-ary union case
//  | id : sig-spec	-- n-ary union case


#light

type T = | D : int -> T
