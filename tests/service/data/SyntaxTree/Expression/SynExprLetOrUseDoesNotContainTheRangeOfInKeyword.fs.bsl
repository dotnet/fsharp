ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprLetOrUseDoesNotContainTheRangeOfInKeyword.fs",
      false,
      QualifiedNameOfFile SynExprLetOrUseDoesNotContainTheRangeOfInKeyword, [],
      [],
      [SynModuleOrNamespace
         ([SynExprLetOrUseDoesNotContainTheRangeOfInKeyword], false, AnonModule,
          [Expr
             (Do
                (LetOrUse
                   (false, false,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named (SynIdent (x, None), false, None, (3,4--3,5)),
                        None, Const (Int32 1, (3,8--3,9)), (3,4--3,5),
                        Yes (3,0--3,9), { LeadingKeyword = Let (3,0--3,3)
                                          InlineKeyword = None
                                          EqualsRange = Some (3,6--3,7) })],
                    Const (Unit, (4,0--4,2)), (3,0--4,2), { InKeyword = None }),
                 (2,0--4,2)), (2,0--4,2))], PreXmlDocEmpty, [], None, (2,0--5,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
