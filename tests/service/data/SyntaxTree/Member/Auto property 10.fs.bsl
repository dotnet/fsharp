ImplFile
  (ParsedImplFileInput
     ("/root/Member/Auto property 10.fs", false, QualifiedNameOfFile Module, [],
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
                        ([], false, P, None, PropertyGet,
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
                         None, None, None, Const (Int32 1, (4,19--4,20)),
                         (4,4--4,29),
                         { LeadingKeyword =
                            MemberVal ((4,4--4,10), (4,11--4,14))
                           WithKeyword = Some (4,21--4,25)
                           EqualsRange = Some (4,17--4,18)
                           GetSetKeywords = Some (Get (4,26--4,29)) })],
                     (4,4--4,29)), [], None, (3,5--4,29),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,29))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,29), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,0) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (4:22). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(5,0)-(5,0) parse error Incomplete structured construct at or before this point. Expected identifier, '(', '(*)' or other token.
