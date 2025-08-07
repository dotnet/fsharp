ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_try_with.fs", false,
      QualifiedNameOfFile InExp_try_with, [],
      [SynModuleOrNamespace
         ([InExp_try_with], false, AnonModule,
          [Expr
             (While
                (Yes (1,0--3,18),
                 Paren
                   (Open
                      (Type
                         (LongIdent
                            (SynLongIdent
                               ([System; Int32], [(2,21--2,22)], [None; None])),
                          (2,15--2,27)), (2,5--2,27), (2,5--3,17),
                       App
                         (NonAtomic, false,
                          App
                            (NonAtomic, true,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([op_LessThan], [],
                                   [Some (OriginalNotation "<")]), None,
                                (3,14--3,15)), Ident MaxValue, (3,5--3,15)),
                          Const (Int32 0, (3,16--3,17)), (3,5--3,17))),
                    (2,4--2,5), Some (3,17--3,18), (2,4--3,18)),
                 Open
                   (Type
                      (LongIdent
                         (SynLongIdent
                            ([System; Console], [(4,20--4,21)], [None; None])),
                       (4,14--4,28)), (4,4--4,28), (4,4--5,36),
                    App
                      (NonAtomic, false, Ident WriteLine,
                       Const
                         (String ("MaxValue is negative", Regular, (5,14--5,36)),
                          (5,14--5,36)), (5,4--5,36))), (1,0--5,36)),
              (1,0--5,36))], PreXmlDocEmpty, [], None, (1,0--6,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
