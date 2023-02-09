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
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named
                          (SynIdent (x, None), false, None,
                           /root/UseRecKeyword.fs (3,12--3,13)), None,
                        App
                          (Atomic, false, Ident X,
                           Const (Unit, /root/UseRecKeyword.fs (3,17--3,19)),
                           /root/UseRecKeyword.fs (3,16--3,19)),
                        /root/UseRecKeyword.fs (3,12--3,13),
                        Yes /root/UseRecKeyword.fs (3,4--3,19),
                        { LeadingKeyword =
                           UseRec
                             (/root/UseRecKeyword.fs (3,4--3,7),
                              /root/UseRecKeyword.fs (3,8--3,11))
                          InlineKeyword = None
                          EqualsRange = Some /root/UseRecKeyword.fs (3,14--3,15) })],
                    Const (Unit, /root/UseRecKeyword.fs (4,4--4,6)),
                    /root/UseRecKeyword.fs (3,4--4,6), { InKeyword = None }),
                 /root/UseRecKeyword.fs (2,0--4,6)),
              /root/UseRecKeyword.fs (2,0--4,6))], PreXmlDocEmpty, [], None,
          /root/UseRecKeyword.fs (2,0--5,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))