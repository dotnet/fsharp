
test.fsx(147,18,147,19): typecheck error FS3391: This expression uses the implicit conversion 'Decimal.op_Implicit(value: int) : decimal' to convert type 'int' to type 'decimal'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".

test.fsx(149,39,149,41): typecheck error FS3391: This expression uses the implicit conversion 'Xml.Linq.XNamespace.op_Implicit(namespaceName: string) : Xml.Linq.XNamespace' to convert type 'string' to type 'Xml.Linq.XNamespace'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".

test.fsx(178,20,178,21): typecheck error FS3391: This expression uses the implicit conversion 'static member C.op_Implicit: x: 'T -> C<'T>' to convert type 'int' to type 'C<int>'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".

test.fsx(180,15,180,16): typecheck error FS3391: This expression uses the implicit conversion 'static member C.op_Implicit: x: 'T -> C<'T>' to convert type 'int' to type 'C<int>'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".

test.fsx(186,27,186,28): typecheck error FS3391: This expression uses the implicit conversion 'Nullable.op_Implicit(value: int) : Nullable<int>' to convert type 'int' to type 'Nullable<int>'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".

test.fsx(188,15,188,16): typecheck error FS3391: This expression uses the implicit conversion 'Nullable.op_Implicit(value: int) : Nullable<int>' to convert type 'int' to type 'Nullable<int>'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".

test.fsx(463,18,463,19): typecheck error FS0001: This expression was expected to have type
    'C'    
but here has type
    'int'    

test.fsx(471,18,471,19): typecheck error FS0044: This construct is deprecated. nope

test.fsx(482,18,482,21): typecheck error FS3387: This expression has type 'B' and is only made compatible with type 'C' through an ambiguous implicit conversion. Consider using an explicit call to 'op_Implicit'. The applicable implicit conversions are:
   static member B.op_Implicit: x: B -> C
   static member C.op_Implicit: x: B -> C

test.fsx(482,18,482,21): typecheck error FS3387: This expression has type 'B' and is only made compatible with type 'C' through an ambiguous implicit conversion. Consider using an explicit call to 'op_Implicit'. The applicable implicit conversions are:
   static member B.op_Implicit: x: B -> C
   static member C.op_Implicit: x: B -> C

test.fsx(546,30,546,31): typecheck error FS0001: This expression was expected to have type
    'float32'    
but here has type
    'int'    

test.fsx(546,32,546,33): typecheck error FS0001: All elements of a list must be implicitly convertible to the type of the first element, which here is 'float32'. This element has type 'int'.

test.fsx(546,34,546,35): typecheck error FS0001: All elements of a list must be implicitly convertible to the type of the first element, which here is 'float32'. This element has type 'int'.

test.fsx(546,36,546,37): typecheck error FS0001: All elements of a list must be implicitly convertible to the type of the first element, which here is 'float32'. This element has type 'int'.

test.fsx(547,28,547,32): typecheck error FS0001: This expression was expected to have type
    'float'    
but here has type
    'float32'    

test.fsx(547,33,547,37): typecheck error FS0001: All elements of a list must be implicitly convertible to the type of the first element, which here is 'float'. This element has type 'float32'.

test.fsx(547,38,547,42): typecheck error FS0001: All elements of a list must be implicitly convertible to the type of the first element, which here is 'float'. This element has type 'float32'.

test.fsx(547,43,547,47): typecheck error FS0001: All elements of a list must be implicitly convertible to the type of the first element, which here is 'float'. This element has type 'float32'.
