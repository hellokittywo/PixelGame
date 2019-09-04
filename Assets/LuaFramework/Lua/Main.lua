require "Common/define"
require "Common/functions"
AudioName = require "Common/AudioName"
require "Manager/TimerManager"
require "Manager/EventsManager"
require "Manager/CtrlManager"
require "Manager/PlayerManager"
require "Manager/BagManager"
require "Manager/CardManager"
-- require "Manager/QuestManager"
require "Scene.Fight.FightScene"
tools = require("Common.tools")
require("Common.tables")

--主入口函数。从这里开始lua逻辑
function Main()
	Global_Platform = LuaToCSFunction.GetApplicationPlatform()
	if tools.Debug  then
		LuaToCSFunction.AddOutLog()
	end
	math.randomseed(os.time())
	CtrlManager.Init()
	PlayerManager.Init()
	tools.SetIphoneXAdapt()
	-- if not GameStart.IsMute then
		-- CtrlManager.openPanel(CtrlManager.CtrlNames.MainCtrl)
		FightScene.Init()
	-- end
end

function LoadingComplete()
	if CtrlManager.GetCtrl(CtrlManager.CtrlNames.CreateRoleCtrl) then
		CtrlManager.GetCtrl(CtrlManager.CtrlNames.CreateRoleCtrl).view:ShowPlayer()
	end
end

function EnterFightScene()
	FightScene.Init()
	tools.SetIphoneXAdapt()
end

function EnterMainScene()
	tools.SetIphoneXAdapt()
end

--设备切到前台
function GameEnterForeground()
	
end

--设备切到后台
function GameEnterBackground()
end

local json = require "cjson"
function PayMoneySuccess(receipt_data)
	if receipt_data ~= "" then
		tools.PrintDebug("################## 发送订单给服务器", receipt_data)
		CtrlManager.clearAllPanel()
		if Global_Platform == 1 then
			--安卓支付的回复
			local data = tools.Split(receipt_data, "|")
			ItemSocketSend.ItemSendList[ItemChildPackageType.PayMoneyAdroid](data);
		elseif Global_Platform == 2 then
			--苹果支付的回复
			local obj = json.decode(receipt_data)
			-- tools.PrintDebug("---------充值成功Json数据", obj['receipt_data'], obj['iapId']);
			local data = {receipt_data = obj['receipt_data'], IapId = obj['iapId']};
			ItemSocketSend.ItemSendList[ItemChildPackageType.PayMoneySuccess](data);
		end
	end
end

function PayMoneyFailed(result)
	tools.PrintDebug("充值失败！")
	--提示
end

function OnDragDropStart(itemid)
	EventsManager.DispatchEvent(EventsManager.EventsName.Event_OnDragDropStart, {ItemID = itemid})
end

function OnDragDropRelease(source, target, itemid)
	EventsManager.DispatchEvent(EventsManager.EventsName.Event_OnDragDropRelease, {Source = source, Target = target, ItemID = itemid})
end