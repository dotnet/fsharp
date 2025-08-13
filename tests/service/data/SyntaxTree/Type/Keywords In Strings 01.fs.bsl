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
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,12--4,14)), None,
                         PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,12), { AsKeyword = None });
                      LetBindings
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
                                  ("This string contains type and module keywords",
                                   Regular, (5,18--5,65)), (5,18--5,65)),
                             (5,8--5,15), Yes (5,4--5,65),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,16--5,17) })], false, false,
                         (5,4--5,65));
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (code, None), false, None, (6,8--6,12)),
                             None,
                             Const
                               (String
                                  ("
        type Example = int
        module Sample = 
            let x = 1
    ",
                                   TripleQuote, (6,15--10,7)), (6,15--10,7)),
                             (6,8--6,12), Yes (6,4--10,7),
                             { LeadingKeyword = Let (6,4--6,7)
                               InlineKeyword = None
                               EqualsRange = Some (6,13--6,14) })], false, false,
                         (6,4--10,7));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((11,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; GetMessage], [(11,12--11,13)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (11,23--11,25)),
                                     (11,23--11,25))], None, (11,11--11,25)),
                            None, Ident message, (11,11--11,25), NoneAtInvisible,
                            { LeadingKeyword = Member (11,4--11,10)
                              InlineKeyword = None
                              EqualsRange = Some (11,26--11,27) }),
                         (11,4--11,35));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((12,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; GetCode], [(12,12--12,13)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (12,20--12,22)),
                                     (12,20--12,22))], None, (12,11--12,22)),
                            None, Ident code, (12,11--12,22), NoneAtInvisible,
                            { LeadingKeyword = Member (12,4--12,10)
                              InlineKeyword = None
                              EqualsRange = Some (12,23--12,24) }),
                         (12,4--12,29))], (5,4--12,29)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,12--4,14)), None,
                        PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,12), { AsKeyword = None })), (4,5--12,29),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,15--4,16)
                    WithKeyword = None })], (4,0--12,29))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--12,29), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,66)] }, set []))
