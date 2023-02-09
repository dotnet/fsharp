ImplFile
  (ParsedImplFileInput
     ("/root/StaticLetKeyword.fs", false, QualifiedNameOfFile StaticLetKeyword,
      [], [],
      [SynModuleOrNamespace
         ([StaticLetKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/StaticLetKeyword.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             LongIdent
                               (SynLongIdent ([PI], [], [None]), None, None,
                                Pats [], None,
                                /root/StaticLetKeyword.fs (2,15--2,17)), None,
                             Const
                               (Double 3.14,
                                /root/StaticLetKeyword.fs (2,20--2,24)),
                             /root/StaticLetKeyword.fs (2,15--2,17),
                             Yes /root/StaticLetKeyword.fs (2,11--2,24),
                             { LeadingKeyword =
                                StaticLet
                                  (/root/StaticLetKeyword.fs (2,4--2,10),
                                   /root/StaticLetKeyword.fs (2,11--2,14))
                               InlineKeyword = None
                               EqualsRange =
                                Some /root/StaticLetKeyword.fs (2,18--2,19) })],
                         true, false, /root/StaticLetKeyword.fs (2,4--2,24))],
                     /root/StaticLetKeyword.fs (2,4--2,24)), [], None,
                  /root/StaticLetKeyword.fs (1,5--2,24),
                  { LeadingKeyword = Type /root/StaticLetKeyword.fs (1,0--1,4)
                    EqualsRange = Some /root/StaticLetKeyword.fs (1,7--1,8)
                    WithKeyword = None })],
              /root/StaticLetKeyword.fs (1,0--2,24))], PreXmlDocEmpty, [], None,
          /root/StaticLetKeyword.fs (1,0--2,24), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))