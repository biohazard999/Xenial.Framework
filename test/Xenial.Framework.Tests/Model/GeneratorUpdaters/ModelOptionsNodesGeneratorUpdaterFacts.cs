using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Bogus;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Utils;

using Shouldly;

using Xenial.Framework.Model.GeneratorUpdaters;

using static Xenial.Tasty;

using Locations = DevExpress.Persistent.Base.Locations;

namespace Xenial.Framework.Tests.Model.GeneratorUpdaters
{
    /// <summary>   A model options nodes generator updater facts. </summary>
    public static class ModelOptionsNodesGeneratorUpdaterFacts
    {
        /// <summary>   Model options nodes generator updater tests. </summary>
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
                var layout = new Faker<LayoutOptions>()
                    .RuleFor(f => f.CaptionColon, (f, _) => f.Random.String())
                    .RuleFor(f => f.EnableCaptionColon, (f, _) => f.Random.Bool())
                    .RuleFor(f => f.CaptionLocation, (f, _) => f.PickRandom<Locations>())
                    .RuleFor(f => f.CaptionHorizontalAlignment, (f, _) => f.PickRandom<HorzAlignment>())
                    .RuleFor(f => f.CaptionVerticalAlignment, (f, _) => f.PickRandom<VertAlignment>())
                    .RuleFor(f => f.CaptionWordWrap, (f, _) => f.PickRandom<WordWrap>())
                    .RuleFor(f => f.EnableLayoutGroupImages, (f, _) => f.Random.Bool())
                    .Generate();

                var faker = new Faker<ApplicationOptions>()
                    .RuleFor(f => f.DataAccessMode, (f, _) => f.PickRandom<CollectionSourceDataAccessMode>())
                    .RuleFor(f => f.LookupSmallCollectionItemCount, (f, _) => f.Random.Int())
                    .RuleFor(f => f.Layout, (f, _) => layout)
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

            It($"should throw with null {nameof(IModelOptions.LayoutManagerOptions)}",
                () => Should.Throw<ArgumentNullException>(() => new ApplicationOptions { Layout = null! })
            );

            Describe($"should assign {nameof(IModelOptions.LayoutManagerOptions)}", () =>
            {
                It($"{nameof(IModelLayoutManagerOptions.CaptionColon)}",
                    () => CreateUpdater((options, model) => model.LayoutManagerOptions.CaptionColon == options.Layout.CaptionColon)
                );

                It($"{nameof(IModelLayoutManagerOptions.EnableCaptionColon)}",
                   () => CreateUpdater((options, model) => model.LayoutManagerOptions.EnableCaptionColon == options.Layout.EnableCaptionColon)
                );

                It($"{nameof(IModelLayoutManagerOptions.CaptionLocation)}",
                   () => CreateUpdater((options, model) => model.LayoutManagerOptions.CaptionLocation == options.Layout.CaptionLocation)
                );

                It($"{nameof(IModelLayoutManagerOptions.CaptionHorizontalAlignment)}",
                    () => CreateUpdater((options, model) => model.LayoutManagerOptions.CaptionHorizontalAlignment == options.Layout.CaptionHorizontalAlignment)
                );

                It($"{nameof(IModelLayoutManagerOptions.CaptionVerticalAlignment)}",
                    () => CreateUpdater((options, model) => model.LayoutManagerOptions.CaptionVerticalAlignment == options.Layout.CaptionVerticalAlignment)
                );

                It($"{nameof(IModelLayoutManagerOptions.CaptionWordWrap)}",
                    () => CreateUpdater((options, model) => model.LayoutManagerOptions.CaptionWordWrap == options.Layout.CaptionWordWrap)
                );

                It($"{nameof(IModelLayoutManagerOptions.EnableLayoutGroupImages)}",
                    () => CreateUpdater((options, model) => model.LayoutManagerOptions.EnableLayoutGroupImages == options.Layout.EnableLayoutGroupImages)
                );
            });
        });
    }
}
