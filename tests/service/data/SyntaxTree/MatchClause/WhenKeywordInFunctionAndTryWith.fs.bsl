ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/WhenKeywordInFunctionAndTryWith.fs", false,
      QualifiedNameOfFile WhenKeywordInFunctionAndTryWith, [],
      [SynModuleOrNamespace
         ([WhenKeywordInFunctionAndTryWith], false, AnonModule,
          [Expr
             (MatchLambda
                (false, (1,0--1,8),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([A], [], [None]), None, None, Pats [],
                        None, (2,2--2,3)), Some (Ident a), Ident b, (2,2--2,15),
                     Yes, { ArrowRange = Some (2,11--2,13)
                            BarRange = Some (2,0--2,1)
                            WhenKeyword = Some (2,4--2,8) });
                  SynMatchClause
                    (LongIdent
                       (SynLongIdent ([B], [], [None]), None, None, Pats [],
                        None, (3,2--3,3)), None, Ident c, (3,2--3,8), Yes,
                     { ArrowRange = Some (3,4--3,6)
                       BarRange = Some (3,0--3,1)
                       WhenKeyword = None })], NoneAtInvisible, (1,0--3,8)),
              (1,0--3,8));
           Expr
             (TryWith
                (Const (Unit, (6,4--6,6)),
                 [SynMatchClause
                    (Named (SynIdent (ex, None), false, None, (8,2--8,4)),
                     Some
                       (App (NonAtomic, false, Ident f, Ident ex, (8,10--8,14))),
                     Const (Unit, (8,18--8,20)), (8,2--8,20), Yes,
                     { ArrowRange = Some (8,15--8,17)
                       BarRange = Some (8,0--8,1)
                       WhenKeyword = Some (8,5--8,9) })], (5,0--8,20),
                 Yes (5,0--5,3), Yes (7,0--7,4),
                 { TryKeyword = (5,0--5,3)
                   TryToWithRange = (5,0--7,4)
                   WithKeyword = (7,0--7,4)
                   WithToEndRange = (7,0--8,20) }), (5,0--8,20))],
          PreXmlDocEmpty, [], None, (1,0--9,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
