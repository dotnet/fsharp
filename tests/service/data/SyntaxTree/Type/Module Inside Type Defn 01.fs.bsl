ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [IFace],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,10)),
                  ObjectModel
                    (Unspecified,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (F, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (LongIdent (SynLongIdent ([int], [], [None])),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (4,17--4,27), { ArrowRange = (4,21--4,23) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (4,4--4,27),
                            { LeadingKeyword = Abstract (4,4--4,12)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (4,4--4,27),
                         { GetSetKeywords = None })], (4,4--4,27)), [], None,
                  (3,5--4,27), { LeadingKeyword = Type (3,0--3,4)
                                 EqualsRange = Some (3,11--3,12)
                                 WithKeyword = None })], (3,0--4,27));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M],
                 PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (5,4--5,12)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([[]], SynArgInfo ([], false, None)),
                         None),
                      LongIdent
                        (SynLongIdent ([f], [], [None]), None, None,
                         Pats [Paren (Const (Unit, (6,14--6,16)), (6,14--6,16))],
                         None, (6,12--6,16)), None, Const (Unit, (6,19--6,21)),
                      (6,12--6,16), NoneAtLet,
                      { LeadingKeyword = Let (6,8--6,11)
                        InlineKeyword = None
                        EqualsRange = Some (6,17--6,18) })], (6,8--6,21))],
              false, (5,4--6,21), { ModuleKeyword = Some (5,4--5,10)
                                    EqualsRange = Some (5,13--5,14) })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,4)-(5,10) parse warning Invalid declaration syntax
