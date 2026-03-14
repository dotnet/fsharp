// #Regression #Conformance #ObjectOrientedTypes #TypeInference 

// attribute must match inferred type

//<Expects status="error" span="(15,6)" id="FS0365">No implementation was given for 'abstract TK_I_005\.M: unit -> unit'$</Expects>
//<Expects status="error" span="(15,6)" id="FS0054">Non-abstract classes cannot contain abstract members\. Either provide a default member implementation or add the '\[<AbstractClass>\]' attribute to your type\.$</Expects>
//<Expects status="error" span="(19,6)" id="FS0365">No implementation was given for 'abstract TK_C_000\.M: int -> int'$</Expects>
//<Expects status="error" span="(19,6)" id="FS0054">Non-abstract classes cannot contain abstract members\. Either provide a default member implementation or add the '\[<AbstractClass>\]' attribute to your type\.$</Expects>

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
