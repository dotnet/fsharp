
neg62.fs(11,3,11,12): typecheck error FS0963: This definition may only be used in a type with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'.

neg62.fs(14,3,14,21): typecheck error FS0963: This definition may only be used in a type with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'.

neg62.fs(13,6,13,41): typecheck error FS0037: Duplicate definition of type, exception or module 'DoBindingInClassWithoutImplicitCtor'

neg62.fs(18,3,18,12): typecheck error FS0960: 'let' and 'do' bindings must come before member and interface definitions in type definitions

neg62.fs(22,5,22,14): typecheck error FS0960: 'let' and 'do' bindings must come before member and interface definitions in type definitions

neg62.fs(28,5,28,18): typecheck error FS0960: 'let' and 'do' bindings must come before member and interface definitions in type definitions

neg62.fs(34,5,34,25): typecheck error FS3133: 'member val' definitions are only permitted in types with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'.

neg62.fs(49,6,49,26): typecheck error FS0081: Implicit object constructors for structs must take at least one argument

neg62.fs(50,5,50,34): typecheck error FS0901: Structs cannot contain value definitions because the default constructor for structs will not execute these bindings. Consider adding additional arguments to the primary constructor for the type.

neg62.fs(54,31,54,34): typecheck error FS3135: To indicate that this property can be set, use 'member val PropertyName = expr with get,set'.

neg62.fs(69,24,69,30): typecheck error FS0670: This code is not sufficiently generic. The type variable 'S could not be generalized because it would escape its scope.

neg62.fs(75,5,75,28): typecheck error FS3151: This member, function or value declaration may not be declared 'inline'

neg62.fs(80,5,80,35): typecheck error FS3151: This member, function or value declaration may not be declared 'inline'
