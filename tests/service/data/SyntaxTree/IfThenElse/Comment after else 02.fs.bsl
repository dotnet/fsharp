ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/Comment after else 02.fs", false,
      QualifiedNameOfFile Comment after else 02, [], [],
      [SynModuleOrNamespace
         ([Comment after else 02], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a,
                 ArbitraryAfterError ("typedSequentialExprBlock1", (1,9--1,9)),
                 None, Yes (1,0--1,9), false, (1,0--1,9),
                 { IfKeyword = (1,0--1,2)
                   IsElif = false
                   ThenKeyword = (1,5--1,9)
                   ElseKeyword = None
                   IfToThenRange = (1,0--1,9) }), (1,0--1,9));
           Expr (Ident b, (2,0--2,1))], PreXmlDocEmpty, [], None, (1,0--3,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [BlockComment (3,5--3,33)] }, set []))

(2,0)-(2,1) parse error Possible incorrect indentation: this token is offside of context started at position (1:1). Try indenting this token further or using standard formatting conventions.
(2,0)-(2,1) parse error Expecting expression
(3,0)-(3,36) parse error Unexpected keyword 'elif' in implementation file
(4,0)-(4,1) parse error Possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this token further or using standard formatting conventions.
(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'Comment after else 02' based on the file name 'Comment after else 02.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
