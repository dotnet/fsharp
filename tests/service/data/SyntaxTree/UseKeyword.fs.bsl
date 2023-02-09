ImplFile
  (ParsedImplFileInput
     ("/root/UseKeyword.fs", false, QualifiedNameOfFile UseKeyword, [], [],
      [SynModuleOrNamespace
         ([UseKeyword], false, AnonModule,
          [Expr
             (Do
                (LetOrUse
                   (false, true,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named
                          (SynIdent (x, None), false, None,
                           /root/UseKeyword.fs (2,8--2,9)), None,
                        App
                          (Atomic, false, Ident X,
                           Const (Unit, /root/UseKeyword.fs (2,13--2,15)),
                           /root/UseKeyword.fs (2,12--2,15)),
                        /root/UseKeyword.fs (2,8--2,9),
                        Yes /root/UseKeyword.fs (2,4--2,15),
                        { LeadingKeyword = Use /root/UseKeyword.fs (2,4--2,7)
                          InlineKeyword = None
                          EqualsRange = Some /root/UseKeyword.fs (2,10--2,11) })],
                    Const (Unit, /root/UseKeyword.fs (3,4--3,6)),
                    /root/UseKeyword.fs (2,4--3,6), { InKeyword = None }),
                 /root/UseKeyword.fs (1,0--3,6)), /root/UseKeyword.fs (1,0--3,6))],
          PreXmlDocEmpty, [], None, /root/UseKeyword.fs (1,0--3,6),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))