ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_for.fs", false, QualifiedNameOfFile InExp_for,
      [],
      [SynModuleOrNamespace
         ([InExp_for], false, AnonModule,
          [Expr
             (ForEach
                (Yes (1,0--1,3), Yes (1,6--1,8), SeqExprOnly false, true,
                 Wild (1,4--1,5),
                 Open
                   (ModuleOrNamespace
                      (SynLongIdent
                         ([System; Linq], [(1,20--1,21)], [None; None]),
                       (1,14--1,25)), (1,9--1,25), (1,9--1,50),
                    App
                      (Atomic, false,
                       LongIdent
                         (false,
                          SynLongIdent
                            ([Enumerable; Range], [(1,37--1,38)], [None; None]),
                          None, (1,27--1,43)),
                       Paren
                         (Tuple
                            (false,
                             [Const (Int32 0, (1,44--1,45));
                              Const (Int32 10, (1,47--1,49))], [(1,45--1,46)],
                             (1,44--1,49)), (1,43--1,44), Some (1,49--1,50),
                          (1,43--1,50)), (1,27--1,50))),
                 Open
                   (Type
                      (LongIdent
                         (SynLongIdent
                            ([System; Console], [(2,20--2,21)], [None; None])),
                       (2,14--2,28)), (2,4--2,28), (2,4--3,29),
                    App
                      (NonAtomic, false, Ident WriteLine,
                       Const
                         (String ("Hello, World!", Regular, (3,14--3,29)),
                          (3,14--3,29)), (3,4--3,29))), (1,0--3,29)),
              (1,0--3,29))], PreXmlDocEmpty, [], None, (1,0--4,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
