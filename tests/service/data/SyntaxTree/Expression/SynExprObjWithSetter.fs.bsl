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
                            ArgExpr =
                             Const
                               (Unit,
                                /root/Expression/SynExprObjWithSetter.fs (2,2--2,15))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/Expression/SynExprObjWithSetter.fs (2,2--2,15) }]
                        Range =
                         /root/Expression/SynExprObjWithSetter.fs (2,0--2,17) }],
                     None, [], [CFoo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/Expression/SynExprObjWithSetter.fs (3,5--3,9)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([],
                            /root/Expression/SynExprObjWithSetter.fs (3,9--3,11)),
                         None,
                         PreXmlDoc ((3,9), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/Expression/SynExprObjWithSetter.fs (3,5--3,9),
                         { AsKeyword = None });
                      AbstractSlot
                        (SynValSig
                           ([], SynIdent (AbstractClassPropertySet, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([string], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None,
                            /root/Expression/SynExprObjWithSetter.fs (4,4--4,54),
                            { LeadingKeyword =
                               Abstract
                                 /root/Expression/SynExprObjWithSetter.fs (4,4--4,12)
                              InlineKeyword = None
                              WithKeyword =
                               Some
                                 /root/Expression/SynExprObjWithSetter.fs (4,46--4,50)
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         /root/Expression/SynExprObjWithSetter.fs (4,4--4,54),
                         { GetSetKeywords =
                            Some
                              (Set
                                 /root/Expression/SynExprObjWithSetter.fs (4,51--4,54)) })],
                     /root/Expression/SynExprObjWithSetter.fs (4,4--4,54)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([],
                           /root/Expression/SynExprObjWithSetter.fs (3,9--3,11)),
                        None,
                        PreXmlDoc ((3,9), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/Expression/SynExprObjWithSetter.fs (3,5--3,9),
                        { AsKeyword = None })),
                  /root/Expression/SynExprObjWithSetter.fs (2,0--4,54),
                  { LeadingKeyword =
                     Type /root/Expression/SynExprObjWithSetter.fs (3,0--3,4)
                    EqualsRange =
                     Some /root/Expression/SynExprObjWithSetter.fs (3,12--3,13)
                    WithKeyword = None })],
              /root/Expression/SynExprObjWithSetter.fs (2,0--4,54));
           Expr
             (ObjExpr
                (LongIdent (SynLongIdent ([CFoo], [], [None])),
                 Some
                   (Const
                      (Unit,
                       /root/Expression/SynExprObjWithSetter.fs (6,10--6,12)),
                    None),
                 Some /root/Expression/SynExprObjWithSetter.fs (6,13--6,17), [],
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
                                 [/root/Expression/SynExprObjWithSetter.fs (7,17--7,18)],
                                 [None; None]), Some set, None,
                              Pats
                                [Paren
                                   (Typed
                                      (Named
                                         (SynIdent (v, None), false, None,
                                          /root/Expression/SynExprObjWithSetter.fs (7,53--7,54)),
                                       LongIdent
                                         (SynLongIdent ([string], [], [None])),
                                       /root/Expression/SynExprObjWithSetter.fs (7,53--7,61)),
                                    /root/Expression/SynExprObjWithSetter.fs (7,52--7,62))],
                              None,
                              /root/Expression/SynExprObjWithSetter.fs (7,48--7,62)),
                           None,
                           Const
                             (Unit,
                              /root/Expression/SynExprObjWithSetter.fs (7,65--7,67)),
                           /root/Expression/SynExprObjWithSetter.fs (7,48--7,62),
                           NoneAtInvisible,
                           { LeadingKeyword =
                              Override
                                /root/Expression/SynExprObjWithSetter.fs (7,4--7,12)
                             InlineKeyword = None
                             EqualsRange =
                              Some
                                /root/Expression/SynExprObjWithSetter.fs (7,63--7,64) })),
                     /root/Expression/SynExprObjWithSetter.fs (7,4--7,67),
                     { InlineKeyword = None
                       WithKeyword =
                        /root/Expression/SynExprObjWithSetter.fs (7,43--7,47)
                       GetKeyword = None
                       AndKeyword = None
                       SetKeyword =
                        Some
                          /root/Expression/SynExprObjWithSetter.fs (7,48--7,51) })],
                 [], /root/Expression/SynExprObjWithSetter.fs (6,2--6,12),
                 /root/Expression/SynExprObjWithSetter.fs (6,0--7,69)),
              /root/Expression/SynExprObjWithSetter.fs (6,0--7,69))],
          PreXmlDocEmpty, [], None,
          /root/Expression/SynExprObjWithSetter.fs (2,0--7,69),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
