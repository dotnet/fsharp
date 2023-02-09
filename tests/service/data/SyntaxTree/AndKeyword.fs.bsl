ImplFile
  (ParsedImplFileInput
     ("/root/AndKeyword.fs", false, QualifiedNameOfFile AndKeyword, [], [],
      [SynModuleOrNamespace
         ([AndKeyword], false, AnonModule,
          [Let
             (true,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some b)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([a], [], [None]), None, None,
                     Pats
                       [Named
                          (SynIdent (b, None), false, None,
                           /root/AndKeyword.fs (1,10--1,11))], None,
                     /root/AndKeyword.fs (1,8--1,11)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/AndKeyword.fs (1,16--1,17)), Ident b,
                        /root/AndKeyword.fs (1,14--1,17)),
                     Const (Int32 1, /root/AndKeyword.fs (1,18--1,19)),
                     /root/AndKeyword.fs (1,14--1,19)),
                  /root/AndKeyword.fs (1,8--1,11), NoneAtLet,
                  { LeadingKeyword =
                     LetRec
                       (/root/AndKeyword.fs (1,0--1,3),
                        /root/AndKeyword.fs (1,4--1,7))
                    InlineKeyword = None
                    EqualsRange = Some /root/AndKeyword.fs (1,12--1,13) });
               SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some e)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([d], [], [None]), None, None,
                     Pats
                       [Named
                          (SynIdent (e, None), false, None,
                           /root/AndKeyword.fs (2,6--2,7))], None,
                     /root/AndKeyword.fs (2,4--2,7)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/AndKeyword.fs (2,12--2,13)), Ident e,
                        /root/AndKeyword.fs (2,10--2,13)),
                     Const (Int32 1, /root/AndKeyword.fs (2,14--2,15)),
                     /root/AndKeyword.fs (2,10--2,15)),
                  /root/AndKeyword.fs (2,4--2,7), NoneAtLet,
                  { LeadingKeyword = And /root/AndKeyword.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some /root/AndKeyword.fs (2,8--2,9) })],
              /root/AndKeyword.fs (1,0--2,15))], PreXmlDocEmpty, [], None,
          /root/AndKeyword.fs (1,0--2,15), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))