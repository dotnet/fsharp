// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSharp1.0:5611
// Title: Give an error on uses of UseNullAsTrueValue

//<Expects status="error" id="FS1196" span="(15,6-15,13)">The 'UseNullAsTrueValue' attribute flag may only be used with union types that have one nullary case and at least one non-nullary case</Expects>
//<Expects status="error" id="FS1196" span="(22,6-22,14)">The 'UseNullAsTrueValue' attribute flag may only be used with union types that have one nullary case and at least one non-nullary case</Expects>
//<Expects status="error" id="FS1196" span="(28,6-28,14)">The 'UseNullAsTrueValue' attribute flag may only be used with union types that have one nullary case and at least one non-nullary case</Expects>
//<Expects status="error" id="FS1196" span="(33,6-33,15)">The 'UseNullAsTrueValue' attribute flag may only be used with union types that have one nullary case and at least one non-nullary case</Expects>
//<Expects status="error" id="FS1196" span="(37,6-37,14)">The 'UseNullAsTrueValue' attribute flag may only be used with union types that have one nullary case and at least one non-nullary case</Expects>
//<Expects status="error" id="FS1196" span="(44,6-44,18)">The 'UseNullAsTrueValue' attribute flag may only be used with union types that have one nullary case and at least one non-nullary case</Expects>
//<Expects status="error" id="FS1196" span="(51,6-51,15)">The 'UseNullAsTrueValue' attribute flag may only be used with union types that have one nullary case and at least one non-nullary case</Expects>

// expect error
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type MyUnion = 
    | A1
    | A
    | B of string

// expect error
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type MyUnion2 =
    | A1
    | A 

// expect error
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type MyUnion3 = 
    | A1

// expect error
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type MyRecord3 = { x : int }

// expect error
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type MyClass4() = 
   class
      member x.P = 1
   end

// expect error
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type MyInterface5 = 
   interface 
       abstract P : int
   end

// expect error
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type MyStruct6(x : int) = 
   struct
       member __.X = x
   end
