using UnityEngine;
using UnityEditor;
using LuaFramework;
using System.Collections.Generic;
using System.IO;

public class CreateLuaUtil : MonoBehaviour
{
    [MenuItem("Custom Editor/Lua/CreateItemLua")]
    static void CreateItemLua()
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        List<GameObject> prefabList = new List<GameObject>();

        foreach (Object obj in SelectedAsset)
        {
            if (obj is GameObject)
            {
                prefabList.Add((GameObject)obj);
            }
        }
        for (int i = 0; i < prefabList.Count; i++)
        {
            GameObject obj = prefabList[i];
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            sourcePath = sourcePath.Replace("Assets/Prefabs/", "");
            sourcePath = sourcePath.Replace("/" + obj.name + ".prefab", "");
            string path = Application.dataPath.ToLower() + "/LuaFramework/Lua/UI/" + sourcePath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string luaStr = "local " + obj.name + " = class(\"" + obj.name + "\")\n\n";
            luaStr += "function " + obj.name + ":ctor(parent, info, luaBehaviour)\n\t" +
                "self.parent = parent\n\tself.info = info\n\tself.luaBehaviour = luaBehaviour\n\tself:InitView()\nend\n\n";
            luaStr += "function " + obj.name + ":InitView()\n\tself.gameObject = LuaToCSFunction.LoadPrefabByName(" +
                "\"Prefabs/" + sourcePath + "/" + obj.name + "\", self.parent)\n\tself.transform = self.gameObject.transform\n\t" +
                "LuaToCSFunction.SetGameObjectLocalPosition(self.gameObject, 0, 0, 0)\n";
            List<LuaObj> luaObjList = GetLuaObjList(obj);
            List<string> btnNameList = new List<string>();
            List<string> labelNameList = new List<string>();
            for (int j = 0; j < luaObjList.Count; j++)
            {
                LuaObj luaObj = luaObjList[j];
                string name = GetGameObjectPath(luaObj.obj).Replace("/" + obj.name + "/", "");
                if (name.EndsWith("-") || name.EndsWith("_"))
                {
                    continue;
                }
                if (!name.Equals("/" + obj.name))
                {
                    if (luaObj.type == "BoxCollider")
                    {
                        luaStr += "\tself." + name.Replace("/", "_") + "_Obj" + GetEndComponentStr(luaObj.type, name);
                    }
                    else if (luaObj.type == "UISprite")
                    {
                        luaStr += "\tself." + name.Replace("/", "_") + "_Sprite" + GetEndComponentStr(luaObj.type, name);
                    }
                    else
                    {
                        luaStr += "\tself." + name.Replace("/", "_") + GetEndComponentStr(luaObj.type, name);
                    }
                }
                if (luaObj.type == "BoxCollider")
                {
                    if (name.Equals("/" + obj.name))
                    {
                        btnNameList.Add("self.gameObject");
                    }
                    else
                    {
                        btnNameList.Add("self." + name.Replace("/", "_") + "_Obj");
                    }
                }
                else if (luaObj.type == "UILabel")
                {
                    labelNameList.Add("self." + name.Replace("/", "_"));
                }
            }
            if (labelNameList.Count > 0)
            {
                for (int o = 0; o < labelNameList.Count; o++)
                {
                    string labelName = labelNameList[o];
                    luaStr += "\t" + labelName + ".text = tables.LanguageDic[\"Bow06\"]" + "\n";
                }
            }
            luaStr += btnNameList.Count > 0 ? "\n" : "";
            for (int m = 0; m < btnNameList.Count; m++)
            {
                string btnName = btnNameList[m];
                luaStr += "\tself.parent.luaBehaviour:RemoveClick(" + btnName + ")\n";
                luaStr += "\tself.parent.luaBehaviour:AddClick(" + btnName + ", self, self.OnClickHandler)" + "\n";
            }
            luaStr += btnNameList.Count == 0 ? "end\n\n" : "" + "end" + "\n\n";
            if (btnNameList.Count > 0)
            {
                luaStr += "function " + obj.name + ":OnClickHandler(go)\n";
                for (int o = 0; o < btnNameList.Count; o++)
                {
                    string btnName = btnNameList[o];
                    luaStr += (o == 0 ? "\tif" : "\n\telseif") + " go == " + btnName + " then\n";
                }
                luaStr += "\n\tend\nend\n\n";
            }

            luaStr += "function " + obj.name + ":Dispose()\n\tLuaToCSFunction.PoolDestroy(self.gameObject)\nend" + "\n\n";

            luaStr += "return " + obj.name;
            if (File.Exists(path + "/" + obj.name + ".lua"))
            {
                File.Delete(path + "/" + obj.name + ".lua");
            }
            File.WriteAllText(path + "/" + obj.name + ".lua", luaStr);
            Debug.Log("生成lua文件完成！");
        }
    }

    [MenuItem("Custom Editor/Lua/CreateMVCLua")]
    static void CreateLuaFiles()
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        List<GameObject> prefabList = new List<GameObject>();

        foreach (Object obj in SelectedAsset)
        {
            if (obj is GameObject)
            {
                prefabList.Add((GameObject)obj);
            }
        }
        for (int i = 0; i < prefabList.Count; i++)
        {
            GameObject obj = prefabList[i];
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            sourcePath = sourcePath.Replace("Assets/Prefabs/", "");
            sourcePath = sourcePath.Replace("/" + obj.name + ".prefab", "");
            string path = Application.dataPath.ToLower() + "/LuaFramework/Lua/UI/" + sourcePath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string luaStr = InitPreString(obj);

            List<LuaObj> luaObjList = GetLuaObjList(obj);
            Dictionary<string, List<string>> componentsDic = new Dictionary<string, List<string>>();
            componentsDic["BoxCollider"] = new List<string>();
            componentsDic["UILabel"] = new List<string>();
            componentsDic["UISlider"] = new List<string>();
            for (int j = 0; j < luaObjList.Count; j++)
            {
                LuaObj luaObj = luaObjList[j];
                string name = GetGameObjectPath(luaObj.obj).Replace("/" + obj.name + "/", "");
                if (name.EndsWith("-") || name.EndsWith("_"))
                {
                    continue;
                }
                if (!name.Equals("/" + obj.name))
                {
                    if (luaObj.type == "BoxCollider")
                    {
                        luaStr += "\tself." + name.Replace("/", "_") + "_Obj" + GetEndComponentStr(luaObj.type, name);
                    }
                    else if (luaObj.type == "UISprite")
                    {
                        luaStr += "\tself." + name.Replace("/", "_") + "_Sprite" + GetEndComponentStr(luaObj.type, name);
                    }
                    else
                    {
                        luaStr += "\tself." + name.Replace("/", "_") + GetEndComponentStr(luaObj.type, name);
                    }
                }
                if(luaObj.type == "BoxCollider")
                {
                    if (name.Equals("/" + obj.name))
                    {
                        componentsDic["BoxCollider"].Add("self.gameObject");
                    }
                    else
                    {
                        componentsDic["BoxCollider"].Add("self." + name.Replace("/", "_") + "_Obj");
                    }
                }
                else if (luaObj.type == "UILabel")
                {
                    componentsDic["UILabel"].Add("self." + name.Replace("/", "_"));
                }
                else if(componentsDic.ContainsKey(luaObj.type))
                {
                    componentsDic[luaObj.type].Add("self." + name.Replace("/", "_"));
                }
            }
            luaStr += "\n\tself:InitEvent()\n";
            luaStr += "end" + "\n\n";

            luaStr += "function " + obj.name + ":InitEvent()\n";
            for (int m = 0; m < componentsDic["BoxCollider"].Count; m++)
            {
                string btnName = componentsDic["BoxCollider"][m];
                luaStr += "\tself.luaBehaviour:AddClick(" + btnName + ", self, self.OnClickHandler)" + "\n";
            }
            luaStr += (componentsDic["BoxCollider"].Count == 0 ? "\nend" : "" + "end") + "\n\n";
            if (componentsDic["BoxCollider"].Count > 0)
            {
                luaStr += "function " + obj.name + ":OnClickHandler(go)\n";
                for (int o = 0; o < componentsDic["BoxCollider"].Count; o++)
                {
                    string btnName = componentsDic["BoxCollider"][o];
                    luaStr += (o == 0 ? "\tif" : "\n\telseif") + " go == " + btnName + " then\n";
                }
                luaStr += "\n\tend\nend\n\n";
            }

            luaStr += "function " + obj.name + ":UpdateView()\n";
            if (componentsDic["UILabel"].Count > 0)
            {
                for (int o = 0; o < componentsDic["UILabel"].Count; o++)
                {
                    string labelName = componentsDic["UILabel"][o];
                    luaStr += "\t" + labelName + ".text = tables.LanguageDic[\"Bow06\"]" + "\n";
                }
            }
            if (componentsDic["UISlider"].Count > 0)
            {
                for (int sl = 0; sl < componentsDic["UISlider"].Count; sl++)
                {
                    string sliderName = componentsDic["UISlider"][sl];
                    luaStr += "\t" + sliderName + ".value = 0" + "\n";
                }
            }
            luaStr += "end\n\n";

            luaStr += "function " + obj.name + ":RemoveEvent()\n";
            for (int n = 0; n < componentsDic["BoxCollider"].Count; n++)
            {
                string btnName = componentsDic["BoxCollider"][n];
                luaStr += "\tself.luaBehaviour:RemoveClick(" + btnName + ")" + "\n";
            }
            luaStr += (componentsDic["BoxCollider"].Count == 0 ? "\nend" : "" + "end") + "\n\n";

            luaStr += "function " + obj.name + ":OnCloseHandler()\n\tself:RemoveEvent()\n\tself:UnloadAssetBundle()\nend" + "\n\n";

            luaStr += "function " + obj.name + ":OnDestroy()\n\tself:RemoveEvent()\n\tself.super.OnDestroy(self)\nend" + "\n\n";

            luaStr += "return " + obj.name;
            if(File.Exists(path + "/" + obj.name + ".lua"))
            {
                File.Delete(path + "/" + obj.name + ".lua");
            }
            File.WriteAllText(path + "/" + obj.name + ".lua", luaStr);

            //生成Ctrl
            CreateCtrlFile(path, obj.name, sourcePath);
            //生成Model
            CreateModelFile(path, obj.name);
            Debug.Log("生成lua文件完成！");
        }
    }

    private static void CreateCtrlFile(string path, string str, string resPath)
    {
        string name = str.Replace("View", "") + "Ctrl";
        if (File.Exists(path + "/" + name + ".lua"))
        {
            File.Delete(path + "/" + name + ".lua");
        }
        string luaStr = "local BaseCtrl = require(\"UI.BaseCtrl\")\n";
        luaStr += "local " + name + " = class(\"" + name + "\", BaseCtrl)\n\n";
        luaStr += "function " + name + ":ctor()\n\tself.super.ctor(self, \"" + resPath + "/" + str + "\")\n";
        luaStr += "\tself.model = require(\"UI." + resPath.Replace("/", ".") + "." + str.Replace("View", "") + "Model" + "\").new()\n";
        luaStr += "\tself.view = require(\"UI." + resPath.Replace("/", ".") + "." + str + "\").new()\n\tself.view.model = self.model\nend\n\n";
        luaStr += "function " + name + ":OnCreate(obj)\n\tself.inited = true\n\tself.view:Awake(obj)\nend\n\n";
        luaStr += "function " + name + ":SetData(data)\n\tself.model:SetData(data)\nend\n\n";
        luaStr += "return " + name;

        File.WriteAllText(path + "/" + name + ".lua", luaStr);
    }

    private static void CreateModelFile(string path, string str)
    {
        string name = str.Replace("View", "") + "Model";
        if (File.Exists(path + "/" + name + ".lua"))
        {
            File.Delete(path + "/" + name + ".lua");
        }
        string luaStr = "local " + name + " = class(\"" + name + "\")" + "\n\n";
        luaStr += "function " + name + ":SetData(data)\n\t\nend\n\nreturn " + name;

        File.WriteAllText(path + "/" + name + ".lua", luaStr);
    }

    private static string InitPreString(GameObject obj)
    {
        string luaStr = "local BaseView = require(\"UI.BaseView\")" + "\n";
        luaStr += "local " + obj.name + " = class(\"" + obj.name + "\", BaseView)" + "\n\n";
        luaStr += "function " + obj.name + ":ctor()\n\tself.super.ctor(self, \"" + obj.name + "\")\nend" + "\n\n";
        luaStr += "function " + obj.name + ":InitPanel()\n\tself:CreateBaseView()\n\t" +
            "self:InitView()\nend" + "\n\n";
        luaStr += "function " + obj.name + ":InitView()" + "\n";
        return luaStr;
    }

    private static string GetEndComponentStr(string type, string name)
    {
        string str;
        switch (type)
        {
            case "BoxCollider":
                str = " = tools.FindChildObj(self.transform, \"" + name + "\")\n";
                break;
            case "UILabel":
                str = " = tools.FindChild(self.transform, \"" + name + "\")\n";
                break;
            case "UISprite":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UISprite)\n";
                break;
            case "UIButton":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UIButton)\n";
                break;
            case "UIScrollView":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UIScrollView)\n";
                break;
            case "UIWrapContent":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UIWrapContent)\n";
                break;
            case "UIPanel":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UIPanel)\n";
                break;
            case "UISlider":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UISlider)\n";
                break;
            case "UIToggle":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UIToggle)\n";
                break;
            case "UIInput":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UIInput)\n";
                break;
            case "UITable":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UITable)\n";
                break;
            case "UIGrid":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UIGrid)\n";
                break;
            case "UI2DSprite":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UI2DSprite)\n";
                break;
            case "Animator":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.Animator)\n";
                break;
            case "UITexture":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UITexture)\n";
                break;
            case "UIPlayTween":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.UIPlayTween)\n";
                break;
            case "TweenColor":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.TweenColor)\n";
                break;
            case "TweenScale":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.TweenScale)\n";
                break;
            case "TweenRotation":
                str = " = tools.FindChild(self.transform, \"" + name + "\", tools.TweenRotation)\n";
                break;
            default:
                str = " = tools.FindChildObj(self.transform, \"" + name + "\")\n";
                break;
        }
        return str;
    }

    private static List<LuaObj> GetLuaObjList(GameObject obj)
    {
        List<LuaObj> luaObjList = new List<LuaObj>();
        foreach (BoxCollider child in obj.GetComponentsInChildren<BoxCollider>(true))
        {
            luaObjList.Add(new LuaObj("BoxCollider", child.gameObject));
        }
        foreach (UILabel child in obj.GetComponentsInChildren<UILabel>(true))
        {
            luaObjList.Add(new LuaObj("UILabel", child.gameObject));
        }
        foreach (UISprite child in obj.GetComponentsInChildren<UISprite>(true))
        {
            luaObjList.Add(new LuaObj("UISprite", child.gameObject));
        }
        foreach (UIScrollView child in obj.GetComponentsInChildren<UIScrollView>(true))
        {
            luaObjList.Add(new LuaObj("UIScrollView", child.gameObject));
        }
        foreach (UITable child in obj.GetComponentsInChildren<UITable>(true))
        {
            luaObjList.Add(new LuaObj("UITable", child.gameObject));
        }
        foreach (UIGrid child in obj.GetComponentsInChildren<UIGrid>(true))
        {
            luaObjList.Add(new LuaObj("UIGrid", child.gameObject));
        }
        foreach (UISlider child in obj.GetComponentsInChildren<UISlider>(true))
        {
            luaObjList.Add(new LuaObj("UISlider", child.gameObject));
        }
        return luaObjList;
    }

    private static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    class LuaObj
    {
        public string type;
        public GameObject obj;
        public LuaObj(string type1, GameObject obj1)
        {
            type = type1;
            obj = obj1;
        }
    }

    [MenuItem("Custom Editor/Tools/CreateSequenceAnimation")]
    static void CreateSequenceAnimation()
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        List<GameObject> prefabList = new List<GameObject>();

        foreach (Object obj in SelectedAsset)
        {
            if (obj is GameObject)
            {
                prefabList.Add((GameObject)obj);
            }
        }
        for (int i = 0; i < prefabList.Count; i++)
        {
            GameObject obj = prefabList[i];
            SequenceAnimation anim = obj.GetComponent<SequenceAnimation>();
            int count = anim.SpriteArray.Length;
            Sprite[] sprites = new Sprite[count];
            for (int k = 0; k < count; k++)
			{
                string ss = "Assets/Animation/" + obj.name + "/" + obj.name + "000" + k + ".png";
                if(k>9)
                {
                    ss = "Assets/Animation/" + obj.name + "/" + obj.name + "00" + k + ".png";
                }
                Object sp = UnityEditor.AssetDatabase.LoadAssetAtPath(ss, typeof(Sprite));
                sprites[k] = sp as Sprite;
			}
            anim.SpriteArray = sprites;
            //obj.name
        }
        AssetDatabase.SaveAssets();
        Debug.Log("CreateSequenceAnimation Complete!");
    }

    [MenuItem("Custom Editor/Tools/Z_CheckSequenceAnimation")]
    static void CheckSequenceAnimation()
    {
        DirectoryInfo direction = new DirectoryInfo("Assets/Prefabs/SkillEffect");
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        Dictionary<string, int> dic = new Dictionary<string,int>();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            dic.Add(files[i].Name.Replace(".prefab", ""), 1);
        }

        string str = "hyps|gunshi|hyt|dcbhit|zlb|tlz|kscj|lps|hz|bingxi|yntc|sbdl|hypshit|jdgx|eyjl|dcb|hyhit|fxzlhit|swft|fljhit|bsyzhit|lss|nhhd|blq|jysy|jdgxhit|start|dzb|wyq|nhhdhit|pkz|hqs|hzhit|shuiqianghit|lyzn|hbbfx|longzhua|hyq|fxzl|shuiqiang|dmdy|tgbg|hyxf|wmqf|";
        string[] list = str.Split('|');
        for (int i = 0; i < list.Length; i++)
        {
            if(!dic.ContainsKey(list[i]))
            {
                Debug.Log("Name:" + list[i]);
            }
        }
    }
}
