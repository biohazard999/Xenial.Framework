using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Bogus;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

using Xenial.Framework.Model.GeneratorUpdaters;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Model.GeneratorUpdaters
{
    public static class ModelOptionsNodesGeneratorUpdaterFacts
    {
        public static void ModelOptionsNodesGeneratorUpdaterTests() => Describe(nameof(ModelOptionsNodesGeneratorUpdater), () =>
        {
            static T CreateInstance<T>(params object[] args)
                where T : class
            {
                var type = typeof(T);

                var instance = type.Assembly.CreateInstance(
                    type.FullName!, false,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null, args, null, null);

                _ = instance ?? throw new ArgumentNullException(nameof(instance));

                return (T)instance;
            }

            var properties = ModelApplicationCreatorProperties.CreateDefault();
            var creator = CreateInstance<ModelApplicationCreator>(properties);
            var node = creator.CreateNode(string.Empty, typeof(IModelOptions));

            //TODO: Check if this is done by ModelApplicationCreator
            var gen = new ModelOptionsNodesGenerator();
            gen.GenerateNodes(node);

            bool CreateUpdater(Func<ApplicationOptions, IModelOptions, bool> options)
            {
                var faker = new Faker<ApplicationOptions>()
                    .RuleFor(f => f.DataAccessMode, (f, _) => f.PickRandom<CollectionSourceDataAccessMode>())
                    .RuleFor(f => f.LookupSmallCollectionItemCount, (f, _) => f.Random.Int())
                    .Generate();

                var updater = new ModelOptionsNodesGeneratorUpdater(faker);
                updater.UpdateNode(node);

                return options(faker, (IModelOptions)node);
            }

            It($"should assign {nameof(IModelOptions.DataAccessMode)}",
                () => CreateUpdater((options, model) => model.DataAccessMode == options.DataAccessMode)
            );

            It($"should assign {nameof(IModelOptions.LookupSmallCollectionItemCount)}",
                () => CreateUpdater((options, model) => model.LookupSmallCollectionItemCount == options.LookupSmallCollectionItemCount)
            );
        });
    }
}
