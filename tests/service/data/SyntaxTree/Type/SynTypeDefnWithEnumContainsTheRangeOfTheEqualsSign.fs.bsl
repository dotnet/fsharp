ImplFile
  (ParsedImplFileInput
     ("/root/Type/SynTypeDefnWithEnumContainsTheRangeOfTheEqualsSign.fs", false,
      QualifiedNameOfFile SynTypeDefnWithEnumContainsTheRangeOfTheEqualsSign, [],
      [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithEnumContainsTheRangeOfTheEqualsSign], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Bear],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,9)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (BlackBear, None),
                            Const (Int32 1, (3,18--3,19)),
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (3,6--3,19), { BarRange = Some (3,4--3,5)
                                           EqualsRange = (3,16--3,17) });
                         SynEnumCase
                           ([], SynIdent (PolarBear, None),
                            Const (Int32 2, (4,18--4,19)),
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (4,6--4,19), { BarRange = Some (4,4--4,5)
                                           EqualsRange = (4,16--4,17) })],
                        (3,4--4,19)), (3,4--4,19)), [], None, (2,5--4,19),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,10--2,11)
                    WithKeyword = None })], (2,0--4,19))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
