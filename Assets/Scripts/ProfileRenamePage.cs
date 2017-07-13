﻿using System.IO;
using System.Linq;
using UnityEngine;

public class ProfileRenamePage : MonoBehaviour
{
    public Profile profile = null;

    public TextMesh newNameText = null;
    public TextMesh[] toggleableLetters = null;
    public TabletPage returnPage = null;

    private TabletPage _tabletPage = null;
    private bool _capsOn = false;

    private static readonly char[] INVALID_CHARACTERS = Path.GetInvalidFileNameChars();

    private void Awake()
    {
        _tabletPage = GetComponent<TabletPage>();
    }

    private void OnEnable()
    {
        if (_tabletPage != null && _tabletPage.header != null)
        {
            _tabletPage.header.text = string.Format("<b>{0}</b>\n<size=16>Rename Profile</size>", profile == null ? "**NULL**" : profile.FriendlyName);
        }

        if (newNameText != null)
        {
            newNameText.text = profile == null ? "NULL" : profile.FriendlyName;
        }

        _capsOn = true;
        UpdateLetters();
    }

    private void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                DeleteCharacter();
            }
            else if (!INVALID_CHARACTERS.Contains(c))
            {
                newNameText.text += c.ToString();
            } 
        }

        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            ToogleCaps();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            _capsOn = true;
            UpdateLetters();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            _capsOn = false;
            UpdateLetters();
        }
    }

    public void ToogleCaps()
    {
        _capsOn = !_capsOn;
        UpdateLetters();
    }

    public void AddCharacter(string text)
    {
        newNameText.text += _capsOn ? text.ToUpper() : text.ToLower();

        if (_capsOn)
        {
            _capsOn = false;
            UpdateLetters();
        }
    }

    public void DeleteCharacter()
    {
        if (newNameText.text.Length == 0)
        {
            return;
        }

        newNameText.text = newNameText.text.Substring(0, newNameText.text.Length - 1);

        if (_capsOn)
        {
            _capsOn = false;
            UpdateLetters();
        }
    }

    public void Apply()
    {
        if (string.IsNullOrEmpty(newNameText.text) || !Profile.CanCreateProfile(newNameText.text))
        {            
            return;
        }

        profile.Rename(newNameText.text);
        InputHelper.InvokeCancel();
    }

    private void UpdateLetters()
    {
        foreach (TextMesh textMesh in toggleableLetters)
        {
            textMesh.text = _capsOn ? textMesh.text.ToUpper() : textMesh.text.ToLower();
        }
    }
}
