// #Misc 
(***
This testcase is designed to exercise uncommonly used grammar productions in the F#
language.
***)

#light

/// RULE: 255     explicitValTyparDeclsCore -> 
let f<'a> (x:'a) = x
assert (f "foo" = "foo")

let f1< > (x:int) = x
assert ((f1 42) = 42)

//376 headBindingPattern -> headBindingPattern COLON_COLON headBindingPattern 
// Note: no parentheses on pattern

let (h::t) = [3]
assert (h = 3)
assert (t = [])

//378 headBindingPattern -> conjPatternElements 
//382 conjPatternElements -> conjPatternElements AMP headBindingPattern 

// Pattern matching on left hand side no parenthesis
let a1 & a2 = 3
let (b1,b2) as tup = 3,3
let c1 | c1 | c1 = 3
let d1 :: d2 = [3]

// Note, this is OK:
let p1, p2 = 3,4
let (|C|_|) x inp = inp

// 484      declExpr -> declExpr DOLLAR declExpr 
// 781      operatorName -> DOLLAR 

let ($) a b = a + b
let v = 3 $ 4

// 496      declExpr -> declExpr QMARK_QMARK declExpr 
//let nullThingy = "foo" ?? null
//assert (nullThingy = "foo")

//let nullThingy2 = null ?? "bar"
//assert (nullThingy2 = "bar")

//717 topAppType -> attributes appType 
//715 topAppType -> attributes appType COLON appType 
//716 topAppType -> attributes QMARK ident COLON appType 
//719 topAppType -> QMARK ident COLON appType 
type Doc() = 
    inherit System.Attribute()
    
type IFoo = 
    interface
        abstract Method2 : [<Doc>] p1:int * [<Doc>] p2:unit -> unit
        abstract Method3 : [<Doc>] ?p1:int -> unit
        abstract Method4 : ?p1:int -> unit
    end

//735 appType -> typar COLON_GREATER typ 
let constraintTest1 (x : 'a :> System.IComparable) = x

//736 appType -> UNDERSCORE COLON_GREATER typ 
let constraintTest2 (x : _ :> System.IComparable) = x

//740 arrayTypeSuffix -> LBRACK COMMA COMMA COMMA RBRACK 
let constraintTest3 (arr : int[,,,]) = arr

//789   operatorName -> AMP 
let (&) v1 v2 = v1 + v2
//790   operatorName -> AMP_AMP 
let (&&) v1 v2 = v1 + v2
//801   operatorName -> DOT LPAREN RPAREN 
let (.()) v1 v2 = v1 + v2

////802   operatorName -> DOT LPAREN COMMA RPAREN 
//let (.(,)) v1 v2 = v1 + v2
//803   operatorName -> DOT LPAREN COMMA COMMA RPAREN 
//let (.(,,)) v1 v2 = v1 + v2

//804   operatorName -> DOT LPAREN RPAREN LARROW 
let (.()<-) v1 v2 = v1 + v2
////805   operatorName -> DOT LPAREN COMMA RPAREN LARROW 
//let (.(,)<-) v1 v2 = v1 + v2
////806   operatorName -> DOT LPAREN COMMA COMMA RPAREN LARROW 
//let (.(,,)<-) v1 v2 = v1 + v2
//807   operatorName -> DOT IDENT 
// ????
//808   operatorName -> SPLICE_SYMBOL
//let (�) v1 v2 = v1 + v2       // � lead to syntax error?
//let (��) v1 v2 = v1 + v2
//810   operatorName -> DOT_DOT 
let (..) v1 v2 = v1 + v2
//811   operatorName -> DOT_DOT DOT_DOT 
let (.. ..) v1 v2 = v1 + v2
//605   monadicExprNonEmptyNonInitialBlock -> monadicExprNonEmptyNonInitial 
// ????

exit 0
