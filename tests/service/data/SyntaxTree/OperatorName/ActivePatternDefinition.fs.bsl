ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/ActivePatternDefinition.fs", false,
      QualifiedNameOfFile ActivePatternDefinition, [], [],
      [SynModuleOrNamespace
         ([ActivePatternDefinition], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some a)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent
                       ([|Odd|Even|], [],
                        [Some (HasParenthesis ((2,4--2,5), (2,15--2,16)))]),
                     None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (a, None), false, None, (2,18--2,19)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (2,18--2,24)), (2,17--2,25))], None, (2,4--2,25)),
                  None,
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
                              (2,37--2,38)),
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, true,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([op_Modulus], [],
                                       [Some (OriginalNotation "%")]), None,
                                    (2,33--2,34)), Ident a, (2,31--2,34)),
                              Const (Int32 2, (2,35--2,36)), (2,31--2,36)),
                           (2,31--2,38)), Const (Int32 0, (2,39--2,40)),
                        (2,31--2,40)), Ident Even, Some (Ident Odd),
                     Yes (2,28--2,45), false, (2,28--2,59),
                     { IfKeyword = (2,28--2,30)
                       IsElif = false
                       ThenKeyword = (2,41--2,45)
                       ElseKeyword = Some (2,51--2,55)
                       IfToThenRange = (2,28--2,45) }), (2,4--2,25), NoneAtLet,
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,26--2,27) })], (2,0--2,59))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
