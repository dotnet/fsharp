ImplFile
  (ParsedImplFileInput
     ("/root/Type/Keywords In Strings 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyClass],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,12)),
                  ObjectModel
                    (Unspecified,
                     [LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (message, None), false, None,
                                (5,8--5,15)), None,
                             Const
                               (String
                                  ("type and module keywords", Regular,
                                   (5,18--5,44)), (5,18--5,44)), (5,8--5,15),
                             Yes (5,4--5,44),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,16--5,17) })], false, false,
                         (5,4--5,44), { InKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([_; GetMessage], [(6,12--6,13)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (6,23--6,25)), (6,23--6,25))],
                               None, (6,11--6,25)), None, Ident message,
                            (6,11--6,25), NoneAtInvisible,
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              EqualsRange = Some (6,26--6,27) }), (6,4--6,35))],
                     (5,4--6,35)), [], None, (4,5--6,35),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,13--4,14)
                    WithKeyword = None })], (4,0--6,35))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--6,35), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,49)] }, set []))
