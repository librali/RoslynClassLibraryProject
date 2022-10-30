﻿namespace LittleToySourceGenerator.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GenerateEventDataTests : CodeGenerationTestBase
{
    [TestMethod]
    public void ComponentDirtyEvent()
    {
        string source = @"
using Plugins.basegame.Events;

[ComponentDirtyEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    [MarkDirty] public Unity.Mathematics.float3 Value;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
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
using Unity.Collections;

public partial struct Position3Data
{
    public bool IsDirty { get; set; }
    public Position3Data Update(Unity.Mathematics.float3 value)
    {
        if(Value.Equals(value)) return this;
        
        IsDirty = true;
        Value = value;
        
        return this;
    }
}

public interface IPosition3Listener
{
    public void OnPosition3Changed(Unity.Mathematics.float3 value);
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void MultipleFields()
    {
        string source = @"
using Plugins.basegame.Events;
[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    [MarkDirty]public Unity.Mathematics.float3 Value;
    [MarkDirty]public Unity.Mathematics.float3 SecondValue;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
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
using Unity.Collections;

public partial struct Position3Data
{
    public bool IsDirty { get; set; }
    public Position3Data Update(Unity.Mathematics.float3 value, Unity.Mathematics.float3 secondValue)
    {
        if(Value.Equals(value) && SecondValue.Equals(secondValue)) return this;
        
        IsDirty = true;
        Value = value;
        SecondValue = secondValue;
        
        return this;
    }
}

public interface IPosition3Listener
{
    public void OnPosition3Changed(Unity.Mathematics.float3 value, Unity.Mathematics.float3 secondValue);
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void IgnoreFieldsMarkedWithAttribute()
    {
        string source = @"
using Plugins.basegame.Events;

[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    [MarkDirty] public Unity.Mathematics.float3 Value;
    public Unity.Mathematics.float3 SecondValue;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
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
using Unity.Collections;

public partial struct Position3Data
{
    public bool IsDirty { get; set; }
    public Position3Data Update(Unity.Mathematics.float3 value)
    {
        if(Value.Equals(value)) return this;
        
        IsDirty = true;
        Value = value;
        
        return this;
    }
}

public interface IPosition3Listener
{
    public void OnPosition3Changed(Unity.Mathematics.float3 value);
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ComponentRemoved()
    {
        string source = @"
using Plugins.basegame.Events;

[Plugins.basegame.Events.ComponentRemovedEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    [MarkDirty] public Unity.Mathematics.float3 Value;
    public Unity.Mathematics.float3 SecondValue;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
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
using Unity.Collections;

public interface IPosition3RemovedListener
{
    public void OnPosition3Removed();
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ComponentAdded()
    {
        string source = @"
using Plugins.basegame.Events;

[Plugins.basegame.Events.ComponentAddedEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    [MarkDirty]public Unity.Mathematics.float3 Value;
    public Unity.Mathematics.float3 SecondValue;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
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
using Unity.Collections;

public interface IPosition3AddedListener
{
    public void OnPosition3Added();
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void LowercaseVariables()
    {
        string source = @"
using Plugins.basegame.Events;

[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    [MarkDirty] public Unity.Mathematics.float3 value;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
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
using Unity.Collections;

public partial struct Position3Data
{
    public bool IsDirty { get; set; }
    public Position3Data Update(Unity.Mathematics.float3 value_)
    {
        if(value.Equals(value_)) return this;
        
        IsDirty = true;
        value = value_;
        
        return this;
    }
}

public interface IPosition3Listener
{
    public void OnPosition3Changed(Unity.Mathematics.float3 value);
}
";
        Assert.AreEqual(expectedOutput, output);
    }
    [TestMethod]
    public void ComponentDirtyEventWithNamespace()
    {
        string source = @"
namespace Test.Internal
{
    using Plugins.basegame.Events;

    [Plugins.basegame.Events.ComponentDirtyEvent]
    public partial struct Position3Data : Unity.Entities.IComponentData
    {
        [MarkDirty] public Unity.Mathematics.float3 Value;
    }
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
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
using Unity.Collections;

namespace Test.Internal
{

    public partial struct Position3Data
    {
        public bool IsDirty { get; set; }
        public Test.Internal.Position3Data Update(Unity.Mathematics.float3 value)
        {
            if(Value.Equals(value)) return this;
            
            IsDirty = true;
            Value = value;
            
            return this;
        }
    }

    public interface IPosition3Listener
    {
        public void OnPosition3Changed(Unity.Mathematics.float3 value);
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ComponentDirtyEventWithFixedList()
    {
        string source = @"
using Plugins.basegame.Events;
using Unity.Collections;

[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    [MarkDirty] public FixedList128Bytes<long> Value;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
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
using Unity.Collections;

public partial struct Position3Data
{
    public bool IsDirty { get; set; }
    public Position3Data Update(Unity.Collections.FixedList128Bytes<long> value)
    {
        if(Value.Equals(value)) return this;
        
        IsDirty = true;
        Value = value;
        
        return this;
    }
}

public interface IPosition3Listener
{
    public void OnPosition3Changed(Unity.Collections.FixedList128Bytes<long> value);
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ComponentDirtyEventWorkOnProperties()
    {
        string source = @"
using Unity.Collections;
using Plugins.basegame.Events;

[Plugins.basegame.Events.ComponentDirtyEvent]
public partial struct Position3Data : Unity.Entities.IComponentData
{
    [MarkDirty] public FixedList128Bytes<long> Value { get; set; }
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
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
using Unity.Collections;

public partial struct Position3Data
{
    public bool IsDirty { get; set; }
    public Position3Data Update(Unity.Collections.FixedList128Bytes<long> value)
    {
        if(Value.Equals(value)) return this;
        
        IsDirty = true;
        Value = value;
        
        return this;
    }
}

public interface IPosition3Listener
{
    public void OnPosition3Changed(Unity.Collections.FixedList128Bytes<long> value);
}
";
        Assert.AreEqual(expectedOutput, output);
    }
}
