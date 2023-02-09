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
                   ([Id
                       (x, None, false, false, false,
                        /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,5--2,6));
                     Id
                       (_arg3, None, true, false, false,
                        /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,8--2,9))],
                    /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,4--2,10)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id
                          (_arg2, None, true, false, false,
                           /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,17))],
                       /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,4--3,18)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id
                             (_arg1, None, true, false, false,
                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--4,19))],
                          /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,4--4,20)),
                       Match
                         (NoneAtInvisible, Ident _arg2,
                          [SynMatchClause
                             (Record
                                ([(([], Y),
                                   /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,9--3,10),
                                   ListCons
                                     (Named
                                        (SynIdent (h, None), false, None,
                                         /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,11--3,12)),
                                      Wild
                                        /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,14--3,15),
                                      /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,11--3,15),
                                      { ColonColonRange =
                                         /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,12--3,14) }))],
                                 /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,17)),
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
                                                 /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,17--4,18)),
                                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,16--4,19))],
                                        None,
                                        /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--4,19)),
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
                                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,10--6,11)),
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
                                                    /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,6--6,7)),
                                                 Ident x,
                                                 /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,4--6,7)),
                                              Ident y,
                                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,4--6,9)),
                                           /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,4--6,11)),
                                        Ident z,
                                        /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,4--6,13)),
                                     /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--4,19),
                                     No, { ArrowRange = None
                                           BarRange = None })],
                                 /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--6,13),
                                 { MatchKeyword =
                                    /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--6,13)
                                   WithKeyword =
                                    /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--6,13) }),
                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,17),
                              No, { ArrowRange = None
                                    BarRange = None })],
                          /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--6,13),
                          { MatchKeyword =
                             /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--6,13)
                            WithKeyword =
                             /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--6,13) }),
                       None,
                       /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,0--6,13),
                       { ArrowRange =
                          Some
                            /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,6) }),
                    None,
                    /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,0--6,13),
                    { ArrowRange =
                       Some
                         /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,6) }),
                 Some
                   ([Paren
                       (Tuple
                          (false,
                           [Named
                              (SynIdent (x, None), false, None,
                               /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,5--2,6));
                            Wild
                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,8--2,9)],
                           /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,5--2,9)),
                        /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,4--2,10));
                     Paren
                       (Record
                          ([(([], Y),
                             /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,9--3,10),
                             ListCons
                               (Named
                                  (SynIdent (h, None), false, None,
                                   /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,11--3,12)),
                                Wild
                                  /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,14--3,15),
                                /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,11--3,15),
                                { ColonColonRange =
                                   /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,12--3,14) }))],
                           /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,17)),
                        /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,4--3,18));
                     Paren
                       (LongIdent
                          (SynLongIdent ([SomePattern], [], [None]), None, None,
                           Pats
                             [Paren
                                (Named
                                   (SynIdent (z, None), false, None,
                                    /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,17--4,18)),
                                 /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,16--4,19))],
                           None,
                           /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--4,19)),
                        /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,4--4,20))],
                    Match
                      (NoneAtInvisible, Ident _arg2,
                       [SynMatchClause
                          (Record
                             ([(([], Y),
                                /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,9--3,10),
                                ListCons
                                  (Named
                                     (SynIdent (h, None), false, None,
                                      /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,11--3,12)),
                                   Wild
                                     /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,14--3,15),
                                   /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,11--3,15),
                                   { ColonColonRange =
                                      /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,12--3,14) }))],
                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,17)),
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
                                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,17--4,18)),
                                           /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,16--4,19))],
                                     None,
                                     /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--4,19)),
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
                                           /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,10--6,11)),
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
                                                 /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,6--6,7)),
                                              Ident x,
                                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,4--6,7)),
                                           Ident y,
                                           /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,4--6,9)),
                                        /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,4--6,11)),
                                     Ident z,
                                     /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (6,4--6,13)),
                                  /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--4,19),
                                  No, { ArrowRange = None
                                        BarRange = None })],
                              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--6,13),
                              { MatchKeyword =
                                 /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--6,13)
                                WithKeyword =
                                 /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (4,5--6,13) }),
                           /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--3,17),
                           No, { ArrowRange = None
                                 BarRange = None })],
                       /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--6,13),
                       { MatchKeyword =
                          /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--6,13)
                         WithKeyword =
                          /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (3,5--6,13) })),
                 /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,0--6,13),
                 { ArrowRange =
                    Some
                      /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (5,4--5,6) }),
              /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,0--6,13))],
          PreXmlDocEmpty, [], None,
          /root/Lambda/ComplexArgumentsLambdaHasArrowRange.fs (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))