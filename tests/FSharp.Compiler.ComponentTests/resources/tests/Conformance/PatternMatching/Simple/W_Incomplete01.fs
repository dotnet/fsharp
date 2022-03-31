// #Regression #Conformance #PatternMatching 
#light

// Regression testcase for FSharp 1.0:2070
// Warning on incomplete match

//<Expects id="FS0025" span="(92,13-92,14)" status="warning">Incomplete pattern matches on this expression. For example, the value 'Result \(_\)' may indicate a case not covered by the pattern\(s\)</Expects>

module M =
  
  type Id = string
  
  type Field = {
    Name:string;
    mutable Type:Type;
    mutable Parent:TypeDecl;
    IsSpec:bool;    
  }
  
  and TypeDecl =    
    | Struct of Id * list<Field>
    | Union of Id * list<Field>
    | MathType of Id
  
  and Type =
    | Void
    | Integer
    | Bool
    | Ptr of Type
    | Ref of TypeDecl
    | Array of Type * int
    | TypeIdT
    | Tptr
    | Map of Type * Type

  
  type VarKind =    
    | Parameter
    | Local
    | Global
    | QuantBound
    
  type Variable = { 
    Name:Id; 
    Type:Type; 
    Kind:VarKind; 
  }
  
  type Token = {
    File: string;
    Line:int;
    Column:int;
    Remarks:string;
  }
  
  type ExprCommon = {
    Token:Token;
    Type:Type;
  }
  
  type Function = {
    Token:Token;
    IsSpec:bool;
    RetType:Type;
    Name:Id;
    Parameters:list<Variable>;
    Requires:list<Expr>;
    Ensures:list<Expr>;
    Invariants:list<Expr>;
    Writes:list<Expr>;
    Reads:list<Expr>;
  } 
  
  and Expr =
    | Ref of ExprCommon * Variable    
    | Prim of ExprCommon * string * list<Expr>
    | Call of ExprCommon * Function * list<Expr>
    | IntLiteral of ExprCommon * string
    | BoolLiteral of ExprCommon * bool
    | Deref of ExprCommon * Expr
    | Addr of ExprCommon * Variable
    | Dot of ExprCommon * Expr * Field   // computes address of the field
    | Index of ExprCommon * Expr * Expr  // computes address of an array element
    | Cast of ExprCommon * Expr          // take the type from ExprCommon
    | Result of ExprCommon
    | VolatileRead of ExprCommon * Expr * Expr
    | Old of ExprCommon * Expr
    | Macro of ExprCommon * string * list<Expr>
  
  type Expr with 
    member x.Common =
      match x with
        | Ref (e, _)
        | Prim (e, _, _)
        | Call (e, _, _)
        | IntLiteral (e, _)
        | BoolLiteral (e, _) 
        | Deref (e, _)
        | Addr (e, _)
        | Dot (e, _, _)
        | Index (e, _, _)
        | Cast (e, _)
        //| Result (e)
        | VolatileRead (e, _, _)
        | Old (e, _)
        | Macro (e, _, _)
          -> e

printfn "Finished"
