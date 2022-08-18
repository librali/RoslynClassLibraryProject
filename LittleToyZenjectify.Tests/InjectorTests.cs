﻿namespace LittleToyZenjectify.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GenerateInstallerTests : CodeGenerationTestBase
{
    [TestMethod]
    public void MonoClassWithSceneObjInstance()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.MonoClassWithSceneObjInstance, InstallerNameEnum.SomeNewInstaller)]
public class ZenGenSomeSystemInNewInstaller: MonoBehaviour
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SomeNewInstaller"")]
[DisallowMultipleComponent]
public partial class SomeNewInstaller : MonoInstaller
{
    [Required]
    [SceneObjectsOnly]
    [SerializeField]
    private ZenGenSomeSystemInNewInstaller zenGenSomeSystemInNewInstaller;


    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
        zenGenSomeSystemInNewInstaller = FindObjectOfType<ZenGenSomeSystemInNewInstaller>();
    }

    private void InstallMonoClassesWithSceneObjInstance()
    {
        Container.Bind<ZenGenSomeSystemInNewInstaller>().FromInstance(zenGenSomeSystemInNewInstaller).AsSingle().NonLazy();
    }

    public override void InstallBindings()
    {
        InstallMonoClassesWithSceneObjInstance();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }
    [TestMethod]
    public void MonoClassWithAssetInstance()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.MonoClassWithAssetInstance, InstallerNameEnum.SomeNewInstaller)]
public class ZenGenSomeSystemInNewInstaller: MonoBehaviour
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SomeNewInstaller"")]
[DisallowMultipleComponent]
public partial class SomeNewInstaller : MonoInstaller
{
    [Required]
    [AssetsOnly]
    [SerializeField]
    private ZenGenSomeSystemInNewInstaller zenGenSomeSystemInNewInstaller;


    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallMonoClassesWithAssetInstance()
    {
        Container.Bind<ZenGenSomeSystemInNewInstaller>().FromInstance(zenGenSomeSystemInNewInstaller).AsSingle().NonLazy();
    }

    public override void InstallBindings()
    {
        InstallMonoClassesWithAssetInstance();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ClassWithoutInstance()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.ClassWithoutInstance, InstallerNameEnum.SomeNewInstaller)]
public class ZenGenSomeSystemInNewInstaller: MonoBehaviour
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SomeNewInstaller"")]
[DisallowMultipleComponent]
public partial class SomeNewInstaller : MonoInstaller
{
    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallClassesWithoutInstance()
    {
        Container.Bind<ZenGenSomeSystemInNewInstaller>().AsSingle().NonLazy();
    }

    public override void InstallBindings()
    {
        InstallClassesWithoutInstance();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void BindInterfacesAndSelf()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.ClassWithoutInstance, InstallerNameEnum.SomeNewInstaller, true)]
public class ZenGenSomeSystemInNewInstaller: MonoBehaviour
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SomeNewInstaller"")]
[DisallowMultipleComponent]
public partial class SomeNewInstaller : MonoInstaller
{
    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallClassesWithoutInstance()
    {
        Container.BindInterfacesAndSelfTo<ZenGenSomeSystemInNewInstaller>().AsSingle().NonLazy();
    }

    public override void InstallBindings()
    {
        InstallClassesWithoutInstance();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void LazyLoading()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.ClassWithoutInstance, InstallerNameEnum.SomeNewInstaller, isLazyLoading: true)]
public class ZenGenSomeSystemInNewInstaller: MonoBehaviour
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SomeNewInstaller"")]
[DisallowMultipleComponent]
public partial class SomeNewInstaller : MonoInstaller
{
    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallClassesWithoutInstance()
    {
        Container.Bind<ZenGenSomeSystemInNewInstaller>().AsSingle().Lazy();
    }

    public override void InstallBindings()
    {
        InstallClassesWithoutInstance();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void Signal()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.Signal, InstallerNameEnum.SomeNewInstaller)]
public class ZenGenSomeSystemInNewInstaller: MonoBehaviour
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SomeNewInstaller"")]
[DisallowMultipleComponent]
public partial class SomeNewInstaller : MonoInstaller
{
    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallSignals()
    {
        Container.DeclareSignal<ZenGenSomeSystemInNewInstaller>();
    }

    public override void InstallBindings()
    {
        InstallSignals();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void Prefab()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.Prefab, InstallerNameEnum.SomeNewInstaller)]
public class ZenGenSomeSystemInNewInstaller: MonoBehaviour
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SomeNewInstaller"")]
[DisallowMultipleComponent]
public partial class SomeNewInstaller : MonoInstaller
{
    [Required]
    [AssetsOnly]
    [SerializeField]
    private ZenGenSomeSystemInNewInstaller zenGenSomeSystemInNewInstaller;


    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallPrefabs()
    {
        Container.BindFactory<ZenGenSomeSystemInNewInstaller, ZenGenSomeSystemInNewInstaller.Factory>().FromComponentInNewPrefab(zenGenSomeSystemInNewInstaller);
    }

