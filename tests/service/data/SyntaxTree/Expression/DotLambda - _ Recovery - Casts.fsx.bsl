ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda - _ Recovery - Casts.fsx", true,
      QualifiedNameOfFile DotLambda - _ Recovery - Casts$fsx, [],
      [SynModuleOrNamespace
         ([DotLambda - _ Recovery - Casts], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (1,4--1,5)), None,
                  Typed
                    (Paren
                       (InferredUpcast
                          (FromParseError (Ident _, (1,17--1,18)), (1,10--1,18)),
                        (1,8--1,9), Some (1,19--1,20), (1,8--1,20)),
                     LongIdent (SynLongIdent ([obj], [], [None])), (1,8--1,26)),
                  (1,4--1,5), Yes (1,0--1,26), { LeadingKeyword = Let (1,0--1,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (1,6--1,7) })],
              (1,0--1,26));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (b, None), false, None, (2,4--2,5)), None,
                  Typed
                    (Paren
                       (Upcast
                          (FromParseError (Ident _, (2,10--2,11)),
                           Anon (2,15--2,16), (2,10--2,16)), (2,8--2,9),
                        Some (2,17--2,18), (2,8--2,18)),
                     LongIdent (SynLongIdent ([obj], [], [None])), (2,8--2,24)),
                  (2,4--2,5), Yes (2,0--2,24), { LeadingKeyword = Let (2,0--2,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (2,6--2,7) })],
              (2,0--2,24));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (c, None), false, None, (3,4--3,5)), None,
                  Paren
                    (Upcast
                       (FromParseError (Ident _, (3,10--3,11)),
                        LongIdent (SynLongIdent ([obj], [], [None])),
                        (3,10--3,18)), (3,8--3,9), Some (3,18--3,19),
                     (3,8--3,19)), (3,4--3,5), Yes (3,0--3,19),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--3,19))],
          PreXmlDocEmpty, [], None, (1,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))

(1,19)-(1,20) parse error Unexpected symbol ')' in expression. Expected '.' or other token.
(2,12)-(2,14) parse error Unexpected symbol ':>' in expression. Expected '.' or other token.
(3,12)-(3,14) parse error Unexpected symbol ':>' in expression. Expected '.' or other token.
