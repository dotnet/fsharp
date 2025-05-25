ImplFile
  (ParsedImplFileInput
     ("/root/Expression/ExprInterpolatedString 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (Atomic, false, Ident C,
                 Paren
                   (App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Equality], [], [Some (OriginalNotation "=")]),
                             None, (3,6--3,7)), Ident Name, (3,2--3,7)),
                       Const (String ("123", Regular, (3,7--3,12)), (3,7--3,12)),
                       (3,2--3,12)), (3,1--3,2), Some (3,12--3,13), (3,1--3,13)),
                 (3,0--3,13)), (3,0--3,13));
           Expr
             (App
                (Atomic, false, Ident C,
                 Paren
                   (App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_EqualsDollar], [],
                                [Some (OriginalNotation "=$")]), None,
                             (4,6--4,8)), Ident Name, (4,2--4,8)),
                       Const (String ("123", Regular, (4,8--4,13)), (4,8--4,13)),
                       (4,2--4,13)), (4,1--4,2), Some (4,13--4,14), (4,1--4,14)),
                 (4,0--4,14)), (4,0--4,14));
           Expr
             (App
                (Atomic, false, Ident C,
                 Paren
                   (App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Equality], [], [Some (OriginalNotation "=")]),
                             None, (5,6--5,7)), Ident Name, (5,2--5,7)),
                       InterpolatedString
                         ([String ("123", (5,8--5,14))], Regular, (5,8--5,14)),
                       (5,2--5,14)), (5,1--5,2), Some (5,14--5,15), (5,1--5,15)),
                 (5,0--5,15)), (5,0--5,15))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,15), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
