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
                        Named
                          (SynIdent (x, None), false, None,
                           /root/LeadingKeyword/UseKeyword.fs (3,8--3,9)), None,
                        App
                          (Atomic, false, Ident X,
                           Const
                             (Unit,
                              /root/LeadingKeyword/UseKeyword.fs (3,13--3,15)),
                           /root/LeadingKeyword/UseKeyword.fs (3,12--3,15)),
                        /root/LeadingKeyword/UseKeyword.fs (3,8--3,9),
                        Yes /root/LeadingKeyword/UseKeyword.fs (3,4--3,15),
                        { LeadingKeyword =
                           Use /root/LeadingKeyword/UseKeyword.fs (3,4--3,7)
                          InlineKeyword = None
                          EqualsRange =
                           Some /root/LeadingKeyword/UseKeyword.fs (3,10--3,11) })],
                    Const (Unit, /root/LeadingKeyword/UseKeyword.fs (4,4--4,6)),
                    /root/LeadingKeyword/UseKeyword.fs (3,4--4,6),
                    { InKeyword = None }),
                 /root/LeadingKeyword/UseKeyword.fs (2,0--4,6)),
              /root/LeadingKeyword/UseKeyword.fs (2,0--4,6))], PreXmlDocEmpty,
          [], None, /root/LeadingKeyword/UseKeyword.fs (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
