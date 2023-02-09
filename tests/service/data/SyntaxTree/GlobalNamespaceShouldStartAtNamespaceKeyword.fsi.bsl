SigFile
  (ParsedSigFileInput
     ("/root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi",
      QualifiedNameOfFile GlobalNamespaceShouldStartAtNamespaceKeyword, [], [],
      [SynModuleOrNamespaceSig
         ([], false, GlobalNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Bar],
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,5--5,8)),
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
                                  PreXmlDoc ((5,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,20--5,26),
                                  { LeadingKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((5,29), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,29--5,32),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((5,11), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,13--5,32),
                            { BarRange =
                               Some
                                 /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,11--5,12) })],
                        /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,11--5,32)),
                     /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,11--5,32)),
                  [],
                  /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,5--5,32),
                  { LeadingKeyword =
                     Type
                       /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,0--5,4)
                    EqualsRange =
                     Some
                       /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,9--5,10)
                    WithKeyword = None })],
              /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (5,0--5,32))],
          PreXmlDocEmpty, [], None,
          /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (3,0--5,32),
          { LeadingKeyword =
             Namespace
               /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (3,0--3,9) })],
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (1,0--1,6);
          LineComment
            /root/GlobalNamespaceShouldStartAtNamespaceKeyword.fsi (2,0--2,6)] },
      set []))