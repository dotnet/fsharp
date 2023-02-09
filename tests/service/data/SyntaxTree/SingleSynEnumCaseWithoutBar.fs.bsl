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
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SingleSynEnumCaseWithoutBar.fs (1,5--1,8)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (Bar, None),
                            Const
                              (Int32 1,
                               /root/SingleSynEnumCaseWithoutBar.fs (1,17--1,18)),
                            PreXmlDoc ((1,11), FSharp.Compiler.Xml.XmlDocCollector),
                            /root/SingleSynEnumCaseWithoutBar.fs (1,11--1,18),
                            { BarRange = None
                              EqualsRange =
                               /root/SingleSynEnumCaseWithoutBar.fs (1,15--1,16) })],
                        /root/SingleSynEnumCaseWithoutBar.fs (1,11--1,18)),
                     /root/SingleSynEnumCaseWithoutBar.fs (1,11--1,18)), [],
                  None, /root/SingleSynEnumCaseWithoutBar.fs (1,5--1,18),
                  { LeadingKeyword =
                     Type /root/SingleSynEnumCaseWithoutBar.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/SingleSynEnumCaseWithoutBar.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/SingleSynEnumCaseWithoutBar.fs (1,0--1,18))], PreXmlDocEmpty,
          [], None, /root/SingleSynEnumCaseWithoutBar.fs (1,0--1,18),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))