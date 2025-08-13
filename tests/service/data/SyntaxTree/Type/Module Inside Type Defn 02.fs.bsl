ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 02.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [C],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (3,7--3,9)), None,
                         PreXmlDoc ((3,7), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; F], [(4,12--4,13)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (4,15--4,17)), (4,15--4,17))],
                               None, (4,11--4,17)), None,
                            Const (Int32 3, (4,20--4,21)), (4,11--4,17),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (4,4--4,10)
                              InlineKeyword = None
                              EqualsRange = Some (4,18--4,19) }), (4,4--4,21))],
                     (4,4--4,21)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (3,7--3,9)), None,
                        PreXmlDoc ((3,7), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--4,21),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,10--3,11)
                    WithKeyword = None })], (3,0--4,21));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M2],
                 PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (5,4--5,13)), false,
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
                                    EqualsRange = Some (5,14--5,15) })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,4)-(5,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
