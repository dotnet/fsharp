ImplFile
  (ParsedImplFileInput
     ("/root/SynTypeOrWithAppTypeOnTheRightHandSide.fs", false,
      QualifiedNameOfFile SynTypeOrWithAppTypeOnTheRightHandSide, [], [],
      [SynModuleOrNamespace
         ([SynTypeOrWithAppTypeOnTheRightHandSide], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
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
                                (SynIdent (x, None), false, None,
                                 /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,14--1,15)),
                              Var
                                (SynTypar (T, None, false),
                                 /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,17--1,19)),
                              /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,14--1,19)),
                           /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,13--1,20))],
                     None,
                     /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,11--1,20)),
                  None,
                  Paren
                    (TraitCall
                       (Paren
                          (Or
                             (Var
                                (SynTypar (T, HeadType, false),
                                 /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,25--1,27)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,25--1,34),
                              { OrKeyword =
                                 /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,28--1,30) }),
                           /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,24--1,35)),
                        Member
                          (SynValSig
                             ([], SynIdent (A, None),
                              SynValTyparDecls (None, true),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              false, false,
                              PreXmlDoc ((1,39), FSharp.Compiler.Xml.XmlDocCollector),
                              None, None,
                              /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,39--1,59),
                              { LeadingKeyword =
                                 StaticMember
                                   (/root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,39--1,45),
                                    /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,46--1,52))
                                InlineKeyword = None
                                WithKeyword = None
                                EqualsRange = None }),
                           { IsInstance = false
                             IsDispatchSlot = false
                             IsOverrideOrExplicitImpl = false
                             IsFinal = false
                             GetterOrSetterIsCompilerGenerated = false
                             MemberKind = PropertyGet },
                           /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,39--1,59),
                           { GetSetKeywords = None }),
                        Const
                          (Unit,
                           /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,61--1,63)),
                        /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,23--1,64)),
                     /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,23--1,24),
                     Some
                       /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,63--1,64),
                     /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,23--1,64)),
                  /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,11--1,20),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,0--1,3)
                    InlineKeyword =
                     Some
                       /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,4--1,10)
                    EqualsRange =
                     Some
                       /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,21--1,22) })],
              /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,0--1,64))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (1,0--1,64),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))