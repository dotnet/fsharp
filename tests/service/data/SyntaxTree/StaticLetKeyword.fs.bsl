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
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/StaticLetKeyword.fs (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             LongIdent
                               (SynLongIdent ([PI], [], [None]), None, None,
                                Pats [], None,
                                /root/StaticLetKeyword.fs (3,15--3,17)), None,
                             Const
                               (Double 3.14,
                                /root/StaticLetKeyword.fs (3,20--3,24)),
                             /root/StaticLetKeyword.fs (3,15--3,17),
                             Yes /root/StaticLetKeyword.fs (3,11--3,24),
                             { LeadingKeyword =
                                StaticLet
                                  (/root/StaticLetKeyword.fs (3,4--3,10),
                                   /root/StaticLetKeyword.fs (3,11--3,14))
                               InlineKeyword = None
                               EqualsRange =
                                Some /root/StaticLetKeyword.fs (3,18--3,19) })],
                         true, false, /root/StaticLetKeyword.fs (3,4--3,24))],
                     /root/StaticLetKeyword.fs (3,4--3,24)), [], None,
                  /root/StaticLetKeyword.fs (2,5--3,24),
                  { LeadingKeyword = Type /root/StaticLetKeyword.fs (2,0--2,4)
                    EqualsRange = Some /root/StaticLetKeyword.fs (2,7--2,8)
                    WithKeyword = None })],
              /root/StaticLetKeyword.fs (2,0--3,24))], PreXmlDocEmpty, [], None,
          /root/StaticLetKeyword.fs (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))