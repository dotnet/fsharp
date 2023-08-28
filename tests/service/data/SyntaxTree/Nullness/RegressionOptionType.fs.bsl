ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/RegressionOptionType.fs", false,
      QualifiedNameOfFile RegressionOptionType, [], [],
      [SynModuleOrNamespace
         ([RegressionOptionType], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl ([], SynTypar (T, None, false))], [],
                           (1,15--1,19))), [], [Option],
                     PreXmlDoc ((1,4), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,9--1,15)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (None, None), Fields [],
                            PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (2,10--2,14), { BarRange = Some (2,8--2,9) });
                         SynUnionCase
                           ([], SynIdent (Some, None),
                            Fields
                              [SynField
                                 ([], false, Some Value,
                                  Var (SynTypar (T, None, false), (3,24--3,26)),
                                  false,
                                  PreXmlDoc ((3,18), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (3,18--3,26), { LeadingKeyword = None })],
                            PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,10--3,26), { BarRange = Some (3,8--3,9) })],
                        (2,8--3,26)), (2,8--3,26)), [], None, (1,9--3,26),
                  { LeadingKeyword = Type (1,4--1,8)
                    EqualsRange = Some (1,20--1,21)
                    WithKeyword = None })], (1,4--3,26))], PreXmlDocEmpty, [],
          None, (1,4--3,26), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
