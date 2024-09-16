ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/DuCaseStringOrNull.fs", false,
      QualifiedNameOfFile DuCaseStringOrNull, [], [],
      [SynModuleOrNamespace
         ([DuCaseStringOrNull], false, AnonModule,
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
                                                        MutableKeyword = None })],
                            PreXmlDoc ((1,10), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (1,10--1,35), { BarRange = None })],
                        (1,10--1,35)), (1,10--1,35)), [], None, (1,5--1,35),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,8--1,9)
                    WithKeyword = None })], (1,0--1,35))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
