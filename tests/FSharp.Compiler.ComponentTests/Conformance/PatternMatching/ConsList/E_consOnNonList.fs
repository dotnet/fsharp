// #Regression #Conformance #PatternMatching
//<Expects span="(4,21-4,28)" status="error" id="FS0001">This expression was expected to have type.    'int'    .but here has type.    ''a list'</Expects>
let f (x : int) = match x with
                  | _ :: [] -> 1
                  | _ :: _ :: [] -> 2
                  | _ -> 0

