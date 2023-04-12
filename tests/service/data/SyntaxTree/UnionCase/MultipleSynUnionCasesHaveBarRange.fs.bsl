ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/MultipleSynUnionCasesHaveBarRange.fs", false,
      QualifiedNameOfFile MultipleSynUnionCasesHaveBarRange, [], [],
      [SynModuleOrNamespace
         ([MultipleSynUnionCasesHaveBarRange], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
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
                                  PreXmlDoc ((3,13), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (3,13--3,19), { LeadingKeyword = None })],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,6--3,19), { BarRange = Some (3,4--3,5) });
                         SynUnionCase
                           ([], SynIdent (Bear, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((4,14), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (4,14--4,17), { LeadingKeyword = None })],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,17), { BarRange = Some (4,4--4,5) })],
                        (3,4--4,17)), (3,4--4,17)), [], None, (2,5--4,17),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--4,17))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
