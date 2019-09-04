using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using UnityEditor;
using UnityEngine;
using PlistCS;

public class APPWindow : EditorWindow
{
    [MenuItem("Window/PlayerPrefs Window")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(APPWindow));
    }

    private const string UNIQUE_STRING = "0987654321qwertyuiopasdfghjklzxcvbnm[];,.";
    private const int UNIQUE_INT = int.MinValue;
    private const float UNIQUE_FLOAT = Mathf.NegativeInfinity;

    private const string UNITY_GRAPHICS_QUALITY = "UnityGraphicsQuality";

    private const float UpdateIntervalInSeconds = 1.0F;

    private bool waitTillPlistHasBeenWritten = false; 
    private FileInfo tmpPlistFile;

    private List<PlayerPrefsEntry> ppeList = new List<PlayerPrefsEntry>();
    private Vector2 scrollPos;
    private string newKey = "";
    private string newValueString = "";
    private int newValueInt = 0;
    private float newValueFloat = 0;
    private float rotation = 0;
    private ValueType selectedType = ValueType.String;

    private bool showNewEntryBox = false;
    private bool isOneSelected = false;
    private bool autoRefresh = false;
    private bool sortAscending = true;   //Ascending is A-Z or 1-2. Descending is Z-A or 2-1.

    private float oldTime = 0;

    private Texture2D _refreshIcon;
    private Texture2D _deleteIcon;
    private Texture2D _addIcon;
    private Texture2D _undoIcon;
    private Texture2D _saveIcon;


    void OnEnable()
    {
        if (!IsUnityWritingToPlist())
            RefreshKeys();

        //Make sure we never subscribe twice as OnEnable will be called more often then you think :)
        EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged()
    {
        waitTillPlistHasBeenWritten = IsUnityWritingToPlist();

        if (!waitTillPlistHasBeenWritten)
            RefreshKeys();
    }

    void Update()
    {
        //Auto refresh on Windows. On Mac this would be annoying because it takes longer so the user must manually refresh.
        if (autoRefresh && Application.platform == RuntimePlatform.WindowsEditor
            && EditorApplication.isPlaying)
        {
            float newtime = Mathf.Repeat(Time.timeSinceLevelLoad, UpdateIntervalInSeconds);
            if (newtime < oldTime)
                RefreshKeys();

            oldTime = newtime;
        }

        if (waitTillPlistHasBeenWritten)
        {
            if (new FileInfo(tmpPlistFile.FullName).Exists)
            {

            }
            else
            {
                RefreshKeys();
                waitTillPlistHasBeenWritten = false;
            }

            rotation += 0.05F;
            Repaint();
        }

        //Only enable certain options when atleast one is selected
        isOneSelected = false;
        foreach (PlayerPrefsEntry item in ppeList)
        {
            if (item.IsSelected)
            {
                isOneSelected = true;
                break;
            }
        }
    }

    void OnGUI()
    {
        GUIStyle boldNumberFieldStyle = new GUIStyle(EditorStyles.numberField);
        boldNumberFieldStyle.font = EditorStyles.boldFont;

        GUIStyle boldToggleStyle = new GUIStyle(EditorStyles.toggle);
        boldToggleStyle.font = EditorStyles.boldFont;

        GUI.enabled = !waitTillPlistHasBeenWritten;

        //Toolbar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            Rect optionsRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(false));

            if (GUILayout.Button(new GUIContent("Sort   " + (sortAscending ? "▼" : "▲"), "Change sorting to " + (sortAscending ? "descending" : "ascending")), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                OnChangeSortModeClicked();
            }

            if (GUILayout.Button(new GUIContent("Options", "Contains additional functionality like \"Add new entry\" and \"Delete all entries\" "), EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false)))
            {
                GenericMenu options = new GenericMenu();
                options.AddItem(new GUIContent("New Entry..."), false, OnNewEntryClicked);
                options.AddSeparator("");
                options.AddItem(new GUIContent("Delete Selected Entries"), false, OnDeleteSelectedClicked);
                options.AddItem(new GUIContent("Delete All Entries"), false, OnDeleteAllClicked);
                options.DropDown(optionsRect);
            }

            GUILayout.FlexibleSpace();

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string refreshTooltip = "Should all entries be automaticly refreshed every " + UpdateIntervalInSeconds + " seconds?";
                autoRefresh = GUILayout.Toggle(autoRefresh, new GUIContent("Auto Refresh ", refreshTooltip), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.MinWidth(20));
            }

            if (GUILayout.Button(new GUIContent(RefreshIcon, "Force a refresh, could take a few seconds."), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                RefreshKeys();
            }

            Rect r;
            if (Application.platform == RuntimePlatform.OSXEditor)
                r = GUILayoutUtility.GetRect(16, 16);
            else
                r = GUILayoutUtility.GetRect(9, 16);

            if (waitTillPlistHasBeenWritten)
            {
                Texture2D t = AssetDatabase.LoadAssetAtPath(IconsPath + "loader/" + (Mathf.FloorToInt(rotation % 12) + 1) + ".png", typeof(Texture2D)) as Texture2D;

                GUI.DrawTexture(new Rect(r.x + 3, r.y, 16, 16), t);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUI.enabled = !waitTillPlistHasBeenWritten;

        if (showNewEntryBox)
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    newKey = EditorGUILayout.TextField("Key", newKey);
                    switch (selectedType)
                    {
                        default:
                        case ValueType.String:
                            newValueString = EditorGUILayout.TextField("Value", newValueString);
                            break;
                        case ValueType.Float:
                            newValueFloat = EditorGUILayout.FloatField("Value", newValueFloat);
                            break;
                        case ValueType.Int:
                            newValueInt = EditorGUILayout.IntField("Value", newValueInt);
                            break;
                    }

                    selectedType = (ValueType)EditorGUILayout.EnumPopup("Type", selectedType);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(1));
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button(new GUIContent("X", "Close"), EditorStyles.boldLabel, GUILayout.ExpandWidth(false)))
                        {
                            showNewEntryBox = false;
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (GUILayout.Button(new GUIContent(AddIcon, "Add a new key-value.")))
                    {
                        if (!string.IsNullOrEmpty(newKey))
                        {
                            switch (selectedType)
                            {
                                case ValueType.Int:
                                    PlayerPrefs.SetInt(newKey, newValueInt);
                                    ppeList.Add(new PlayerPrefsEntry(newKey, newValueInt));
                                    break;
                                case ValueType.Float:
                                    PlayerPrefs.SetFloat(newKey, newValueFloat);
                                    ppeList.Add(new PlayerPrefsEntry(newKey, newValueFloat));
                                    break;
                                default:
                                case ValueType.String:
                                    PlayerPrefs.SetString(newKey, newValueString);
                                    ppeList.Add(new PlayerPrefsEntry(newKey, newValueString));
                                    break;
                            }
                            PlayerPrefs.Save();
                        }

                        newKey = newValueString = "";
                        newValueInt = 0;
                        newValueFloat = 0;
                        GUIUtility.keyboardControl = 0;	//move focus from textfield, else the text won't be cleared
                        showNewEntryBox = false;
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(2);


        GUI.backgroundColor = Color.white;
        EditorGUI.indentLevel++;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        {
            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < ppeList.Count; i++)
                {
                    if (ppeList[i].Value != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            
                            ppeList[i].IsSelected = GUILayout.Toggle(ppeList[i].IsSelected, new GUIContent(ppeList[i].Key, "Toggle selection."), ppeList[i].HasChanged ? boldToggleStyle : EditorStyles.toggle, GUILayout.MinWidth(40), GUILayout.MaxWidth(125), GUILayout.ExpandWidth(true));
                            
                            GUIStyle numberFieldStyle = ppeList[i].HasChanged ? boldNumberFieldStyle : EditorStyles.numberField;

                            switch (ppeList[i].Type)
                            {
                                default:
                                case ValueType.String:
                                    ppeList[i].Value = EditorGUILayout.TextField("", (string)ppeList[i].Value, numberFieldStyle, GUILayout.MinWidth(40));
                                    break;
                                case ValueType.Float:
                                    ppeList[i].Value = EditorGUILayout.FloatField("", (float)ppeList[i].Value, numberFieldStyle, GUILayout.MinWidth(40));
                                    break;
                                case ValueType.Int:
                                    ppeList[i].Value = EditorGUILayout.IntField("", (int)ppeList[i].Value, numberFieldStyle, GUILayout.MinWidth(40));
                                    break;
                            }

                            GUI.enabled = ppeList[i].HasChanged && !waitTillPlistHasBeenWritten;
                            if (GUILayout.Button(new GUIContent(SaveIcon, "Save changes made to this value."), GUILayout.ExpandWidth(false)))
                            {
                                ppeList[i].SaveChanges();
                            }

                            if (GUILayout.Button(new GUIContent(UndoIcon, "Discard changes made to this value."), GUILayout.ExpandWidth(false)))
                            {
                                ppeList[i].RevertChanges();
                            }

                            GUI.enabled = !waitTillPlistHasBeenWritten;

                            if (GUILayout.Button(new GUIContent(DeleteIcon, "Delete this key-value."), GUILayout.ExpandWidth(false)))
                            {
                                PlayerPrefs.DeleteKey(ppeList[i].Key);
                                ppeList.Remove(ppeList[i]);
                                PlayerPrefs.Save();
                                break;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
        EditorGUI.indentLevel--;
    }

    #region Menu Actions

    private void OnChangeSortModeClicked()
    {
        sortAscending = !sortAscending;
        Sort();
    }

    private void OnNewEntryClicked()
    {
        showNewEntryBox = true;
    }

    private void OnDeleteSelectedClicked()
    {
        if (isOneSelected)
        {
            if (!waitTillPlistHasBeenWritten)
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete the selected keys? There is no undo!", "Delete", "Cancel"))
                {
                    int count = ppeList.Count - 1;
                    for (int i = count; i >= 0; i--)
                    {
                        if (ppeList[i].IsSelected)
                        {
                            PlayerPrefs.DeleteKey(ppeList[i].Key);
                            ppeList.Remove(ppeList[i]);
                        }
                    }

                    PlayerPrefs.Save();
                }
            }
            else
                Debug.LogError("Cannot delete PlayerPrefs entries because it is still loading.");
        }
        else
            Debug.LogError("Cannot delete PlayerPrefs entries because no entries has been selected.");
    }

    private void OnDeleteAllClicked()
    {
        for (int i = 0; i < ppeList.Count; i++)
        {
            ppeList[i].IsSelected = true;
        }
        isOneSelected = true;

        OnDeleteSelectedClicked();
    }

    #endregion

    private void Sort()
    {
        if (sortAscending)
            ppeList.Sort(PlayerPrefsEntry.SortByNameAscending);
        else
            ppeList.Sort(PlayerPrefsEntry.SortByNameDescending);
    }

    private void RefreshKeys()
    {
        ppeList.Clear();
        string[] allKeys = GetAllKeys();

        for (int i = 0; i < allKeys.Length; i++)
        {
            ppeList.Add(new PlayerPrefsEntry(allKeys[i]));
        }

        Sort();
        Repaint();
    }

    private string[] GetAllKeys()
    {
        List<string> result = new List<string>();

        if (Application.platform == RuntimePlatform.WindowsEditor)
            result.AddRange(GetAllWindowsKeys());
        else if (Application.platform == RuntimePlatform.OSXEditor)
            result.AddRange(GetAllMacKeys());
        else
        {
            Debug.LogError("Unsupported platform detected, please contact support@rejected-games.com and let us know.");
        }

        //Remove UnityGraphicsQuality, thats something Unity always saves in your PlayerPrefs, apparently
        if (result.Contains(UNITY_GRAPHICS_QUALITY))
            result.Remove(UNITY_GRAPHICS_QUALITY);
        string[] removes = { "lasthotupdatePath", "hotupdatePath", "prefabPath1", "prefabPath2", "atlasPath1", "atlasPath2"};
        for (int i = 0; i < removes.Length; i++)
        {
            if (result.Contains(removes[i]))
                result.Remove(removes[i]);
        }
        return result.ToArray();
    }

    /// <summary>
    /// On Mac OS X PlayerPrefs are stored in ~/Library/Preferences folder, in a file named unity.[company name].[product name].plist, where company and product names are the names set up in Project Settings. The same .plist file is used for both Projects run in the Editor and standalone players. 
    /// </summary>
    private string[] GetAllMacKeys()
    {
        FileInfo fi = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist");
        Dictionary<string, object> plist = (Dictionary<string, object>)Plist.readPlist(fi.FullName);

        string[] keys = new string[plist.Count];
        plist.Keys.CopyTo(keys, 0);

        return keys;
    }

    /// <summary>
    /// On Windows, PlayerPrefs are stored in the registry under HKCU\Software\[company name]\[product name] key, where company and product names are the names set up in Project Settings.
    /// </summary>
    private string[] GetAllWindowsKeys()
    {
        RegistryKey cuKey = Registry.CurrentUser;
        RegistryKey unityKey = cuKey.CreateSubKey("Software\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);

        string[] values = unityKey.GetValueNames();
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = values[i].Substring(0, values[i].LastIndexOf("_"));
        }

        return values;
    }

    private bool IsUnityWritingToPlist()
    {
        bool result = false;

        //Mac specific, unfortunately no editor_mac preprocessor is available
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            //Find the tempPlistFile, while it exists we know Unity is still busy writen the last version of PlayerPrefs
            FileInfo plistFile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist");
            DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/");
            FileInfo[] allPreferenceFiles = di.GetFiles();

            foreach (FileInfo fi in allPreferenceFiles)
            {
                if (fi.FullName.Contains(plistFile.FullName))
                {
                    if (!fi.FullName.EndsWith(".plist"))
                    {
                        tmpPlistFile = fi;
                        result = true;
                    }
                }
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
            result = false;

        return result;
    }

    private class PlayerPrefsEntry
    {
        private string key;
        private object value;

        public ValueType Type;
        public bool IsSelected = false;
        public bool HasChanged = false;

        public PlayerPrefsEntry(string key)
        {
            Key = key;
        }

        public PlayerPrefsEntry(string key, string value)
        {
            this.key = key;
            this.value = value;
            this.Type = ValueType.String;
        }

        public PlayerPrefsEntry(string key, float value)
        {
            this.key = key;
            this.value = value;
            this.Type = ValueType.Float;
        }

        public PlayerPrefsEntry(string key, int value)
        {
            this.key = key;
            this.value = value;
            this.Type = ValueType.Int;
        }

        public void SaveChanges()
        {
            switch (Type)
            {
                default:
                case ValueType.String:
                    PlayerPrefs.SetString(Key, (string)Value);
                    break;
                case ValueType.Float:
                    PlayerPrefs.SetFloat(Key, (float)Value);
                    break;
                case ValueType.Int:
                    PlayerPrefs.SetInt(Key, (int)Value);
                    break;
            }

            HasChanged = false;

            //Incase the user exits without saving project
            PlayerPrefs.Save();
        }

        public void RevertChanges()
        {
            RetrieveValue();
        }

        public void RetrieveValue()
        {
            if (PlayerPrefs.GetString(Key, UNIQUE_STRING) != UNIQUE_STRING)
            {
                Type = ValueType.String;
                value = PlayerPrefs.GetString(Key);
            }
            else if (PlayerPrefs.GetInt(Key, UNIQUE_INT) != UNIQUE_INT)
            {
                Type = ValueType.Int;
                value = PlayerPrefs.GetInt(Key);
            }
            else if (PlayerPrefs.GetFloat(Key, UNIQUE_FLOAT) != UNIQUE_FLOAT)
            {
                Type = ValueType.Float;
                value = PlayerPrefs.GetFloat(Key);
            }

            //Don't mark the first set Value as changed
            HasChanged = false;
        }

        public string Key
        {
            get
            {
                return key;
            }

            set
            {
                key = value;

                RetrieveValue();
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (!value.Equals(this.value))
                {
                    this.value = value;

                    HasChanged = true;
                }
            }
        }

        public static int SortByNameAscending(PlayerPrefsEntry a, PlayerPrefsEntry b)
        {
            return string.Compare(a.Key, b.Key);
        }

        public static int SortByNameDescending(PlayerPrefsEntry a, PlayerPrefsEntry b)
        {
            return string.Compare(b.Key, a.Key);
        }
    }

    private enum ValueType
    {
        String,
        Float,
        Int
    }

    public static void Separator(Color color)
    {
        Color old = GUI.color;
        GUI.color = color;
        Rect lineRect = GUILayoutUtility.GetRect(10, 1);
        GUI.DrawTexture(new Rect(lineRect.x, lineRect.y, lineRect.width, 1), EditorGUIUtility.whiteTexture);
        GUI.color = old;
    }

    public Texture2D DeleteIcon
    {
        get
        {
            if ((UnityEngine.Object)_deleteIcon == (UnityEngine.Object)null)
                _deleteIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "delete.png", typeof(Texture2D)) as Texture2D;
            return _deleteIcon;
        }
    }

    public Texture2D AddIcon
    {
        get
        {
            if ((UnityEngine.Object)_addIcon == (UnityEngine.Object)null)
                _addIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "add.png", typeof(Texture2D)) as Texture2D;
            return _addIcon;
        }
    }

    public Texture2D UndoIcon
    {
        get
        {
            if ((UnityEngine.Object)_undoIcon == (UnityEngine.Object)null)
                _undoIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "undo.png", typeof(Texture2D)) as Texture2D;
            return _undoIcon;
        }
    }

    public Texture2D SaveIcon
    {
        get
        {
            if ((UnityEngine.Object)_saveIcon == (UnityEngine.Object)null)
                _saveIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "save.png", typeof(Texture2D)) as Texture2D;
            return _saveIcon;
        }
    }

    public Texture2D RefreshIcon
    {
        get
        {
            if ((UnityEngine.Object)_refreshIcon == (UnityEngine.Object)null)
                _refreshIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "refresh.png", typeof(Texture2D)) as Texture2D;
            return _refreshIcon;
        }
    }

    private string iconsPath;
    private string IconsPath
    {
        get
        {
            if (string.IsNullOrEmpty(iconsPath))
            {
                //Assets/UltimatePlayerPrefsEditor/Editor/PlayerPrefsEditor.cs
                string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));

                //Strip PlayerPrefsEditor.cs
                path = path.Substring(0, path.LastIndexOf('/'));
                 
                //Strip Editor/
                path = path.Substring(0, path.LastIndexOf('/') + 1);

                iconsPath = path + "Icons/";
            }

            return iconsPath;
        }
    }
}