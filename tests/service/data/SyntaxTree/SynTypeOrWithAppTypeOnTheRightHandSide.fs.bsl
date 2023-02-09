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
                                (SynIdent (x, None), false, None,
                                 /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,14--2,15)),
                              Var
                                (SynTypar (T, None, false),
                                 /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,17--2,19)),
                              /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,14--2,19)),
                           /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,13--2,20))],
                     None,
                     /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,11--2,20)),
                  None,
                  Paren
                    (TraitCall
                       (Paren
                          (Or
                             (Var
                                (SynTypar (T, HeadType, false),
                                 /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,25--2,27)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,25--2,34),
                              { OrKeyword =
                                 /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,28--2,30) }),
                           /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,24--2,35)),
                        Member
                          (SynValSig
                             ([], SynIdent (A, None),
                              SynValTyparDecls (None, true),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              false, false,
                              PreXmlDoc ((2,39), FSharp.Compiler.Xml.XmlDocCollector),
                              None, None,
                              /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,39--2,59),
                              { LeadingKeyword =
                                 StaticMember
                                   (/root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,39--2,45),
                                    /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,46--2,52))
                                InlineKeyword = None
                                WithKeyword = None
                                EqualsRange = None }),
                           { IsInstance = false
                             IsDispatchSlot = false
                             IsOverrideOrExplicitImpl = false
                             IsFinal = false
                             GetterOrSetterIsCompilerGenerated = false
                             MemberKind = PropertyGet },
                           /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,39--2,59),
                           { GetSetKeywords = None }),
                        Const
                          (Unit,
                           /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,61--2,63)),
                        /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,23--2,64)),
                     /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,23--2,24),
                     Some
                       /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,63--2,64),
                     /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,23--2,64)),
                  /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,11--2,20),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,0--2,3)
                    InlineKeyword =
                     Some
                       /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,4--2,10)
                    EqualsRange =
                     Some
                       /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,21--2,22) })],
              /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,0--2,64))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeOrWithAppTypeOnTheRightHandSide.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))