ImplFile
  (ParsedImplFileInput
     ("/root/Type/MultipleSynEnumCaseContainsRangeOfConstant.fs", false,
      QualifiedNameOfFile MultipleSynEnumCaseContainsRangeOfConstant, [],
      [SynModuleOrNamespace
         ([MultipleSynEnumCaseContainsRangeOfConstant], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (One, None),
                            Const (Int32 1, (3,13--3,23)),
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (3,6--3,23), { BarRange = Some (3,4--3,5)
                                           EqualsRange = (3,10--3,11) });
                         SynEnumCase
                           ([], SynIdent (Two, None),
                            Const (Int32 2, (4,12--4,13)),
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (4,6--4,13), { BarRange = Some (4,4--4,5)
                                           EqualsRange = (4,10--4,11) })],
                        (3,4--4,13)), (3,4--4,13)), [], None, (2,5--4,13),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--4,13))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
