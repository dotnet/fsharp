# workaround for Windows 8 issue with stream redirection
# my $dir = `cd`;
system "cd > cd.tmp";
open TMP, "cd.tmp";
$dir = <TMP>;
close TMP;

if (-e "tdirs")
{
    printf "tdirs exists in $dir, you should not be running run.pl in a non-leaf folder\n" ;
    exit 1;
}

# use build-*.bat logic

    if(-e "build.bat")
    {
        printf "calling build.bat in $dir\n" 	;
        my $res = system ("call build.bat");
        if ($res != 0)
        {
            printf "\nFAILED: call to build.bat failed" ;
            exit 1;
	}
    }
    if(-e "run.bat")
    {
        printf "calling run.bat in $dir\n" 	;
        my $res = system ("call run.bat");
        if ($res != 0)
        {
            printf "\nFAILED: call to run.bat failed" ;
            exit 1;
	}
    }
    if(-e "perf.bat")
    {
        printf "calling perf.bat in $dir\n" 	;
        my $res = system ("call perf.bat");
        if ($res != 0)
        {
            printf "\nFAILED: call to perf.bat failed" ;
            exit 1;
	}
    }
    if ((not -e "build.bat") && (not -e "run.bat") && (not -e "perf.bat"))
    {
        printf "\nFAILED: could not find any of build.bat or run.bat or perf.bat";
        exit 1;
    }
