SigFile
  (ParsedSigFileInput
     ("/root/LeadingKeyword/StaticValKeyword.fsi",
      QualifiedNameOfFile StaticValKeyword, [], [],
      [SynModuleOrNamespaceSig
         ([Meh], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  ObjectModel
                    (Unspecified,
                     [ValField
                        (SynField
                           ([], true, Some Y,
                            Fun
                              (LongIdent (SynLongIdent ([int], [], [None])),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (5,19--5,29), { ArrowRange = (5,23--5,25) }),
                            false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,4--5,29),
                            { LeadingKeyword =
                               Some (StaticVal ((5,4--5,10), (5,11--5,14))) }),
                         (5,4--5,29))], (5,4--5,29)), [], (4,5--5,29),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--5,29))], PreXmlDocEmpty, [],
          None, (2,0--5,29), { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
