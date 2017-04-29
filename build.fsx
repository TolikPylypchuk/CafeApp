#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing

let buildDir = "./build"
let testDir = "./tests"
let nunitRunnerPath = "./packages/NUnit.ConsoleRunner/tools/nunit3-console.exe"

Target "Clean" (fun _ -> CleanDirs [ buildDir; testDir ])

Target "BuildApp" (fun _ ->
            !! "src/**/*.fsproj"
            -- "src/**/*.Tests.fsproj"
            |> MSBuildDebug buildDir "Build"
            |> Log "AppBuild-Output: ")

Target "ReleaseApp" (fun _ ->
            !! "src/**/*.fsproj"
            -- "src/**/*.Tests.fsproj"
            |> MSBuildRelease buildDir "Build"
            |> Log "AppBuild-Output: ")

Target "BuildTests" (fun _ ->
        !! "src/**/*.Tests.fsproj"
        |> MSBuildDebug testDir "Build"
        |> Log "BuildTests-Output: ")

Target "RunUnitTests" (fun _ ->
        !! (testDir + "/*.Tests.dll")
        |> NUnit3 (fun p -> { p with ToolPath = nunitRunnerPath; }))

"Clean"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "RunUnitTests"

"Clean" ==> "ReleaseApp"

RunTargetOrDefault "BuildApp"
