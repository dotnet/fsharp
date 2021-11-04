module Parser = 
  
  (*
  F# implementation of a generic Top-Down-Operator-Precedence Parser 
  as described in this paper http://portal.acm.org/citation.cfm?id=512931.

  The parser has been extended to allow for statements in comparison to Pratt's
  original algorithm which only parsed languages which use expression-only grammar.

  The parsers is "impure" in the sense that is uses a ref-cell for storing the
  input in the T<_, _, _> record class, this is soley for performance reasons
  as it's to expensive to create a new record object for every consumed token.
  Certain functions also throw exceptions, which generally also is considered impure.

  The parser produces nice error message in this style:

  Error on line: 5 col: 9
  4: if(x == y) {
  5:   print'x equals y');
  ----------^
  Unexpected: #string "x equals y"

  More information:
  * http://en.wikipedia.org/wiki/Vaughan_Pratt (Original Inventor)
  * http://en.wikipedia.org/wiki/Pratt_parser (Alias name)
  * http://effbot.org/zone/simple-top-down-parsing.htm (Python implementation)
  * http://javascript.crockford.com/tdop/tdop.html (JavaScript implementation)
  *)

  type Pos = int * int

  type T<'a, 'b, 'c> when 'c : comparison = {
      Input : 'a list ref
      Lines : string option

      Type : 'a -> 'c
      Position : 'a -> Pos
      PrettyPrint : ('a -> string) option

      //Parser definitions and binding powers
      BindingPower : Map<'c, int>
      Null : Map<'c, 'a -> T<'a, 'b, 'c> -> 'b>
      Stmt : Map<'c, 'a -> T<'a, 'b, 'c> -> 'b>
      Left : Map<'c, 'a -> 'b -> T<'a, 'b, 'c> -> 'b>
  }
  
  type Pattern<'a, 'b, 'c> when 'c : comparison 
    = Sym of 'c
    | Get of (T<'a, 'b, 'c> -> 'b)

  //Errors
  type Exn (msg, pos) = 
    inherit System.Exception(msg)
    member x.Position = pos

  (*
    Creates a string error snippet 
    that points out the exact source position
    where the error occured, for example:
  
    4: if(x == y) {
    5:   print'x equals y');
    ----------^
  *)
  let errorSource pos source =
    
    let splitLines (text:string) = 
      let text = text.Replace("\r\n", "\n").Replace("\r", "\n")
      System.Text.RegularExpressions.Regex.Split(text, "\n")

    let lineNum (input:int) n = 
      (input.ToString()).PadLeft(n, '0')

    let stringRepeat n input =
      if System.String.IsNullOrEmpty input then input
      else
        let result = new System.Text.StringBuilder(input.Length * n)
        result.Insert(0, input, n).ToString()

    match source with
    | None -> ""
    | Some(source:string) -> 
      let source = source |> splitLines 
      let result = ref ""
      let line, column = pos

      if line <= source.Length && line > 1 then
        let nr = line.ToString()
        let nrl = nr.Length

        //previous line
        let pline = line - 1
        if pline >= 1 then 
          let num = lineNum pline nrl
          result := num+": "+source.[pline-1]+"\n"

        //current line
        let text = source.[line-1]
        if column <= text.Length then
          let arrow = "-" |> stringRepeat (nrl + column)
          result := !result+nr+": "+text+"\n"+arrow+"^\n"

      !result

  let exn msg = Exn(msg, (0, 0)) |> raise
  let exnLine pos msg = 
    let line = sprintf "Error on line: %i col: %i\n" (fst pos) (snd pos)
    Exn(line + msg, pos) |> raise

  let private unexpectedEnd () = "Unexpected end of input" |> exn
  let private unexpectedToken token parser =
    let type' =
      match parser.PrettyPrint with
      | None -> (token |> parser.Type).ToString()
      | Some f -> token |> f

    let pos = token |> parser.Position
    let source = parser.Lines |> errorSource pos 
    let expected = sprintf "Unexpected: %s" type'
    (source + expected) |> exnLine pos

  let inline private getBindingPower tok parser = 
    let pwr = parser.BindingPower.TryFind (parser.Type tok)
    match pwr with Some pwr -> pwr | _ -> 0

  let current parser =  
    match !parser.Input with
    | token::_ -> token
    | _ -> unexpectedEnd ()

  let currentTry parser = 
    match !parser.Input with
    | token::_ -> Some token
    | _ -> None

  let currentType parser = 
    parser |> current |> parser.Type

  let currentTypeTry parser =
    match parser |> currentTry with
    | Some token -> Some(token |> parser.Type)
    | _ -> None

  let skip parser =
    match !parser.Input with
    | _::input -> parser.Input := input
    | _ -> unexpectedEnd ()

  let skipIf type' parser =
    match !parser.Input with
    | token::xs when parser.Type token = type' -> 
      parser.Input := xs

    | token::_ -> 
      unexpectedToken token parser

    | _ -> unexpectedEnd ()

  let skipCurrent parser =
    let current = parser |> current
    parser |> skip
    current
   
  let exprPwr rbpw parser =
    let rec expr left =
      match parser |> currentTry with
      | Some token when rbpw < (parser |> getBindingPower token) -> 
        parser |> skip

        let type' = parser.Type token
        let led = 
          match parser.Left.TryFind type' with
          | None -> unexpectedToken token parser
          | Some led -> led

        led token left parser |> expr

      | _ -> left

    let tok = parser |> skipCurrent
    let type' = parser.Type tok
    let nud =
      match parser.Null.TryFind type' with
      | None -> unexpectedToken tok parser
      | Some nud -> nud

    nud tok parser |> expr 

  let expr parser = 
    parser |> exprPwr 0

  let exprSkip type' parser =
    let expr = parser |> expr
    parser |> skipIf type'
    expr

  let rec exprList parser =
    match !parser.Input with
    | [] -> []
    | _ -> (parser |> expr) :: (parser |> exprList)

  let stmt term parser =
    let token = parser |> current
    match parser.Stmt.TryFind (token |> parser.Type) with
    | Some stmt -> parser |> skip; stmt token parser
    | None -> parser |> exprSkip term

  let rec stmtList term parser =
    match !parser.Input with
    | [] -> []
    | _ -> (parser |> stmt term) :: (parser |> stmtList term)

  let match' pattern parser =
    let rec match' acc pattern parser =
      match pattern with
      | [] -> acc |> List.rev

      | Sym(symbol)::pattern -> 
        parser |> skipIf symbol
        parser |> match' acc pattern

      | Get(f)::pattern ->
        let acc = (f parser) :: acc
        parser |> match' acc pattern 

    parser |> match' [] pattern

  (*
    Convenience functions exposed for 
    easing parser definition and usage
  *)

  let create<'a, 'b, 'c when 'c : comparison> type' position prettyPrint = {
    Input = ref []
    Lines = None
    
    Type = type'
    Position = position
    PrettyPrint = prettyPrint
    
    BindingPower = Map.empty<'c, int>
    Null = Map.empty<'c, 'a -> T<'a, 'b, 'c> -> 'b>
    Stmt = Map.empty<'c, 'a -> T<'a, 'b, 'c> -> 'b>
    Left = Map.empty<'c, 'a -> 'b -> T<'a, 'b, 'c> -> 'b>
  }
  
  let matchError () = exn "Match pattern failed"
  let smd token funct parser = {parser with T.Stmt = parser.Stmt.Add(token, funct)}
  let nud token funct parser = {parser with T.Null = parser.Null.Add(token, funct)}
  let led token funct parser = {parser with T.Left = parser.Left.Add(token, funct)}
  let bpw token power parser = {parser with T.BindingPower = parser.BindingPower.Add(token, power)}
  
  (*Defines a left-associative infix operator*)
  let infix f typ pwr p =
    let infix tok left p = 
      f tok left (p |> exprPwr pwr)

    p |> bpw typ pwr |> led typ infix
    
  (*Defines a right-associative infix operator*)
  let infixr f typ pwr p =
    let lpwr = pwr - 1

    let infix tok left p = 
      f tok left (p |> exprPwr lpwr)

    p |> bpw typ pwr |> led typ infix

  (*Defines a prefix/unary operator*)
  let prefix f typ pwr p =
    let prefix tok parser = 
      f tok (parser |> exprPwr pwr)

    p |> nud typ prefix

  (*Defines a constant*)
  let constant symbol value p =
    p |> nud symbol (fun _ _ -> value)
    
  (*  
    Runs the parser and treats all 
    top level construct as expressions 
  *)
  let runExpr input source parser =
    {parser with 
      T.Input = ref input
      T.Lines = source
    } |> exprList
    
  (*  
    Runs the parser and treats all 
    top level construct as statements 
  *)
  let runStmt input source term parser =
    {parser with 
      T.Input = ref input
      T.Lines = source
    } |> stmtList term

