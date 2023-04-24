ImplFile
  (ParsedImplFileInput
     ("/root/SynType/SynTypeOrWithAppTypeOnTheRightHandSide.fs", false,
      QualifiedNameOfFile SynTypeOrWithAppTypeOnTheRightHandSide, [], [],
      [SynModuleOrNamespace
         ([SynTypeOrWithAppTypeOnTheRightHandSide], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None, (2,14--2,15)),
                              Var (SynTypar (T, None, false), (2,17--2,19)),
                              (2,14--2,19)), (2,13--2,20))], None, (2,11--2,20)),
                  None,
                  Paren
                    (TraitCall
                       (Paren
                          (Or
                             (Var (SynTypar (T, HeadType, false), (2,25--2,27)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (2,25--2,34), { OrKeyword = (2,28--2,30) }),
                           (2,24--2,35)),
                        Member
                          (SynValSig
                             ([], SynIdent (A, None),
                              SynValTyparDecls (None, true),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              false, false,
                              PreXmlDoc ((2,39), FSharp.Compiler.Xml.XmlDocCollector),
                              None, None, (2,39--2,59),
                              { LeadingKeyword =
                                 StaticMember ((2,39--2,45), (2,46--2,52))
                                InlineKeyword = None
                                WithKeyword = None
                                EqualsRange = None }),
                           { IsInstance = false
                             IsDispatchSlot = false
                             IsOverrideOrExplicitImpl = false
                             IsFinal = false
                             GetterOrSetterIsCompilerGenerated = false
                             MemberKind = PropertyGet }, (2,39--2,59),
                           { GetSetKeywords = None }),
                        Const (Unit, (2,61--2,63)), (2,23--2,64)), (2,23--2,24),
                     Some (2,63--2,64), (2,23--2,64)), (2,11--2,20), NoneAtLet,
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = Some (2,4--2,10)
                    EqualsRange = Some (2,21--2,22) })], (2,0--2,64))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
