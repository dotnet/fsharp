ImplFile
  (ParsedImplFileInput
     ("/root/Type/Let Inside Class 01.fs", false, QualifiedNameOfFile Module, [],
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
                               (SynIdent (privateField, None), false, None,
                                (5,8--5,20)), None,
                             Const (Int32 10, (5,23--5,25)), (5,8--5,20),
                             Yes (5,4--5,25),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,21--5,22) })], false, false,
                         (5,4--5,25), { InKeyword = None });
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
                                 ([_; GetValue], [(6,12--6,13)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (6,21--6,23)), (6,21--6,23))],
                               None, (6,11--6,23)), None, Ident privateField,
                            (6,11--6,23), NoneAtInvisible,
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              EqualsRange = Some (6,24--6,25) }), (6,4--6,38))],
                     (5,4--6,38)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,12--4,14)), None,
                        PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,12), { AsKeyword = None })), (4,5--6,38),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,15--4,16)
                    WithKeyword = None })], (4,0--6,38))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--6,38), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,45)] }, set []))
