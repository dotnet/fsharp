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
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some y)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([x], [], [None]), None, None,
                     Pats
                       [Named
                          (SynIdent (y, None), false, None,
                           /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,6--2,7))],
                     None,
                     /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,4--2,7)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([int], [], [None])),
                        /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,10--2,13),
                        [],
                        { ColonRange =
                           Some
                             /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,8--2,9) })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("todo", Regular,
                              /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,25--2,31)),
                           /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,25--2,31)),
                        /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,16--2,31)),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,16--2,31)),
                  /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,4--2,7),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,14--2,15) })],
              /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,0--2,31))],
          PreXmlDocEmpty, [], None,
          /root/Binding/ColonBeforeReturnTypeIsPartOfTrivia.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
