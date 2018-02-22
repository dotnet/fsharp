import jobs.generation.Utilities;
import jobs.generation.JobReport;

def project = GithubProject
def branch = GithubBranchName

def osList = ['Windows_NT', 'Ubuntu14.04']  //, 'OSX'], 'CentOS7.1'

def static getBuildJobName(def configuration, def os) {
    return configuration.toLowerCase() + '_' + os.toLowerCase()
}

[true, false].each { isPullRequest ->
    osList.each { os ->
        def configurations = [];
        if (os == 'Windows_NT') {
            configurations = ['Debug_default', 'Release_ci_part1', 'Release_ci_part2', 'Release_ci_part3', 'Release_ci_part4', 'Release_net40_no_vs', 'Release_fcs' ];
        }
        else
        {
            // Linux
            // TODO: It should be possible to enable these configurations immediately subsequent to the PR containing this line
            //configurations = ['Debug_default', 'Release_net40_test', 'Release_fcs' ];
            configurations = [ 'Release_default', 'Release_fcs' ];
        }
        
        configurations.each { configuration ->

            def jobName = getBuildJobName(configuration, os)
            def buildCommand = '';
            def buildOutput= '';
            def buildArgs= '';

            if (configuration == "Release_fcs" && branch != "dev15.5") {
                // Build and test FCS NuGet package
                buildOutput = "Release"
                if (os == 'Windows_NT') {
                    buildCommand = ".\\fcs\\build.cmd TestAndNuget"
                }
                else {
                    buildCommand = "./fcs/build.sh Build"
                }
            }
            else if (configuration == "Debug_default") {
                buildOutput = "Debug"
                if (os == 'Windows_NT') {
                    buildCommand = "build.cmd debug"
                }
                else {
                    buildCommand = "make Configuration=Debug"
                }
            }
            else if (configuration == "Release_default") {
                buildOutput = "Release"
                if (os == 'Windows_NT') {
                    buildCommand = "build.cmd release"
                }
                else {
                    buildCommand = "make Configuration=Release"
                }
            }
            else if (configuration == "Release_net40_test") {
                buildOutput = "Release"
                buildCommand = "build.cmd release net40 test"
            }
            else if (onfiguration == "Release_ci_part1") {
                buildOutput = "Release"
                buildCommand = "build.cmd release ci_part1"
            }
            else if (configuration == "Release_ci_part2") {
                buildOutput = "Release"
                buildCommand = "build.cmd release ci_part2"
            }
            else if (configuration == "Release_ci_part3") {
                buildOutput = "Release"
                buildCommand = "build.cmd release ci_part3"
            }
            else if (configuration == "Release_ci_part4") {
                buildOutput = "Release"
                buildCommand = "build.cmd release ci_part4"
            }
            else if (configuration == "Release_net40_no_vs") {
                buildOutput = "Release"
                buildCommand = "build.cmd release net40"
            }


            def newJobName = Utilities.getFullJobName(project, jobName, isPullRequest)
            def newJob = job(newJobName) {
                steps {
                    if (os == 'Windows_NT') {
                        batchFile(buildCommand)
                    }
                    else {
                        shell(buildCommand)
                    }
                }
            }

            // TODO: set to false after tests are fully enabled
            def skipIfNoTestFiles = true

            def affinity = configuration == 'Release_net40_no_vs' ? 'latest-or-auto' : (os == 'Windows_NT' ? 'latest-dev15-5' : 'latest-or-auto')
            Utilities.setMachineAffinity(newJob, os, affinity)
            Utilities.standardJobSetup(newJob, project, isPullRequest, "*/${branch}")

            Utilities.addArchival(newJob, "tests/TestResults/*.*", "", skipIfNoTestFiles, false)
            Utilities.addArchival(newJob, "${buildOutput}/**")
            if (isPullRequest) {
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "${os} ${configuration} Build")
            }
            else {
                Utilities.addGithubPushTrigger(newJob)
            }
        }
    }
}

JobReport.Report.generateJobReport(out)