    public override void InstallBindings()
    {
        InstallPrefabs();
    }
}

public partial class ZenGenSomeSystemInNewInstaller
{

    public class Factory : PlaceholderFactory<ZenGenSomeSystemInNewInstaller>
    {
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void PrefabCustomConstructor()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.Prefab, InstallerNameEnum.SomeNewInstaller)]
public class ZenGenSomeSystemInNewInstaller: MonoBehaviour
{
    [Inject]
    public void Construct(int intParam, float floatParam)
    {
    }
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SomeNewInstaller"")]
[DisallowMultipleComponent]
public partial class SomeNewInstaller : MonoInstaller
{
    [Required]
    [AssetsOnly]
    [SerializeField]
    private ZenGenSomeSystemInNewInstaller zenGenSomeSystemInNewInstaller;


    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallPrefabs()
    {
        Container.BindFactory<int, float, ZenGenSomeSystemInNewInstaller, ZenGenSomeSystemInNewInstaller.Factory>().FromComponentInNewPrefab(zenGenSomeSystemInNewInstaller);
    }

    public override void InstallBindings()
    {
        InstallPrefabs();
    }
}

public partial class ZenGenSomeSystemInNewInstaller
{

    public class Factory : PlaceholderFactory<int, float, ZenGenSomeSystemInNewInstaller>
    {
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void Suffix()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.Signal, InstallerNameEnum.AnotherNewInstaller, suffix: "".OptionalSubscriber()"")]
public class ZenGenSomeSignalWithOptionalSubscriber
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/AnotherNewInstaller"")]
[DisallowMultipleComponent]
public partial class AnotherNewInstaller : MonoInstaller
{
    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallSignals()
    {
        Container.DeclareSignal<ZenGenSomeSignalWithOptionalSubscriber>().OptionalSubscriber();
    }

    public override void InstallBindings()
    {
        InstallSignals();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void InstallerSuffix()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.Signal, InstallerNameEnum.SoundTest, suffix: "".OptionalSubscriber()"")]
public class ZenGenSomeSignalWithOptionalSubscriber
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SoundTestInstaller"")]
[DisallowMultipleComponent]
public partial class SoundTestInstaller : MonoInstaller
{
    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallSignals()
    {
        Container.DeclareSignal<ZenGenSomeSignalWithOptionalSubscriber>().OptionalSubscriber();
    }

    public override void InstallBindings()
    {
        InstallSignals();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void InstallerUsingStrings()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.Signal, ""SuperInstaller"")]
public class ZenGenSomeSignalWithOptionalSubscriber
{
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SuperInstaller"")]
[DisallowMultipleComponent]
public partial class SuperInstaller : MonoInstaller
{
    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallSignals()
    {
        Container.DeclareSignal<ZenGenSomeSignalWithOptionalSubscriber>();
    }

    public override void InstallBindings()
    {
        InstallSignals();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void AccountNamespaceOfPredefinedInstaller()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.Signal, ""SuperInstaller"")]
public class ZenGenSomeSignalWithOptionalSubscriber
{
}

namespace test.something
{
    public partial class SuperInstaller {}
}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace test.something
{

    [AddComponentMenu(""Installers/SuperInstaller"")]
    [DisallowMultipleComponent]
    public partial class SuperInstaller : MonoInstaller
    {
        private void Reset()
        {
            var sceneContext = GetComponent<SceneContext>();
            if (sceneContext != null)
            {
                var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
                if (listInstallers.IndexOf(this) == -1)
                {
                    listInstallers.Add(this);
                }
                sceneContext.Installers = listInstallers;
            }
        }

        private void InstallSignals()
        {
            Container.DeclareSignal<ZenGenSomeSignalWithOptionalSubscriber>();
        }

        public override void InstallBindings()
        {
            InstallSignals();
        }
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void UsePredefinedInstaller()
    {
        string source = @"
using UnityEngine;
using Zenject;

[ZenGen(ZenGenTypeEnum.Signal, ""SuperInstaller"")]
public class ZenGenSomeSignalWithOptionalSubscriber
{
}

public partial class SuperInstaller {}

";
        var generator = new Generator();
        string output = this.GetGeneratedOutput(source, generator, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[AddComponentMenu(""Installers/SuperInstaller"")]
[DisallowMultipleComponent]
public partial class SuperInstaller : MonoInstaller
{
    private void Reset()
    {
        var sceneContext = GetComponent<SceneContext>();
        if (sceneContext != null)
        {
            var listInstallers = new List<MonoInstaller>(sceneContext.Installers);
            if (listInstallers.IndexOf(this) == -1)
            {
                listInstallers.Add(this);
            }
            sceneContext.Installers = listInstallers;
        }
    }

    private void InstallSignals()
    {
        Container.DeclareSignal<ZenGenSomeSignalWithOptionalSubscriber>();
    }

    public override void InstallBindings()
    {
        InstallSignals();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }
}