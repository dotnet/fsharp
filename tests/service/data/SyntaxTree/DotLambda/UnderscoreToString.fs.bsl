ImplFile
  (ParsedImplFileInput
     ("/root/DotLambda/UnderscoreToString.fs", false,
      QualifiedNameOfFile UnderscoreToString, [], [],
      [SynModuleOrNamespace
         ([UnderscoreToString], false, AnonModule,
          [Expr
             (DotLambda
                (App
                   (Atomic, false, Ident ToString, Const (Unit, (1,10--1,12)),
                    (1,2--1,12)), (1,0--1,12), { UnderscoreRange = (1,0--1,1)
                                                 DotRange = (1,1--1,2) }),
              (1,0--1,12))], PreXmlDocEmpty, [], None, (1,0--1,12),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,0)-(1,1) parse error  _. shorthand syntax for lambda functions can only be used with atomic expressions. That means expressions with no whitespace unless enclosed in parentheses.
