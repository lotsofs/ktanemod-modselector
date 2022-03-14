using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModActivitySelectPage : Pagination<KeyValuePair<string, string>, UIElement>
{ 
    public ModActivityInfoPage InfoPagePrefab = null;

    public FilterButton FilterButton = null;
    public UIElement[] Options = null;

    public Color EnabledColor = Color.white;
    public Color DisabledColor = Color.white;

    protected override KeyValuePair<string, string>[] ValueCollection
    {
        get
        {
            return _mods;
        }
    }

    protected override UIElement[] ElementCollection
    {
        get
        {
            return Options;
        }
    }

    private IEnumerable<KeyValuePair<string, string>> FilteredModNames
    {
        get
        {
            IEnumerable<KeyValuePair<string, string>> modNames = ModSelectorService.Instance.GetAllModNamesAndDisplayNames();

            string filter = FilterButton.FilterText.ToLowerInvariant();
            if (!string.IsNullOrEmpty(filter))
            {
                modNames = modNames.Where((x) => x.Key.ToLowerInvariant().Contains(filter) || x.Value.ToLowerInvariant().Contains(filter));
            }

            return modNames.OrderBy((x) => ProfileManager.ActiveDisableSet.Contains(x.Key)).ThenBy((y) => y.Value);
        }
    }

    private KeyValuePair<string, string>[] _mods = null;

    protected override void Awake()
    {
        base.Awake();

        for (int optionIndex = 0; optionIndex < Options.Length; ++optionIndex)
        {
            int localOptionIndex = optionIndex;

            KMSelectable selectable = Options[optionIndex].GetComponent<KMSelectable>();
            selectable.OnInteract += delegate ()
            {
                Page.GetPageWithComponent(InfoPagePrefab).ModNameAndDisplayName = _mods[ValueOffset + localOptionIndex];
                Page.GoToPage(InfoPagePrefab);
                return true;
            };
        }

        FilterButton.FilterTextChange.AddListener(FilterTextChange);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        FilterButton.FilterTextChange.RemoveListener(FilterTextChange);
    }

    private void OnEnable()
    {
        if (ModSelectorService.Instance == null)
        {
            return;
        }

        //Detailed information is not automatically updated all the time - do it here now
        ProfileManager.UpdateProfileSelectionDetails();

        _mods = FilteredModNames.ToArray();
        SetPage(0);
    }

    public override void SetPage(int pageIndex, int pageOffset = 0)
    {
        if (_mods == null)
        {
            return;
        }

        base.SetPage(pageIndex, pageOffset);

        Page.HeaderText = string.Format("<b>Enabled/Disabled Mods</b>\n<size=16>{0}</size>", PageName);

        if (_mods.Length == 0)
        {
            KMSelectable pageSelectable = GetComponent<KMSelectable>();
            pageSelectable.DefaultSelectableIndex = 1;
            pageSelectable.Reproxy();
            return;
        }
    }

    protected override void SetElement(KeyValuePair<string, string> value, UIElement element)
    {
        element.Text = value.Value;
        element.BackgroundHighlight.UnselectedColor = ProfileManager.ActiveDisableSet.Contains(value.Key) ? DisabledColor : EnabledColor;
    }

    private void FilterTextChange(string filterText)
    {
        _mods = FilteredModNames.ToArray();
        SetPage(0);
    }
}
