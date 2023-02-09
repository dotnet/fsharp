ImplFile
  (ParsedImplFileInput
     ("/root/AndKeyword.fs", false, QualifiedNameOfFile AndKeyword, [], [],
      [SynModuleOrNamespace
         ([AndKeyword], false, AnonModule,
          [Let
             (true,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
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
                           /root/AndKeyword.fs (2,10--2,11))], None,
                     /root/AndKeyword.fs (2,8--2,11)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/AndKeyword.fs (2,16--2,17)), Ident b,
                        /root/AndKeyword.fs (2,14--2,17)),
                     Const (Int32 1, /root/AndKeyword.fs (2,18--2,19)),
                     /root/AndKeyword.fs (2,14--2,19)),
                  /root/AndKeyword.fs (2,8--2,11), NoneAtLet,
                  { LeadingKeyword =
                     LetRec
                       (/root/AndKeyword.fs (2,0--2,3),
                        /root/AndKeyword.fs (2,4--2,7))
                    InlineKeyword = None
                    EqualsRange = Some /root/AndKeyword.fs (2,12--2,13) });
               SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                           /root/AndKeyword.fs (3,6--3,7))], None,
                     /root/AndKeyword.fs (3,4--3,7)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/AndKeyword.fs (3,12--3,13)), Ident e,
                        /root/AndKeyword.fs (3,10--3,13)),
                     Const (Int32 1, /root/AndKeyword.fs (3,14--3,15)),
                     /root/AndKeyword.fs (3,10--3,15)),
                  /root/AndKeyword.fs (3,4--3,7), NoneAtLet,
                  { LeadingKeyword = And /root/AndKeyword.fs (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some /root/AndKeyword.fs (3,8--3,9) })],
              /root/AndKeyword.fs (2,0--3,15))], PreXmlDocEmpty, [], None,
          /root/AndKeyword.fs (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))