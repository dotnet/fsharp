ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_if_elif.fs", false,
      QualifiedNameOfFile InExp_if_elif, [],
      [SynModuleOrNamespace
         ([InExp_if_elif], false, AnonModule,
          [Expr
             (IfThenElse
                (Paren
                   (Open
                      (Type
                         (LongIdent
                            (SynLongIdent
                               ([System; Int32], [(1,20--1,21)], [None; None])),
                          (1,14--1,26)), (1,4--1,26), (1,4--1,48),
                       App
                         (NonAtomic, false,
                          App
                            (NonAtomic, true,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([op_Inequality], [],
                                   [Some (OriginalNotation "<>")]), None,
                                (1,37--1,39)), Ident MaxValue, (1,28--1,39)),
                          Ident MinValue, (1,28--1,48))), (1,3--1,4),
                    Some (1,48--1,49), (1,3--1,49)),
                 Open
                   (Type
                      (LongIdent
                         (SynLongIdent
                            ([System; Console], [(2,20--2,21)], [None; None])),
                       (2,14--2,28)), (2,4--2,28), (2,4--3,49),
                    App
                      (NonAtomic, false, Ident WriteLine,
                       Const
                         (String
                            ("MaxValue is not equal to MinValue", Regular,
                             (3,14--3,49)), (3,14--3,49)), (3,4--3,49))),
                 Some
                   (IfThenElse
                      (Paren
                         (Open
                            (Type
                               (LongIdent
                                  (SynLongIdent
                                     ([System; Int32], [(4,22--4,23)],
                                      [None; None])), (4,16--4,28)), (4,6--4,28),
                             (4,6--4,42),
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_LessThan], [],
                                         [Some (OriginalNotation "<")]), None,
                                      (4,39--4,40)), Ident MaxValue,
                                   (4,30--4,40)), Const (Int32 0, (4,41--4,42)),
                                (4,30--4,42))), (4,5--4,6), Some (4,42--4,43),
                          (4,5--4,43)),
                       Open
                         (Type
                            (LongIdent
                               (SynLongIdent
                                  ([System; Console], [(5,20--5,21)],
                                   [None; None])), (5,14--5,28)), (5,4--5,28),
                          (5,4--6,36),
                          App
                            (NonAtomic, false, Ident WriteLine,
                             Const
                               (String
                                  ("MaxValue is negative", Regular, (6,14--6,36)),
                                (6,14--6,36)), (6,4--6,36))),
                       Some
                         (Open
                            (Type
                               (LongIdent
                                  (SynLongIdent
                                     ([System; Console], [(8,20--8,21)],
                                      [None; None])), (8,14--8,28)), (8,4--8,28),
                             (8,4--9,36),
                             App
                               (NonAtomic, false, Ident WriteLine,
                                Const
                                  (String
                                     ("MaxValue is positive", Regular,
                                      (9,14--9,36)), (9,14--9,36)), (9,4--9,36)))),
                       Yes (4,0--4,48), false, (4,0--9,36),
                       { IfKeyword = (4,0--4,4)
                         IsElif = true
                         ThenKeyword = (4,44--4,48)
                         ElseKeyword = Some (7,0--7,4)
                         IfToThenRange = (4,0--4,48) })), Yes (1,0--1,54), false,
                 (1,0--9,36), { IfKeyword = (1,0--1,2)
                                IsElif = false
                                ThenKeyword = (1,50--1,54)
                                ElseKeyword = None
                                IfToThenRange = (1,0--1,54) }), (1,0--9,36))],
          PreXmlDocEmpty, [], None, (1,0--10,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
