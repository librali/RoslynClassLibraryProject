﻿namespace LittleToySourceGenerator.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GenerateNetworkComponentTests : CodeGenerationTestBase
{
    [TestMethod]
    public void DirtyEventViewInterfaces()
    {
        string source = @"
using Plugins.basegame.Events;
using DOTSNET;

[Plugins.basegame.Events.CodeGenNetComponentAttribute(DOTSNET.SyncDirection.SERVER_TO_CLIENT)]
public partial struct Position3Data
{
    [MarkDirty] [SyncField] public int HatIndex;
    [MarkDirty] [SyncField] public int BodyIndex;
    [SyncField] public Unity.Collections.FixedString64Bytes UserName;
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

public partial struct Position3Data : NetworkComponent
{
    public bool IsDirty { get; set; }
    public SyncDirection GetSyncDirection()
    {
        return SyncDirection.SERVER_TO_CLIENT;
    }

    public bool Serialize(ref NetworkWriter128 writer)
    {
        return writer.WriteInt(HatIndex) && writer.WriteInt(BodyIndex) && writer.WriteFixedString64(UserName);
    }

    public bool Deserialize(ref NetworkReader128 reader)
    {
        if (reader.ReadInt(out var hatindex) && reader.ReadInt(out var bodyindex) && reader.ReadFixedString64(out UserName))
        {
            if (HatIndex != hatindex || BodyIndex != bodyindex)
            {
                IsDirty = true;
                HatIndex = hatindex;
                BodyIndex = bodyindex;
            }
            return true;
        }
        return false;
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void DirtyEventWithoutSync()
    {
        string source = @"
using Plugins.basegame.Events;
using DOTSNET;

[Plugins.basegame.Events.CodeGenNetComponentAttribute(DOTSNET.SyncDirection.SERVER_TO_CLIENT)]
public partial struct Position3Data
{
    [MarkDirty] public int HatIndex;
    [MarkDirty] [SyncField] public int BodyIndex;
    [SyncField] public Unity.Collections.FixedString64Bytes UserName;
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

public partial struct Position3Data : NetworkComponent
{
    public bool IsDirty { get; set; }
    public Position3Data Update(Int32 hatIndex)
    {
        if(HatIndex.Equals(hatIndex)) return this;
        
        IsDirty = true;
        HatIndex = hatIndex;
        
        return this;
    }

    public SyncDirection GetSyncDirection()
    {
        return SyncDirection.SERVER_TO_CLIENT;
    }

    public bool Serialize(ref NetworkWriter128 writer)
    {
        return writer.WriteInt(BodyIndex) && writer.WriteFixedString64(UserName);
    }

    public bool Deserialize(ref NetworkReader128 reader)
    {
        if (reader.ReadInt(out var bodyindex) && reader.ReadFixedString64(out UserName))
        {
            if (BodyIndex != bodyindex)
            {
                IsDirty = true;
                BodyIndex = bodyindex;
            }
            return true;
        }
        return false;
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void OnlySyncFields()
    {
        string source = @"
using Plugins.basegame.Events;
using DOTSNET;

[Plugins.basegame.Events.CodeGenNetComponentAttribute(DOTSNET.SyncDirection.SERVER_TO_CLIENT)]
public partial struct Position3Data
{
    [SyncField] public int HatIndex;
    [SyncField] public int BodyIndex;
    [SyncField] public Unity.Collections.FixedString64Bytes UserName;
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

public partial struct Position3Data : NetworkComponent
{
    public SyncDirection GetSyncDirection()
    {
        return SyncDirection.SERVER_TO_CLIENT;
    }

    public bool Serialize(ref NetworkWriter128 writer)
    {
        return writer.WriteInt(HatIndex) && writer.WriteInt(BodyIndex) && writer.WriteFixedString64(UserName);
    }

    public bool Deserialize(ref NetworkReader128 reader)
    {
        return reader.ReadInt(out HatIndex) && reader.ReadInt(out BodyIndex) && reader.ReadFixedString64(out UserName);
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void OnlyMarkDirtyFields()
    {
        string source = @"
using Plugins.basegame.Events;
using DOTSNET;

[Plugins.basegame.Events.CodeGenNetComponentAttribute(DOTSNET.SyncDirection.SERVER_TO_CLIENT)]
public partial struct Position3Data
{
    [MarkDirty] public int HatIndex;
    [MarkDirty] public int BodyIndex;
    [MarkDirty] public Unity.Collections.FixedString64Bytes UserName;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
        var diagnostics = this.GetDiagnosticsFromGenerator(source, generator, NullableContextOptions.Disable);

        Diagnostic? value = diagnostics.FirstOrDefault();
        Assert.IsNotNull(value);
        Assert.AreEqual("LT0002", value.Id);
        Assert.AreEqual("(5,1): error LT0002: Type Position3Data does not have any field marked with SyncFieldAttribute", value.ToString());
    }

    [TestMethod]
    public void UnknownTypeReported()
    {
        string source = @"
using Plugins.basegame.Events;
using DOTSNET;

[Plugins.basegame.Events.CodeGenNetComponentAttribute(DOTSNET.SyncDirection.SERVER_TO_CLIENT)]
public partial struct Position3Data
{
    [MarkDirty] [SyncField] public string HatIndex;
    [MarkDirty] [SyncField] public int BodyIndex;
    [SyncField] public Unity.Collections.FixedString64Bytes UserName;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableEventDataGeneration = true;
        var diagnostics = this.GetDiagnosticsFromGenerator(source, generator, NullableContextOptions.Disable);

        Diagnostic? value = diagnostics.FirstOrDefault();
        Assert.IsNotNull(value);
        Assert.AreEqual("LT0001", value.Id);
        Assert.AreEqual("(5,1): error LT0001: Cannot convert field type String to dotsnet read/write methods", value.ToString());
    }
    [TestMethod]
    public void AllFieldMapping()
    {
        string source = @"
using Plugins.basegame.Events;
using DOTSNET;

[Plugins.basegame.Events.CodeGenNetComponentAttribute(DOTSNET.SyncDirection.SERVER_TO_CLIENT)]
public partial struct UserInfoData
{
    [SyncField] public byte ByteField;
    [SyncField] public bool BoolField;
    [SyncField] public short ShortField;
    [SyncField] public ushort UShortField;
    [SyncField] public int Int32Field;
    [SyncField] public uint UInt32Field;
    [SyncField] public Unity.Mathematics.int2 Int2Field;
    [SyncField] public Unity.Mathematics.int3 Int3Field;
    [SyncField] public Unity.Mathematics.int4 Int4Field;
    [SyncField] public long Int64Field;
    [SyncField] public ulong UInt64Field;
    [SyncField] public DOTSNET.long3 Long3Field;
    [SyncField] public float FloatField;
    [SyncField] public Unity.Mathematics.float2 Float2Field;
    [SyncField] public Unity.Mathematics.float3 Float3Field;
    [SyncField] public Unity.Mathematics.float4 Float4Field;
    [SyncField] public double DoubleField;
    [SyncField] public Unity.Mathematics.double2 Double2Field;
    [SyncField] public Unity.Mathematics.double3 Double3Field;
    [SyncField] public Unity.Mathematics.double4 Double4Field;
    [SyncField] public decimal DecimalField;
    [SyncField] public Unity.Mathematics.quaternion QuaternionField;
    [SyncField] public Unity.Collections.FixedBytes16 Bytes16Field;
    [SyncField] public Unity.Collections.FixedBytes30 Bytes30Field;
    [SyncField] public Unity.Collections.FixedBytes62 Bytes62Field;
    [SyncField] public Unity.Collections.FixedBytes126 Bytes126Field;
    [SyncField] public Unity.Collections.FixedBytes510 Bytes510Field;
    [SyncField] public Unity.Collections.FixedBytes4094 Bytes4094Field;
    [SyncField] public Unity.Collections.FixedString32Bytes FixedString32Field;
    [SyncField] public Unity.Collections.FixedString64Bytes FixedString64Field;
    [SyncField] public Unity.Collections.FixedString128Bytes FixedString128Field;
    [SyncField] public Unity.Collections.FixedString512Bytes FixedString512Field;
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

public partial struct UserInfoData : NetworkComponent
{
    public SyncDirection GetSyncDirection()
    {
        return SyncDirection.SERVER_TO_CLIENT;
    }

    public bool Serialize(ref NetworkWriter128 writer)
    {
        return writer.WriteByte(ByteField) && writer.WriteBool(BoolField) && writer.WriteShort(ShortField) && writer.WriteUShort(UShortField) && writer.WriteInt(Int32Field) && writer.WriteUInt(UInt32Field) && writer.WriteInt2(Int2Field) && writer.WriteInt3(Int3Field) && writer.WriteInt4(Int4Field) && writer.WriteLong(Int64Field) && writer.WriteULong(UInt64Field) && writer.WriteLong3(Long3Field) && writer.WriteFloat(FloatField) && writer.WriteFloat2(Float2Field) && writer.WriteFloat3(Float3Field) && writer.WriteFloat4(Float4Field) && writer.WriteDouble(DoubleField) && writer.WriteDouble2(Double2Field) && writer.WriteDouble3(Double3Field) && writer.WriteDouble4(Double4Field) && writer.WriteDecimal(DecimalField) && writer.WriteQuaternion(QuaternionField) && writer.WriteBytes16(Bytes16Field) && writer.WriteBytes30(Bytes30Field) && writer.WriteBytes62(Bytes62Field) && writer.WriteBytes126(Bytes126Field) && writer.WriteBytes510(Bytes510Field) && writer.WriteBytes4094(Bytes4094Field) && writer.WriteFixedString32(FixedString32Field) && writer.WriteFixedString64(FixedString64Field) && writer.WriteFixedString128(FixedString128Field) && writer.WriteFixedString512(FixedString512Field);
    }

    public bool Deserialize(ref NetworkReader128 reader)
    {
        return reader.ReadByte(out ByteField) && reader.ReadBool(out BoolField) && reader.ReadShort(out ShortField) && reader.ReadUShort(out UShortField) && reader.ReadInt(out Int32Field) && reader.ReadUInt(out UInt32Field) && reader.ReadInt2(out Int2Field) && reader.ReadInt3(out Int3Field) && reader.ReadInt4(out Int4Field) && reader.ReadLong(out Int64Field) && reader.ReadULong(out UInt64Field) && reader.ReadLong3(out Long3Field) && reader.ReadFloat(out FloatField) && reader.ReadFloat2(out Float2Field) && reader.ReadFloat3(out Float3Field) && reader.ReadFloat4(out Float4Field) && reader.ReadDouble(out DoubleField) && reader.ReadDouble2(out Double2Field) && reader.ReadDouble3(out Double3Field) && reader.ReadDouble4(out Double4Field) && reader.ReadDecimal(out DecimalField) && reader.ReadQuaternion(out QuaternionField) && reader.ReadBytes16(out Bytes16Field) && reader.ReadBytes30(out Bytes30Field) && reader.ReadBytes62(out Bytes62Field) && reader.ReadBytes126(out Bytes126Field) && reader.ReadBytes510(out Bytes510Field) && reader.ReadBytes4094(out Bytes4094Field) && reader.ReadFixedString32(out FixedString32Field) && reader.ReadFixedString64(out FixedString64Field) && reader.ReadFixedString128(out FixedString128Field) && reader.ReadFixedString512(out FixedString512Field);
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void AllFieldMappingShortSyntax()
    {
        string source = @"
using Plugins.basegame.Events;
using DOTSNET;
using Unity.Mathematics;
using Unity.Collections;

[Plugins.basegame.Events.CodeGenNetComponentAttribute(DOTSNET.SyncDirection.SERVER_TO_CLIENT)]
public partial struct UserInfoData
{
    [SyncField] public byte ByteField;
    [SyncField] public bool BoolField;
    [SyncField] public short ShortField;
    [SyncField] public ushort UShortField;
    [SyncField] public int Int32Field;
    [SyncField] public uint UInt32Field;
    [SyncField] public int2 Int2Field;
    [SyncField] public int3 Int3Field;
    [SyncField] public int4 Int4Field;
    [SyncField] public long Int64Field;
    [SyncField] public ulong UInt64Field;
    [SyncField] public long3 Long3Field;
    [SyncField] public float FloatField;
    [SyncField] public float2 Float2Field;
    [SyncField] public float3 Float3Field;
    [SyncField] public float4 Float4Field;
    [SyncField] public double DoubleField;
    [SyncField] public double2 Double2Field;
    [SyncField] public double3 Double3Field;
    [SyncField] public double4 Double4Field;
    [SyncField] public decimal DecimalField;
    [SyncField] public quaternion QuaternionField;
    [SyncField] public FixedBytes16 Bytes16Field;
    [SyncField] public FixedBytes30 Bytes30Field;
    [SyncField] public FixedBytes62 Bytes62Field;
    [SyncField] public FixedBytes126 Bytes126Field;
    [SyncField] public FixedBytes510 Bytes510Field;
    [SyncField] public FixedBytes4094 Bytes4094Field;
    [SyncField] public FixedString32Bytes FixedString32Field;
    [SyncField] public FixedString64Bytes FixedString64Field;
    [SyncField] public FixedString128Bytes FixedString128Field;
    [SyncField] public FixedString512Bytes FixedString512Field;
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

public partial struct UserInfoData : NetworkComponent
{
    public SyncDirection GetSyncDirection()
    {
        return SyncDirection.SERVER_TO_CLIENT;
    }

    public bool Serialize(ref NetworkWriter128 writer)
    {
        return writer.WriteByte(ByteField) && writer.WriteBool(BoolField) && writer.WriteShort(ShortField) && writer.WriteUShort(UShortField) && writer.WriteInt(Int32Field) && writer.WriteUInt(UInt32Field) && writer.WriteInt2(Int2Field) && writer.WriteInt3(Int3Field) && writer.WriteInt4(Int4Field) && writer.WriteLong(Int64Field) && writer.WriteULong(UInt64Field) && writer.WriteLong3(Long3Field) && writer.WriteFloat(FloatField) && writer.WriteFloat2(Float2Field) && writer.WriteFloat3(Float3Field) && writer.WriteFloat4(Float4Field) && writer.WriteDouble(DoubleField) && writer.WriteDouble2(Double2Field) && writer.WriteDouble3(Double3Field) && writer.WriteDouble4(Double4Field) && writer.WriteDecimal(DecimalField) && writer.WriteQuaternion(QuaternionField) && writer.WriteBytes16(Bytes16Field) && writer.WriteBytes30(Bytes30Field) && writer.WriteBytes62(Bytes62Field) && writer.WriteBytes126(Bytes126Field) && writer.WriteBytes510(Bytes510Field) && writer.WriteBytes4094(Bytes4094Field) && writer.WriteFixedString32(FixedString32Field) && writer.WriteFixedString64(FixedString64Field) && writer.WriteFixedString128(FixedString128Field) && writer.WriteFixedString512(FixedString512Field);
    }

    public bool Deserialize(ref NetworkReader128 reader)
    {
        return reader.ReadByte(out ByteField) && reader.ReadBool(out BoolField) && reader.ReadShort(out ShortField) && reader.ReadUShort(out UShortField) && reader.ReadInt(out Int32Field) && reader.ReadUInt(out UInt32Field) && reader.ReadInt2(out Int2Field) && reader.ReadInt3(out Int3Field) && reader.ReadInt4(out Int4Field) && reader.ReadLong(out Int64Field) && reader.ReadULong(out UInt64Field) && reader.ReadLong3(out Long3Field) && reader.ReadFloat(out FloatField) && reader.ReadFloat2(out Float2Field) && reader.ReadFloat3(out Float3Field) && reader.ReadFloat4(out Float4Field) && reader.ReadDouble(out DoubleField) && reader.ReadDouble2(out Double2Field) && reader.ReadDouble3(out Double3Field) && reader.ReadDouble4(out Double4Field) && reader.ReadDecimal(out DecimalField) && reader.ReadQuaternion(out QuaternionField) && reader.ReadBytes16(out Bytes16Field) && reader.ReadBytes30(out Bytes30Field) && reader.ReadBytes62(out Bytes62Field) && reader.ReadBytes126(out Bytes126Field) && reader.ReadBytes510(out Bytes510Field) && reader.ReadBytes4094(out Bytes4094Field) && reader.ReadFixedString32(out FixedString32Field) && reader.ReadFixedString64(out FixedString64Field) && reader.ReadFixedString128(out FixedString128Field) && reader.ReadFixedString512(out FixedString512Field);
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void NetworkComponentGeneration()
    {
        string source = @"
using Plugins.basegame.Events;
using Unity.Collections;

[CodeGenNetComponentAttribute]
public partial struct JoinWorldMessage
{
    public long PlayerId;

    public int PlayerHatIndex;
    public int PlayerBodyIndex;
    public FixedString64Bytes PlayerName;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableNetworkComponentGeneration = true;
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
using Unity.Jobs;
[assembly: RegisterGenericJobType(typeof(DOTSNetworkComponentSerializer<JoinWorldMessage>.NetworkComponentSerializerJob))]
[assembly: RegisterGenericJobType(typeof(DOTSNetworkComponentSerializer<JoinWorldMessage>.NetworkComponentDeserializerJob))]

public class JoinWorldMessageSerializer : DOTSNetworkComponentSerializer<JoinWorldMessage>
{
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void NetComponentInsideNamespace()
    {
        string source = @"
using Plugins.basegame.Events;
using DOTSNET;

namespace Test.InnerNamespace;

[Plugins.basegame.Events.CodeGenNetComponentAttribute(DOTSNET.SyncDirection.SERVER_TO_CLIENT)]
public partial struct Position3Data
{
    [MarkDirty] [SyncField] public int HatIndex;
    [MarkDirty] [SyncField] public int BodyIndex;
    [SyncField] public Unity.Collections.FixedString64Bytes UserName;
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

namespace Test.InnerNamespace
{

    public partial struct Position3Data : NetworkComponent
    {
        public bool IsDirty { get; set; }
        public SyncDirection GetSyncDirection()
        {
            return SyncDirection.SERVER_TO_CLIENT;
        }

        public bool Serialize(ref NetworkWriter128 writer)
        {
            return writer.WriteInt(HatIndex) && writer.WriteInt(BodyIndex) && writer.WriteFixedString64(UserName);
        }

        public bool Deserialize(ref NetworkReader128 reader)
        {
            if (reader.ReadInt(out var hatindex) && reader.ReadInt(out var bodyindex) && reader.ReadFixedString64(out UserName))
            {
                if (HatIndex != hatindex || BodyIndex != bodyindex)
                {
                    IsDirty = true;
                    HatIndex = hatindex;
                    BodyIndex = bodyindex;
                }
                return true;
            }
            return false;
        }
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void NetworkComponentGenerationWithNamespace()
    {
        string source = @"
using Plugins.basegame.Events;
using Unity.Collections;

namespace Test.Inside;

[CodeGenNetComponentAttribute]
public partial struct JoinWorldMessage
{
    public long PlayerId;

    public int PlayerHatIndex;
    public int PlayerBodyIndex;
    public FixedString64Bytes PlayerName;
}
";
        var generator = new Generator();
        generator.DisableAllGeneration();
        generator.EnableNetworkComponentGeneration = true;
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
using Unity.Jobs;
[assembly: RegisterGenericJobType(typeof(DOTSNetworkComponentSerializer<Test.Inside.JoinWorldMessage>.NetworkComponentSerializerJob))]
[assembly: RegisterGenericJobType(typeof(DOTSNetworkComponentSerializer<Test.Inside.JoinWorldMessage>.NetworkComponentDeserializerJob))]

namespace Test.Inside
{

    public class JoinWorldMessageSerializer : DOTSNetworkComponentSerializer<Test.Inside.JoinWorldMessage>
    {
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }
}
