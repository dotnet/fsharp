ImplFile
  (ParsedImplFileInput
     ("/root/Type/Abstract Class With Invalid Constructs 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName =
                             SynLongIdent ([AbstractClass], [], [None])
                            ArgExpr = Const (Unit, (4,2--4,15))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (4,2--4,15) }]
                        Range = (4,0--4,17) }], None, [], [AbstractBase],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,5--5,17)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (5,17--5,19)), None,
                         PreXmlDoc ((5,17), FSharp.Compiler.Xml.XmlDocCollector),
                         (5,5--5,17), { AsKeyword = None });
                      AbstractSlot
                        (SynValSig
                           ([], SynIdent (Method, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (LongIdent (SynLongIdent ([int], [], [None])),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (6,29--6,39), { ArrowRange = (6,33--6,35) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (6,4--6,39),
                            { LeadingKeyword =
                               AbstractMember ((6,4--6,12), (6,13--6,19))
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (6,4--6,39),
                         { GetSetKeywords = None })], (6,4--6,39)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (5,17--5,19)), None,
                        PreXmlDoc ((5,17), FSharp.Compiler.Xml.XmlDocCollector),
                        (5,5--5,17), { AsKeyword = None })), (4,0--6,39),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,20--5,21)
                    WithKeyword = None })], (4,0--6,39));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (8,4--8,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((9,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (helper, None), false, None, (9,12--9,18)),
                      None, Const (Int32 10, (9,21--9,23)), (9,12--9,18),
                      Yes (9,8--9,23), { LeadingKeyword = Let (9,8--9,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (9,19--9,20) })],
                  (9,8--9,23))], false, (8,4--9,23),
              { ModuleKeyword = Some (8,4--8,10)
                EqualsRange = Some (8,25--8,26) });
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [InvalidType],
                     PreXmlDoc ((11,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (11,9--11,20)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([string], [], [None])),
                        (11,23--11,29)), (11,23--11,29)), [], None,
                  (11,9--11,29), { LeadingKeyword = Type (11,4--11,8)
                                   EqualsRange = Some (11,21--11,22)
                                   WithKeyword = None })], (11,4--11,29))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--11,29), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,50)] }, set []))

(8,4)-(8,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(11,4)-(11,8) parse warning Nested type definitions are not allowed. Types must be defined at module or namespace level.
(13,4)-(13,11) parse error Unexpected keyword 'default' in definition. Expected incomplete structured construct at or before this point or other token.
