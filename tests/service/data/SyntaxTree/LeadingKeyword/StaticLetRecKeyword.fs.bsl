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
                     false, None, (2,5--2,6)),
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
                                     (Const (Unit, (3,27--3,29)), (3,27--3,29))],
                                None, (3,19--3,29)), None,
                             App
                               (Atomic, false, Ident forever,
                                Const (Unit, (3,39--3,41)), (3,32--3,41)),
                             (3,19--3,29), NoneAtLet,
                             { LeadingKeyword =
                                StaticLetRec
                                  ((3,4--3,10), (3,11--3,14), (3,15--3,18))
                               InlineKeyword = None
                               EqualsRange = Some (3,30--3,31) })], true, true,
                         (3,4--3,41))], (3,4--3,41)), [], None, (2,5--3,41),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--3,41))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
