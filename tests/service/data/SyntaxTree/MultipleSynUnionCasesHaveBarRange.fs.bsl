ImplFile
  (ParsedImplFileInput
     ("/root/MultipleSynUnionCasesHaveBarRange.fs", false,
      QualifiedNameOfFile MultipleSynUnionCasesHaveBarRange, [], [],
      [SynModuleOrNamespace
         ([MultipleSynUnionCasesHaveBarRange], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/MultipleSynUnionCasesHaveBarRange.fs (1,5--1,8)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Bar, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  PreXmlDoc ((2,13), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/MultipleSynUnionCasesHaveBarRange.fs (2,13--2,19),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/MultipleSynUnionCasesHaveBarRange.fs (2,6--2,19),
                            { BarRange =
                               Some
                                 /root/MultipleSynUnionCasesHaveBarRange.fs (2,4--2,5) });
                         SynUnionCase
                           ([], SynIdent (Bear, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((3,14), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/MultipleSynUnionCasesHaveBarRange.fs (3,14--3,17),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/MultipleSynUnionCasesHaveBarRange.fs (3,6--3,17),
                            { BarRange =
                               Some
                                 /root/MultipleSynUnionCasesHaveBarRange.fs (3,4--3,5) })],
                        /root/MultipleSynUnionCasesHaveBarRange.fs (2,4--3,17)),
                     /root/MultipleSynUnionCasesHaveBarRange.fs (2,4--3,17)), [],
                  None, /root/MultipleSynUnionCasesHaveBarRange.fs (1,5--3,17),
                  { LeadingKeyword =
                     Type /root/MultipleSynUnionCasesHaveBarRange.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/MultipleSynUnionCasesHaveBarRange.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/MultipleSynUnionCasesHaveBarRange.fs (1,0--3,17))],
          PreXmlDocEmpty, [], None,
          /root/MultipleSynUnionCasesHaveBarRange.fs (1,0--3,17),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))