(*
  Example parser for a very simple grammar
*)

//AST Types
type UnaryOp
  = Plus
  | Minus
  
type BinaryOp
  = Multiply
  | Add
  | Subtract
  | Divide
  | Assign
  | Equals

type Ast
  = Number of int
  | Identifier of string
  | String of string
  | Binary of BinaryOp * Ast * Ast
  | Unary of UnaryOp * Ast
  | Ternary of Ast * Ast * Ast // test * ifTrue * ifFalse
  | If of Ast * Ast * Ast option // test + ifTrue and possibly ifFalse (else branch)
  | Call of Ast * Ast list // target + arguments list
  | Block of Ast list // statements list
  | True
  | False

//Shorthand types for convenience
module P = Parser
type Token = string * string * (Parser.Pos)
type P = Parser.T<Token, Ast, string>

//Utility functions for extracting values out of Token
let type' ((t, _, _):Token) = t
let value ((_, v, _):Token) = v
let pos ((_, _, p):Token) = p
let value_num (t:Token) = t |> value |> int

//Utility functions for creating new tokens
let number value pos : Token = "#number", value, pos
let string value pos : Token = "#string", value, pos
let identifier name pos : Token = "#identifier", name, pos

let symbol type' pos : Token = type', "", pos
let add = symbol "+"
let sub = symbol "-"
let mul = symbol "*"
let div = symbol "/"
let assign = symbol "="
let equals = symbol "=="
let lparen = symbol "("
let rparen = symbol ")"
let lbrace = symbol "{"
let rbrace = symbol "}"
let comma = symbol ","
let qmark = symbol "?"
let colon = symbol ":"
let scolon = symbol ";"
let if' = symbol "if"
let true' = symbol "true"
let else' = symbol "else"

