ImplFile
  (ParsedImplFileInput
     ("/root/MultipleSynEnumCasesHaveBarRange.fs", false,
      QualifiedNameOfFile MultipleSynEnumCasesHaveBarRange, [], [],
      [SynModuleOrNamespace
         ([MultipleSynEnumCasesHaveBarRange], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/MultipleSynEnumCasesHaveBarRange.fs (1,5--1,8)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (Bar, None),
                            Const
                              (Int32 1,
                               /root/MultipleSynEnumCasesHaveBarRange.fs (2,12--2,13)),
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            /root/MultipleSynEnumCasesHaveBarRange.fs (2,6--2,13),
                            { BarRange =
                               Some
                                 /root/MultipleSynEnumCasesHaveBarRange.fs (2,4--2,5)
                              EqualsRange =
                               /root/MultipleSynEnumCasesHaveBarRange.fs (2,10--2,11) });
                         SynEnumCase
                           ([], SynIdent (Bear, None),
                            Const
                              (Int32 2,
                               /root/MultipleSynEnumCasesHaveBarRange.fs (3,13--3,14)),
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            /root/MultipleSynEnumCasesHaveBarRange.fs (3,6--3,14),
                            { BarRange =
                               Some
                                 /root/MultipleSynEnumCasesHaveBarRange.fs (3,4--3,5)
                              EqualsRange =
                               /root/MultipleSynEnumCasesHaveBarRange.fs (3,11--3,12) })],
                        /root/MultipleSynEnumCasesHaveBarRange.fs (2,4--3,14)),
                     /root/MultipleSynEnumCasesHaveBarRange.fs (2,4--3,14)), [],
                  None, /root/MultipleSynEnumCasesHaveBarRange.fs (1,5--3,14),
                  { LeadingKeyword =
                     Type /root/MultipleSynEnumCasesHaveBarRange.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/MultipleSynEnumCasesHaveBarRange.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/MultipleSynEnumCasesHaveBarRange.fs (1,0--3,14))],
          PreXmlDocEmpty, [], None,
          /root/MultipleSynEnumCasesHaveBarRange.fs (1,0--3,14),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))