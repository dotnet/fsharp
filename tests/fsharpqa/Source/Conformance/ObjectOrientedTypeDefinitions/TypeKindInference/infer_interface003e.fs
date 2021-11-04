// #Regression #Conformance #ObjectOrientedTypes #TypeInference 

// attribute must match inferred type

//<Expects status="error" span="(15,6)" id="FS0365">No implementation was given for 'abstract TK_I_005\.M: unit -> unit'$</Expects>
//<Expects status="error" span="(15,6)" id="FS0054">This type is 'abstract' since some abstract members have not been given an implementation\. If this is intentional then add the '\[<AbstractClass>\]' attribute to your type\.$</Expects>
//<Expects status="error" span="(19,6)" id="FS0365">No implementation was given for 'abstract TK_C_000\.M: int -> int'$</Expects>
//<Expects status="error" span="(19,6)" id="FS0054">This type is 'abstract' since some abstract members have not been given an implementation\. If this is intentional then add the '\[<AbstractClass>\]' attribute to your type\.$</Expects>

[<AbstractClass>]
type TK_C_000 =
   abstract M : int -> int
   
[<Class>]
type TK_I_005 =
  abstract M  : unit -> unit

[<Class>]
type TK_I_007 = 
  inherit TK_C_000
  
exit 1
