SigFile
  (ParsedSigFileInput
     ("/root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi",
      QualifiedNameOfFile RangeMemberReturnsRangeOfSynModuleOrNamespaceSig, [],
      [],
      [SynModuleOrNamespaceSig
         ([Foobar], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Bar],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,5--4,8)),
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
                                  PreXmlDoc ((4,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,20--4,26),
                                  { LeadingKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((4,29), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,29--4,32),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((4,11), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,13--4,32),
                            { BarRange =
                               Some
                                 /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,11--4,12) })],
                        /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,11--4,32)),
                     /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,11--4,32)),
                  [],
                  /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,5--4,32),
                  { LeadingKeyword =
                     Type
                       /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,0--4,4)
                    EqualsRange =
                     Some
                       /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,9--4,10)
                    WithKeyword = None })],
              /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (4,0--4,32))],
          PreXmlDocEmpty, [], None,
          /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (2,0--4,32),
          { LeadingKeyword =
             Namespace
               /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))