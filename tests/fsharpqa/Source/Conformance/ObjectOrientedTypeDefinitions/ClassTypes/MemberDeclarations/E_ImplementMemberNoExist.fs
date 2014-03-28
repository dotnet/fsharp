// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Regression test for FSharp1.0:3752 - internal error: dest_top_tau_type internal error, #curriedArgInfos.Length = 2, #argtys.Length = 1 (Failure)
//<Expects status="error" span="(11,25-11,26)" id="FS0035">This construct is deprecated: This form of object expression is not used in F#\. Use 'member this\.MemberName \.\.\. = \.\.\.' to define member implementations in object expressions\.$</Expects>
//<Expects span="(11,25-11,26)" status="error" id="FS0767">The member 'M' does not correspond to any abstract or virtual method available to override or implement</Expects>
//<Expects span="(11,25-11,26)" status="error" id="FS0251">Invalid member signature encountered because of an earlier error</Expects>
module M
type T () =
    class
    end
  
let a = { new T () with M = 0 }

exit 1

