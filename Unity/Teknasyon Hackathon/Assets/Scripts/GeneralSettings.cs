using System;
using UnityEngine;

[CreateAssetMenu(fileName = " GeneralSettings")]
public class GeneralSettings : ScriptableObject
{
    private static GeneralSettings _GeneralSettings;

    private static GeneralSettings generalSettings
    {
        get
        {
            if (!_GeneralSettings)
            {
                _GeneralSettings = Resources.Load<GeneralSettings>($"GeneralSettings");

                if (!_GeneralSettings)
                {
                    throw new Exception("Global settings could not be loaded");
                }
            }

            return _GeneralSettings;
        }
    }

    public static GeneralSettings Get()
    {
        return generalSettings;
    }

    public bool FreeVersion;
}