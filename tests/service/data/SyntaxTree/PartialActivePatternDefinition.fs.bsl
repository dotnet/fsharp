ImplFile
  (ParsedImplFileInput
     ("/root/PartialActivePatternDefinition.fs", false,
      QualifiedNameOfFile PartialActivePatternDefinition, [], [],
      [SynModuleOrNamespace
         ([PartialActivePatternDefinition], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some a)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent
                       ([|Int32Const|_|], [],
                        [Some
                           (HasParenthesis
                              (/root/PartialActivePatternDefinition.fs (1,4--1,5),
                               /root/PartialActivePatternDefinition.fs (1,19--1,20)))]),
                     None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (a, None), false, None,
                                 /root/PartialActivePatternDefinition.fs (1,22--1,23)),
                              LongIdent (SynLongIdent ([SynConst], [], [None])),
                              /root/PartialActivePatternDefinition.fs (1,22--1,33)),
                           /root/PartialActivePatternDefinition.fs (1,21--1,34))],
                     None, /root/PartialActivePatternDefinition.fs (1,4--1,34)),
                  None,
                  Match
                    (Yes /root/PartialActivePatternDefinition.fs (1,37--1,49),
                     Ident a,
                     [SynMatchClause
                        (LongIdent
                           (SynLongIdent
                              ([SynConst; Int32],
                               [/root/PartialActivePatternDefinition.fs (1,58--1,59)],
                               [None; None]), None, None,
                            Pats
                              [Wild
                                 /root/PartialActivePatternDefinition.fs (1,65--1,66)],
                            None,
                            /root/PartialActivePatternDefinition.fs (1,50--1,66)),
                         None,
                         App
                           (NonAtomic, false, Ident Some, Ident a,
                            /root/PartialActivePatternDefinition.fs (1,70--1,76)),
                         /root/PartialActivePatternDefinition.fs (1,50--1,76),
                         Yes,
                         { ArrowRange =
                            Some
                              /root/PartialActivePatternDefinition.fs (1,67--1,69)
                           BarRange = None });
                      SynMatchClause
                        (Wild
                           /root/PartialActivePatternDefinition.fs (1,79--1,80),
                         None, Ident None,
                         /root/PartialActivePatternDefinition.fs (1,79--1,88),
                         Yes,
                         { ArrowRange =
                            Some
                              /root/PartialActivePatternDefinition.fs (1,81--1,83)
                           BarRange =
                            Some
                              /root/PartialActivePatternDefinition.fs (1,77--1,78) })],
                     /root/PartialActivePatternDefinition.fs (1,37--1,88),
                     { MatchKeyword =
                        /root/PartialActivePatternDefinition.fs (1,37--1,42)
                       WithKeyword =
                        /root/PartialActivePatternDefinition.fs (1,45--1,49) }),
                  /root/PartialActivePatternDefinition.fs (1,4--1,34), NoneAtLet,
                  { LeadingKeyword =
                     Let /root/PartialActivePatternDefinition.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some /root/PartialActivePatternDefinition.fs (1,35--1,36) })],
              /root/PartialActivePatternDefinition.fs (1,0--1,88))],
          PreXmlDocEmpty, [], None,
          /root/PartialActivePatternDefinition.fs (1,0--1,88),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))