﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Scripts_LuaToCSFunctionWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Scripts.LuaToCSFunction), typeof(System.Object));
		L.RegFunction("GetIosPushToken", GetIosPushToken);
		L.RegFunction("GetIosUuid", GetIosUuid);
		L.RegFunction("LoadPrefabByName", LoadPrefabByName);
		L.RegFunction("SetUITexture", SetUITexture);
		L.RegFunction("Release", Release);
		L.RegFunction("PoolDestroy", PoolDestroy);
		L.RegFunction("SetGameObjectLocalScale", SetGameObjectLocalScale);
		L.RegFunction("SetGameObjectLocalEulerAngles", SetGameObjectLocalEulerAngles);
		L.RegFunction("SetGameObjectLocalPosition", SetGameObjectLocalPosition);
		L.RegFunction("SetGameObjectPosition", SetGameObjectPosition);
		L.RegFunction("SetGameObjectAndChildrenLayer", SetGameObjectAndChildrenLayer);
		L.RegFunction("CallToLuaFunction", CallToLuaFunction);
		L.RegFunction("LoadSceneAsync", LoadSceneAsync);
		L.RegFunction("PreLoadResources", PreLoadResources);
		L.RegFunction("DestroyHotUpgradeView", DestroyHotUpgradeView);
		L.RegFunction("UnloadAssetBundle", UnloadAssetBundle);
		L.RegFunction("GetApplicationPlatform", GetApplicationPlatform);
		L.RegFunction("GetAccountName", GetAccountName);
		L.RegFunction("SetAccountName", SetAccountName);
		L.RegFunction("PlayerPrefsSave", PlayerPrefsSave);
		L.RegFunction("PlayerPrefsGet", PlayerPrefsGet);
		L.RegFunction("PlayerPrefsDeleteAll", PlayerPrefsDeleteAll);
		L.RegFunction("PayMoney", PayMoney);
		L.RegFunction("PayMoneyCallBack", PayMoneyCallBack);
		L.RegFunction("PlayBgMusic", PlayBgMusic);
		L.RegFunction("PlaySound", PlaySound);
		L.RegFunction("SavePlayerMusic", SavePlayerMusic);
		L.RegFunction("SavePlayerSound", SavePlayerSound);
		L.RegFunction("GetScreenWidthW", GetScreenWidthW);
		L.RegFunction("GetScreenHeightH", GetScreenHeightH);
		L.RegFunction("GetScreenRatio", GetScreenRatio);
		L.RegFunction("SetUICameraRect", SetUICameraRect);
		L.RegFunction("IsIphoneX", IsIphoneX);
		L.RegFunction("GetMusic", GetMusic);
		L.RegFunction("SetMusic", SetMusic);
		L.RegFunction("AddOutLog", AddOutLog);
		L.RegFunction("Encrypt", Encrypt);
		L.RegFunction("Decrypt", Decrypt);
		L.RegFunction("New", _CreateScripts_LuaToCSFunction);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("IosPushToken", get_IosPushToken, set_IosPushToken);
		L.RegVar("IosUuid", get_IosUuid, set_IosUuid);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateScripts_LuaToCSFunction(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				Scripts.LuaToCSFunction obj = new Scripts.LuaToCSFunction();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: Scripts.LuaToCSFunction.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetIosPushToken(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = Scripts.LuaToCSFunction.GetIosPushToken();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetIosUuid(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = Scripts.LuaToCSFunction.GetIosUuid();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadPrefabByName(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			UnityEngine.Transform arg1 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 2);
			UnityEngine.GameObject o = Scripts.LuaToCSFunction.LoadPrefabByName(arg0, arg1);
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetUITexture(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UITexture arg0 = (UITexture)ToLua.CheckObject<UITexture>(L, 1);
			string arg1 = ToLua.CheckString(L, 2);
			Scripts.LuaToCSFunction.SetUITexture(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Release(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			Scripts.LuaToCSFunction.Release(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PoolDestroy(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			Scripts.LuaToCSFunction.PoolDestroy(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetGameObjectLocalScale(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 2);
			float arg2 = (float)LuaDLL.luaL_checknumber(L, 3);
			float arg3 = (float)LuaDLL.luaL_checknumber(L, 4);
			Scripts.LuaToCSFunction.SetGameObjectLocalScale(arg0, arg1, arg2, arg3);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetGameObjectLocalEulerAngles(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 2);
			float arg2 = (float)LuaDLL.luaL_checknumber(L, 3);
			float arg3 = (float)LuaDLL.luaL_checknumber(L, 4);
			Scripts.LuaToCSFunction.SetGameObjectLocalEulerAngles(arg0, arg1, arg2, arg3);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetGameObjectLocalPosition(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 2);
			float arg2 = (float)LuaDLL.luaL_checknumber(L, 3);
			float arg3 = (float)LuaDLL.luaL_checknumber(L, 4);
			Scripts.LuaToCSFunction.SetGameObjectLocalPosition(arg0, arg1, arg2, arg3);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetGameObjectPosition(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 2);
			float arg2 = (float)LuaDLL.luaL_checknumber(L, 3);
			float arg3 = (float)LuaDLL.luaL_checknumber(L, 4);
			Scripts.LuaToCSFunction.SetGameObjectPosition(arg0, arg1, arg2, arg3);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetGameObjectAndChildrenLayer(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
			Scripts.LuaToCSFunction.SetGameObjectAndChildrenLayer(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CallToLuaFunction(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1 && TypeChecker.CheckTypes<string>(L, 1))
			{
				string arg0 = ToLua.ToString(L, 1);
				Scripts.LuaToCSFunction.CallToLuaFunction(arg0);
				return 0;
			}
			else if (TypeChecker.CheckTypes<string>(L, 1) && TypeChecker.CheckParamsType<object>(L, 2, count - 1))
			{
				string arg0 = ToLua.ToString(L, 1);
				object[] arg1 = ToLua.ToParamsObject(L, 2, count - 1);
				Scripts.LuaToCSFunction.CallToLuaFunction(arg0, arg1);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Scripts.LuaToCSFunction.CallToLuaFunction");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadSceneAsync(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			Scripts.LuaToCSFunction.LoadSceneAsync(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PreLoadResources(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			Scripts.LuaToCSFunction.PreLoadResources(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DestroyHotUpgradeView(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Scripts.LuaToCSFunction.DestroyHotUpgradeView();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnloadAssetBundle(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			Scripts.LuaToCSFunction.UnloadAssetBundle(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetApplicationPlatform(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			int o = Scripts.LuaToCSFunction.GetApplicationPlatform();
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAccountName(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			string o = Scripts.LuaToCSFunction.GetAccountName();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetAccountName(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			Scripts.LuaToCSFunction.SetAccountName(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayerPrefsSave(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			string arg1 = ToLua.CheckString(L, 2);
			Scripts.LuaToCSFunction.PlayerPrefsSave(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayerPrefsGet(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			string o = Scripts.LuaToCSFunction.PlayerPrefsGet(arg0);
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayerPrefsDeleteAll(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Scripts.LuaToCSFunction.PlayerPrefsDeleteAll();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PayMoney(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			string arg0 = ToLua.CheckString(L, 1);
			string arg1 = ToLua.CheckString(L, 2);
			int arg2 = (int)LuaDLL.luaL_checknumber(L, 3);
			string arg3 = ToLua.CheckString(L, 4);
			Scripts.LuaToCSFunction.PayMoney(arg0, arg1, arg2, arg3);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PayMoneyCallBack(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				Scripts.LuaToCSFunction.PayMoneyCallBack(arg0);
				return 0;
			}
			else if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
				Scripts.LuaToCSFunction.PayMoneyCallBack(arg0, arg1);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Scripts.LuaToCSFunction.PayMoneyCallBack");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayBgMusic(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			string arg0 = ToLua.CheckString(L, 1);
			bool arg1 = LuaDLL.luaL_checkboolean(L, 2);
			bool arg2 = LuaDLL.luaL_checkboolean(L, 3);
			Scripts.LuaToCSFunction.PlayBgMusic(arg0, arg1, arg2);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlaySound(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			bool arg1 = LuaDLL.luaL_checkboolean(L, 2);
			Scripts.LuaToCSFunction.PlaySound(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SavePlayerMusic(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			bool arg0 = LuaDLL.luaL_checkboolean(L, 1);
			Scripts.LuaToCSFunction.SavePlayerMusic(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SavePlayerSound(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			bool arg0 = LuaDLL.luaL_checkboolean(L, 1);
			Scripts.LuaToCSFunction.SavePlayerSound(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetScreenWidthW(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			float o = Scripts.LuaToCSFunction.GetScreenWidthW();
			LuaDLL.lua_pushnumber(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetScreenHeightH(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			float o = Scripts.LuaToCSFunction.GetScreenHeightH();
			LuaDLL.lua_pushnumber(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetScreenRatio(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			float o = Scripts.LuaToCSFunction.GetScreenRatio();
			LuaDLL.lua_pushnumber(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetUICameraRect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 5);
			UnityEngine.Camera arg0 = (UnityEngine.Camera)ToLua.CheckObject(L, 1, typeof(UnityEngine.Camera));
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 2);
			float arg2 = (float)LuaDLL.luaL_checknumber(L, 3);
			float arg3 = (float)LuaDLL.luaL_checknumber(L, 4);
			float arg4 = (float)LuaDLL.luaL_checknumber(L, 5);
			Scripts.LuaToCSFunction.SetUICameraRect(arg0, arg1, arg2, arg3, arg4);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsIphoneX(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			bool o = Scripts.LuaToCSFunction.IsIphoneX();
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMusic(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			bool o = Scripts.LuaToCSFunction.GetMusic();
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetMusic(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			bool arg0 = LuaDLL.luaL_checkboolean(L, 1);
			Scripts.LuaToCSFunction.SetMusic(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddOutLog(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			Scripts.LuaToCSFunction.AddOutLog();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Encrypt(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			string arg1 = ToLua.CheckString(L, 2);
			string o = Scripts.LuaToCSFunction.Encrypt(arg0, arg1);
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Decrypt(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			string arg1 = ToLua.CheckString(L, 2);
			string o = Scripts.LuaToCSFunction.Decrypt(arg0, arg1);
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IosPushToken(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, Scripts.LuaToCSFunction.IosPushToken);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IosUuid(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, Scripts.LuaToCSFunction.IosUuid);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_IosPushToken(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			Scripts.LuaToCSFunction.IosPushToken = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_IosUuid(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			Scripts.LuaToCSFunction.IosUuid = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

