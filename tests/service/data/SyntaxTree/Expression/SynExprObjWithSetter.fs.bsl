ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprObjWithSetter.fs", false,
      QualifiedNameOfFile SynExprObjWithSetter, [], [],
      [SynModuleOrNamespace
         ([SynExprObjWithSetter], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName =
                             SynLongIdent ([AbstractClass], [], [None])
                            ArgExpr = Const (Unit, (2,2--2,15))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (2,2--2,15) }]
                        Range = (2,0--2,17) }], None, [], [CFoo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,9)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], SimplePats ([], (3,9--3,11)), None,
                         PreXmlDoc ((3,9), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,9), { AsKeyword = None });
                      AbstractSlot
                        (SynValSig
                           ([], SynIdent (AbstractClassPropertySet, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([string], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None, (4,4--4,54),
                            { LeadingKeyword = Abstract (4,4--4,12)
                              InlineKeyword = None
                              WithKeyword = Some (4,46--4,50)
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet }, (4,4--4,54),
                         { GetSetKeywords = Some (Set (4,51--4,54)) })],
                     (4,4--4,54)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], (3,9--3,11)), None,
                        PreXmlDoc ((3,9), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,9), { AsKeyword = None })), (2,0--4,54),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,12--3,13)
                    WithKeyword = None })], (2,0--4,54));
           Expr
             (ObjExpr
                (LongIdent (SynLongIdent ([CFoo], [], [None])),
                 Some (Const (Unit, (6,10--6,12)), None), Some (6,13--6,17), [],
                 [GetSetMember
                    (None,
                     Some
                       (SynBinding
                          (None, Normal, false, false, [],
                           PreXmlMerge
  (PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                           SynValData
                             (Some { IsInstance = true
                                     IsDispatchSlot = false
                                     IsOverrideOrExplicitImpl = true
                                     IsFinal = false
                                     GetterOrSetterIsCompilerGenerated = false
                                     MemberKind = PropertySet },
                              SynValInfo
                                ([[SynArgInfo ([], false, None)];
                                  [SynArgInfo ([], false, Some v)]],
                                 SynArgInfo ([], false, None)), None),
                           LongIdent
                             (SynLongIdent
                                ([this; AbstractClassPropertySet],
                                 [(7,17--7,18)], [None; None]), Some set, None,
                              Pats
                                [Paren
                                   (Typed
                                      (Named
                                         (SynIdent (v, None), false, None,
                                          (7,53--7,54)),
                                       LongIdent
                                         (SynLongIdent ([string], [], [None])),
                                       (7,53--7,61)), (7,52--7,62))], None,
                              (7,48--7,62)), None, Const (Unit, (7,65--7,67)),
                           (7,48--7,62), NoneAtInvisible,
                           { LeadingKeyword = Override (7,4--7,12)
                             InlineKeyword = None
                             EqualsRange = Some (7,63--7,64) })), (7,4--7,67),
                     { InlineKeyword = None
                       WithKeyword = (7,43--7,47)
                       GetKeyword = None
                       AndKeyword = None
                       SetKeyword = Some (7,48--7,51) })], [], (6,2--6,12),
                 (6,0--7,69)), (6,0--7,69))], PreXmlDocEmpty, [], None,
          (2,0--7,69), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
