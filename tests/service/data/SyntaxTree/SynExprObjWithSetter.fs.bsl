ImplFile
  (ParsedImplFileInput
     ("/root/SynExprObjWithSetter.fs", false,
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
                               (Unit, /root/SynExprObjWithSetter.fs (1,2--1,15))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = /root/SynExprObjWithSetter.fs (1,2--1,15) }]
                        Range = /root/SynExprObjWithSetter.fs (1,0--1,17) }],
                     None, [], [CFoo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/SynExprObjWithSetter.fs (2,5--2,9)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([], /root/SynExprObjWithSetter.fs (2,9--2,11)), None,
                         PreXmlDoc ((2,9), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/SynExprObjWithSetter.fs (2,5--2,9),
                         { AsKeyword = None });
                      AbstractSlot
                        (SynValSig
                           ([], SynIdent (AbstractClassPropertySet, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([string], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None,
                            /root/SynExprObjWithSetter.fs (3,4--3,54),
                            { LeadingKeyword =
                               Abstract
                                 /root/SynExprObjWithSetter.fs (3,4--3,12)
                              InlineKeyword = None
                              WithKeyword =
                               Some /root/SynExprObjWithSetter.fs (3,46--3,50)
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         /root/SynExprObjWithSetter.fs (3,4--3,54),
                         { GetSetKeywords =
                            Some
                              (Set /root/SynExprObjWithSetter.fs (3,51--3,54)) })],
                     /root/SynExprObjWithSetter.fs (3,4--3,54)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([], /root/SynExprObjWithSetter.fs (2,9--2,11)), None,
                        PreXmlDoc ((2,9), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/SynExprObjWithSetter.fs (2,5--2,9),
                        { AsKeyword = None })),
                  /root/SynExprObjWithSetter.fs (1,0--3,54),
                  { LeadingKeyword =
                     Type /root/SynExprObjWithSetter.fs (2,0--2,4)
                    EqualsRange =
                     Some /root/SynExprObjWithSetter.fs (2,12--2,13)
                    WithKeyword = None })],
              /root/SynExprObjWithSetter.fs (1,0--3,54));
           Expr
             (ObjExpr
                (LongIdent (SynLongIdent ([CFoo], [], [None])),
                 Some
                   (Const (Unit, /root/SynExprObjWithSetter.fs (5,10--5,12)),
                    None), Some /root/SynExprObjWithSetter.fs (5,13--5,17), [],
                 [GetSetMember
                    (None,
                     Some
                       (SynBinding
                          (None, Normal, false, false, [],
                           PreXmlMerge
  (PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                 [/root/SynExprObjWithSetter.fs (6,17--6,18)],
                                 [None; None]), Some set, None,
                              Pats
                                [Paren
                                   (Typed
                                      (Named
                                         (SynIdent (v, None), false, None,
                                          /root/SynExprObjWithSetter.fs (6,53--6,54)),
                                       LongIdent
                                         (SynLongIdent ([string], [], [None])),
                                       /root/SynExprObjWithSetter.fs (6,53--6,61)),
                                    /root/SynExprObjWithSetter.fs (6,52--6,62))],
                              None, /root/SynExprObjWithSetter.fs (6,48--6,62)),
                           None,
                           Const
                             (Unit, /root/SynExprObjWithSetter.fs (6,65--6,67)),
                           /root/SynExprObjWithSetter.fs (6,48--6,62),
                           NoneAtInvisible,
                           { LeadingKeyword =
                              Override /root/SynExprObjWithSetter.fs (6,4--6,12)
                             InlineKeyword = None
                             EqualsRange =
                              Some /root/SynExprObjWithSetter.fs (6,63--6,64) })),
                     /root/SynExprObjWithSetter.fs (6,4--6,67),
                     { InlineKeyword = None
                       WithKeyword = /root/SynExprObjWithSetter.fs (6,43--6,47)
                       GetKeyword = None
                       AndKeyword = None
                       SetKeyword =
                        Some /root/SynExprObjWithSetter.fs (6,48--6,51) })], [],
                 /root/SynExprObjWithSetter.fs (5,2--5,12),
                 /root/SynExprObjWithSetter.fs (5,0--6,69)),
              /root/SynExprObjWithSetter.fs (5,0--6,69))], PreXmlDocEmpty, [],
          None, /root/SynExprObjWithSetter.fs (1,0--6,69),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))