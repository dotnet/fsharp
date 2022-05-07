
neg_interface_generics.fsx(14,1,18,2): typecheck error FS0505: The member or object constructor 'Foo' does not take 13073 argument(s). An overload was found taking 1 arguments.

neg_interface_generics.fsx(20,9,20,27): typecheck error FS0041: No overloads match for method 'Foo'.

Known types of arguments: string * XmlReader

Available overloads:
 - abstract IFoo.Foo: t: Type * r: TextReader -> obj * 't // Argument 't' doesn't match
 - abstract IFoo.Foo: t: string * r: TextReader -> obj * 't // Argument 'r' doesn't match
