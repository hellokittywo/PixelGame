TimerManager = {}
local self = TimerManager
self.Tick = LuaFramework.AppConst.TimerInterval
self.timerInfoList = {}

--*秒执行一次的定时器
function TimerManager.AddTimerEvent(table, func)
	if not table or not func then
		return
	end
	if self.timerInfoList[table] and self.timerInfoList[table][func] then
		-- tools.PrintDebug("-----------定时器已经添加过", table.__cname, table, func)
		return
	end
	if not self.timerInfoList[table] then
		self.timerInfoList[table] = {}
	end
	local timerInfo = LuaFramework.TimerInfo(table, func)
	self.timerInfoList[table][func] = timerInfo
	timerMgr:AddTimerEvent(timerInfo);
	-- tools.PrintDebug("-----------AddTimerEvent添加定时器", table.__cname, table, func)
end

function TimerManager.RemoveTimerEvent(table, func)
	if not table or not func then
		return
	end
	if not self.timerInfoList[table] or not self.timerInfoList[table][func] then
		-- tools.PrintDebug("-----------RemoveTimerEvent找不到要移除的定时器！", table.__cname, table, func)
		return
	end
	timerMgr:RemoveTimerEvent(self.timerInfoList[table][func])
	self.timerInfoList[table][func] = nil
	-- tools.PrintDebug("-----------RemoveTimerEvent移除定时器", table.__cname, table, func)
end

function TimerManager.ClearTimerEvent()
	for k,dic in pairs(self.timerInfoList) do
		for func,n in pairs(dic) do
			self.RemoveTimerEvent(k, func)
		end
	end
end