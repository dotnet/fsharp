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
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,5--3,8)),
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
                                  PreXmlDoc ((3,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,20--3,26),
                                  { LeadingKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((3,29), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,29--3,32),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((3,11), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,13--3,32),
                            { BarRange =
                               Some
                                 /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,11--3,12) })],
                        /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,11--3,32)),
                     /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,11--3,32)),
                  [],
                  /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,5--3,32),
                  { LeadingKeyword =
                     Type
                       /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,0--3,4)
                    EqualsRange =
                     Some
                       /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,9--3,10)
                    WithKeyword = None })],
              /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (3,0--3,32))],
          PreXmlDocEmpty, [], None,
          /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (1,0--3,32),
          { LeadingKeyword =
             Namespace
               /root/RangeMemberReturnsRangeOfSynModuleOrNamespaceSig.fsi (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))