
  #blaaaaaa // blaaaaaa is not a known command;;
  ^

stdin(220,1): warning FS3353: Invalid directive '#blaaaaaa '


  type Regression4319_T0  = static member (+-+-+)             = "0 arguments";;
  -----------------------------------------^^^^^

stdin(572,42): warning FS1172: Infix operator member '+-+-+' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_T1  = static member (+-+-+) x           = "1 argument";;
  -----------------------------------------^^^^^

stdin(573,42): warning FS1173: Infix operator member '+-+-+' has 1 initial argument(s). Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_T1b = static member (+-+-+) (x)         = "1 (argument) [brackets make no diff]";;
  -----------------------------------------^^^^^

stdin(574,42): warning FS1173: Infix operator member '+-+-+' has 1 initial argument(s). Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_T1c = static member (+-+-+) x           = let a,b = x in "1 argument, tuple typed from RHS. Still not OK";;
  -----------------------------------------^^^^^

stdin(575,42): warning FS1173: Infix operator member '+-+-+' has 1 initial argument(s). Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_T1d = static member (+-+-+) (x:int*int) = "1 argument, tuple typed from LHS. Still not OK";;
  -----------------------------------------^^^^^

stdin(576,42): warning FS1173: Infix operator member '+-+-+' has 1 initial argument(s). Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_T3  = static member (+-+-+) (x,y,z)          = "3 arguments";;
  -----------------------------------------^^^^^

stdin(578,42): warning FS1173: Infix operator member '+-+-+' has 3 initial argument(s). Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_U1  = static member (+-+-+) x       moreArgs = "1 argument and further args";;
  -----------------------------------------^^^^^

stdin(579,42): warning FS1173: Infix operator member '+-+-+' has 1 initial argument(s). Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_U1  = static member (+-+-+) x       moreArgs = "1 argument and further args";;
  -----------------------------------------^^^^^

stdin(579,42): warning FS1174: Infix operator member '+-+-+' has extra curried arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_U1b = static member (+-+-+) (x)     moreArgs = "1 (argument) [brackets make no diff] and further args";;
  -----------------------------------------^^^^^

stdin(580,42): warning FS1173: Infix operator member '+-+-+' has 1 initial argument(s). Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_U1b = static member (+-+-+) (x)     moreArgs = "1 (argument) [brackets make no diff] and further args";;
  -----------------------------------------^^^^^

stdin(580,42): warning FS1174: Infix operator member '+-+-+' has extra curried arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_U2  = static member (+-+-+) (x,y)   moreArgs = "1 argument and further args";;
  -----------------------------------------^^^^^

stdin(581,42): warning FS1174: Infix operator member '+-+-+' has extra curried arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_U3  = static member (+-+-+) (x,y,z) moreArgs = "1 argument and further args";;
  -----------------------------------------^^^^^

stdin(582,42): warning FS1173: Infix operator member '+-+-+' has 3 initial argument(s). Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


  type Regression4319_U3  = static member (+-+-+) (x,y,z) moreArgs = "1 argument and further args";;
  -----------------------------------------^^^^^

stdin(582,42): warning FS1174: Infix operator member '+-+-+' has extra curried arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (:=)             = "COLON_EQUALS"
  -------------------^^

stdin(585,20): warning FS1172: Infix operator member ':=' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (:=)             = "COLON_EQUALS"
  -------------------^^

stdin(585,20): warning FS0086: The name '(:=)' should not be used as a member name because it is given a standard definition in the F# library over fixed types


      static member (&)              = "AMP"
  -------------------^

stdin(589,20): warning FS1172: Infix operator member '&' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (&)              = "AMP"
  -------------------^

stdin(589,20): warning FS0086: The name '(&)' should not be used as a member name. If defining a static member for use from other CLI languages then use the name 'op_Amp' instead.


      static member (&^)             = "AMP_AMP"
  -------------------^^

stdin(590,20): warning FS1172: Infix operator member '&^' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (=)              = "EQUALS"
  -------------------^

stdin(591,20): warning FS1172: Infix operator member '=' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (=)              = "EQUALS"
  -------------------^

stdin(591,20): warning FS0086: The name '(=)' should not be used as a member name. To define equality semantics for a type, override the 'Object.Equals' member. If defining a static member for use from other CLI languages then use the name 'op_Equality' instead.


      static member (!=)             = "INFIX_COMPARE_OP"
  -------------------^^

stdin(593,20): warning FS1172: Infix operator member '!=' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (...=)          = "INFIX_COMPARE_OP" // with $. prefix    
  -------------------^^^^

stdin(597,20): warning FS1172: Infix operator member '...=' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (...!=)         = "INFIX_COMPARE_OP" // with $. prefix    
  -------------------^^^^^

stdin(598,20): warning FS1172: Infix operator member '...!=' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (...<)          = "INFIX_COMPARE_OP" // with $. prefix    
  -------------------^^^^

stdin(599,20): warning FS1172: Infix operator member '...<' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (...>)          = "INFIX_COMPARE_OP" // with $. prefix    
  -------------------^^^^

stdin(600,20): warning FS1172: Infix operator member '...>' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member ($)              = "DOLLAR"
  -------------------^

stdin(602,20): warning FS1172: Infix operator member '$' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (<)              = "LESS"
  -------------------^

stdin(603,20): warning FS1172: Infix operator member '<' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (<)              = "LESS"
  -------------------^

stdin(603,20): warning FS0086: The name '(<)' should not be used as a member name. To define comparison semantics for a type, implement the 'System.IComparable' interface. If defining a static member for use from other CLI languages then use the name 'op_LessThan' instead.


      static member (>)              = "GREATER"
  -------------------^

