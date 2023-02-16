SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/SynTypeDefnSigWithEnumContainsTheRangeOfTheEqualsSign.fsi",
      QualifiedNameOfFile SynTypeDefnSigWithEnumContainsTheRangeOfTheEqualsSign,
      [], [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Bear],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,9)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (BlackBear, None),
                            Const (Int32 1, (5,18--5,19)),
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (5,6--5,19), { BarRange = Some (5,4--5,5)
                                           EqualsRange = (5,16--5,17) });
                         SynEnumCase
                           ([], SynIdent (PolarBear, None),
                            Const (Int32 2, (6,18--6,19)),
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (6,6--6,19), { BarRange = Some (6,4--6,5)
                                           EqualsRange = (6,16--6,17) })],
                        (5,4--6,19)), (5,4--6,19)), [], (4,5--6,19),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,10--4,11)
                    WithKeyword = None })], (4,0--6,19))], PreXmlDocEmpty, [],
          None, (2,0--6,19), { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
