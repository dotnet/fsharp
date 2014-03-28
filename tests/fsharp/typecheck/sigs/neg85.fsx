module M

module MultipleClassConstraints = 
    open System
    open System.Linq.Expressions
 
    let convertExpressionTreeToFSharp (e : #Expression) =
        match e.NodeType with
        | ExpressionType.Constant -> 
            let c = e :> ConstantExpression
            ()
        | ExpressionType.Assign ->
            let a = e :> BinaryExpression
            ()
        | _ -> 
            ()


module OneClassConstraint = 
    open System
    open System.Linq.Expressions

    let convertExpressionTreeToFSharp (e : #Expression) =
        match e.NodeType with
        | ExpressionType.Constant -> 
            let c = e :> ConstantExpression
            ()
        | ExpressionType.Assign ->
            let a = e :> IDisposable
            ()
        | _ -> 
            ()

        //convertExpressionTreeToFSharp (Expression.Constant(1)) // error
 
