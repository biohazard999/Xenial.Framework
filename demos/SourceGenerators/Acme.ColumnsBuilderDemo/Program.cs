using System;
using System.Linq;

using Acme.Module.BusinessObjects;

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;

using static Acme.Module.Helpers.ApplicationModelCreator;
using static Acme.Module.Helpers.VisualizeNodeHelper;

var application = CreateModel(typeof(Person), typeof(Address), typeof(Country));

var modelListView = application
    .Views
    .OfType<IModelListView>()
    .First(view => view.Id == ModelNodeIdHelper.GetListViewId(typeof(Person)));

modelListView.PrintModelNode();
