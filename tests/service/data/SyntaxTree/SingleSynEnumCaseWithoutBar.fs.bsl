ImplFile
  (ParsedImplFileInput
     ("/root/SingleSynEnumCaseWithoutBar.fs", false,
      QualifiedNameOfFile SingleSynEnumCaseWithoutBar, [], [],
      [SynModuleOrNamespace
         ([SingleSynEnumCaseWithoutBar], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SingleSynEnumCaseWithoutBar.fs (2,5--2,8)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (Bar, None),
                            Const
                              (Int32 1,
                               /root/SingleSynEnumCaseWithoutBar.fs (2,17--2,18)),
                            PreXmlDoc ((2,11), FSharp.Compiler.Xml.XmlDocCollector),
                            /root/SingleSynEnumCaseWithoutBar.fs (2,11--2,18),
                            { BarRange = None
                              EqualsRange =
                               /root/SingleSynEnumCaseWithoutBar.fs (2,15--2,16) })],
                        /root/SingleSynEnumCaseWithoutBar.fs (2,11--2,18)),
                     /root/SingleSynEnumCaseWithoutBar.fs (2,11--2,18)), [],
                  None, /root/SingleSynEnumCaseWithoutBar.fs (2,5--2,18),
                  { LeadingKeyword =
                     Type /root/SingleSynEnumCaseWithoutBar.fs (2,0--2,4)
                    EqualsRange =
                     Some /root/SingleSynEnumCaseWithoutBar.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/SingleSynEnumCaseWithoutBar.fs (2,0--2,18))], PreXmlDocEmpty,
          [], None, /root/SingleSynEnumCaseWithoutBar.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))