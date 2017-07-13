﻿using System.IO;
using System.Linq;
using UnityEngine;

public class ProfileNewPage : MonoBehaviour
{
    public TextMesh newNameText = null;
    public TextMesh[] toggleableLetters = null;
    public KMSelectable[] selectablesOnCreate = null;

    public ProfileSettingsPage profileSettingsPage = null;

    private bool _capsOn = false;

    private static readonly char[] INVALID_CHARACTERS = Path.GetInvalidFileNameChars();

    private void OnEnable()
    {
        if (newNameText != null)
        {
            newNameText.text = "";
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

    public void Create()
    {
        if (string.IsNullOrEmpty(newNameText.text) || !Profile.CanCreateProfile(newNameText.text))
        {
            return;
        }

        InputHelper.InvokeCancel();
        foreach(KMSelectable selectable in selectablesOnCreate)
        {
            selectable.InvokeSelect(false, true);
        }

        InputInvoker.Instance.Enqueue(delegate ()
        {
            profileSettingsPage.profile = Profile.CreateProfile(newNameText.text);
            profileSettingsPage.OnEnable();
        });
    }

    private void UpdateLetters()
    {
        foreach (TextMesh textMesh in toggleableLetters)
        {
            textMesh.text = _capsOn ? textMesh.text.ToUpper() : textMesh.text.ToLower();
        }
    }
}
