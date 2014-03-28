// #Regression #Libraries #Collections 
// Regression test for FSharp1.0:5641
// Title: List.hd/tl --> List.head/tail

//<Expects status="error" span="(30,6-30,8)" id="FS0039">The value, constructor, namespace or type 'hd' is not defined$</Expects>
//<Expects status="error" span="(31,6-31,8)" id="FS0039">The value, constructor, namespace or type 'tl' is not defined$</Expects>

// Positive tests...
if  (List.head [1 .. 10] <> 1)
 || (List.head ["a"] <> "a")
 || (List.tail [1 .. 10] <> [2 .. 10])
 || (List.tail [1] <> [])
 || (List.head ['a'; 'a'] <> List.head (List.tail ['a'; 'a']))
then exit 1

// Negative tests...
try
  List.head [] |> ignore
  exit 1
with
  | :? System.ArgumentException -> ()
  
try
  List.tail [] |> ignore
  exit 1
with
  | :? System.ArgumentException -> ()

// Test deprecation message (now it's an error!)
List.hd [1] |> ignore
List.tl [1] |> ignore

exit 0
