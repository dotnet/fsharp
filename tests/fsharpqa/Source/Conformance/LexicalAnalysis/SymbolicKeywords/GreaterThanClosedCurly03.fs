// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:1077
// closing brace following generic type bracket is syntax error without whitespace (lexed into symbolic token).
//<Expects status="success"></Expects>

#light

type a3 = {x:list<list<int>>}
