using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.Utils.Taskbar;

using Xenial.Framework.Deeplinks.Model;
using Xenial.Framework.Deeplinks.Win.Helpers;

namespace Xenial.Framework.Deeplinks.Win;

/// <summary>
/// 
/// </summary>
public sealed class TaskbarJumpListWindowController : WindowController
{
    private IModelOptionsJumplists? Jumplists
        => Application.Model.Options is IModelOptionsJumplists modelOptionsJumplists
            ? modelOptionsJumplists
            : null;

    /// <summary>
    /// 
    /// </summary>
    public TaskbarAssistant? TaskbarAssistant { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public TaskbarJumpListWindowController()
        => TargetWindowType = WindowType.Main;

    /// <summary>
    /// 
    /// </summary>
    protected override void OnActivated()
    {
        base.OnActivated();

        Active[$"{nameof(IModelJumplists)}{nameof(IModelJumplists.EnableJumpList)}"] =
            Jumplists?.Jumplists?.EnableJumpList ?? false;

        if (Active)
        {
            SetTaskbarAssistant(CreateTaskbarAssistant());
            if (Window is not null)
            {
                Window.TemplateChanged -= Window_TemplateChanged;
                Window.TemplateChanged += Window_TemplateChanged;
            }
        }
    }

    private void Window_TemplateChanged(object? sender, EventArgs e)
        => SetTaskbarAssistant(CreateTaskbarAssistant());

    /// <summary>
    /// 
    /// </summary>
    protected override void OnDeactivated()
    {
        base.OnDeactivated();
        if (Window is not null)
        {
            Window.TemplateChanged -= Window_TemplateChanged;
        }
        SetTaskbarAssistant(null);
    }

    /// <summary>
    /// 
    /// </summary>
    public TaskbarAssistant? CreateTaskbarAssistant()
    {
        var jumplistOptions = Jumplists;
        if (jumplistOptions?.Jumplists is null || !jumplistOptions.Jumplists.EnableJumpList)
        {
            return null;
        }

        var imageNames = jumplistOptions.Jumplists.CustomCategories
            .SelectMany(m => m)
            .OfType<IModelJumplistItemBase>()
            .Select(m => m.ImageName)
            .Concat(
            jumplistOptions.Jumplists.TaskCategory
                .OfType<IModelJumplistItemBase>()
                .Select(m => m.ImageName)
        )
            .Where(imageName => !string.IsNullOrEmpty(imageName))
            .Distinct()
            .ToList();

        var resourceManager = new RuntimeImageResourceManager(null);
        var icons = resourceManager.GenerateIcons(imageNames!);

        var taskbarAssistant = new TaskbarAssistant();

        foreach (var item in jumplistOptions.Jumplists.TaskCategory.OrderBy(m => m.Index))
        {
            InitJumpList(taskbarAssistant.JumpListTasksCategory, item, icons);
        }

        foreach (var itemCategory in jumplistOptions.Jumplists.CustomCategories.OrderBy(m => m.Index))
        {
            var category = new JumpListCategory(itemCategory.Caption);

            foreach (var item in itemCategory)
            {
                InitJumpList(category.JumpItems, item, icons);
            }

            taskbarAssistant.JumpListCustomCategories.Add(category);
        }

        return taskbarAssistant;
    }

    private void SetTaskbarAssistant(TaskbarAssistant? taskbarAssistant)
    {
        TaskbarAssistant?.Dispose();
        TaskbarAssistant = null;
        TaskbarAssistant = taskbarAssistant;
        if (taskbarAssistant is not null)
        {
            if (Window is WinWindow winWindow)
            {
                taskbarAssistant.ParentControl = winWindow.Form;
            }
        }
    }

    private static void InitJumpList(JumpListCategoryItemCollection collection, IModelJumplistItem item,
        IDictionary<string, string> icons)
        => collection.Add(CreateJumpListItem(item, icons));

    private static IJumpListItem CreateJumpListItem(
            IModelJumplistItem item,
            IDictionary<string, string> icons)
    {
        if (item is IModelJumplistItemSeperator)
        {
            return new JumpListItemSeparator();
        }

        if (item is IModelJumplistItemBase baseItem)
        {
            var (icoPath, iconIndex) = icons.TryGetValue(baseItem.ImageName ?? "", out var iconPath) switch
            {
                true => (iconPath, 0),
                false => (null, -1)
            };

            if (item is IModelJumplistItemLaunch launcher)
            {
                return new JumpListItemTask(launcher.Caption)
                {
                    Arguments = launcher.Arguments,
                    Path = launcher.ProcessPath,
                    WorkingDirectory = launcher.WorkingDirectory,
                    IconPath = icoPath,
                    IconIndex = iconIndex,
                };
            }

            if (item is IModelJumplistItemProtocol protocol)
            {
                return new JumpListItemTask(protocol.Caption)
                {
                    Path = protocol.LaunchUri,
                    IconPath = icoPath,
                    IconIndex = iconIndex,
                };
            }

            if (item is IModelJumplistItemBase view)
            {
                return view.Protocol is null
                    ? new JumpListItemTask(view.Caption)
                    {
                        Path = System.Windows.Forms.Application.ExecutablePath,
                        Arguments = view.Arguments,
                        IconPath = icoPath,
                        IconIndex = iconIndex,
                    }
                    : new JumpListItemTask(view.Caption)
                    {
                        Path = view.LaunchUri,
                        IconPath = icoPath,
                        IconIndex = iconIndex,
                    };
            }
        }

        throw new InvalidOperationException($"Can not create JumplistItem from type {item.GetType()} {item}");
    }

}
