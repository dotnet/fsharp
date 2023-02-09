ImplFile
  (ParsedImplFileInput
     ("/root/UseRecKeyword.fs", false, QualifiedNameOfFile UseRecKeyword, [], [],
      [SynModuleOrNamespace
         ([UseRecKeyword], false, AnonModule,
          [Expr
             (Do
                (LetOrUse
                   (true, true,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named
                          (SynIdent (x, None), false, None,
                           /root/UseRecKeyword.fs (2,12--2,13)), None,
                        App
                          (Atomic, false, Ident X,
                           Const (Unit, /root/UseRecKeyword.fs (2,17--2,19)),
                           /root/UseRecKeyword.fs (2,16--2,19)),
                        /root/UseRecKeyword.fs (2,12--2,13),
                        Yes /root/UseRecKeyword.fs (2,4--2,19),
                        { LeadingKeyword =
                           UseRec
                             (/root/UseRecKeyword.fs (2,4--2,7),
                              /root/UseRecKeyword.fs (2,8--2,11))
                          InlineKeyword = None
                          EqualsRange = Some /root/UseRecKeyword.fs (2,14--2,15) })],
                    Const (Unit, /root/UseRecKeyword.fs (3,4--3,6)),
                    /root/UseRecKeyword.fs (2,4--3,6), { InKeyword = None }),
                 /root/UseRecKeyword.fs (1,0--3,6)),
              /root/UseRecKeyword.fs (1,0--3,6))], PreXmlDocEmpty, [], None,
          /root/UseRecKeyword.fs (1,0--3,6), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))