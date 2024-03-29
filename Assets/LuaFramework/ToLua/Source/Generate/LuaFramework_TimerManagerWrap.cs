﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class LuaFramework_TimerManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(LuaFramework.TimerManager), typeof(Manager));
		L.RegFunction("StartTimer", StartTimer);
		L.RegFunction("StopTimer", StopTimer);
		L.RegFunction("AddTimerEvent", AddTimerEvent);
		L.RegFunction("RemoveTimerEvent", RemoveTimerEvent);
		L.RegFunction("StopTimerEvent", StopTimerEvent);
		L.RegFunction("ResumeTimerEvent", ResumeTimerEvent);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("GameTime", get_GameTime, set_GameTime);
		L.RegVar("Interval", get_Interval, set_Interval);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StartTimer(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)ToLua.CheckObject<LuaFramework.TimerManager>(L, 1);
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.StartTimer(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopTimer(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)ToLua.CheckObject<LuaFramework.TimerManager>(L, 1);
			obj.StopTimer();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddTimerEvent(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)ToLua.CheckObject<LuaFramework.TimerManager>(L, 1);
			LuaFramework.TimerInfo arg0 = (LuaFramework.TimerInfo)ToLua.CheckObject<LuaFramework.TimerInfo>(L, 2);
			obj.AddTimerEvent(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveTimerEvent(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)ToLua.CheckObject<LuaFramework.TimerManager>(L, 1);
			LuaFramework.TimerInfo arg0 = (LuaFramework.TimerInfo)ToLua.CheckObject<LuaFramework.TimerInfo>(L, 2);
			obj.RemoveTimerEvent(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopTimerEvent(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)ToLua.CheckObject<LuaFramework.TimerManager>(L, 1);
			LuaFramework.TimerInfo arg0 = (LuaFramework.TimerInfo)ToLua.CheckObject<LuaFramework.TimerInfo>(L, 2);
			obj.StopTimerEvent(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ResumeTimerEvent(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)ToLua.CheckObject<LuaFramework.TimerManager>(L, 1);
			LuaFramework.TimerInfo arg0 = (LuaFramework.TimerInfo)ToLua.CheckObject<LuaFramework.TimerInfo>(L, 2);
			obj.ResumeTimerEvent(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_GameTime(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)o;
			int ret = obj.GameTime;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index GameTime on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Interval(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)o;
			float ret = obj.Interval;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Interval on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_GameTime(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.GameTime = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index GameTime on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Interval(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			LuaFramework.TimerManager obj = (LuaFramework.TimerManager)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.Interval = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Interval on a nil value");
		}
	}
}

