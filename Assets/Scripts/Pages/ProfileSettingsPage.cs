using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class ProfileSettingsPage : MonoBehaviour
{
    public ModTogglePage ModTogglePagePrefab = null;
    public ProfileCopyPage CopyPagePrefab = null;
    public ProfileRenamePage RenamePagePrefab = null;
    public ProfileConfirmDeletePage ConfirmDeletePagePrefab = null;

    public Profile Profile = null;
    public UIElement RegularModulesSelectable = null;
    public UIElement NeedyModulesSelectable = null;
    public UIElement ServicesSelectable = null;
    public UIElement BombCasingsSelectable = null;
    public UIElement GameplayRoomsSelectable = null;
    public UIElement WidgetsSelectable = null;
    public UIElement SetOperationSelectable = null;

    private Page _page = null;

    private void Awake()
    {
        _page = GetComponent<Page>();
    }

    public void OnEnable()
    {
        _page.HeaderText = string.Format("<b>{0}</b>\n<size=16>Profile Settings</size>", Profile == null ? "**NULL**" : Profile.FriendlyName);

        SetProfileCategoryCounts((UIProfileCategoryElement)RegularModulesSelectable, ModSelectorService.ModType.SolvableModule);
        SetProfileCategoryCounts((UIProfileCategoryElement)NeedyModulesSelectable, ModSelectorService.ModType.NeedyModule);
        SetProfileCategoryCounts((UIProfileCategoryElement)ServicesSelectable, ModSelectorService.ModType.Service);
        SetProfileCategoryCounts((UIProfileCategoryElement)BombCasingsSelectable, ModSelectorService.ModType.Bomb);
        SetProfileCategoryCounts((UIProfileCategoryElement)GameplayRoomsSelectable, ModSelectorService.ModType.GameplayRoom);
        SetProfileCategoryCounts((UIProfileCategoryElement)WidgetsSelectable, ModSelectorService.ModType.Widget);

        SetOperationSelectable.Text = GetSetOperationString();
        SetOperationSelectable.BackgroundHighlight.UnselectedColor = Profile != null ? Profile.Operation.GetColor() : Color.white;
    }

    private void SortModulesNames(KeyValuePair<string, string>[] names)
    {
        System.Array.Sort(names, (x, y) => string.Compare(x.Value, y.Value));
    }

    public void EditSolvableModules()
    {
        ModTogglePage modTogglePage = _page.GetPageWithComponent(ModTogglePagePrefab);
        modTogglePage.Profile = Profile;
        modTogglePage.ModType = ModSelectorService.ModType.SolvableModule;
        modTogglePage.Entries = ModSelectorService.Instance.GetModNamesAndDisplayNames(ModSelectorService.ModType.SolvableModule).ToArray();

        SortModulesNames(modTogglePage.Entries);
        _page.GoToPage(ModTogglePagePrefab);
    }

    public void EditNeedyModules()
    {
        ModTogglePage modTogglePage = _page.GetPageWithComponent(ModTogglePagePrefab);
        modTogglePage.Profile = Profile;
        modTogglePage.ModType = ModSelectorService.ModType.NeedyModule;
        modTogglePage.Entries = ModSelectorService.Instance.GetModNamesAndDisplayNames(ModSelectorService.ModType.NeedyModule).ToArray();

        SortModulesNames(modTogglePage.Entries);
        _page.GoToPage(ModTogglePagePrefab);
    }

    public void EditBombs()
    {
        ModTogglePage modTogglePage = _page.GetPageWithComponent(ModTogglePagePrefab);
        modTogglePage.Profile = Profile;
        modTogglePage.ModType = ModSelectorService.ModType.Bomb;
        modTogglePage.Entries = ModSelectorService.Instance.GetModNamesAndDisplayNames(ModSelectorService.ModType.Bomb).ToArray();

        SortModulesNames(modTogglePage.Entries);
        _page.GoToPage(ModTogglePagePrefab);
    }

    public void EditGameplayRooms()
    {
        ModTogglePage modTogglePage = _page.GetPageWithComponent(ModTogglePagePrefab);
        modTogglePage.Profile = Profile;
        modTogglePage.ModType = ModSelectorService.ModType.GameplayRoom;
        modTogglePage.Entries = ModSelectorService.Instance.GetModNamesAndDisplayNames(ModSelectorService.ModType.GameplayRoom).ToArray();

        SortModulesNames(modTogglePage.Entries);
        _page.GoToPage(ModTogglePagePrefab);
    }

    public void EditWidgets()
    {
        ModTogglePage modTogglePage = _page.GetPageWithComponent(ModTogglePagePrefab);
        modTogglePage.Profile = Profile;
        modTogglePage.ModType = ModSelectorService.ModType.Widget;
        modTogglePage.Entries = ModSelectorService.Instance.GetModNamesAndDisplayNames(ModSelectorService.ModType.Widget).ToArray();

        SortModulesNames(modTogglePage.Entries);
        _page.GoToPage(ModTogglePagePrefab);
    }

    public void EditServices()
    {
        ModTogglePage modTogglePage = _page.GetPageWithComponent(ModTogglePagePrefab);
        modTogglePage.Profile = Profile;
        modTogglePage.ModType = ModSelectorService.ModType.Service;
        modTogglePage.Entries = ModSelectorService.Instance.GetModNamesAndDisplayNames(ModSelectorService.ModType.Service).ToArray();

        SortModulesNames(modTogglePage.Entries);
        _page.GoToPage(ModTogglePagePrefab);
    }

    public void CycleSetOperation()
    {
        switch (Profile.Operation)
        {
            case Profile.SetOperation.Expert:
                Profile.SetSetOperation(Profile.SetOperation.Defuser);
                break;
            case Profile.SetOperation.Defuser:
                Profile.SetSetOperation(Profile.SetOperation.Expert);
                break;

            default:
                break;
        }

        OnEnable();
    }

    public void Copy()
    {
        _page.GetPageWithComponent(CopyPagePrefab).Profile = Profile;
        _page.GoToPage(CopyPagePrefab);
    }

    public void Rename()
    {
        _page.GetPageWithComponent(RenamePagePrefab).Profile = Profile;
        _page.GoToPage(RenamePagePrefab);
    }

    public void Delete()
    {
        _page.GetPageWithComponent(ConfirmDeletePagePrefab).Profile = Profile;
        _page.GoToPage(ConfirmDeletePagePrefab);
    }

    private void SetProfileCategoryCounts(UIProfileCategoryElement element, ModSelectorService.ModType modType)
    {
        element.TotalText = Profile.GetTotalOfType(modType).ToString();
        if (Profile != null)
        {
            if (Profile.Operation == Profile.SetOperation.Expert && modType != ModSelectorService.ModType.SolvableModule &&
                modType != ModSelectorService.ModType.NeedyModule && modType != ModSelectorService.ModType.Widget)
            {
                element.CanSelect = false;
                element.DisabledText = "-";
                element.EnabledText = "-";
            }
            else
            {
                element.CanSelect = true;
                element.DisabledText = Profile.GetDisabledTotalOfType(modType).ToString();
                element.EnabledText = Profile.GetEnabledTotalOfType(modType).ToString();
            }
        }
        else
        {
            element.CanSelect = false;
            element.DisabledText = "-";
            element.EnabledText = "-";
        }
    }

    private string GetSetOperationString()
    {
        if (Profile != null)
        {
            string explanation = null;
            switch (Profile.Operation)
            {
                case Profile.SetOperation.Expert:
                    explanation = "<b>Experting</b> <i>(Disable mods if all other experting profiles agree)</i>";
                    break;
                case Profile.SetOperation.Defuser:
                    explanation = "<b>Defusing</b> <i>(Disable mods regardless of other profiles)</i>";
                    break;

                default:
                    break;
            }

            return string.Format("Set Operation: {0}", explanation);
        }

        return "Set Operation: <b>**NULL**</b>";
    }
}
