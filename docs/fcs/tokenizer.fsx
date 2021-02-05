(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Using the F# tokenizer
=========================================

This tutorial demonstrates how to call the F# language tokenizer. Given F# 
source code, the tokenizer generates a list of source code lines that contain
information about tokens on each line. For each token, you can get the type
of the token, exact location as well as color kind of the token (keyword, 
identifier, number, operator, etc.).

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published


Creating the tokenizer
---------------------

To use the tokenizer, reference `FSharp.Compiler.Service.dll` and open the
`FSharp.Compiler.Tokenization` namespace:
*)
#r "FSharp.Compiler.Service.dll"
open FSharp.Compiler.Tokenization
(**
Now you can create an instance of `FSharpSourceTokenizer`. The class takes two 
arguments - the first is the list of defined symbols and the second is the
file name of the source code. The defined symbols are required because the
tokenizer handles `#if` directives. The file name is required only to specify
locations of the source code (and it does not have to exist):
*)
let sourceTok = FSharpSourceTokenizer([], Some "C:\\test.fsx")
(**
Using the `sourceTok` object, we can now (repeatedly) tokenize lines of 
F# source code.

Tokenizing F# code
------------------

The tokenizer operates on individual lines rather than on the entire source
file. After getting a token, the tokenizer also returns new state (as `int64` value).
This can be used to tokenize F# code more efficiently. When source code changes,
you do not need to re-tokenize the entire file - only the parts that have changed.

### Tokenizing single line

To tokenize a single line, we create a `FSharpLineTokenizer` by calling `CreateLineTokenizer`
on the `FSharpSourceTokenizer` object that we created earlier:
*)
let tokenizer = sourceTok.CreateLineTokenizer("let answer=42")
(**
Now, we can write a simple recursive function that calls `ScanToken` on the `tokenizer`
until it returns `None` (indicating the end of line). When the function succeeds, it 
returns `FSharpTokenInfo` object with all the interesting details:
*)
/// Tokenize a single line of F# code
let rec tokenizeLine (tokenizer:FSharpLineTokenizer) state =
  match tokenizer.ScanToken(state) with
  | Some tok, state ->
      // Print token name
      printf "%s " tok.TokenName
      // Tokenize the rest, in the new state
      tokenizeLine tokenizer state
  | None, state -> state
(**
The function returns the new state, which is needed if you need to tokenize multiple lines
and an earlier line ends with a multi-line comment. As an initial state, we can use `0L`:
*)
tokenizeLine tokenizer FSharpTokenizerLexState.Initial
(**
The result is a sequence of tokens with names LET, WHITESPACE, IDENT, EQUALS and INT32.
There is a number of interesting properties on `FSharpTokenInfo` including:

 - `CharClass` and `ColorClass` return information about the token category that
   can be used for colorizing F# code.
 - `LeftColumn` and `RightColumn` return the location of the token inside the line.
 - `TokenName` is the name of the token (as defined in the F# lexer) 

Note that the tokenizer is stateful - if you want to tokenize single line multiple times,
you need to call `CreateLineTokenizer` again.

### Tokenizing sample code

To run the tokenizer on a longer sample code or an entire file, you need to read the
sample input as a collection of `string` values:
*)
let lines = """
  // Hello world
  let hello() =
     printfn "Hello world!" """.Split('\r','\n')
(**
To tokenize multi-line input, we again need a recursive function that keeps the current
state. The following function takes the lines as a list of strings (together with line number
and the current state). We create a new tokenizer for each line and call `tokenizeLine`
using the state from the *end* of the previous line:
*)
/// Print token names for multiple lines of code
let rec tokenizeLines state count lines = 
  match lines with
  | line::lines ->
      // Create tokenizer & tokenize single line
      printfn "\nLine %d" count
      let tokenizer = sourceTok.CreateLineTokenizer(line)
      let state = tokenizeLine tokenizer state
      // Tokenize the rest using new state
      tokenizeLines state (count+1) lines
  | [] -> ()
(**
The function simply calls `tokenizeLine` (defined earlier) to print the names of all
the tokens on each line. We can call it on the previous input with `0L` as the initial
state and `1` as the number of the first line:
*)
lines
|> List.ofSeq
|> tokenizeLines FSharpTokenizerLexState.Initial 1
(**
Ignoring some unimportant details (like whitespace at the beginning of each line and
the first line which is just whitespace), the code generates the following output:

    [lang=text]
    Line 1
      LINE_COMMENT LINE_COMMENT (...) LINE_COMMENT 
    Line 2
      LET WHITESPACE IDENT LPAREN RPAREN WHITESPACE EQUALS 
    Line 3
      IDENT WHITESPACE STRING_TEXT (...) STRING_TEXT STRING 

It is worth noting that the tokenizer yields multiple `LINE_COMMENT` tokens and multiple
`STRING_TEXT` tokens for each single comment or string (roughly, one for each word), so
if you want to get the entire text of a comment/string, you need to concatenate the 
tokens.
*)