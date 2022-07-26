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
using Plugins.basegame.Events;

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
        generator.EnableReadWriteEcsGeneration = true;
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591


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
using Plugins.basegame.Events;

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
        generator.EnableReadWriteEcsGeneration = true;
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591


public partial class HUDView
{
    public bool HasPosition3Data
    {
        get => EntityManager.HasComponent<Position3Data>(LinkedEntity);
    }

    public bool HasRotationQuaternionData
    {
        get => EntityManager.HasComponent<RotationQuaternionData>(LinkedEntity);
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ReadWriteEcsComponentsWithDifferentNamespaces()
    {
        string source = @"
using Plugins.basegame.Events;

namespace Test1.Internal
{
    [Plugins.basegame.Events.ComponentDirtyEvent]
    public partial struct Position3Data : Unity.Entities.IComponentData
    {
        public Unity.Mathematics.float3 Value;
    }
}

namespace Test2.ChlidNamespace
{
    [Plugins.basegame.Events.ComponentDirtyEvent]
    public partial struct RotationQuaternionData : Unity.Entities.IComponentData
    {
        public Unity.Mathematics.quaternion Value;
    }
}

namespace My.SubNamespace
{
    using Test1.Internal;
    using Test2.ChlidNamespace;

    [ReadWriteEcs(typeof(Position3Data), typeof(RotationQuaternionData))]
    public partial class HUDView
    {
    }
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableReadWriteEcsGeneration = true;
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591


namespace My.SubNamespace
{

    public partial class HUDView
    {
        public Test1.Internal.Position3Data Position3Data
        {
            get => EntityManager.GetComponentData<Test1.Internal.Position3Data>(LinkedEntity);
            set => EntityManager.SetComponentData(LinkedEntity, value);
        }

        public Test2.ChlidNamespace.RotationQuaternionData RotationQuaternionData
        {
            get => EntityManager.GetComponentData<Test2.ChlidNamespace.RotationQuaternionData>(LinkedEntity);
            set => EntityManager.SetComponentData(LinkedEntity, value);
        }
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ReadWriteEcsHasComponentsWithDifferentNamespaces()
    {
        string source = @"
using Plugins.basegame.Events;

namespace Test1.Internal
{
    [Plugins.basegame.Events.ComponentDirtyEvent]
    public partial struct Position3Data : Unity.Entities.IComponentData
    {
        public Unity.Mathematics.float3 Value;
    }
}

namespace Test2.ChlidNamespace
{
    [Plugins.basegame.Events.ComponentDirtyEvent]
    public partial struct RotationQuaternionData : Unity.Entities.IComponentData
    {
        public Unity.Mathematics.quaternion Value;
    }
}

namespace My.SubNamespace
{
    using Test1.Internal;
    using Test2.ChlidNamespace;

    [HasComponentEcs(typeof(Position3Data), typeof(RotationQuaternionData))]
    public partial class HUDView
    {
    }
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableReadWriteEcsGeneration = true;
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591


namespace My.SubNamespace
{

    public partial class HUDView
    {
        public bool HasPosition3Data
        {
            get => EntityManager.HasComponent<Test1.Internal.Position3Data>(LinkedEntity);
        }

        public bool HasRotationQuaternionData
        {
            get => EntityManager.HasComponent<Test2.ChlidNamespace.RotationQuaternionData>(LinkedEntity);
        }
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }
    [TestMethod]
    public void ReadWriteEcsComponentsSingle()
    {
        string source = @"
using Plugins.basegame.Events;

[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    public Unity.Mathematics.float3 Value;
}

[ReadWriteEcs(typeof(Position3Data))]
public partial class HUDView
{
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableReadWriteEcsGeneration = true;
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591


public partial class HUDView
{
    public Position3Data Position3Data
    {
        get => EntityManager.GetComponentData<Position3Data>(LinkedEntity);
        set => EntityManager.SetComponentData(LinkedEntity, value);
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }
}
