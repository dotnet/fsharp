ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonymousRecords-01.fs", false,
      QualifiedNameOfFile AnonymousRecords-01, [],
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
           Expr
             (AnonRecd
                (false, Some (Null (5,3--5,7), ((5,7--5,7), None)), [],
                 (5,0--5,10), { OpeningBraceRange = (5,0--5,2) }), (5,0--5,10));
           Expr
             (AnonRecd
                (true, Some (Null (6,10--6,14), ((6,14--6,14), None)), [],
                 (6,0--6,17), { OpeningBraceRange = (6,7--6,9) }), (6,0--6,17))],
          PreXmlDocEmpty, [], None, (1,0--6,17), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))

(5,3)-(5,7) parse error Field bindings must have the form 'id = expr;'
(6,10)-(6,14) parse error Field bindings must have the form 'id = expr;'
(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'AnonymousRecords-01' based on the file name 'AnonymousRecords-01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
