ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/AbstractClassProperty.fs", false,
      QualifiedNameOfFile AbstractClassProperty, [], [],
      [SynModuleOrNamespace
         ([AbstractClassProperty], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName =
                             SynLongIdent ([AbstractClass], [], [None])
                            ArgExpr = Const (Unit, (1,2--1,15))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (1,2--1,15) }]
                        Range = (1,0--1,17) }], None, [], [AbstractBase],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,17)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (2,17--2,19)), None,
                         PreXmlDoc ((2,17), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,17), { AsKeyword = None });
                      AbstractSlot
                        (SynValSig
                           ([], SynIdent (Property1, None),
                            SynValTyparDecls (None, true),
                            WithNull
                              (LongIdent (SynLongIdent ([string], [], [None])),
                               false, (3,24--3,37), { BarRange = (3,31--3,32) }),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((3,3), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (3,3--3,51),
                            { LeadingKeyword = Abstract (3,3--3,11)
                              InlineKeyword = None
                              WithKeyword = Some (3,38--3,42)
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGetSet }, (3,3--3,51),
                         { GetSetKeywords =
                            Some (GetSet ((3,43--3,46), (3,48--3,51))) })],
                     (3,3--3,51)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (2,17--2,19)), None,
                        PreXmlDoc ((2,17), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,17), { AsKeyword = None })), (1,0--3,51),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,20--2,21)
                    WithKeyword = None })], (1,0--3,51))], PreXmlDocEmpty, [],
          None, (1,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
