// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.MemberDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MethodsAndProperties =

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    // SOURCE=AbstractProperties01.fs								# AbstractProperties01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AbstractProperties01.fs"|])>]
    let ``AbstractProperties01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_AbstractAndConcereteProp.fs SCFLAGS="--test:ErrorRanges"		# E_AbstractAndConcereteProp.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AbstractAndConcereteProp.fs"|])>]
    let ``E_AbstractAndConcereteProp_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 435, Line 10, Col 14, Line 10, Col 19, "The property 'State' of type 'X' has a getter and a setter that do not match. If one is abstract then the other must be as well.")
        ]

    // SOURCE=E_AbstractProperties02.fs SCFLAGS="--test:ErrorRanges --flaterrors"		# E_AbstractProperties02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AbstractProperties02.fs"|])>]
    let ``E_AbstractProperties02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 435, Line 7, Col 13, Line 7, Col 14, "The property 'A' of type 'T' has a getter and a setter that do not match. If one is abstract then the other must be as well.")
            (Error 3172, Line 7, Col 13, Line 7, Col 14, "A property's getter and setter must have the same type. Property 'A' has getter of type 'int' but setter of type 'obj'.")
        ]

    // SOURCE=E_AbstractProperties03.fs SCFLAGS="--test:ErrorRanges"				# E_AbstractProperties03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AbstractProperties03.fs"|])>]
    let ``E_AbstractProperties03_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 435, Line 8, Col 14, Line 8, Col 19, "The property 'State' of type 'X' has a getter and a setter that do not match. If one is abstract then the other must be as well.")
        ]

    // SOURCE=E_ActivePatternMember01.fs  SCFLAGS="--test:ErrorRanges"	# E_ActivePatternMember01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ActivePatternMember01.fs"|])>]
    let ``E_ActivePatternMember01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 827, Line 10, Col 19, Line 10, Col 37, "This is not a valid name for an active pattern")
            (Error 39, Line 21, Col 10, Line 21, Col 13, "The type 'FaaBor' does not define the field, constructor or member 'Foo'.")
        ]

    // SOURCE=E_ActivePatternMember02.fs  SCFLAGS="--test:ErrorRanges"	# E_ActivePatternMember02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ActivePatternMember02.fs"|])>]
    let ``E_ActivePatternMember02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 827, Line 6, Col 12, Line 6, Col 27, "This is not a valid name for an active pattern")
        ]

    // SOURCE=E_DuplicateProperty01.fs SCFLAGS="--test:ErrorRanges"	# E_DuplicateProperty01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_DuplicateProperty01.fs"|])>]
    let ``E_DuplicateProperty01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 438, Line 10, Col 18, Line 10, Col 29, "Duplicate method. The method 'get_Property001' has the same name and signature as another method in type 'NM.ClassMembers'.")
            (Error 438, Line 8, Col 18, Line 8, Col 29, "Duplicate method. The method 'get_Property001' has the same name and signature as another method in type 'NM.ClassMembers'.")
        ]

    // SOURCE=E_IndexerArityMismatch01.fs     SCFLAGS="--test:ErrorRanges --flaterrors"	# E_IndexerArityMismatch01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IndexerArityMismatch01.fs"|])>]
    let ``E_IndexerArityMismatch01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 11, Col 24, Line 11, Col 33, "This expression was expected to have type\n    ''a array2d'    \nbut here has type\n    'int array'    ")
            (Error 1, Line 12, Col 25, Line 12, Col 32, "This expression was expected to have type\n    ''a array'    \nbut here has type\n    'int array2d'    ")
            (Error 1, Line 13, Col 26, Line 13, Col 37, "This expression was expected to have type\n    ''a array'    \nbut here has type\n    'int array3d'    ")
            (Error 1, Line 14, Col 27, Line 14, Col 38, "This expression was expected to have type\n    ''a array'    \nbut here has type\n    'int array4d'    ")
        ]

    // SOURCE=E_IndexerArityMismatch02.fs     SCFLAGS="--test:ErrorRanges --flaterrors"	# E_IndexerArityMismatch02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IndexerArityMismatch02.fs"|])>]
    let ``E_IndexerArityMismatch02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 11, Col 24, Line 11, Col 35, "This expression was expected to have type\n    ''a array3d'    \nbut here has type\n    'int array'    ")
            (Error 1, Line 12, Col 25, Line 12, Col 32, "This expression was expected to have type\n    ''a array'    \nbut here has type\n    'int array2d'    ")
            (Error 1, Line 13, Col 27, Line 13, Col 38, "This expression was expected to have type\n    ''a array3d'    \nbut here has type\n    'int array4d'    ")
            (Error 1, Line 14, Col 27, Line 14, Col 36, "This expression was expected to have type\n    ''a array2d'    \nbut here has type\n    'int array4d'    ")
        ]

    // SOURCE=E_IndexerIndeterminateType01.fs SCFLAGS=--test:ErrorRanges			# E_IndexerIndeterminateType01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IndexerIndeterminateType01.fs"|])>]
    let ``E_IndexerIndeterminateType01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 752, Line 5, Col 11, Line 5, Col 16, "The operator 'expr.[idx]' has been used on an object of indeterminate type based on information prior to this program point. Consider adding further type constraints")
        ]

    // SOURCE=E_IndexerNotSpecified01.fs							# E_IndexerNotSpecified01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IndexerNotSpecified01.fs"|])>]
    let ``E_IndexerNotSpecified01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 13, Col 9, Line 13, Col 15, "The type 'Foo' does not define the field, constructor or member 'Item'.")
        ]

    // SOURCE=E_OutscopeThisPtr01.fs      SCFLAGS="--test:ErrorRanges"	# E_OutscopeThisPtr01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OutscopeThisPtr01.fs"|])>]
    let ``E_OutscopeThisPtr01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 7, Col 24, Line 7, Col 28, "'this' is bound twice in this pattern")
        ]

    // SOURCE=E_Properties02.fs SCFLAGS="--test:ErrorRanges"		# E_Properties02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Properties02.fs"|])>]
    let ``E_Properties02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 554, Line 9, Col 27, Line 9, Col 35, "Invalid declaration syntax")
        ]

    // SOURCE=E_Properties06.fs SCFLAGS="--test:ErrorRanges  --flaterrors"	# E_Properties06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Properties06.fs"|])>]
    let ``E_Properties06_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 15, Col 13, Line 15, Col 16, "This expression was expected to have type\n    'unit'    \nbut here has type\n    'int'    ")
        ]

    // SOURCE=E_SettersMustHaveUnit01.fs		# E_SettersMustHaveUnit01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SettersMustHaveUnit01.fs"|])>]
    let ``E_SettersMustHaveUnit01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 10, Col 66, Line 10, Col 80, "This expression was expected to have type\n    'unit'    \nbut here has type\n    'immut'    ")
        ]

    // SOURCE=E_SettersMustHaveUnit02.fs SCFLAGS="--test:ErrorRanges --flaterrors"		# E_SettersMustHaveUnit02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SettersMustHaveUnit02.fs"|])>]
    let ``E_SettersMustHaveUnit02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 9, Col 53, Line 9, Col 71, "This expression was expected to have type\n    'unit'    \nbut here has type\n    'string'    ")
        ]

    // SOURCE=E_UndefinedThisVariable.fs				# E_UndefinedThisVariable.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_UndefinedThisVariable.fs"|])>]
    let ``E_UndefinedThisVariable_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 673, Line 6, Col 12, Line 6, Col 19, "This instance member needs a parameter to represent the object being invoked. Make the member static or use the notation 'member x.Member(args) = ...'.")
        ]

    // SOURCE=E_UndefinedThisVariable02.fs				# E_UndefinedThisVariable02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_UndefinedThisVariable02.fs"|])>]
    let ``E_UndefinedThisVariable02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 673, Line 9, Col 14, Line 9, Col 22, "This instance member needs a parameter to represent the object being invoked. Make the member static or use the notation 'member x.Member(args) = ...'.")
        ]

    // SOURCE=E_useInstMethodThroughStatic.fs		# E_useInstMethodThroughStatic.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_useInstMethodThroughStatic.fs"|])>]
    let ``E_UseInstMethodThroughStatic_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3214, Line 10, Col 14, Line 10, Col 25, "Method or object constructor 'DoStuff' is not static")
        ]

    // SOURCE=E_useStaticMethodThroughInstance.fs	# E_useStaticMethodThroughInstance.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_useStaticMethodThroughInstance.fs"|])>]
    let ``E_useStaticMethodThroughInstance_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 493, Line 10, Col 14, Line 10, Col 30, "StaticMethod is not an instance method")
        ]

    // SOURCE=genericGenericClass.fs			# genericGenericClass.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"genericGenericClass.fs"|])>]
    let ``genericGenericClass_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=GetAndSetKeywords01.fs			# GetAndSetKeywords01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GetAndSetKeywords01.fs"|])>]
    let ``GetAndSetKeywords01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=GetterSetterDiff01.fs			# GetterSetterDiff01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GetterSetterDiff01.fs"|])>]
    let ``GetterSetterDiff01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Indexer01.fs				# Indexer01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Indexer01.fs"|])>]
    let ``Indexer01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Indexer02.fs				# Indexer02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Indexer02.fs"|])>]
    let ``Indexer02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Indexer02.fs SCFLAGS=-a			# Indexer03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Indexer03.fs"|])>]
    let ``Indexer03_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=instMembers-class.fs			# instMembers-class.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"instMembers-class.fs"|])>]
    let ``instMembers-class_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:221"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=instMembers-DU.fs			# instMembers-DU.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"instMembers-DU.fs"|])>]
    let ``InstMembers-DU_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:221"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=instMembers-Records.fs			# InstMembers-Records.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"instMembers-Records.fs"|])>]
    let ``InstMembers-Records_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:221"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=multiParamIndexer.fs			# MultiParamIndexer.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"multiParamIndexer.fs"|])>]
    let ``MultiParamIndexer_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Properties01.fs						# Properties01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Properties01.fs"|])>]
    let ``Properties01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Properties02.fs						# Properties02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Properties02.fs"|])>]
    let ``Properties02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Properties03.fs						# Properties03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Properties03.fs"|])>]
    let ``Properties03_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Properties04.fs						# Properties04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Properties04.fs"|])>]
    let ``Properties04_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Properties05.fs						# Properties05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Properties05.fs"|])>]
    let ``Properties05_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=RecursiveLetValues.fs					# RecursiveLetValues.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveLetValues.fs"|])>]
    let ``RecursiveLetValues_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=StaticGenericField01.fs					# StaticGenericField01
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticGenericField01.fs"|])>]
    let ``StaticGenericField01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=staticMembers-class.fs			# StaticMembers-Class.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"staticMembers-class.fs"|])>]
    let ``StaticMembers-Class_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:221"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=staticMembers-DU.fs			# StaticMembers-DU.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"staticMembers-DU.fs"|])>]
    let ``StaticMembers-DU_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:221"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=staticMembers-Records.fs			# StaticMembers-Record.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"staticMembers-Records.fs"|])>]
    let ``staticMembers-Records_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:221"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=staticMembers-instance.fs		# StaticMembers-instance.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"staticMembers-instance.fs"|])>]
    let ``StaticMembers-instance_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=tupesAndFuncsAsArgs.fs			# TupesAndFuncsAsArgs.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"tupesAndFuncsAsArgs.fs"|])>]
    let ``TupesAndFuncsAsArgs_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=TupledIndexer.fs				# TupledIndexer.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TupledIndexer.fs"|])>]
    let ``TupledIndexer_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=typeMethodsCurrable.fs			# TypeMethodsCurrable.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeMethodsCurrable.fs"|])>]
    let ``TypeMethodsCurrable_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=tupledValueProperties01.fs							# tupledValueProperties01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"tupledValueProperties01.fs"|])>]
    let ``tupledValueProperties01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=tupledValueProperties02.fsx SCFLAGS="--nologo" FSIMODE=PIPE COMPILE_ONLY=1	# tupledValueProperties02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"tupledValueProperties02.fsx"|])>]
    let ``tupledValueProperties02_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=E_Abstract_ReusedParam.fs			# E_Abstract_ReusedParam.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Abstract_ReusedParam.fs"|])>]
    let ``E_Abstract_Methods_ReusedParam_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3549, Line 5, Col 5, Line 5, Col 38, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 7, Col 5, Line 7, Col 39, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 9, Col 5, Line 9, Col 47, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'j' has been used more that once in this method.")
        ]
        
    [<Fact>]
    let ``Error in signature file with not implementation file with abstract methods when reusing parameters`` () =
        let encodeFsi =
            Fsi """
namespace Foo
type I =
    // Tupled.
    abstract M : i:int * i:int -> int
    // Curried.
    abstract N : i:int -> i:int -> int
    // More than two.
    abstract O : i:int * i: int * i:int -> int
    // Multiple distinct names repeated.
    abstract P : i:int * j:int * i:int * j:int -> int
    """
        encodeFsi
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3549, Line 5, Col 5, Line 5, Col 38, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 7, Col 5, Line 7, Col 39, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 9, Col 5, Line 9, Col 47, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'j' has been used more that once in this method.")
            (Error 240, Line 2, Col 1, Line 11, Col 54, "The signature file 'Test' does not have a corresponding implementation file. If an implementation file exists then check the 'module' and 'namespace' declarations in the signature and implementation files match.")
        ]
        
    [<Fact>]
    let ``Error in signature file with not implementation file with abstract methods when reusing parameters in recursive namespace`` () =
        let encodeFsi =
            Fsi """
namespace rec Foo
type I =
    // Tupled.
    abstract M : i:int * i:int -> int
    // Curried.
    abstract N : i:int -> i:int -> int
    // More than two.
    abstract O : i:int * i: int * i:int -> int
    // Multiple distinct names repeated.
    abstract P : i:int * j:int * i:int * j:int -> int
    """
        encodeFsi
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3549, Line 5, Col 5, Line 5, Col 38, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 7, Col 5, Line 7, Col 39, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 9, Col 5, Line 9, Col 47, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'j' has been used more that once in this method.")
            (Error 240, Line 2, Col 1, Line 11, Col 54, "The signature file 'Test' does not have a corresponding implementation file. If an implementation file exists then check the 'module' and 'namespace' declarations in the signature and implementation files match.")
        ]

    [<Fact>]
    let ``Errors in signature and implementation files with abstract methods when reusing parameters`` () =
        let encodeFsi =
            Fsi """
namespace Foo
type I =
    // Tupled.
    abstract M : i:int * i:int -> int
    // Curried.
    abstract N : i:int -> i:int -> int
    // More than two.
    abstract O : i:int * i: int * i:int -> int
    // Multiple distinct names repeated.
    abstract P : i:int * j:int * i:int * j:int -> int
    """
        let encodeFs =
            FsSource """
namespace Foo
type I =
    abstract M : i:int * i:int -> int
    abstract N : i:int -> i:int -> int
    abstract O : i:int * i: int * i:int -> int
    abstract P : i:int * j:int * i:int * j:int -> int
        """
        encodeFsi
        |> withAdditionalSourceFile encodeFs
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3549, Line 5, Col 5, Line 5, Col 38, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 7, Col 5, Line 7, Col 39, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 9, Col 5, Line 9, Col 47, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'j' has been used more that once in this method.")
            (Error 3549, Line 4, Col 5, Line 4, Col 38, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 5, Col 5, Line 5, Col 39, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 6, Col 5, Line 6, Col 47, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 7, Col 5, Line 7, Col 54, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 7, Col 5, Line 7, Col 54, "Duplicate parameter. The parameter 'j' has been used more that once in this method.")
        ]

    [<Fact>]
    let ``Errors in signature and implementation files with abstract methods when reusing parameters in recursive namespaces`` () =
        let encodeFsi =
            Fsi """
namespace rec Foo
type I =
    // Tupled.
    abstract M : i:int * i:int -> int
    // Curried.
    abstract N : i:int -> i:int -> int
    // More than two.
    abstract O : i:int * i: int * i:int -> int
    // Multiple distinct names repeated.
    abstract P : i:int * j:int * i:int * j:int -> int
    """
        let encodeFs =
            FsSource """
namespace rec Foo
type I =
    abstract M : i:int * i:int -> int
    abstract N : i:int -> i:int -> int
    abstract O : i:int * i: int * i:int -> int
    abstract P : i:int * j:int * i:int * j:int -> int
        """
        encodeFsi
        |> withAdditionalSourceFile encodeFs
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3549, Line 5, Col 5, Line 5, Col 38, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 7, Col 5, Line 7, Col 39, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 9, Col 5, Line 9, Col 47, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 11, Col 5, Line 11, Col 54, "Duplicate parameter. The parameter 'j' has been used more that once in this method.")
            (Error 3549, Line 4, Col 5, Line 4, Col 38, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 5, Col 5, Line 5, Col 39, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 6, Col 5, Line 6, Col 47, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 7, Col 5, Line 7, Col 54, "Duplicate parameter. The parameter 'i' has been used more that once in this method.")
            (Error 3549, Line 7, Col 5, Line 7, Col 54, "Duplicate parameter. The parameter 'j' has been used more that once in this method.")
        ]
