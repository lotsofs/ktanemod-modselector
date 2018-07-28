﻿using System.Collections.Generic;
using UnityEngine;

public class PageNavigation : MonoBehaviour
{
    public PageManager PageManager = null;

    public KMSelectable MainMenuPrefab = null;

    public KMSelectable RootSelectable = null;
    public KMSelectable TempSelectable = null;

    private Stack<KMSelectable> _backStack = new Stack<KMSelectable>();

    private void Awake()
    {
        RootSelectable = GetComponent<KMSelectable>();

        GoToPage(MainMenuPrefab.name);
    }

    public void GoToPage(string pageName)
    {
        KMSelectable page = PageManager[pageName];
        if (page == null)
        {
            return;
        }

        if (_backStack.Count > 0)
        {
            SwapPages(_backStack.Peek(), page);
        }
        else
        {
            EnablePage(page);
        }

        _backStack.Push(page);
    }

    public void GoBack()
    {
        if (_backStack.Count <= 1)
        {
            //This should do a drop of the holdable in theory
            return;
        }

        KMSelectable oldPage = _backStack.Pop();
        KMSelectable newPage = _backStack.Peek();
        SwapPages(oldPage, newPage);
    }

    private bool OnCancel()
    {
        if (_backStack.Count > 1)
        {
            GoBack();
            return false;
        }

        return true;
    }

    private void SwapPages(KMSelectable oldPage, KMSelectable newPage)
    {
        RootSelectable.Children = new KMSelectable[1] { TempSelectable };
        RootSelectable.ChildRowLength = 1;
        RootSelectable.DefaultSelectableIndex = 0;

        foreach (KMSelectable selectable in oldPage.Children)
        {
            if (selectable != null)
            {
                selectable.Parent = oldPage;
                selectable.OnCancel -= OnCancel;
            }
        }

        RootSelectable.Reproxy();
        oldPage.gameObject.SetActive(false);

        if (newPage != null)
        {
            EnablePage(newPage);
        }
        else
        {
            RootSelectable.Reproxy();
        }
    }

    private void EnablePage(KMSelectable page)
    {
        RootSelectable.Children = page.Children;
        RootSelectable.ChildRowLength = page.ChildRowLength;
        RootSelectable.DefaultSelectableIndex = page.DefaultSelectableIndex;
        foreach (KMSelectable selectable in page.Children)
        {
            if (selectable != null)
            {
                selectable.Parent = RootSelectable;
                selectable.OnCancel += OnCancel;
                selectable.Reproxy();
            }
        }
        page.gameObject.SetActive(true);

        RootSelectable.Reproxy();
        RootSelectable.UpdateChildren(RootSelectable.Children[RootSelectable.DefaultSelectableIndex]);
    }
}
