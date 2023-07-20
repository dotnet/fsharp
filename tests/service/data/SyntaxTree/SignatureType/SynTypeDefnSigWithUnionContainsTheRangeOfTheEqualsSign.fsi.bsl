SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/SynTypeDefnSigWithUnionContainsTheRangeOfTheEqualsSign.fsi",
      QualifiedNameOfFile SynTypeDefnSigWithUnionContainsTheRangeOfTheEqualsSign,
      [], [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Shape],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,10)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Square, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((5,12), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (5,12--5,15), { LeadingKeyword = None })],
                            PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,2--5,15), { BarRange = Some (5,0--5,1) });
                         SynUnionCase
                           ([], SynIdent (Rectangle, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((6,15), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (6,15--6,18), { LeadingKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((6,21), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (6,21--6,24), { LeadingKeyword = None })],
                            PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,2--6,24), { BarRange = Some (6,0--6,1) })],
                        (5,0--6,24)), (5,0--6,24)), [], (4,5--6,24),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,11--4,12)
                    WithKeyword = None })], (4,0--6,24))], PreXmlDocEmpty, [],
          None, (2,0--6,24), { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
