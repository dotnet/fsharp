// #Regression #NoMT #FSI 
// Microsoft.Analysis.Server overrides ToString() to return isNull
// instead of a string. This would make FSI fail in pretty-printing 
// when displaying results. 
//<Expects status="success">val n: NullToString = </Expects>

type NullToString() = 
  override __.ToString() = null;;

let n = NullToString();;
#q;;

