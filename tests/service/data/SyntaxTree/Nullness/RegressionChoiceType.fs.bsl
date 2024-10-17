ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/RegressionChoiceType.fs", false,
      QualifiedNameOfFile RegressionChoiceType, [], [],
      [SynModuleOrNamespace
         ([RegressionChoiceType], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T1, None, false), [],
                               { AmpersandRanges = [] });
                            SynTyparDecl
                              ([], SynTypar (T2, None, false), [],
                               { AmpersandRanges = [] })], [], (1,13--1,22))),
                     [], [MyChoice],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,13)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (MyChoice, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  Var (SynTypar (T1, None, false), (4,16--4,19)),
                                  false,
                                  PreXmlDoc ((4,16), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (4,16--4,19), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((4,2), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,2--4,19), { BarRange = Some (4,2--4,3) });
                         SynUnionCase
                           ([], SynIdent (MyChoice, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  Var (SynTypar (T2, None, false), (7,16--7,19)),
                                  false,
                                  PreXmlDoc ((7,16), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (7,16--7,19), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((7,2), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,2--7,19), { BarRange = Some (7,2--7,3) })],
                        (3,2--7,19)), (3,2--7,19)), [], None, (1,5--7,19),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,23--1,24)
                    WithKeyword = None })], (1,0--7,19))], PreXmlDocEmpty, [],
          None, (1,0--8,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
