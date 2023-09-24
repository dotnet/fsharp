T().indexed1 [ok] <- 1 !	001
T().indexed1 ok <- 2 !	002
T().indexed1 ok <- 3 !	003
T().indexed1 ok !	004
T().indexed1 ok !	005
T().indexed1 ok <- 1 !	006
T().indexed1 nok !	007
T().indexed1 ok !	008
type extensions aa1 ok !	009	001
T().indexed1 [nok] <- 1 !	010
T().indexed1 nok <- 2 !	011
T().indexed1 nok <- 3 !	012
T().indexed1 nok !	013
T().indexed1 [nok] !	014
T().indexed1 nok !	015
T().indexed1 nok_015 <- 1 !	016
T().indexed1 nok_016 <- 2 !	017
T().indexed1 ok_017 <- 1 !	018
type extension aa1 ok_018 <- 1!	019	002
