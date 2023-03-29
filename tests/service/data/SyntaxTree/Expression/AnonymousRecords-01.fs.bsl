ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonymousRecords-01.fs", false,
      QualifiedNameOfFile AnonymousRecords-01, [], [],
      [SynModuleOrNamespace
         ([AnonymousRecords-01], false, AnonModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([X], [], [None]), Some (1,5--1,6),
                   Const (Int32 1, (1,7--1,8)))], (1,0--1,11),
                 { OpeningBraceRange = (1,0--1,2) }), (1,0--1,11));
           Expr
             (AnonRecd
                (true, None,
                 [(SynLongIdent ([Y], [], [None]), Some (2,12--2,13),
                   Const (Int32 2, (2,14--2,15)))], (2,0--2,18),
                 { OpeningBraceRange = (2,7--2,9) }), (2,0--2,18));
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
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(5,8)-(5,10) parse error Unexpected symbol '|}' in definition
(6,15)-(6,17) parse error Unexpected symbol '|}' in expression
(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'AnonymousRecords-01' based on the file name 'AnonymousRecords-01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
