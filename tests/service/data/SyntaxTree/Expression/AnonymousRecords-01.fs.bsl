ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonymousRecords-01.fs", false,
      QualifiedNameOfFile AnonymousRecords-01, [], [],
      [SynModuleOrNamespace
         ([AnonymousRecords-01], false, AnonModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(X, Some (1,5--1,6), Const (Int32 1, (1,7--1,8)))],
                 (1,0--1,11), { OpeningBraceRange = (1,0--1,2) }), (1,0--1,11));
           Expr
             (AnonRecd
                (true, None,
                 [(Y, Some (2,12--2,13), Const (Int32 2, (2,14--2,15)))],
                 (2,0--2,18), { OpeningBraceRange = (2,7--2,9) }), (2,0--2,18));
           Expr
             (AnonRecd
                (false, None, [], (3,0--3,5), { OpeningBraceRange = (3,0--3,2) }),
              (3,0--3,5));
           Expr
             (AnonRecd
                (true, None, [], (4,0--4,12), { OpeningBraceRange = (4,7--4,9) }),
              (4,0--4,12));
           Expr (ArbitraryAfterError ("braceBarExpr", (5,0--5,10)), (5,0--5,10));
           Expr (ArbitraryAfterError ("braceBarExpr", (6,0--6,17)), (6,0--6,17))],
          PreXmlDocEmpty, [], None, (1,0--6,17), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
