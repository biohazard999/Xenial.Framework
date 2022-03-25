using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils.Design;

namespace Xenial.Design.Engine;

public class StandaloneModelEditorModelLoader
{
    private FileModelStore? fileModelStore;
    private IModelApplication? modelApplication;

    public StandaloneModelEditorModelLoader()
    {
        XafTypesInfo.HardReset();
        ImageLoader.Reset();
        ImageLoader.Instance.IsDesignMode = true;
    }

    private static IEnumerable<Type> GetRegularTypes(IList<ModuleBase> modules)
    {
        var regularTypes1 = new List<Type>();
        foreach (var module in (IEnumerable<ModuleBase>)modules)
        {
            var regularTypes2 = ModuleHelper.GetRegularTypes(module);
            if (regularTypes2 != null)
            {
                regularTypes1.AddRange(regularTypes2);
            }
        }
        return regularTypes1;
    }

    private static void InitializeTypeInfoSources(IList<ModuleBase> modules, string assembliesPath)
        => DefaultTypesInfoInitializer.Initialize((TypesInfo)XafTypesInfo.Instance, baseType => GetRegularTypes(modules).Where(type => baseType.IsAssignableFrom(type)), (assemblyName, typeName) => DefaultTypesInfoInitializer.CreateTypesInfoInitializer(assembliesPath, assemblyName, typeName));

    public void LoadModel(
      string targetFileName,
      string modelDifferencesStorePath,
      string deviceSpecificDifferencesStoreName,
      string? assembliesPath)
    {
#if FULL_FRAMEWORK
        //DesignTimeTools.isDesignModeCore = true;
        var field = typeof(DesignTimeTools).GetField("isDesignModeCore",
                    BindingFlags.Static |
                    BindingFlags.NonPublic);

        // Normally the first argument to "SetValue" is the instance
        // of the type but since we are mutating a static field we pass "null"
        field.SetValue(null, true);
#endif

        if (string.IsNullOrEmpty(assembliesPath))
        {
            assembliesPath = Path.GetDirectoryName(targetFileName);
            if (string.IsNullOrEmpty(assembliesPath))
            {
                assembliesPath = Environment.CurrentDirectory;
            }
        }
        fileModelStore = null;
        var designerModelFactory = new DesignerModelFactory();
        modelApplication = null;
        if (designerModelFactory.IsApplication(targetFileName))
        {
            if (string.IsNullOrEmpty(modelDifferencesStorePath))
            {
                modelDifferencesStorePath = assembliesPath;
            }

            var applicationByConfigFile = designerModelFactory.CreateApplicationByConfigFile(targetFileName, /*MainClass.targetDllFileName*/ /*TODO: ModelDLL*/null, ref assembliesPath);
            InitializeTypeInfoSources(applicationByConfigFile.Modules, assembliesPath);
            if (string.IsNullOrEmpty(deviceSpecificDifferencesStoreName))
            {
                fileModelStore = designerModelFactory.CreateApplicationModelStore(modelDifferencesStorePath);
                modelApplication = designerModelFactory.CreateApplicationModel(applicationByConfigFile, designerModelFactory.CreateModulesManager(applicationByConfigFile, targetFileName, assembliesPath), targetFileName, fileModelStore);
            }
            else
            {
                var applicationModelStore = designerModelFactory.CreateApplicationModelStore(modelDifferencesStorePath);
                fileModelStore = designerModelFactory.CreateApplicationModelStore(modelDifferencesStorePath, deviceSpecificDifferencesStoreName);
                modelApplication = designerModelFactory.CreateApplicationModel(applicationByConfigFile, designerModelFactory.CreateModulesManager(applicationByConfigFile, targetFileName, assembliesPath), targetFileName, applicationModelStore, fileModelStore);
            }
        }
        else
        {
            try
            {
                var moduleFromFile = designerModelFactory.CreateModuleFromFile(targetFileName, assembliesPath);

                var modules = new List<ModuleBase>
                {
                    moduleFromFile
                };

                foreach (var requiredModuleType in (IEnumerable<Type>)moduleFromFile.RequiredModuleTypes)
                {
                    if (Activator.CreateInstance(requiredModuleType) is ModuleBase module)
                    {
                        modules.Add(module);
                    }
                }

                InitializeTypeInfoSources(modules, assembliesPath);
                if (string.IsNullOrEmpty(modelDifferencesStorePath))
                {
                    modelDifferencesStorePath = assembliesPath;
                }

                fileModelStore = designerModelFactory.CreateModuleModelStore(modelDifferencesStorePath);
                modelApplication = designerModelFactory.CreateApplicationModel(moduleFromFile, designerModelFactory.CreateModulesManager(moduleFromFile, assembliesPath), fileModelStore);
            }
            catch (ArgumentException ex)
            {
                if (!ex.Message.Contains("assembly doesn't contain a ModuleBase descendants"
#if NET5_0_OR_GREATER
                    , StringComparison.OrdinalIgnoreCase
#endif
                ))
                {
                    throw;
                }
                else
                {
                    var applicationFromFile = designerModelFactory.CreateApplicationFromFile(targetFileName, assembliesPath);
                    if (applicationFromFile == null)
                    {
                        throw;
                    }
                    else
                    {
                        InitializeTypeInfoSources(applicationFromFile.Modules, assembliesPath);
                        fileModelStore = designerModelFactory.CreateApplicationModelStore(modelDifferencesStorePath);
                        modelApplication = designerModelFactory.CreateApplicationModel(applicationFromFile, designerModelFactory.CreateModulesManager(applicationFromFile, null, assembliesPath), null, fileModelStore);
                    }
                }
            }
        }
        CaptionHelper.Setup(modelApplication);
    }

    public FileModelStore? FileModelStore => fileModelStore;

    public IModelApplication? ModelApplication => modelApplication;
}
