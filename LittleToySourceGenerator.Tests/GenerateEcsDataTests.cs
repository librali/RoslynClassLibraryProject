﻿namespace LittleToySourceGenerator.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GenerateEcsDataTests : CodeGenerationTestBase
{
    [TestMethod]
    public void ReadWriteEcsComponents()
    {
        string source = @"
[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    public Unity.Mathematics.float3 Value;
}

[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct RotationQuaternionData : Unity.Entities.IComponentData
{
    public Unity.Mathematics.quaternion Value;
}

[ReadWriteEcs(typeof(Position3Data), typeof(RotationQuaternionData))]
public partial class HUDView
{
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.GenerateReadWriteEcs = true;
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
using System;
using Unity.Entities;
using Unity.Mathematics;
using Plugins.basegame.Events;
using DOTSNET;

public partial class HUDView
{
    public Position3Data Position3Data
    {
        get => EntityManager.GetComponentData<Position3Data>(LinkedEntity);
        set => EntityManager.SetComponentData(LinkedEntity, value);
    }
    public RotationQuaternionData RotationQuaternionData
    {
        get => EntityManager.GetComponentData<RotationQuaternionData>(LinkedEntity);
        set => EntityManager.SetComponentData(LinkedEntity, value);
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ReadWriteEcsHasComponents()
    {
        string source = @"
[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    public Unity.Mathematics.float3 Value;
}

[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct RotationQuaternionData : Unity.Entities.IComponentData
{
    public Unity.Mathematics.quaternion Value;
}

[HasComponentEcs(typeof(Position3Data), typeof(RotationQuaternionData))]
public partial class HUDView
{
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.GenerateReadWriteEcs = true;
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
using System;
using Unity.Entities;
using Unity.Mathematics;
using Plugins.basegame.Events;
using DOTSNET;

public partial class HUDView
{
    public bool HasPosition3Data => EntityManager.HasComponent<Position3Data>(LinkedEntity);
    public bool HasRotationQuaternionData => EntityManager.HasComponent<RotationQuaternionData>(LinkedEntity);
}
";
        Assert.AreEqual(expectedOutput, output);
    }
}