CtrlManager = {}
CtrlManager.CtrlNames = {
	FightCtrl = "UI.Fight.FightCtrl",
}
local this = CtrlManager
local ctrlList = {}	--控制器列表--
local ctrlNameTable = {

}

function CtrlManager.Init()
	this.InitCommonGameMask()
	return this
end

function CtrlManager.InitCommonGameMask()
	this.UIRoot = GameObject.Find("UI_Root")
	-- this.CommonGameMask = LuaToCSFunction.LoadPrefabByName("Prefabs/Common/GameMask", this.UIRoot.transform)
	-- UnityEngine.Object.DontDestroyOnLoad(this.CommonGameMask)
	-- this.CommonGameMask.transform.localPosition = Vector3.zero
	-- this.CommonGameMask:SetActive(false)
	-- this.CommonMaskSprite = tools.FindChildObj(this.CommonGameMask.transform, "Mask"):GetComponent(tools.UISprite)
	-- this.maskVisible = false
end

--添加控制器--
function CtrlManager.AddCtrl(ctrlName, ctrlObj)
	ctrlList[ctrlName] = ctrlObj
end

--获取控制器--
function CtrlManager.GetCtrl(ctrlName)
	return ctrlList[ctrlName]
end

function CtrlManager.CreateCtrl(ctrlName)
	local panelCtrl = this.GetCtrl(ctrlName)
	if not panelCtrl  then
		panelCtrl = require(ctrlName):new()
		panelCtrl.view.ctrlName = ctrlName
		this.AddCtrl(ctrlName, panelCtrl)
	end
	return panelCtrl
end

function CtrlManager.setData(ctrlName, data)
	local panelCtrl = this.CreateCtrl(ctrlName)
	panelCtrl:SetData(data)
end

function CtrlManager.openPanel(ctrlName)
	--隐藏前一个界面
	--添加当前需要显示的界面 table
	--显示当前界面
	if #ctrlNameTable > 0 and ctrlNameTable[#ctrlNameTable] then
		local hideCtrl = ctrlNameTable[#ctrlNameTable]
		this.hidePanel(hideCtrl)
	end
	ctrlNameTable[#ctrlNameTable + 1] = ctrlName
	-- this.CommonGameMask:SetActive(true)
	return this.showPanel(ctrlName)
end

function CtrlManager.showPanel(ctrlName)
	-- if not this.CommonGameMask then
	-- 	return
	-- end
	local panelCtrl = this.CreateCtrl(ctrlName)
	panelCtrl:Awake()
	return panelCtrl
end

function CtrlManager.closePanel(ctrlName)
	--关闭界面
	--从table里面移除
	--显示table里面最后的一个界面
	this.hidePanel(ctrlName)
	table.remove(ctrlNameTable, #ctrlNameTable, 1)
	if #ctrlNameTable > 0 and ctrlNameTable[#ctrlNameTable] then
		this.showPanel(ctrlNameTable[#ctrlNameTable])
	else
		-- if this.CommonGameMask then
		-- 	this.CommonGameMask:SetActive(false)
		-- end
	end

end

function CtrlManager.hidePanel(ctrlName)
	if not ctrlList[ctrlName] then 
		error("Not panel:"..ctrlName.."!") 
		return
	end
	ctrlList[ctrlName]:Close()
end

function CtrlManager.RemoveCtrl(ctrlName)
	ctrlList[ctrlName] = nil
end

function CtrlManager.SwithSceneClearCtrl()
	-- if this.CommonGameMask then
	-- 	LuaToCSFunction.PoolDestroy(this.CommonGameMask)
	-- end
	-- this.CommonGameMask = nil
	-- this.CommonMaskSprite = nil
	if this.timer then
		this.timer:Stop()
		this.timer = nil
	end
	for k,v in pairs(ctrlList) do
		tools.PrintDebug("--------------切换账号销毁View", v.name)
		v:OnDestroy();
		ctrlList[k] = nil
	end
	ctrlNameTable = {}
	ctrlList = {}
end

function CtrlManager.clearAllPanel()
	for i=1,#ctrlNameTable do
		this.hidePanel(ctrlNameTable[i])
		ctrlList[ctrlNameTable[i]].view:Unload()
	end
	ctrlNameTable = {}
	-- this.CommonGameMask:SetActive(false)
end

function CtrlManager.GetCtrlTabel()
	return ctrlNameTable;
end

function CtrlManager.IsCurrentViewShow(ctrlName)
	for name,v in pairs(ctrlList) do
		if name == ctrlName and v.active then
			return true;
		end
	end
	return false;
end