﻿namespace LittleToySourceGenerator.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GenerateEventViewInterfaceTests : CodeGenerationTestBase
{
    [TestMethod]
    public void DirtyEventViewInterfaces()
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

[OnDirtyEventView(typeof(Position3Data), typeof(RotationQuaternionData))]
public partial class TransformView : LinkedView
{
    public virtual void OnPosition3Changed(float3 value)
    {
        transform.position = value;
    }
    
    public virtual void OnRotationQuaternionChanged(quaternion value)
    {
        transform.rotation = value;
    }
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.GenerateEventViewInterface = true;
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

public partial class TransformView : IPosition3Listener, IRotationQuaternionListener
{
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void OnAddedEventViewInterfaces()
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

[OnAddedEventView(typeof(Position3Data), typeof(RotationQuaternionData))]
public partial class TransformView : LinkedView
{
    public virtual void OnPosition3Added()
    {
    }
    
    public virtual void OnRotationQuaternionAdded()
    {
    }
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.GenerateEventViewInterface = true;
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

public partial class TransformView : IPosition3AddedListener, IRotationQuaternionAddedListener
{
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void OnRemovedEventViewInterfaces()
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

[OnRemovedEventView(typeof(Position3Data), typeof(RotationQuaternionData))]
public partial class TransformView : LinkedView
{
    public virtual void OnPosition3Removed()
    {
    }
    
    public virtual void OnRotationQuaternionRemoved()
    {
    }
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.GenerateEventViewInterface = true;
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

public partial class TransformView : IPosition3RemovedListener, IRotationQuaternionRemovedListener
{
}
";
        Assert.AreEqual(expectedOutput, output);
    }
}