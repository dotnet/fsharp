ImplFile
  (ParsedImplFileInput
     ("/root/ParenthesesOfSynArgPatsNamePatPairs.fs", false,
      QualifiedNameOfFile ParenthesesOfSynArgPatsNamePatPairs, [], [],
      [SynModuleOrNamespace
         ([ParenthesesOfSynArgPatsNamePatPairs], false, AnonModule,
          [Expr
             (Match
                (Yes /root/ParenthesesOfSynArgPatsNamePatPairs.fs (1,0--1,15),
                 Ident data,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([OnePartData], [], [None]), None, None,
                        NamePatPairs
                          ([(part1,
                             /root/ParenthesesOfSynArgPatsNamePatPairs.fs (3,10--3,11),
                             Named
                               (SynIdent (p1, None), false, None,
                                /root/ParenthesesOfSynArgPatsNamePatPairs.fs (3,12--3,14)))],
                           /root/ParenthesesOfSynArgPatsNamePatPairs.fs (3,4--4,13),
                           { ParenRange =
                              /root/ParenthesesOfSynArgPatsNamePatPairs.fs (2,13--4,13) }),
                        None,
                        /root/ParenthesesOfSynArgPatsNamePatPairs.fs (2,2--4,13)),
                     None, Ident p1,
                     /root/ParenthesesOfSynArgPatsNamePatPairs.fs (2,2--4,19),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/ParenthesesOfSynArgPatsNamePatPairs.fs (4,14--4,16)
                       BarRange =
                        Some
                          /root/ParenthesesOfSynArgPatsNamePatPairs.fs (2,0--2,1) });
                  SynMatchClause
                    (Wild
                       /root/ParenthesesOfSynArgPatsNamePatPairs.fs (5,2--5,3),
                     None,
                     App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("todo", Regular,
                              /root/ParenthesesOfSynArgPatsNamePatPairs.fs (5,16--5,22)),
                           /root/ParenthesesOfSynArgPatsNamePatPairs.fs (5,16--5,22)),
                        /root/ParenthesesOfSynArgPatsNamePatPairs.fs (5,7--5,22)),
                     /root/ParenthesesOfSynArgPatsNamePatPairs.fs (5,2--5,22),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/ParenthesesOfSynArgPatsNamePatPairs.fs (5,4--5,6)
                       BarRange =
                        Some
                          /root/ParenthesesOfSynArgPatsNamePatPairs.fs (5,0--5,1) })],
                 /root/ParenthesesOfSynArgPatsNamePatPairs.fs (1,0--5,22),
                 { MatchKeyword =
                    /root/ParenthesesOfSynArgPatsNamePatPairs.fs (1,0--1,5)
                   WithKeyword =
                    /root/ParenthesesOfSynArgPatsNamePatPairs.fs (1,11--1,15) }),
              /root/ParenthesesOfSynArgPatsNamePatPairs.fs (1,0--5,22))],
          PreXmlDocEmpty, [], None,
          /root/ParenthesesOfSynArgPatsNamePatPairs.fs (1,0--5,22),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment /root/ParenthesesOfSynArgPatsNamePatPairs.fs (2,15--2,21);
          BlockComment /root/ParenthesesOfSynArgPatsNamePatPairs.fs (4,2--4,11)] },
      set []))