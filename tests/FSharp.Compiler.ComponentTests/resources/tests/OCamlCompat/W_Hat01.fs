// #OCaml
// Should compile with a warning
//<Expects status="warning" span="(6,13-6,14)" id="FS0062">This construct is for ML compatibility\. Consider using the '\+' operator instead\. This may require a type annotation to indicate it acts on strings\. This message can be disabled using '--nowarn:62' or '#nowarn "62"'\.$</Expects>
module TestModule

let r = "a" ^ "b"
(if r = "ab" then 0 else 1) |> exit
