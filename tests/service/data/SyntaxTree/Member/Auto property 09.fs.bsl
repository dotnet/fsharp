ImplFile
  (ParsedImplFileInput
     ("/root/Member/Auto property 09.fs", false, QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [AutoProperty
                        ([], false, P1, None, PropertyGet,
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                         GetSet (None, None, None),
                         Const (Int32 1, (4,20--4,21)), (4,4--4,30),
                         { LeadingKeyword =
                            MemberVal ((4,4--4,10), (4,11--4,14))
                           WithKeyword = Some (4,22--4,26)
                           EqualsRange = Some (4,18--4,19)
                           GetSetKeywords = Some (Get (4,27--4,30)) });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([this; P2], [(5,15--5,16)], [None; None]),
                               None, None, Pats [], None, (5,11--5,18)), None,
                            Const (Int32 1, (5,21--5,22)), (5,11--5,18),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (5,4--5,10)
                              InlineKeyword = None
                              EqualsRange = Some (5,19--5,20) }), (5,4--5,22))],
                     (4,4--5,22)), [], None, (3,5--5,22),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--5,22));
           Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,4)-(5,10) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (4:23). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(5,4)-(5,10) parse error Incomplete structured construct at or before this point. Expected identifier, '(', '(*)' or other token.
