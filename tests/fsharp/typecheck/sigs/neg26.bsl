
neg26.fs(15,13,15,56): typecheck error FS0366: No implementation was given for 'abstract ITest.Meth1: string -> string'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.

neg26.fs(27,13,27,66): typecheck error FS0366: No implementation was given for 'abstract ITestSub.Meth2: int -> int'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.

neg26.fs(40,27,40,32): typecheck error FS0361: The override 'Meth1: int -> int' implements more than one abstract slot, e.g. 'abstract ITestSub.Meth1: int -> int' and 'abstract ITest.Meth1: int -> int'

neg26.fs(53,27,53,32): typecheck error FS3213: The member 'Meth1: 'a -> 'a' matches multiple overloads of the same method.
Please restrict it to one of the following:
   Meth1: int -> int
   Meth1: int -> int.

neg26.fs(52,15,52,23): typecheck error FS0783: At least one override did not correctly implement its corresponding abstract member
