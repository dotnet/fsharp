// #Regression #Diagnostics
// Regression test for FSharp1.0:2681
//<Expects id="FS0010" span="(6,8-6,20)" status="error">Unexpected start of structured construct in definition\. Expected identifier, 'global' or other token</Expects>
#light

module '\U00002620'
