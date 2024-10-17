ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/UseRecKeyword.fs", false,
      QualifiedNameOfFile UseRecKeyword, [], [],
      [SynModuleOrNamespace
         ([UseRecKeyword], false, AnonModule,
          [Expr
             (Do
                (LetOrUse
                   (true, true,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named (SynIdent (x, None), false, None, (3,12--3,13)),
                        None,
                        App
                          (Atomic, false, Ident X, Const (Unit, (3,17--3,19)),
                           (3,16--3,19)), (3,12--3,13), Yes (3,4--3,19),
                        { LeadingKeyword = UseRec ((3,4--3,7), (3,8--3,11))
                          InlineKeyword = None
                          EqualsRange = Some (3,14--3,15) })],
                    Const (Unit, (4,4--4,6)), (3,4--4,6),
                    { LetOrUseKeyword = (3,4--3,11)
                      InKeyword = None }), (2,0--4,6)), (2,0--4,6))],
          PreXmlDocEmpty, [], None, (2,0--5,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
