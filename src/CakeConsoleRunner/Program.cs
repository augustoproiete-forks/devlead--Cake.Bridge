﻿using System.Linq;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using CakeConsoleRunner;
using static CakeBridge;

var target = Context.Argument<string>("target", "Build");

Setup(context =>
{
    context.Information("Setting up...");

    return new BuildData(
        Context
            .GetFiles("./src/*.sln")
            .FirstOrDefault()
        ??
        Context
            .GetFiles("../../src/*.sln")
            .FirstOrDefault()
        ??
        Context
            .GetFiles("../../../../*.sln")
            .FirstOrDefault());
});

Teardown(context =>
{
    context.Information("Tearing down...");
});

var restore = Task("Restore")
    .Does<BuildData>(buildData =>
    {
        Context.DotNetCoreRestore(buildData.Solution.FullPath);
    });

var build = Task("Build")
    .IsDependentOn(restore)
    .Does<BuildData>(buildData =>
    {
        Context.DotNetCoreBuild(buildData.Solution.FullPath);
    });

RunTarget(target);