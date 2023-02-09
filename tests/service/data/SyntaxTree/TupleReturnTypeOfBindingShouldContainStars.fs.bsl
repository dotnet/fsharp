ImplFile
  (ParsedImplFileInput
     ("/root/TupleReturnTypeOfBindingShouldContainStars.fs", false,
      QualifiedNameOfFile TupleReturnTypeOfBindingShouldContainStars, [], [],
      [SynModuleOrNamespace
         ([TupleReturnTypeOfBindingShouldContainStars], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (a, None), false, None,
                     /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,4--1,5)),
                  Some
                    (SynBindingReturnInfo
                       (Tuple
                          (false,
                           [Type (LongIdent (SynLongIdent ([int], [], [None])));
                            Star
                              /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,12--1,13);
                            Type
                              (LongIdent (SynLongIdent ([string], [], [None])))],
                           /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,8--1,20)),
                        /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,8--1,20),
                        [],
                        { ColonRange =
                           Some
                             /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,6--1,7) })),
                  Typed
                    (App
                       (NonAtomic, false, Ident failwith,
                        Const
                          (String
                             ("todo", Regular,
                              /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,32--1,38)),
                           /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,32--1,38)),
                        /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,23--1,38)),
                     Tuple
                       (false,
                        [Type (LongIdent (SynLongIdent ([int], [], [None])));
                         Star
                           /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,12--1,13);
                         Type (LongIdent (SynLongIdent ([string], [], [None])))],
                        /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,8--1,20)),
                     /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,23--1,38)),
                  /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,4--1,5),
                  Yes
                    /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,0--1,38),
                  { LeadingKeyword =
                     Let
                       /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,21--1,22) })],
              /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,0--1,38));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (b, None), false, None,
                     /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,4--2,5)),
                  Some
                    (SynBindingReturnInfo
                       (Tuple
                          (false,
                           [Type (LongIdent (SynLongIdent ([int], [], [None])));
                            Star
                              /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,12--2,13);
                            Type
                              (LongIdent (SynLongIdent ([string], [], [None])));
                            Star
                              /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,21--2,22);
                            Type (LongIdent (SynLongIdent ([bool], [], [None])))],
                           /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,8--2,27)),
                        /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,8--2,27),
                        [],
                        { ColonRange =
                           Some
                             /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,6--2,7) })),
                  Typed
                    (Tuple
                       (false,
                        [Const
                           (Int32 1,
                            /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,30--2,31));
                         Const
                           (String
                              ("", Regular,
                               /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,33--2,35)),
                            /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,33--2,35));
                         Const
                           (Bool false,
                            /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,37--2,42))],
                        [/root/TupleReturnTypeOfBindingShouldContainStars.fs (2,31--2,32);
                         /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,35--2,36)],
                        /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,30--2,42)),
                     Tuple
                       (false,
                        [Type (LongIdent (SynLongIdent ([int], [], [None])));
                         Star
                           /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,12--2,13);
                         Type (LongIdent (SynLongIdent ([string], [], [None])));
                         Star
                           /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,21--2,22);
                         Type (LongIdent (SynLongIdent ([bool], [], [None])))],
                        /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,8--2,27)),
                     /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,30--2,42)),
                  /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,4--2,5),
                  Yes
                    /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,0--2,42),
                  { LeadingKeyword =
                     Let
                       /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,28--2,29) })],
              /root/TupleReturnTypeOfBindingShouldContainStars.fs (2,0--2,42))],
          PreXmlDocEmpty, [], None,
          /root/TupleReturnTypeOfBindingShouldContainStars.fs (1,0--2,42),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))