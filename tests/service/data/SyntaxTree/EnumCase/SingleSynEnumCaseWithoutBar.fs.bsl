ImplFile
  (ParsedImplFileInput
     ("/root/EnumCase/SingleSynEnumCaseWithoutBar.fs", false,
      QualifiedNameOfFile SingleSynEnumCaseWithoutBar, [], [],
      [SynModuleOrNamespace
         ([SingleSynEnumCaseWithoutBar], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (Bar, None),
                            Const (Int32 1, (2,17--2,18)),
                            PreXmlDoc ((2,11), FSharp.Compiler.Xml.XmlDocCollector),
                            (2,11--2,18), { BarRange = None
                                            EqualsRange = (2,15--2,16) })],
                        (2,11--2,18)), (2,11--2,18)), [], None, (2,5--2,18),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--2,18))], PreXmlDocEmpty, [],
          None, (2,0--3,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
