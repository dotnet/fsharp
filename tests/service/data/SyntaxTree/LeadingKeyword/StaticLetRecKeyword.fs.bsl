ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/StaticLetRecKeyword.fs", false,
      QualifiedNameOfFile StaticLetRecKeyword, [], [],
      [SynModuleOrNamespace
         ([StaticLetRecKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/LeadingKeyword/StaticLetRecKeyword.fs (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                         /root/LeadingKeyword/StaticLetRecKeyword.fs (3,27--3,29)),
                                      /root/LeadingKeyword/StaticLetRecKeyword.fs (3,27--3,29))],
                                None,
                                /root/LeadingKeyword/StaticLetRecKeyword.fs (3,19--3,29)),
                             None,
                             App
                               (Atomic, false, Ident forever,
                                Const
                                  (Unit,
                                   /root/LeadingKeyword/StaticLetRecKeyword.fs (3,39--3,41)),
                                /root/LeadingKeyword/StaticLetRecKeyword.fs (3,32--3,41)),
                             /root/LeadingKeyword/StaticLetRecKeyword.fs (3,19--3,29),
                             NoneAtLet,
                             { LeadingKeyword =
                                StaticLetRec
                                  (/root/LeadingKeyword/StaticLetRecKeyword.fs (3,4--3,10),
                                   /root/LeadingKeyword/StaticLetRecKeyword.fs (3,11--3,14),
                                   /root/LeadingKeyword/StaticLetRecKeyword.fs (3,15--3,18))
                               InlineKeyword = None
                               EqualsRange =
                                Some
                                  /root/LeadingKeyword/StaticLetRecKeyword.fs (3,30--3,31) })],
                         true, true,
                         /root/LeadingKeyword/StaticLetRecKeyword.fs (3,4--3,41))],
                     /root/LeadingKeyword/StaticLetRecKeyword.fs (3,4--3,41)),
                  [], None,
                  /root/LeadingKeyword/StaticLetRecKeyword.fs (2,5--3,41),
                  { LeadingKeyword =
                     Type /root/LeadingKeyword/StaticLetRecKeyword.fs (2,0--2,4)
                    EqualsRange =
                     Some /root/LeadingKeyword/StaticLetRecKeyword.fs (2,7--2,8)
                    WithKeyword = None })],
              /root/LeadingKeyword/StaticLetRecKeyword.fs (2,0--3,41))],
          PreXmlDocEmpty, [], None,
          /root/LeadingKeyword/StaticLetRecKeyword.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
