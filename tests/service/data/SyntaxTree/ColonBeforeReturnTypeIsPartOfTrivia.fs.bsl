ImplFile
  (ParsedImplFileInput
     ("/root/ColonBeforeReturnTypeIsPartOfTrivia.fs", false,
      QualifiedNameOfFile ColonBeforeReturnTypeIsPartOfTrivia, [], [],
      [SynModuleOrNamespace
         ([ColonBeforeReturnTypeIsPartOfTrivia], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some y)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([x], [], [None]), None, None,
                     Pats
                       [Named
                          (SynIdent (y, None), false, None,
                           /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,6--1,7))],
                     None,
                     /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,4--1,7)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([int], [], [None])),
                        /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,10--1,13),
                        [],
                        { ColonRange =
                           Some
                             /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,8--1,9) })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("todo", Regular,
                              /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,25--1,31)),
                           /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,25--1,31)),
                        /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,16--1,31)),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,16--1,31)),
                  /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,4--1,7),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,14--1,15) })],
              /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,0--1,31))],
          PreXmlDocEmpty, [], None,
          /root/ColonBeforeReturnTypeIsPartOfTrivia.fs (1,0--1,31),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))