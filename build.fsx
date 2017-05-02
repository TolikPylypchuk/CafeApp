#r "packages/FAKE/tools/FakeLib.dll"

open Fake

let buildDir = "./build"
let releaseDir = "./release"

Target "CleanBuild" (fun _ -> CleanDir buildDir)
Target "CleanRelease" (fun _ -> CleanDir releaseDir)

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

"CleanBuild" ==> "BuildApp"
"CleanRelease" ==> "ReleaseApp"

RunTargetOrDefault "BuildApp"
