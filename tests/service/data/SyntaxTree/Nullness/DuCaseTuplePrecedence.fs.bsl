ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/DuCaseTuplePrecedence.fs", false,
      QualifiedNameOfFile DuCaseTuplePrecedence, [], [],
      [SynModuleOrNamespace
         ([DuCaseTuplePrecedence], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [DU],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,7)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (MyCase, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  Paren
                                    (WithNull
                                       (LongIdent
                                          (SynLongIdent ([string], [], [None])),
                                        false, (1,21--1,34),
                                        { BarRange = (1,28--1,29) }),
                                     (1,20--1,35)), false,
                                  PreXmlDoc ((1,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (1,20--1,35), { LeadingKeyword = None
                                                        MutableKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((1,38), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (1,38--1,41), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((1,10), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (1,10--1,41), { BarRange = None })],
                        (1,10--1,41)), (1,10--1,41)), [], None, (1,5--1,41),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,8--1,9)
                    WithKeyword = None })], (1,0--1,41))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
