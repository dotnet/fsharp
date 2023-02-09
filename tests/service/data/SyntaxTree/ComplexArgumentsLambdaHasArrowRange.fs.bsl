ImplFile
  (ParsedImplFileInput
     ("/root/ComplexArgumentsLambdaHasArrowRange.fs", false,
      QualifiedNameOfFile ComplexArgumentsLambdaHasArrowRange, [], [],
      [SynModuleOrNamespace
         ([ComplexArgumentsLambdaHasArrowRange], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id
                       (x, None, false, false, false,
                        /root/ComplexArgumentsLambdaHasArrowRange.fs (1,5--1,6));
                     Id
                       (_arg3, None, true, false, false,
                        /root/ComplexArgumentsLambdaHasArrowRange.fs (1,8--1,9))],
                    /root/ComplexArgumentsLambdaHasArrowRange.fs (1,4--1,10)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id
                          (_arg2, None, true, false, false,
                           /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--2,17))],
                       /root/ComplexArgumentsLambdaHasArrowRange.fs (2,4--2,18)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id
                             (_arg1, None, true, false, false,
                              /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,19))],
                          /root/ComplexArgumentsLambdaHasArrowRange.fs (3,4--3,20)),
                       Match
                         (NoneAtInvisible, Ident _arg2,
                          [SynMatchClause
                             (Record
                                ([(([], Y),
                                   /root/ComplexArgumentsLambdaHasArrowRange.fs (2,9--2,10),
                                   ListCons
                                     (Named
                                        (SynIdent (h, None), false, None,
                                         /root/ComplexArgumentsLambdaHasArrowRange.fs (2,11--2,12)),
                                      Wild
                                        /root/ComplexArgumentsLambdaHasArrowRange.fs (2,14--2,15),
                                      /root/ComplexArgumentsLambdaHasArrowRange.fs (2,11--2,15),
                                      { ColonColonRange =
                                         /root/ComplexArgumentsLambdaHasArrowRange.fs (2,12--2,14) }))],
                                 /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--2,17)),
                              None,
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
                                                 /root/ComplexArgumentsLambdaHasArrowRange.fs (3,17--3,18)),
                                              /root/ComplexArgumentsLambdaHasArrowRange.fs (3,16--3,19))],
                                        None,
                                        /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,19)),
                                     None,
                                     App
                                       (NonAtomic, false,
                                        App
                                          (NonAtomic, true,
                                           LongIdent
                                             (false,
                                              SynLongIdent
                                                ([op_Addition], [],
                                                 [Some (OriginalNotation "+")]),
                                              None,
                                              /root/ComplexArgumentsLambdaHasArrowRange.fs (5,10--5,11)),
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
                                                    None,
                                                    /root/ComplexArgumentsLambdaHasArrowRange.fs (5,6--5,7)),
                                                 Ident x,
                                                 /root/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,7)),
                                              Ident y,
                                              /root/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,9)),
                                           /root/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,11)),
                                        Ident z,
                                        /root/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,13)),
                                     /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,19),
                                     No, { ArrowRange = None
                                           BarRange = None })],
                                 /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--5,13),
                                 { MatchKeyword =
                                    /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--5,13)
                                   WithKeyword =
                                    /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--5,13) }),
                              /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--2,17),
                              No, { ArrowRange = None
                                    BarRange = None })],
                          /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--5,13),
                          { MatchKeyword =
                             /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--5,13)
                            WithKeyword =
                             /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--5,13) }),
                       None,
                       /root/ComplexArgumentsLambdaHasArrowRange.fs (1,0--5,13),
                       { ArrowRange =
                          Some
                            /root/ComplexArgumentsLambdaHasArrowRange.fs (4,4--4,6) }),
                    None,
                    /root/ComplexArgumentsLambdaHasArrowRange.fs (1,0--5,13),
                    { ArrowRange =
                       Some
                         /root/ComplexArgumentsLambdaHasArrowRange.fs (4,4--4,6) }),
                 Some
                   ([Paren
                       (Tuple
                          (false,
                           [Named
                              (SynIdent (x, None), false, None,
                               /root/ComplexArgumentsLambdaHasArrowRange.fs (1,5--1,6));
                            Wild
                              /root/ComplexArgumentsLambdaHasArrowRange.fs (1,8--1,9)],
                           /root/ComplexArgumentsLambdaHasArrowRange.fs (1,5--1,9)),
                        /root/ComplexArgumentsLambdaHasArrowRange.fs (1,4--1,10));
                     Paren
                       (Record
                          ([(([], Y),
                             /root/ComplexArgumentsLambdaHasArrowRange.fs (2,9--2,10),
                             ListCons
                               (Named
                                  (SynIdent (h, None), false, None,
                                   /root/ComplexArgumentsLambdaHasArrowRange.fs (2,11--2,12)),
                                Wild
                                  /root/ComplexArgumentsLambdaHasArrowRange.fs (2,14--2,15),
                                /root/ComplexArgumentsLambdaHasArrowRange.fs (2,11--2,15),
                                { ColonColonRange =
                                   /root/ComplexArgumentsLambdaHasArrowRange.fs (2,12--2,14) }))],
                           /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--2,17)),
                        /root/ComplexArgumentsLambdaHasArrowRange.fs (2,4--2,18));
                     Paren
                       (LongIdent
                          (SynLongIdent ([SomePattern], [], [None]), None, None,
                           Pats
                             [Paren
                                (Named
                                   (SynIdent (z, None), false, None,
                                    /root/ComplexArgumentsLambdaHasArrowRange.fs (3,17--3,18)),
                                 /root/ComplexArgumentsLambdaHasArrowRange.fs (3,16--3,19))],
                           None,
                           /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,19)),
                        /root/ComplexArgumentsLambdaHasArrowRange.fs (3,4--3,20))],
                    Match
                      (NoneAtInvisible, Ident _arg2,
                       [SynMatchClause
                          (Record
                             ([(([], Y),
                                /root/ComplexArgumentsLambdaHasArrowRange.fs (2,9--2,10),
                                ListCons
                                  (Named
                                     (SynIdent (h, None), false, None,
                                      /root/ComplexArgumentsLambdaHasArrowRange.fs (2,11--2,12)),
                                   Wild
                                     /root/ComplexArgumentsLambdaHasArrowRange.fs (2,14--2,15),
                                   /root/ComplexArgumentsLambdaHasArrowRange.fs (2,11--2,15),
                                   { ColonColonRange =
                                      /root/ComplexArgumentsLambdaHasArrowRange.fs (2,12--2,14) }))],
                              /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--2,17)),
                           None,
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
                                              /root/ComplexArgumentsLambdaHasArrowRange.fs (3,17--3,18)),
                                           /root/ComplexArgumentsLambdaHasArrowRange.fs (3,16--3,19))],
                                     None,
                                     /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,19)),
                                  None,
                                  App
                                    (NonAtomic, false,
                                     App
                                       (NonAtomic, true,
                                        LongIdent
                                          (false,
                                           SynLongIdent
                                             ([op_Addition], [],
                                              [Some (OriginalNotation "+")]),
                                           None,
                                           /root/ComplexArgumentsLambdaHasArrowRange.fs (5,10--5,11)),
                                        App
                                          (NonAtomic, false,
                                           App
                                             (NonAtomic, true,
                                              LongIdent
                                                (false,
                                                 SynLongIdent
                                                   ([op_Multiply], [],
                                                    [Some (OriginalNotation "*")]),
                                                 None,
                                                 /root/ComplexArgumentsLambdaHasArrowRange.fs (5,6--5,7)),
                                              Ident x,
                                              /root/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,7)),
                                           Ident y,
                                           /root/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,9)),
                                        /root/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,11)),
                                     Ident z,
                                     /root/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,13)),
                                  /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,19),
                                  No, { ArrowRange = None
                                        BarRange = None })],
                              /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--5,13),
                              { MatchKeyword =
                                 /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--5,13)
                                WithKeyword =
                                 /root/ComplexArgumentsLambdaHasArrowRange.fs (3,5--5,13) }),
                           /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--2,17),
                           No, { ArrowRange = None
                                 BarRange = None })],
                       /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--5,13),
                       { MatchKeyword =
                          /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--5,13)
                         WithKeyword =
                          /root/ComplexArgumentsLambdaHasArrowRange.fs (2,5--5,13) })),
                 /root/ComplexArgumentsLambdaHasArrowRange.fs (1,0--5,13),
                 { ArrowRange =
                    Some /root/ComplexArgumentsLambdaHasArrowRange.fs (4,4--4,6) }),
              /root/ComplexArgumentsLambdaHasArrowRange.fs (1,0--5,13))],
          PreXmlDocEmpty, [], None,
          /root/ComplexArgumentsLambdaHasArrowRange.fs (1,0--5,13),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))