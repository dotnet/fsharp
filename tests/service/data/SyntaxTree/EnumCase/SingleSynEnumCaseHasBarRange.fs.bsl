ImplFile
  (ParsedImplFileInput
     ("/root/EnumCase/SingleSynEnumCaseHasBarRange.fs", false,
      QualifiedNameOfFile SingleSynEnumCaseHasBarRange, [], [],
      [SynModuleOrNamespace
         ([SingleSynEnumCaseHasBarRange], false, AnonModule,
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
                            Const (Int32 1, (2,19--2,20)),
                            PreXmlDoc ((2,11), FSharp.Compiler.Xml.XmlDocCollector),
                            (2,13--2,20), { BarRange = Some (2,11--2,12)
                                            EqualsRange = (2,17--2,18) })],
                        (2,11--2,20)), (2,11--2,20)), [], None, (2,5--2,20),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--2,20))], PreXmlDocEmpty, [],
          None, (2,0--3,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
