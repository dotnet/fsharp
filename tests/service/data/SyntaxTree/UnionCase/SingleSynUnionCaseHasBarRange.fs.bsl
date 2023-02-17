ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/SingleSynUnionCaseHasBarRange.fs", false,
      QualifiedNameOfFile SingleSynUnionCaseHasBarRange, [], [],
      [SynModuleOrNamespace
         ([SingleSynUnionCaseHasBarRange], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
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
                                  PreXmlDoc ((2,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (2,20--2,26), { LeadingKeyword = None })],
                            PreXmlDoc ((2,11), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (2,13--2,26), { BarRange = Some (2,11--2,12) })],
                        (2,11--2,26)), (2,11--2,26)), [], None, (2,5--2,26),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--2,26))], PreXmlDocEmpty, [],
          None, (2,0--3,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