stdin(604,20): warning FS1172: Infix operator member '>' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (>)              = "GREATER"
  -------------------^

stdin(604,20): warning FS0086: The name '(>)' should not be used as a member name. To define comparison semantics for a type, implement the 'System.IComparable' interface. If defining a static member for use from other CLI languages then use the name 'op_GreaterThan' instead.


      static member (@)              = "INFIX_AT_HAT_OP"
  -------------------^

stdin(605,20): warning FS1172: Infix operator member '@' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (@)              = "INFIX_AT_HAT_OP"
  -------------------^

stdin(605,20): warning FS0086: The name '(@)' should not be used as a member name because it is given a standard definition in the F# library over fixed types


      static member (^)              = "INFIX_AT_HAT_OP"        
  -------------------^

stdin(606,20): warning FS1172: Infix operator member '^' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (^)              = "INFIX_AT_HAT_OP"        
  -------------------^

stdin(606,20): warning FS0086: The name '(^)' should not be used as a member name because it is given a standard definition in the F# library over fixed types


      static member (...@)          = "INFIX_AT_HAT_OP" // with $. prefix    
  -------------------^^^^

stdin(607,20): warning FS1172: Infix operator member '...@' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (...^)          = "INFIX_AT_HAT_OP" // with $. prefix    
  -------------------^^^^

stdin(608,20): warning FS1172: Infix operator member '...^' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (%)              = "PERCENT_OP"        
  -------------------^

stdin(609,20): warning FS1172: Infix operator member '%' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (-)              = "MINUS"
  -------------------^

stdin(611,20): warning FS1172: Infix operator member '-' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member ( * )            = "STAR"
  --------------------^

stdin(612,21): warning FS1172: Infix operator member '*' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member (/)              = "INFIX_STAR_DIV_MOD_OP"
  -------------------^

stdin(614,20): warning FS1172: Infix operator member '/' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member ( ...* )        = "INFIX_STAR_DIV_MOD_OP" // with $. prefix    
  --------------------^^^^

stdin(616,21): warning FS1172: Infix operator member '...*' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member ( .../ )        = "INFIX_STAR_DIV_MOD_OP" // with $. prefix    
  --------------------^^^^

stdin(617,21): warning FS1172: Infix operator member '.../' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member ( ...% )        = "INFIX_STAR_DIV_MOD_OP" // with $. prefix
  --------------------^^^^

stdin(618,21): warning FS1172: Infix operator member '...%' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      static member ( ** )           = "INFIX_STAR_STAR_OP"
  --------------------^^

stdin(619,21): warning FS1172: Infix operator member '**' has no arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...


      member this.ToString() = "ABC"
  ----------------^^^^^^^^

stdin(624,17): warning FS0864: This new member hides the abstract member 'System.Object.ToString() : string'. Rename the member or use 'override' instead.


      member this.M() = "string"
  ----------------^

stdin(765,17): error FS0438: Duplicate method. The method 'M' has the same name and signature as another method in type 'ExpectDupMethod'.


      member this.P = "string"
  ----------------^

stdin(772,17): error FS0438: Duplicate method. The method 'get_P' has the same name and signature as another method in type 'ExpectDupProperty'.


      type public   IBPublic   = interface inherit IAPrivate abstract Q : int end
  ------------------^^^^^^^^

stdin(779,19): error FS0410: The type 'IAPrivate' is less accessible than the value, member or type 'IBPublic' it is used in.


      type internal IBInternal = interface inherit IAPrivate abstract Q : int end
  ------------------^^^^^^^^^^

stdin(784,19): error FS0410: The type 'IAPrivate' is less accessible than the value, member or type 'IBInternal' it is used in.


      type public   IBPublic   = interface inherit IAInternal abstract Q : int end
  ------------------^^^^^^^^

stdin(793,19): error FS0410: The type 'IAInternal' is less accessible than the value, member or type 'IBPublic' it is used in.


          override x.M(a:string) = 1
  -------------------^

stdin(825,20): error FS0361: The override 'M : string -> int' implements more than one abstract slot, e.g. 'abstract member Regression4232.D.M : 'U -> int' and 'abstract member Regression4232.D.M : 'T -> int'


  let (|A|B|) (x:int) = A x;;
  -----^^^^^

stdin(833,6): error FS1210: Active pattern '|A|B|' has a result type containing type variables that are not determined by the input. The common cause is a when a result case is not mentioned, e.g. 'let (|A|B|) (x:int) = A x'. This can be fixed with a type constraint, e.g. 'let (|A|B|) (x:int) : Choice<int,unit> = A x'


  let (|A|B|) (x:'a) = A x;;
  -----^^^^^

stdin(836,6): error FS1210: Active pattern '|A|B|' has a result type containing type variables that are not determined by the input. The common cause is a when a result case is not mentioned, e.g. 'let (|A|B|) (x:int) = A x'. This can be fixed with a type constraint, e.g. 'let (|A|B|) (x:int) : Choice<int,unit> = A x'


  let (|A|B|) (p:'a) (x:int) = A p;;
  -----^^^^^

stdin(839,6): error FS1210: Active pattern '|A|B|' has a result type containing type variables that are not determined by the input. The common cause is a when a result case is not mentioned, e.g. 'let (|A|B|) (x:int) = A x'. This can be fixed with a type constraint, e.g. 'let (|A|B|) (x:int) : Choice<int,unit> = A x'


  let (|A|B|) = failwith "" : Choice<int,int>;;
  -----^^^^^

stdin(845,6): error FS1209: Active pattern '|A|B|' is not a function

