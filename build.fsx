#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.NpmHelper
open Fake.Testing.XUnit2

let buildDir = "./build"
let testDir = "./test"
let clientDir = "./client"
let clientAssetDir = clientDir @@ "public"
let assetBuildDir = buildDir @@ "public"

let runnerPath = "./packages/xunit.runner.console/tools/xunit.console.exe"

Target "Clean" (fun _ -> CleanDirs [ buildDir; testDir ])

Target "BuildApp" (fun _ ->
    !! "src/**/*.fsproj"
    -- "src/**/*.Tests.fsproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: ")

Target "BuildTests" (fun _ ->
    !! "src/**/*.Tests.fsproj"
    |> MSBuildDebug testDir "Build"
    |> Log "TestsBuild-Output: ")

Target "RunUnitTests" (fun _ ->
    !! (testDir @@ "*.Tests.dll")
    |> xUnit2 (fun p -> { p with ToolPath = runnerPath }))

Target "Client" (fun _ ->
    let npmFilePath =
        environVarOrDefault "NPM_FILE_PATH" defaultNpmParams.NpmFilePath
    Npm (fun p ->
        { p with
            Command = Install Standard
            WorkingDirectory = clientDir
            NpmFilePath = npmFilePath
        })
    Npm (fun p ->
        { p with
            Command = (Run "build")
            WorkingDirectory = clientDir
            NpmFilePath = npmFilePath
        })
    CreateDir assetBuildDir
    CopyRecursive clientAssetDir assetBuildDir true |> ignore)

"Clean"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "RunUnitTests"
    ==> "Client"

RunTargetOrDefault "Client"
