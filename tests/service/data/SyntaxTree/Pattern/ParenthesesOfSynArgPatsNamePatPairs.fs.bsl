ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/ParenthesesOfSynArgPatsNamePatPairs.fs", false,
      QualifiedNameOfFile ParenthesesOfSynArgPatsNamePatPairs, [], [],
      [SynModuleOrNamespace
         ([ParenthesesOfSynArgPatsNamePatPairs], false, AnonModule,
          [Expr
             (Match
                (Yes (2,0--2,15), Ident data,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([OnePartData], [], [None]), None, None,
                        NamePatPairs
                          ([(part1, (4,10--4,11),
                             Named
                               (SynIdent (p1, None), false, None, (4,12--4,14)))],
                           (4,4--5,13), { ParenRange = (3,13--5,13) }), None,
                        (3,2--5,13)), None, Ident p1, (3,2--5,19), Yes,
                     { ArrowRange = Some (5,14--5,16)
                       BarRange = Some (3,0--3,1) });
                  SynMatchClause
                    (Wild (6,2--6,3), None,
                     App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String ("todo", Regular, (6,16--6,22)), (6,16--6,22)),
                        (6,7--6,22)), (6,2--6,22), Yes,
                     { ArrowRange = Some (6,4--6,6)
                       BarRange = Some (6,0--6,1) })], (2,0--6,22),
                 { MatchKeyword = (2,0--2,5)
                   WithKeyword = (2,11--2,15) }), (2,0--6,22))], PreXmlDocEmpty,
          [], None, (2,0--7,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [LineComment (3,15--3,21); BlockComment (5,2--5,11)] },
      set []))
