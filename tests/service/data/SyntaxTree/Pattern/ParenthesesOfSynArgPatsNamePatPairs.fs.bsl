ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs", false,
      QualifiedNameOfFile ParenthesesOfSynArgPatsNamePatPairs, [], [],
      [SynModuleOrNamespace
         ([ParenthesesOfSynArgPatsNamePatPairs], false, AnonModule,
          [Expr
             (Match
                (Yes
                   /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (2,0--2,15),
                 Ident data,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([OnePartData], [], [None]), None, None,
                        NamePatPairs
                          ([(part1,
                             /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (4,10--4,11),
                             Named
                               (SynIdent (p1, None), false, None,
                                /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (4,12--4,14)))],
                           /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (4,4--5,13),
                           { ParenRange =
                              /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (3,13--5,13) }),
                        None,
                        /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (3,2--5,13)),
                     None, Ident p1,
                     /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (3,2--5,19),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (5,14--5,16)
                       BarRange =
                        Some
                          /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (3,0--3,1) });
                  SynMatchClause
                    (Wild
                       /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (6,2--6,3),
                     None,
                     App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("todo", Regular,
                              /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (6,16--6,22)),
                           /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (6,16--6,22)),
                        /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (6,7--6,22)),
                     /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (6,2--6,22),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (6,4--6,6)
                       BarRange =
                        Some
                          /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (6,0--6,1) })],
                 /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (2,0--6,22),
                 { MatchKeyword =
                    /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (2,0--2,5)
                   WithKeyword =
                    /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (2,11--2,15) }),
              /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (2,0--6,22))],
          PreXmlDocEmpty, [], None,
          /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (3,15--3,21);
          BlockComment
            /root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs (5,2--5,11)] },
      set []))