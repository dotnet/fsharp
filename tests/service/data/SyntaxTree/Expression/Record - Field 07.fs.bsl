ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 07.fs", false,
      QualifiedNameOfFile Record - Field 07, [], [],
      [SynModuleOrNamespace
         ([Record - Field 07], false, AnonModule,
          [Expr
             (ComputationExpr
                (false,
                 Sequential
                   (SuppressNeither, true,
                    DiscardAfterMissingQualificationAfterDot
                      (Ident A, (1,3--1,4), (1,2--1,4)),
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Equality], [], [Some (OriginalNotation "=")]),
                             None, (2,4--2,5)), Ident B, (2,2--2,5)),
                       Const (Int32 1, (2,6--2,7)), (2,2--2,7)), (1,2--2,7)),
                 (1,0--2,9)), (1,0--2,9))], PreXmlDocEmpty, [], None, (1,0--2,9),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
