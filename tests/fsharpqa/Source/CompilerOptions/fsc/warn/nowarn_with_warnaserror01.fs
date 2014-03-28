// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:4867
// nowarn has no effect if "Warning level = 4" and "Warnings as errors"
//<Expects id="FS0040" span="(6,48)" status="error">This and other recursive references to the object\(s\) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference\. This is because you are defining one or more recursive objects, rather than recursive functions\. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'\.$</Expects>

let rec f = new System.EventHandler(fun _ _ -> f.Invoke(null,null))
