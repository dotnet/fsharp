ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/PartialActivePatternDefinition.fs", false,
      QualifiedNameOfFile PartialActivePatternDefinition, [], [],
      [SynModuleOrNamespace
         ([PartialActivePatternDefinition], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
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
                              (/root/OperatorName/PartialActivePatternDefinition.fs (2,4--2,5),
                               /root/OperatorName/PartialActivePatternDefinition.fs (2,19--2,20)))]),
                     None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (a, None), false, None,
                                 /root/OperatorName/PartialActivePatternDefinition.fs (2,22--2,23)),
                              LongIdent (SynLongIdent ([SynConst], [], [None])),
                              /root/OperatorName/PartialActivePatternDefinition.fs (2,22--2,33)),
                           /root/OperatorName/PartialActivePatternDefinition.fs (2,21--2,34))],
                     None,
                     /root/OperatorName/PartialActivePatternDefinition.fs (2,4--2,34)),
                  None,
                  Match
                    (Yes
                       /root/OperatorName/PartialActivePatternDefinition.fs (2,37--2,49),
                     Ident a,
                     [SynMatchClause
                        (LongIdent
                           (SynLongIdent
                              ([SynConst; Int32],
                               [/root/OperatorName/PartialActivePatternDefinition.fs (2,58--2,59)],
                               [None; None]), None, None,
                            Pats
                              [Wild
                                 /root/OperatorName/PartialActivePatternDefinition.fs (2,65--2,66)],
                            None,
                            /root/OperatorName/PartialActivePatternDefinition.fs (2,50--2,66)),
                         None,
                         App
                           (NonAtomic, false, Ident Some, Ident a,
                            /root/OperatorName/PartialActivePatternDefinition.fs (2,70--2,76)),
                         /root/OperatorName/PartialActivePatternDefinition.fs (2,50--2,76),
                         Yes,
                         { ArrowRange =
                            Some
                              /root/OperatorName/PartialActivePatternDefinition.fs (2,67--2,69)
                           BarRange = None });
                      SynMatchClause
                        (Wild
                           /root/OperatorName/PartialActivePatternDefinition.fs (2,79--2,80),
                         None, Ident None,
                         /root/OperatorName/PartialActivePatternDefinition.fs (2,79--2,88),
                         Yes,
                         { ArrowRange =
                            Some
                              /root/OperatorName/PartialActivePatternDefinition.fs (2,81--2,83)
                           BarRange =
                            Some
                              /root/OperatorName/PartialActivePatternDefinition.fs (2,77--2,78) })],
                     /root/OperatorName/PartialActivePatternDefinition.fs (2,37--2,88),
                     { MatchKeyword =
                        /root/OperatorName/PartialActivePatternDefinition.fs (2,37--2,42)
                       WithKeyword =
                        /root/OperatorName/PartialActivePatternDefinition.fs (2,45--2,49) }),
                  /root/OperatorName/PartialActivePatternDefinition.fs (2,4--2,34),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/OperatorName/PartialActivePatternDefinition.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/OperatorName/PartialActivePatternDefinition.fs (2,35--2,36) })],
              /root/OperatorName/PartialActivePatternDefinition.fs (2,0--2,88))],
          PreXmlDocEmpty, [], None,
          /root/OperatorName/PartialActivePatternDefinition.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
