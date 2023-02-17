ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/StaticLetKeyword.fs", false,
      QualifiedNameOfFile StaticLetKeyword, [], [],
      [SynModuleOrNamespace
         ([StaticLetKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
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
                                Pats [], None, (3,15--3,17)), None,
                             Const (Double 3.14, (3,20--3,24)), (3,15--3,17),
                             Yes (3,11--3,24),
                             { LeadingKeyword =
                                StaticLet ((3,4--3,10), (3,11--3,14))
                               InlineKeyword = None
                               EqualsRange = Some (3,18--3,19) })], true, false,
                         (3,4--3,24))], (3,4--3,24)), [], None, (2,5--3,24),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--3,24))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
