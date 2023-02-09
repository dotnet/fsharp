ImplFile
  (ParsedImplFileInput
     ("/root/StaticLetRecKeyword.fs", false,
      QualifiedNameOfFile StaticLetRecKeyword, [], [],
      [SynModuleOrNamespace
         ([StaticLetRecKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/StaticLetRecKeyword.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([[]], SynArgInfo ([], false, None)),
                                None),
                             LongIdent
                               (SynLongIdent ([forever], [], [None]), None, None,
                                Pats
                                  [Paren
                                     (Const
                                        (Unit,
                                         /root/StaticLetRecKeyword.fs (2,27--2,29)),
                                      /root/StaticLetRecKeyword.fs (2,27--2,29))],
                                None, /root/StaticLetRecKeyword.fs (2,19--2,29)),
                             None,
                             App
                               (Atomic, false, Ident forever,
                                Const
                                  (Unit,
                                   /root/StaticLetRecKeyword.fs (2,39--2,41)),
                                /root/StaticLetRecKeyword.fs (2,32--2,41)),
                             /root/StaticLetRecKeyword.fs (2,19--2,29),
                             NoneAtLet,
                             { LeadingKeyword =
                                StaticLetRec
                                  (/root/StaticLetRecKeyword.fs (2,4--2,10),
                                   /root/StaticLetRecKeyword.fs (2,11--2,14),
                                   /root/StaticLetRecKeyword.fs (2,15--2,18))
                               InlineKeyword = None
                               EqualsRange =
                                Some /root/StaticLetRecKeyword.fs (2,30--2,31) })],
                         true, true, /root/StaticLetRecKeyword.fs (2,4--2,41))],
                     /root/StaticLetRecKeyword.fs (2,4--2,41)), [], None,
                  /root/StaticLetRecKeyword.fs (1,5--2,41),
                  { LeadingKeyword =
                     Type /root/StaticLetRecKeyword.fs (1,0--1,4)
                    EqualsRange = Some /root/StaticLetRecKeyword.fs (1,7--1,8)
                    WithKeyword = None })],
              /root/StaticLetRecKeyword.fs (1,0--2,41))], PreXmlDocEmpty, [],
          None, /root/StaticLetRecKeyword.fs (1,0--2,41),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))