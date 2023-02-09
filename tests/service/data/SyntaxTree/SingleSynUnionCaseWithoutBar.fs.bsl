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
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SingleSynUnionCaseWithoutBar.fs (2,5--2,8)),
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
                                  PreXmlDoc ((2,18), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/SingleSynUnionCaseWithoutBar.fs (2,18--2,24),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((2,11), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/SingleSynUnionCaseWithoutBar.fs (2,11--2,24),
                            { BarRange = None })],
                        /root/SingleSynUnionCaseWithoutBar.fs (2,11--2,24)),
                     /root/SingleSynUnionCaseWithoutBar.fs (2,11--2,24)), [],
                  None, /root/SingleSynUnionCaseWithoutBar.fs (2,5--2,24),
                  { LeadingKeyword =
                     Type /root/SingleSynUnionCaseWithoutBar.fs (2,0--2,4)
                    EqualsRange =
                     Some /root/SingleSynUnionCaseWithoutBar.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/SingleSynUnionCaseWithoutBar.fs (2,0--2,24))],
          PreXmlDocEmpty, [], None,
          /root/SingleSynUnionCaseWithoutBar.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))