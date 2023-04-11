ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/UseKeyword.fs", false,
      QualifiedNameOfFile UseKeyword, [], [],
      [SynModuleOrNamespace
         ([UseKeyword], false, AnonModule,
          [Expr
             (Do
                (LetOrUse
                   (false, true,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named (SynIdent (x, None), false, None, (3,8--3,9)),
                        None,
                        App
                          (Atomic, false, Ident X, Const (Unit, (3,13--3,15)),
                           (3,12--3,15)), (3,8--3,9), Yes (3,4--3,15),
                        { LeadingKeyword = Use (3,4--3,7)
                          InlineKeyword = None
                          EqualsRange = Some (3,10--3,11) })],
                    Const (Unit, (4,4--4,6)), (3,4--4,6), { InKeyword = None }),
                 (2,0--4,6)), (2,0--4,6))], PreXmlDocEmpty, [], None, (2,0--5,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
