using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;

namespace Xenial.FeatureCenter.Module.Model.GeneratorUpdaters
{
    public sealed partial class FeatureCenterNavigationItemNodesUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator>
    {
        private static readonly Dictionary<string, string> imageNames = new()
        {
            ["ModelBuilders"] = "direction1",
            ["Editors"] = "EditNames"
        };

        public override void UpdateNode(ModelNode node)
        {
            const string defaultGroupName = "Default";

            var nodesDictionary =
                new Dictionary<string, List<IModelNavigationItem>>
                {
                    {defaultGroupName, new List<IModelNavigationItem>()}
                };

            if (node is IModelRootNavigationItems rootNavigationItems)
            {
                foreach (var item in rootNavigationItems.Items)
                {
                    CollectNodes(item);
                }

                rootNavigationItems.Items.ClearNodes();

                foreach (var groupName in nodesDictionary.Keys)
                {
                    var newItem = rootNavigationItems.Items.AddNode<IModelNavigationItem>(groupName);
                    if (!imageNames.TryGetValue(newItem.Id, out var imageName))
                    {
                        imageName = null;
                    }

                    newItem.ImageName = imageName;

                    var newNavItems = nodesDictionary[groupName];
                    foreach (var navItem in newNavItems)
                    {
                        var newNavItem = newItem.Items.AddNode<IModelNavigationItem>(navItem.Id);

                        CopyPropertiesTo(navItem, newNavItem);
                        if (newNavItem is IModelBaseChoiceActionItem newChoiceActionItem && navItem is IModelBaseChoiceActionItem choiceActionItem)
                        {
                            newChoiceActionItem.Caption = choiceActionItem.Caption;
                        }

                        if (newNavItem.View is IModelObjectView objectView && objectView.ModelClass.TypeInfo.IsAttributeDefined<FeatureStatusAttribute>(true))
                        {
                            var attribute = objectView.ModelClass.TypeInfo.FindAttribute<FeatureStatusAttribute>(true);
                            if (attribute is not null)
                            {
                                newNavItem.SetXenialStaticBadgeProperties(modelBadgeStaticTextItem =>
                                {
                                    modelBadgeStaticTextItem.XenialBadgeStaticText = attribute.BadgeText;
                                    modelBadgeStaticTextItem.XenialBadgeStaticPaintStyle = attribute.BadgePaintStyle;
                                });
                            }
                        }
                    }
                }
            }

            void AddOrAppendToDictionary(string groupName, IModelNavigationItem item)
            {
                if (nodesDictionary is not null)
                {
                    if (nodesDictionary.TryGetValue(groupName, out var modelNavigationItems))
                    {
                        modelNavigationItems.Add(item);
                    }
                    else
                    {
                        nodesDictionary.Add(groupName, new List<IModelNavigationItem> { item });
                    }
                }
            }

            void AddToDictionary(IModelNavigationItem item, string? caption)
            {
                if (caption?.Contains("-") == true)
                {
                    var groupName = caption.Split('-')[0].Trim();
                    var newCaption = caption.Split('-')[1].Trim();
                    item.Caption = newCaption;

                    if (item is IModelBaseChoiceActionItem choiceActionItem)
                    {
                        choiceActionItem.Caption = newCaption;
                    }
                    AddOrAppendToDictionary(groupName, item);
                }
                else
                {
                    AddOrAppendToDictionary(defaultGroupName, item);
                }
            }

            void CollectNodes(IModelNavigationItem item)
            {
                if (item.View is IModelObjectView modelObjectView)
                {
                    AddToDictionary(item, item.Caption);
                }

                foreach (var nestedNode in item.Items)
                {
                    CollectNodes(nestedNode);
                }
            }

            static void CopyPropertiesTo<T>(T source, T dest)
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        property.SetValue(dest, property.GetValue(source, null), null);
                    }
                }
            }
        }
    }
}
