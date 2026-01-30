// #Regression #FSharpQA #Diagnostics  
// Regression for 4858
//<Expects status="error" id="FS0251" span="(12,11-12,15)">Invalid member signature encountered because of an earlier error</Expects>
//<Expects status="error" id="FS0767" span="(12,11-12,15)">The member 'Text' does not correspond to any abstract or virtual method available to override or implement</Expects>
//<Expects status="error" id="FS0035" span="(12,11-12,15)">This construct is deprecated: This form of object expression is not used in F#\. Use 'member this\.MemberName \.\.\. = \.\.\.' to define member implementations in object expressions\.</Expects>

type T() = class end

let form = 
    { new T()
      with
          Text="Hej"
    }

exit 1
