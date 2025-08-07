ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_match.fs", false,
      QualifiedNameOfFile InExp_match, [],
      [SynModuleOrNamespace
         ([InExp_match], false, AnonModule,
          [Expr
             (Match
                (Yes (1,0--1,17),
                 App
                   (NonAtomic, false, Ident Some, Const (Int32 1, (1,11--1,12)),
                    (1,6--1,12)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Some], [], [None]), None, None,
                        Pats [Const (Int32 1, (2,7--2,8))], None, (2,2--2,8)),
                     Some
                       (Open
                          (ModuleOrNamespace
                             (SynLongIdent ([System], [], [None]), (2,19--2,25)),
                           (2,14--2,25), (2,14--2,45),
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, true,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([op_LessThan], [],
                                       [Some (OriginalNotation "<")]), None,
                                    (2,42--2,43)),
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([Int32; MinValue], [(2,32--2,33)],
                                       [None; None]), None, (2,27--2,41)),
                                 (2,27--2,43)), Const (Int32 0, (2,44--2,45)),
                              (2,27--2,45)))),
                     Open
                       (Type
                          (LongIdent
                             (SynLongIdent
                                ([System; Console], [(3,20--3,21)], [None; None])),
                           (3,14--3,28)), (3,4--3,28), (3,4--4,20),
                        App
                          (NonAtomic, false, Ident WriteLine,
                           Const
                             (String ("Is 1", Regular, (4,14--4,20)),
                              (4,14--4,20)), (4,4--4,20))), (2,2--4,20), Yes,
                     { ArrowRange = Some (2,46--2,48)
                       BarRange = Some (2,0--2,1) });
                  SynMatchClause
                    (Wild (5,2--5,3), None, Const (Unit, (5,7--5,9)), (5,2--5,9),
                     Yes, { ArrowRange = Some (5,4--5,6)
                            BarRange = Some (5,0--5,1) })], (1,0--5,9),
                 { MatchKeyword = (1,0--1,5)
                   WithKeyword = (1,13--1,17) }), (1,0--5,9))], PreXmlDocEmpty,
          [], None, (1,0--6,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
