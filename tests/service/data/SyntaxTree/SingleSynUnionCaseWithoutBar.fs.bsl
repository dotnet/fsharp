ImplFile
  (ParsedImplFileInput
     ("/root/SingleSynUnionCaseWithoutBar.fs", false,
      QualifiedNameOfFile SingleSynUnionCaseWithoutBar, [], [],
      [SynModuleOrNamespace
         ([SingleSynUnionCaseWithoutBar], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SingleSynUnionCaseWithoutBar.fs (1,5--1,8)),
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
                                  PreXmlDoc ((1,18), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/SingleSynUnionCaseWithoutBar.fs (1,18--1,24),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((1,11), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/SingleSynUnionCaseWithoutBar.fs (1,11--1,24),
                            { BarRange = None })],
                        /root/SingleSynUnionCaseWithoutBar.fs (1,11--1,24)),
                     /root/SingleSynUnionCaseWithoutBar.fs (1,11--1,24)), [],
                  None, /root/SingleSynUnionCaseWithoutBar.fs (1,5--1,24),
                  { LeadingKeyword =
                     Type /root/SingleSynUnionCaseWithoutBar.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/SingleSynUnionCaseWithoutBar.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/SingleSynUnionCaseWithoutBar.fs (1,0--1,24))],
          PreXmlDocEmpty, [], None,
          /root/SingleSynUnionCaseWithoutBar.fs (1,0--1,24),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))