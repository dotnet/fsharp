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
                                  WithNull
                                    (LongIdent
                                       (SynLongIdent ([string], [], [None])),
                                     false, (1,20--1,33)), false,
                                  PreXmlDoc ((1,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (1,20--1,33), { LeadingKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((1,36), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (1,36--1,39), { LeadingKeyword = None })],
                            PreXmlDoc ((1,10), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (1,10--1,39), { BarRange = None })],
                        (1,10--1,39)), (1,10--1,39)), [], None, (1,5--1,39),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,8--1,9)
                    WithKeyword = None })], (1,0--1,39))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
