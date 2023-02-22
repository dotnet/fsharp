ImplFile
  (ParsedImplFileInput
     ("/root/EnumCase/MultipleSynEnumCasesHaveBarRange.fs", false,
      QualifiedNameOfFile MultipleSynEnumCasesHaveBarRange, [], [],
      [SynModuleOrNamespace
         ([MultipleSynEnumCasesHaveBarRange], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (Bar, None),
                            Const (Int32 1, (3,12--3,13)),
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (3,6--3,13), { BarRange = Some (3,4--3,5)
                                           EqualsRange = (3,10--3,11) });
                         SynEnumCase
                           ([], SynIdent (Bear, None),
                            Const (Int32 2, (4,13--4,14)),
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (4,6--4,14), { BarRange = Some (4,4--4,5)
                                           EqualsRange = (4,11--4,12) })],
                        (3,4--4,14)), (3,4--4,14)), [], None, (2,5--4,14),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--4,14))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
