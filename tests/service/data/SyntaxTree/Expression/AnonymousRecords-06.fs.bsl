ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonymousRecords-06.fs", false,
      QualifiedNameOfFile AnonymousRecords-06, [], [],
      [SynModuleOrNamespace
         ([AnonymousRecords-06], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats [Named (SynIdent (x, None), false, None, (1,6--1,7))],
                     None, (1,4--1,7)), None,
                  AnonRecd
                    (false, Some (Ident x, ((1,15--1,19), None)),
                     [(SynLongIdent ([R; D], [(1,21--1,22)], [None; None]),
                       Some (1,24--1,25),
                       Const (String ("s", Regular, (1,26--1,29)), (1,26--1,29)));
                      (SynLongIdent ([A], [], [None]), Some (1,33--1,34),
                       Const (Int32 3, (1,35--1,36)))], (1,10--1,39),
                     { OpeningBraceRange = (1,10--1,12) }), (1,4--1,7),
                  NoneAtLet, { LeadingKeyword = Let (1,0--1,3)
                               InlineKeyword = None
                               EqualsRange = Some (1,8--1,9) })], (1,0--1,39))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'AnonymousRecords-06' based on the file name 'AnonymousRecords-06.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
