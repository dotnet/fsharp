
test2.fsx(181,9,181,22): typecheck warning FS0193: This expression is a function value, i.e. is missing arguments. Its type is byref<int> -> unit.

test2.fsx(199,9,199,20): typecheck warning FS0193: This expression is a function value, i.e. is missing arguments. Its type is int -> byref<int> -> unit.

test2.fsx(214,9,214,24): typecheck warning FS0193: This expression is a function value, i.e. is missing arguments. Its type is inref<int> * int -> unit.

test2.fsx(7,43,7,45): typecheck info FS3370: The use of ':=' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'cell := expr' to 'cell.Value <- expr'.

test2.fsx(12,26,12,28): typecheck info FS3370: The use of ':=' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'cell := expr' to 'cell.Value <- expr'.

test2.fsx(12,29,12,30): typecheck info FS3370: The use of '!' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change '!cell' to 'cell.Value'.

test2.fsx(29,18,29,19): typecheck error FS3209: The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(36,18,36,19): typecheck error FS3209: The address of the variable 'z' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(45,14,45,15): typecheck error FS3209: The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(49,14,49,15): typecheck error FS3209: The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(56,14,56,15): typecheck error FS3209: The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(59,14,59,15): typecheck error FS3209: The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(68,18,68,19): typecheck error FS3209: The address of the variable 'z' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(69,10,69,11): typecheck error FS3209: The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(79,14,79,29): typecheck error FS3228: The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(87,14,87,29): typecheck error FS3228: The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(93,13,93,14): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(93,17,93,29): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(93,28,93,29): typecheck error FS0421: The address of the variable 'x' cannot be used at this point

test2.fsx(93,17,93,29): typecheck error FS0425: The type of a first-class function cannot contain byrefs

test2.fsx(93,17,93,29): typecheck error FS0425: The type of a first-class function cannot contain byrefs

test2.fsx(112,53,112,54): typecheck error FS3209: The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(124,41,124,42): typecheck error FS3209: The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test2.fsx(133,23,133,33): typecheck error FS0438: Duplicate method. The method 'TestMethod' has the same name and signature as another method in type 'NegativeTests.TestNegativeOverloading'.

test2.fsx(131,23,131,33): typecheck error FS0438: Duplicate method. The method 'TestMethod' has the same name and signature as another method in type 'NegativeTests.TestNegativeOverloading'.

test2.fsx(129,23,129,33): typecheck error FS0438: Duplicate method. The method 'TestMethod' has the same name and signature as another method in type 'NegativeTests.TestNegativeOverloading'.

test2.fsx(137,18,137,22): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(137,18,137,22): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(139,18,139,23): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(141,9,141,14): typecheck error FS3301: The function or method has an invalid return type '(byref<int> * int)'. This is not permitted by the rules of Common IL.

test2.fsx(141,34,141,39): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(141,35,141,36): typecheck error FS0418: The byref typed value 'x' cannot be used at this point

test2.fsx(143,9,143,14): typecheck error FS3301: The function or method has an invalid return type '(byref<int> -> unit)'. This is not permitted by the rules of Common IL.

test2.fsx(145,14,145,15): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(147,17,147,18): typecheck error FS3300: The parameter 'x' has an invalid type '((byref<int> -> unit) * int)'. This is not permitted by the rules of Common IL.

test2.fsx(149,17,149,18): typecheck error FS3300: The parameter 'x' has an invalid type '(byref<int> -> unit)'. This is not permitted by the rules of Common IL.

test2.fsx(149,41,149,42): typecheck error FS3300: The parameter 'y' has an invalid type '(byref<int> * int)'. This is not permitted by the rules of Common IL.

test2.fsx(155,36,155,39): typecheck error FS3300: The parameter 'tup' has an invalid type '(inref<int> * int)'. This is not permitted by the rules of Common IL.

test2.fsx(156,13,156,33): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(158,37,158,38): typecheck error FS3300: The parameter 'x' has an invalid type '(byref<int> -> unit)'. This is not permitted by the rules of Common IL.

test2.fsx(160,37,160,38): typecheck error FS3300: The parameter 'x' has an invalid type 'byref<int> option'. This is not permitted by the rules of Common IL.

test2.fsx(162,17,162,18): typecheck error FS3300: The parameter 'x' has an invalid type 'byref<int> option'. This is not permitted by the rules of Common IL.

test2.fsx(167,13,167,14): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(167,17,167,30): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(170,9,170,15): typecheck error FS3301: The function or method has an invalid return type '(byref<int> -> unit)'. This is not permitted by the rules of Common IL.

test2.fsx(171,9,171,22): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(174,13,174,14): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(176,13,176,26): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(181,9,181,22): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(185,13,185,14): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(185,17,185,28): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(188,9,188,15): typecheck error FS3301: The function or method has an invalid return type '(int -> byref<int> -> unit)'. This is not permitted by the rules of Common IL.

test2.fsx(189,9,189,20): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(192,13,192,14): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(194,13,194,24): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(199,9,199,20): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(203,13,203,14): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(203,17,203,32): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(203,17,203,32): typecheck error FS0425: The type of a first-class function cannot contain byrefs

test2.fsx(207,13,207,14): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(209,13,209,28): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(214,9,214,24): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(217,9,217,15): typecheck error FS3301: The function or method has an invalid return type '(byref<int> * int)'. This is not permitted by the rules of Common IL.

test2.fsx(219,10,219,15): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(219,11,219,12): typecheck error FS0421: The address of the variable 'x' cannot be used at this point

test2.fsx(222,9,222,18): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(226,13,226,14): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(226,17,226,26): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

test2.fsx(276,6,276,7): typecheck info FS3370: The use of '!' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change '!cell' to 'cell.Value'.
