ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprLetOrUseWithAndBangIn 01.fs", false,
      QualifiedNameOfFile SynExprLetOrUseWithAndBangIn 01, [],
      [SynModuleOrNamespace
         ([SynExprLetOrUseWithAndBangIn 01], false, AnonModule,
          [Expr
             (App
                (NonAtomic, false, Ident comp,
                 ComputationExpr
                   (false,
                    LetOrUse
                      { IsRecursive = false
                        Bindings =
                         [SynBinding
                            (None, Normal, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (a, None), false, None, (2,9--2,10)),
                             None, Ident b, (2,4--2,14), Yes (2,4--2,14),
                             { LeadingKeyword = LetBang (2,4--2,8)
                               InlineKeyword = None
                               EqualsRange = Some (2,11--2,12) });
                          SynBinding
                            (None, Normal, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (c, None), false, None, (3,9--3,10)),
                             None, Ident d, (3,4--3,14), Yes (3,4--3,14),
                             { LeadingKeyword = AndBang (3,4--3,8)
                               InlineKeyword = None
                               EqualsRange = Some (3,11--3,12) });
                          SynBinding
                            (None, Normal, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (e, None), false, None, (4,9--4,10)),
                             None, Ident f, (4,4--4,14), Yes (4,4--4,14),
                             { LeadingKeyword = AndBang (4,4--4,8)
                               InlineKeyword = None
                               EqualsRange = Some (4,11--4,12) })]
                        Body = Const (Unit, (5,4--5,6))
                        Range = (2,4--5,6)
                        Trivia = { InKeyword = Some (2,15--2,17) }
                        IsFromSource = true }, (1,5--6,1)), (1,0--6,1)),
              (1,0--6,1))], PreXmlDocEmpty, [], None, (1,0--6,1),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'SynExprLetOrUseWithAndBangIn 01' based on the file name 'SynExprLetOrUseWithAndBangIn 01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
