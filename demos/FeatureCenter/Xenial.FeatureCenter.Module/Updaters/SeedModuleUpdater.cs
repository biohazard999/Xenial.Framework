using System;
using System.Collections.Generic;
using System.Text;

using Bogus;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

using Xenial.FeatureCenter.Module.BusinessObjects.Editors;

namespace Xenial.FeatureCenter.Module.Updaters
{
    public class SeedModuleUpdater : ModuleUpdater
    {
        public SeedModuleUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            ObjectSpace.EnsureSingletons();

            //if (ObjectSpace.GetObjectsCount(typeof(TokenEditorPersistentTokens), null) == 0)
            //{
            //    var faker = new Faker<TokenEditorPersistentTokens>()
            //        .CustomInstantiator(f => ObjectSpace.CreateObject<TokenEditorPersistentTokens>())
            //        .RuleFor(r => r.Name, f => f.Name.FirstName());

            //    var tokens = faker.Generate(100);

            //    ObjectSpace.CommitChanges();
            //}
        }
    }
}
