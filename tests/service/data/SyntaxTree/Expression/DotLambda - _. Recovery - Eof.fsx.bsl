ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda - _. Recovery - Eof.fsx", true,
      QualifiedNameOfFile DotLambda - _. Recovery - Eof$fsx, [], [],
      [SynModuleOrNamespace
         ([DotLambda - _;  Recovery - Eof], false, AnonModule,
          [Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, true,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_PipeRight], [], [Some (OriginalNotation "|>")]),
                       None, (1,2--1,4)), Const (Int32 1, (1,0--1,1)),
                    (1,0--1,4)),
                 DotLambda
                   (ArbitraryAfterError ("dotLambda1", (1,7--1,7)), (1,5--1,7),
                    { UnderscoreRange = (1,5--1,6)
                      DotRange = (1,6--1,7) }), (1,0--1,7)), (1,0--1,7))],
          PreXmlDocEmpty, [], None, (1,0--1,7), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,6)-(1,7) parse error Unexpected end of input in expression