//Utility functions for converting tokens to binary and unary operators
let toBinaryOp tok =
  match type' tok with
  | "=" -> BinaryOp.Assign
  | "+" -> BinaryOp.Add
  | "-" -> BinaryOp.Subtract
  | "*" -> BinaryOp.Multiply
  | "/" -> BinaryOp.Divide
  | "==" -> BinaryOp.Equals
  | _ -> P.exn (sprintf "Couldn't convert %s-token to BinaryOp" (type' tok))

let toUnaryOp tok =
  match type' tok with
  | "+" -> UnaryOp.Plus
  | "-" -> UnaryOp.Minus
  | _ -> P.exn (sprintf "Couldn't convert %s-token to UnaryOp" (type' tok))

//Utility function for defining infix operators
let infix = 
  P.infix (fun token left right -> 
    Binary(token |> toBinaryOp, left, right))
  
//Utility function for defining prefix operators
let prefix =
  P.prefix (fun token ast ->
    Unary(token |> toUnaryOp, ast))

//Utility function for defining constants
let constant typ value p =
  p |> P.nud typ (fun _ _ -> value)

//Utility function for parsing a block 
let block p =
  let rec stmts p =
    match p |> P.currentTypeTry with
    | None -> []
    | Some "}" -> p |> P.skip; []
    | _ -> (p |> P.stmt ";") :: (stmts p)

  p |> P.skipIf "{"
  Block(p |> stmts)

//Pretty printing function for error messages
let prettyPrint (tok:Token) =
  match tok with
  | "#number", value, _ -> sprintf "#number %s" value
  | "#identifier", name, _ -> sprintf "#identifier %s" name
  | "#string", value, _ -> sprintf "#string \"%s\"" value
  | type', _, _ -> type'

//The parser definition
let example_parser =
  (P.create type' pos (Some prettyPrint))

  //Literals and identifiers
  |> P.nud "#number" (fun t _ -> t |> value |> int |> Number) 
  |> P.nud "#identifier" (fun t _ -> t |> value |> Identifier)
  |> P.nud "#string" (fun t _ -> t |> value |> String)

  //Constants
  |> constant "true" Ast.True
  |> constant "false" Ast.False
  
  //Infix Operators <expr> <op> <expr>
  |> infix "==" 40
  |> infix "+" 50
  |> infix "-" 50
  |> infix "*" 60
  |> infix "/" 60
  |> infix "=" 80

  //Prefix Operators <op> <expr>
  |> prefix "+" 70
  |> prefix "-" 70
  
  //Grouping expressions (<expr>)
  |> P.nud "(" (fun t p -> p |> P.exprSkip ")")

  //Ternary operator <expr> ? <expr> : <expr>
  |> P.bpw "?" 70
  |> P.led "?" (fun _ left p ->
      let ternary = [P.Get P.expr; P.Sym ":"; P.Get P.expr]
      match p |> P.match' ternary with
      | ifTrue::ifFalse::_ -> Ternary(left, ifTrue, ifFalse)
      | _ -> P.matchError()
    )

  //If/Else statement if(<condition>) { <exprs } [else { <exprs> }]
  |> P.smd "if" (fun _ p ->
      let if' = [P.Sym "("; P.Get P.expr; P.Sym ")"; P.Get block]
      let else' = [P.Sym "else"; P.Get block]

      match p |> P.match' if' with
      | test::ifTrue::_ -> 
        match p |> P.match' else' with
        | ifFalse::_ -> If(test, ifTrue, Some(ifFalse))
        | _ -> If(test, ifTrue, None)
      | _ -> P.matchError()
    )

  //Function call
  |> P.bpw "(" 80
  |> P.led "(" (fun _ func p ->
      let rec args (p:P) =
        match p |> P.currentType with
        | ")" -> p |> P.skip; []
        | "," -> p |> P.skip; args p
        | _ -> (p |> P.expr) :: args p

      Call(func, args p)
    )
    
//Code to parse
(*
1: x = 5;
2: y = 5;
3: 
4: if(x == y) {
5:   print("x equals y");
6: } else {
7:   print("x doesn't equal y");
8: }
*)

let code = @"x = 5;
y = 5;

if(x == y) {
  print('x equals y');
} else {
  print('x doesn't equal y');
}"

//The code in tokens, manually entered
//since we don't have a lexer to produce
//the tokens for us
let tokens = 
  [
    //x = 5;
    identifier "x" (1, 1)
    assign (1, 3)
    number "5" (1, 5)
    scolon (1, 6)

    //y = 5;
    identifier "y" (2, 1)
    assign (2, 3)
    number "5" (2, 5)
    scolon (2, 6)

    //if(x == y) {
    if' (4, 1)
    lparen (4, 3)
    identifier "x" (4, 4)
    equals (4, 6)
    identifier "y" (4, 9)
    rparen (4, 10)
    lbrace (4, 12)

    //print("x equals y");
    identifier "print" (5, 3)
    lparen (5, 8)
    string "x equals y" (5, 9)
    rparen (5, 21)
    scolon (5, 22)

    //} else {
    rbrace (6, 1)
    else' (6, 3)
    lbrace (6, 7)

    //print("x doesn't equal y");
    identifier "print" (7, 3)
    lparen (7, 7)
    string "x doesn't equal y" (7, 9)
    rparen (7, 27)
    scolon (7, 28)

    //}
    rbrace (8, 1)
  ]

let ast = example_parser |> P.runStmt tokens (Some code) ";"
