ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/RegressionResultType.fs", false,
      QualifiedNameOfFile RegressionResultType, [], [],
      [SynModuleOrNamespace
         ([RegressionResultType], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] });
                            SynTyparDecl
                              ([], SynTypar (TError, None, false), [],
                               { AmpersandRanges = [] })], [], (1,17--1,29))),
                     [], [MyResult],
                     PreXmlDoc ((1,4), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,9--1,17)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Ok, None),
                            Fields
                              [SynField
                                 ([], false, Some ResultValue,
                                  Var (SynTypar (T, None, false), (4,26--4,28)),
                                  false,
                                  PreXmlDoc ((4,14), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (4,14--4,28), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((4,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,6--4,28), { BarRange = Some (4,6--4,7) });
                         SynUnionCase
                           ([], SynIdent (Error, None),
                            Fields
                              [SynField
                                 ([], false, Some ErrorValue,
                                  Var
                                    (SynTypar (TError, None, false),
                                     (7,28--7,35)), false,
                                  PreXmlDoc ((7,17), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (7,17--7,35), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((7,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--7,35), { BarRange = Some (7,6--7,7) })],
                        (3,6--7,35)), (3,6--7,35)), [], None, (1,9--7,35),
                  { LeadingKeyword = Type (1,4--1,8)
                    EqualsRange = Some (1,30--1,31)
                    WithKeyword = None })], (1,4--7,35))], PreXmlDocEmpty, [],
          None, (1,4--7,35), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
