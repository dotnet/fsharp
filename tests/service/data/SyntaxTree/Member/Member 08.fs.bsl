ImplFile
  (ParsedImplFileInput
     ("/root/Member/Member 08.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] });
                            SynTyparDecl
                              ([], SynTypar (Measure, None, false), [],
                               { AmpersandRanges = [] })], [], (3,17--3,30))),
                     [], [INumericNorm],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (3,5--3,17)),
                  ObjectModel
                    (Unspecified,
                     [Interface
                        (App
                           (LongIdent (SynLongIdent ([INumeric], [], [None])),
                            Some (3,52--3,53),
                            [Var (SynTypar (T, None, false), (3,53--3,55))], [],
                            Some (3,55--3,56), false, (3,44--3,56)), None, None,
                         (3,34--3,56));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,35), FSharp.Compiler.Xml.XmlDocCollector),
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
                              (SynLongIdent ([Norm], [], [None]), None, None,
                               Pats [], None, (4,42--4,46)),
                            Some
                              (SynBindingReturnInfo
                                 (Fun
                                    (Var
                                       (SynTypar (T, None, false), (4,49--4,51)),
                                     Var
                                       (SynTypar (Measure, None, false),
                                        (4,55--4,63)), (4,49--4,63),
                                     { ArrowRange = (4,52--4,54) }),
                                  (4,49--4,63), [],
                                  { ColonRange = Some (4,47--4,48) })),
                            Typed
                              (ArbitraryAfterError ("memberCore2", (4,63--4,63)),
                               Fun
                                 (Var (SynTypar (T, None, false), (4,49--4,51)),
                                  Var
                                    (SynTypar (Measure, None, false),
                                     (4,55--4,63)), (4,49--4,63),
                                  { ArrowRange = (4,52--4,54) }), (4,63--4,63)),
                            (4,42--4,46), NoneAtInvisible,
                            { LeadingKeyword = Member (4,35--4,41)
                              InlineKeyword = None
                              EqualsRange = None }), (4,35--4,63))],
                     (3,34--4,63)), [], None, (3,5--4,63),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,32--3,33)
                    WithKeyword = None })], (3,0--4,63))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,63), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,0) parse error Expecting member body
