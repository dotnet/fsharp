
neg18.fs(7,13,7,20): typecheck error FS0001: The type 'int' is not compatible with the type ''a seq'

neg18.fs(25,21,25,24): typecheck error FS0033: The type 'Test.AmbiguousTypeNameTests.TwoAmbiguousGenericTypes.M.C<_>' expects 1 type argument(s) but is given 0

neg18.fs(27,17,27,20): typecheck error FS1124: Multiple types exist called 'C', taking different numbers of generic parameters. Provide a type instantiation to disambiguate the type resolution, e.g. 'C<_>'.

neg18.fs(33,17,33,20): typecheck error FS1124: Multiple types exist called 'C', taking different numbers of generic parameters. Provide a type instantiation to disambiguate the type resolution, e.g. 'C<_>'.

neg18.fs(39,21,39,22): typecheck error FS0033: The type 'Test.AmbiguousTypeNameTests.TwoAmbiguousGenericTypes.M.C<_,_>' expects 2 type argument(s) but is given 0

neg18.fs(41,17,41,18): typecheck error FS1124: Multiple types exist called 'C', taking different numbers of generic parameters. Provide a type instantiation to disambiguate the type resolution, e.g. 'C<_>'.

neg18.fs(47,17,47,20): typecheck error FS1124: Multiple types exist called 'C', taking different numbers of generic parameters. Provide a type instantiation to disambiguate the type resolution, e.g. 'C<_>'.

neg18.fs(54,21,54,22): typecheck error FS0033: The type 'Test.AmbiguousTypeNameTests.TwoAmbiguousGenericTypes.M.C<_,_>' expects 2 type argument(s) but is given 0

neg18.fs(56,17,56,18): typecheck error FS1124: Multiple types exist called 'C', taking different numbers of generic parameters. Provide a type instantiation to disambiguate the type resolution, e.g. 'C<_>'.

neg18.fs(62,17,62,20): typecheck error FS1124: Multiple types exist called 'C', taking different numbers of generic parameters. Provide a type instantiation to disambiguate the type resolution, e.g. 'C<_>'.

neg18.fs(121,21,121,25): typecheck error FS0033: The type 'Test.AmbiguousTypeNameTests.OneNonAmbiguousGenericType.M3.C<_>' expects 1 type argument(s) but is given 0

neg18.fs(125,17,125,21): typecheck error FS1125: The instantiation of the generic type 'C' is missing and can't be inferred from the arguments or return type of this member. Consider providing a type instantiation when accessing this type, e.g. 'C<_>'.

neg18.fs(130,21,130,22): typecheck error FS0033: The type 'Test.AmbiguousTypeNameTests.OneNonAmbiguousGenericType.M3.C<_>' expects 1 type argument(s) but is given 0

neg18.fs(134,17,134,20): typecheck error FS1125: The instantiation of the generic type 'C' is missing and can't be inferred from the arguments or return type of this member. Consider providing a type instantiation when accessing this type, e.g. 'C<_>'.

neg18.fs(140,13,140,14): typecheck error FS0033: The type 'Test.AmbiguousTypeNameTests.OneNonAmbiguousGenericType.M3.C<_>' expects 1 type argument(s) but is given 0

neg18.fs(144,17,144,20): typecheck error FS1125: The instantiation of the generic type 'C' is missing and can't be inferred from the arguments or return type of this member. Consider providing a type instantiation when accessing this type, e.g. 'C<_>'.

neg18.fs(154,19,154,22): typecheck error FS1124: Multiple types exist called 'Foo', taking different numbers of generic parameters. Provide a type instantiation to disambiguate the type resolution, e.g. 'Foo<_>'.

neg18.fs(160,26,160,29): typecheck error FS1125: The instantiation of the generic type 'T' is missing and can't be inferred from the arguments or return type of this member. Consider providing a type instantiation when accessing this type, e.g. 'T<_>'.

neg18.fs(170,10,170,13): typecheck error FS0086: The '.[]' operator cannot be redefined. Consider using a different operator name

neg18.fs(182,28,182,29): typecheck error FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type ''b'.

neg18.fs(185,26,185,27): typecheck error FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'b has been constrained to be type ''a'.

neg18.fs(186,12,186,16): typecheck error FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type ''b'.
