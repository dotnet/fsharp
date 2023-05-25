SigFile
  (ParsedSigFileInput
     ("/root/ModuleOrNamespaceSig/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi",
      QualifiedNameOfFile GlobalNamespaceShouldStartAtNamespaceKeyword, [], [],
      [SynModuleOrNamespaceSig
         ([], false, GlobalNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Bar],
                     PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,8)),
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
                                  PreXmlDoc ((6,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (6,20--6,26), { LeadingKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((6,29), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (6,29--6,32), { LeadingKeyword = None })],
                            PreXmlDoc ((6,11), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,13--6,32), { BarRange = Some (6,11--6,12) })],
                        (6,11--6,32)), (6,11--6,32)), [], (6,5--6,32),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,9--6,10)
                    WithKeyword = None })], (6,0--6,32))], PreXmlDocEmpty, [],
          None, (4,0--6,32), { LeadingKeyword = Namespace (4,0--4,9) })],
      { ConditionalDirectives = []
        CodeComments = [LineComment (2,0--2,6); LineComment (3,0--3,6)] },
      set []))
