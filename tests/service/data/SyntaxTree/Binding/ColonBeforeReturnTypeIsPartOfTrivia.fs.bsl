ImplFile
  (ParsedImplFileInput
     ("/root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs", false,
      QualifiedNameOfFile ColonBeforeReturnTypeIsPartOfTrivia, [], [],
      [SynModuleOrNamespace
         ([ColonBeforeReturnTypeIsPartOfTrivia], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Typed
                    (LongIdent
                       (SynLongIdent ([x], [], [None]), None, None,
                        Pats
                          [Named (SynIdent (y, None), false, None, (2,6--2,7))],
                        None, (2,4--2,7)),
                     LongIdent (SynLongIdent ([int], [], [None])), (2,4--2,13)),
                  None,
                  App
                    (NonAtomic, false, Ident failwith,
                     Const
                       (String ("todo", Regular, (2,25--2,31)), (2,25--2,31)),
                     (2,16--2,31)), (2,4--2,13), Yes (2,0--2,31),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,14--2,15) })], (2,0--2,31))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
