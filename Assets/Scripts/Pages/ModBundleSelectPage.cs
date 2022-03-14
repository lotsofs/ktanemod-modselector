using System.Collections.Generic;
using System.Linq;

public class ModBundleSelectPage : Pagination<ModSelectorService.ModWrapper, UIElement>
{
    public ModBundleInfoPage InfoPagePrefab = null;

    public FilterButton FilterButton = null;
    public UIElement[] Options = null;

    protected override ModSelectorService.ModWrapper[] ValueCollection
    {
        get
        {
            return _modWrappers;
        }
    }

    protected override UIElement[] ElementCollection
    {
        get
        {
            return Options;
        }
    }

    private IEnumerable<ModSelectorService.ModWrapper> FilteredModWrappers
    {
        get
        {
            IEnumerable<ModSelectorService.ModWrapper> modWrappers = ModSelectorService.Instance.GetModWrappers();

            string filter = FilterButton.FilterText.ToLowerInvariant();
            if (!string.IsNullOrEmpty(filter))
            {
                modWrappers = modWrappers.Where((x) => x.ModTitle.ToLowerInvariant().Contains(filter));
            }

            return modWrappers;
        }
    }

    private ModSelectorService.ModWrapper[] _modWrappers = null;

    protected override void Awake()
    {
        base.Awake();

        for (int optionIndex = 0; optionIndex < Options.Length; ++optionIndex)
        {
            int localOptionIndex = optionIndex;

            KMSelectable selectable = Options[optionIndex].GetComponent<KMSelectable>();
            selectable.OnInteract += delegate()
            {
                Page.GetPageWithComponent(InfoPagePrefab).ModWrapper = _modWrappers[ValueOffset + localOptionIndex];
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

        _modWrappers = FilteredModWrappers.ToArray();
        System.Array.Sort(_modWrappers, (x, y) => string.Compare(x.ModTitle, y.ModTitle));
        SetPage(0);
    }

    public override void SetPage(int pageIndex, int pageOffset = 0)
    {
        if (_modWrappers == null)
        {
            return;
        }

        base.SetPage(pageIndex, pageOffset);

        Page.HeaderText = string.Format("<b>Select Mod For Info...</b>\n<size=16>{0}</size>", PageName);
    }

    protected override void SetElement(ModSelectorService.ModWrapper value, UIElement element)
    {
        element.Text = value.ModTitle;
    }

    private void FilterTextChange(string filterText)
    {
        _modWrappers = FilteredModWrappers.ToArray();
        System.Array.Sort(_modWrappers, (x, y) => string.Compare(x.ModTitle.Replace("xxxzzzThe ", ""), y.ModTitle.Replace("xxxzzzThe ", "")));
        SetPage(0);
    }
}
