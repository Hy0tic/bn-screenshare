#addin "nuget:?package=Cake.Docker&version=1.2.3"

using System;
using System.IO;
using Cake.Core;
using Cake.Docker;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var imageName = "bn-screenshare-api:latest"; 
var context = "./";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Build-Docker-Image")
    .Does((cakeContext) =>
    {
        try {
            // Build the Docker image
            cakeContext.Information("Starting Docker build...");
            cakeContext.DockerBuild(
                new DockerImageBuildSettings {
                    Rm = true,
                    NoCache = true, 
                    Tag = new string[]{ imageName } 
                }, context);
            cakeContext.Information("Docker build completed.");
        } catch (Exception ex) {
            cakeContext.Error("Error during Docker build: " + ex.Message);
            throw;
        }
    });

Task("Default")
    .IsDependentOn("Build-Docker-Image");

RunTarget(target);
