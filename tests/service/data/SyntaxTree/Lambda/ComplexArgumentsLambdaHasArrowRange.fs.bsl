ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs", false,
      QualifiedNameOfFile ComplexArgumentsLambdaHasArrowRange, [], [],
      [SynModuleOrNamespace
         ([ComplexArgumentsLambdaHasArrowRange], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id (x, None, false, false, false, (2,5--2,6));
                     Id (_arg3, None, true, false, false, (2,8--2,9))],
                    [(2,6--2,7)], (2,4--2,10)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id (_arg2, None, true, false, false, (3,5--3,17))], [],
                       (3,4--3,18)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id (_arg1, None, true, false, false, (4,5--4,19))],
                          [], (4,4--4,20)),
                       Match
                         (NoneAtInvisible, Ident _arg2,
                          [SynMatchClause
                             (Record
                                ([(([], Y), (3,9--3,10),
                                   ListCons
                                     (Named
                                        (SynIdent (h, None), false, None,
                                         (3,11--3,12)), Wild (3,14--3,15),
                                      (3,11--3,15),
                                      { ColonColonRange = (3,12--3,14) }))],
                                 (3,5--3,17)), None,
                              Match
                                (NoneAtInvisible, Ident _arg1,
                                 [SynMatchClause
                                    (LongIdent
                                       (SynLongIdent ([SomePattern], [], [None]),
                                        None, None,
                                        Pats
                                          [Paren
                                             (Named
                                                (SynIdent (z, None), false, None,
                                                 (4,17--4,18)), (4,16--4,19))],
                                        None, (4,5--4,19)), None,
                                     App
                                       (NonAtomic, false,
                                        App
                                          (NonAtomic, true,
                                           LongIdent
                                             (false,
                                              SynLongIdent
                                                ([op_Addition], [],
                                                 [Some (OriginalNotation "+")]),
                                              None, (6,10--6,11)),
                                           App
                                             (NonAtomic, false,
                                              App
                                                (NonAtomic, true,
                                                 LongIdent
                                                   (false,
                                                    SynLongIdent
                                                      ([op_Multiply], [],
                                                       [Some
                                                          (OriginalNotation "*")]),
                                                    None, (6,6--6,7)), Ident x,
                                                 (6,4--6,7)), Ident y,
                                              (6,4--6,9)), (6,4--6,11)), Ident z,
                                        (6,4--6,13)), (4,5--4,19), No,
                                     { ArrowRange = None
                                       BarRange = None })], (4,5--6,13),
                                 { MatchKeyword = (4,5--6,13)
                                   WithKeyword = (4,5--6,13) }), (3,5--3,17), No,
                              { ArrowRange = None
                                BarRange = None })], (3,5--6,13),
                          { MatchKeyword = (3,5--6,13)
                            WithKeyword = (3,5--6,13) }), None, (2,0--6,13),
                       { ArrowRange = Some (5,4--5,6) }), None, (2,0--6,13),
                    { ArrowRange = Some (5,4--5,6) }),
                 Some
                   ([Paren
                       (Tuple
                          (false,
                           [Named (SynIdent (x, None), false, None, (2,5--2,6));
                            Wild (2,8--2,9)], [(2,6--2,7)], (2,5--2,9)),
                        (2,4--2,10));
                     Paren
                       (Record
                          ([(([], Y), (3,9--3,10),
                             ListCons
                               (Named
                                  (SynIdent (h, None), false, None, (3,11--3,12)),
                                Wild (3,14--3,15), (3,11--3,15),
                                { ColonColonRange = (3,12--3,14) }))],
                           (3,5--3,17)), (3,4--3,18));
                     Paren
                       (LongIdent
                          (SynLongIdent ([SomePattern], [], [None]), None, None,
                           Pats
                             [Paren
                                (Named
                                   (SynIdent (z, None), false, None,
                                    (4,17--4,18)), (4,16--4,19))], None,
                           (4,5--4,19)), (4,4--4,20))],
                    Match
                      (NoneAtInvisible, Ident _arg2,
                       [SynMatchClause
                          (Record
                             ([(([], Y), (3,9--3,10),
                                ListCons
                                  (Named
                                     (SynIdent (h, None), false, None,
                                      (3,11--3,12)), Wild (3,14--3,15),
                                   (3,11--3,15),
                                   { ColonColonRange = (3,12--3,14) }))],
                              (3,5--3,17)), None,
                           Match
                             (NoneAtInvisible, Ident _arg1,
                              [SynMatchClause
                                 (LongIdent
                                    (SynLongIdent ([SomePattern], [], [None]),
                                     None, None,
                                     Pats
                                       [Paren
                                          (Named
                                             (SynIdent (z, None), false, None,
                                              (4,17--4,18)), (4,16--4,19))],
                                     None, (4,5--4,19)), None,
                                  App
                                    (NonAtomic, false,
                                     App
                                       (NonAtomic, true,
                                        LongIdent
                                          (false,
                                           SynLongIdent
                                             ([op_Addition], [],
                                              [Some (OriginalNotation "+")]),
                                           None, (6,10--6,11)),
                                        App
                                          (NonAtomic, false,
                                           App
                                             (NonAtomic, true,
                                              LongIdent
                                                (false,
                                                 SynLongIdent
                                                   ([op_Multiply], [],
                                                    [Some (OriginalNotation "*")]),
                                                 None, (6,6--6,7)), Ident x,
                                              (6,4--6,7)), Ident y, (6,4--6,9)),
                                        (6,4--6,11)), Ident z, (6,4--6,13)),
                                  (4,5--4,19), No, { ArrowRange = None
                                                     BarRange = None })],
                              (4,5--6,13), { MatchKeyword = (4,5--6,13)
                                             WithKeyword = (4,5--6,13) }),
                           (3,5--3,17), No, { ArrowRange = None
                                              BarRange = None })], (3,5--6,13),
                       { MatchKeyword = (3,5--6,13)
                         WithKeyword = (3,5--6,13) })), (2,0--6,13),
                 { ArrowRange = Some (5,4--5,6) }), (2,0--6,13))],
          PreXmlDocEmpty, [], None, (2,0--7,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
