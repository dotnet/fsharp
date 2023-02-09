ImplFile
  (ParsedImplFileInput
     ("/root/ActivePatternDefinition.fs", false,
      QualifiedNameOfFile ActivePatternDefinition, [], [],
      [SynModuleOrNamespace
         ([ActivePatternDefinition], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some a)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent
                       ([|Odd|Even|], [],
                        [Some
                           (HasParenthesis
                              (/root/ActivePatternDefinition.fs (1,4--1,5),
                               /root/ActivePatternDefinition.fs (1,15--1,16)))]),
                     None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (a, None), false, None,
                                 /root/ActivePatternDefinition.fs (1,18--1,19)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              /root/ActivePatternDefinition.fs (1,18--1,24)),
                           /root/ActivePatternDefinition.fs (1,17--1,25))], None,
                     /root/ActivePatternDefinition.fs (1,4--1,25)), None,
                  IfThenElse
                    (App
                       (NonAtomic, false,
                        App
                          (NonAtomic, true,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([op_Equality], [],
                                 [Some (OriginalNotation "=")]), None,
                              /root/ActivePatternDefinition.fs (1,37--1,38)),
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, true,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([op_Modulus], [],
                                       [Some (OriginalNotation "%")]), None,
                                    /root/ActivePatternDefinition.fs (1,33--1,34)),
                                 Ident a,
                                 /root/ActivePatternDefinition.fs (1,31--1,34)),
                              Const
                                (Int32 2,
                                 /root/ActivePatternDefinition.fs (1,35--1,36)),
                              /root/ActivePatternDefinition.fs (1,31--1,36)),
                           /root/ActivePatternDefinition.fs (1,31--1,38)),
                        Const
                          (Int32 0,
                           /root/ActivePatternDefinition.fs (1,39--1,40)),
                        /root/ActivePatternDefinition.fs (1,31--1,40)),
                     Ident Even, Some (Ident Odd),
                     Yes /root/ActivePatternDefinition.fs (1,28--1,45), false,
                     /root/ActivePatternDefinition.fs (1,28--1,59),
                     { IfKeyword = /root/ActivePatternDefinition.fs (1,28--1,30)
                       IsElif = false
                       ThenKeyword =
                        /root/ActivePatternDefinition.fs (1,41--1,45)
                       ElseKeyword =
                        Some /root/ActivePatternDefinition.fs (1,51--1,55)
                       IfToThenRange =
                        /root/ActivePatternDefinition.fs (1,28--1,45) }),
                  /root/ActivePatternDefinition.fs (1,4--1,25), NoneAtLet,
                  { LeadingKeyword =
                     Let /root/ActivePatternDefinition.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some /root/ActivePatternDefinition.fs (1,26--1,27) })],
              /root/ActivePatternDefinition.fs (1,0--1,59))], PreXmlDocEmpty, [],
          None, /root/ActivePatternDefinition.fs (1,0--1,59),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))