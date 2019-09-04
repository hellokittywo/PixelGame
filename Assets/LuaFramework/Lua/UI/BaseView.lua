local BaseView = class("BaseView")

function BaseView:ctor(name)
	self.name = name
end

function BaseView:Awake(obj)
	self.gameObject = obj 
	self.transform = obj.transform
	self.panel = self.transform:GetComponent('UIPanel')
	if self.panel then
		self.panelOriginDepth = self.panel.depth
	end
	self.luaBehaviour = self.transform:GetComponent('LuaBehaviour')
	self:InitPanel()
	self.transform.localPosition = Vector3.zero
	self:UpdateView()
    self.panelInited = true
end

function BaseView:InitPanel()
	
end

function BaseView:UpdateView()
	
end

function BaseView:CreateBaseSceneBg()
	self.BaseSceneBg = LuaToCSFunction.LoadPrefabByName("Prefabs/Common/BaseSceneBg", self.transform);
	LuaToCSFunction.SetGameObjectLocalPosition(self.BaseSceneBg, 0, 0, 0)
	-- self.MaskObj = tools.FindChildObj(self.BaseSceneBg.transform, "Mask")
	self.BaseSceneBg_ReturnBtn = tools.FindChildObj(self.BaseSceneBg.transform, "ReturnBtn")
	self.luaBehaviour:AddClick(self.BaseSceneBg_ReturnBtn, self, self.OnRetunBtnClickHandler)
end

function BaseView:CreateBaseView(width, height)
	local width = width or 1170
	local height = height or 648
    self.BasePanelBg = LuaToCSFunction.LoadPrefabByName("Prefabs/Common/BasePanelBg", self.transform);
    local transform = self.BasePanelBg.transform
	LuaToCSFunction.SetGameObjectLocalPosition(self.BasePanelBg, 0, 0, 0)
	self.MaskObj = tools.FindChildObj(transform, "Mask")
	self.luaBehaviour:AddClick(self.MaskObj, self, self.OnMaskClickHandler)
	self.BasePanelBg_Title = tools.FindChildObj(transform, "Title")
	self.BasePanelBg_Title1 = tools.FindChildObj(transform, "Title/Title1")
	self.BasePanelBg_Title1Icon = tools.FindChild(transform, "Title/Title1/Icon", tools.UISprite)
	self.BasePanelBg_Title2 = tools.FindChildObj(transform, "Title/Title2")
	self.BasePanelBg_Title2Icon = tools.FindChild(transform, "Title/Title2/Icon", tools.UISprite)
	self.BasePanelBg_Title3 = tools.FindChildObj(transform, "Title/Title3")
	self.BasePanelBg_Title3Label = tools.FindChild(transform, "Title/Title3/Label")
	self.BasePanelBg_CloseBtn = tools.FindChildObj(transform, "CloseBtn")
	LuaToCSFunction.SetGameObjectLocalPosition(self.BasePanelBg_CloseBtn, 499, 214, 0)
	self.BasePanelBg_CloseBtn:SetActive(true)
	self.BasePanelBg_Bg = tools.FindChild(transform, "Bg", tools.UISprite)
	self.BasePanelBg_Bg.width = width
	self.BasePanelBg_Bg.height = height
	LuaToCSFunction.SetGameObjectLocalPosition(self.BasePanelBg_Bg.gameObject, 0, -40, 0)
	self.luaBehaviour:AddClick(self.BasePanelBg_CloseBtn, self, self.OnCloseHandler)
end

-- function BaseView:CreateMask()
-- 	self.MaskObj = LuaToCSFunction.LoadPrefabByName("Prefabs/Common/BaseMask", self.transform)
-- 	LuaToCSFunction.SetGameObjectLocalPosition(self.MaskObj, 0, 0, 0)
-- 	self.MaskSprite = self.MaskObj:GetComponent(tools.UISprite)
--     self.MaskSprite.alpha = 151 / 255
-- end

function BaseView:OnRetunBtnClickHandler()

end

function BaseView:OnMaskClickHandler()

end

function BaseView:OnCloseHandler()
	
end

function BaseView:UnloadAssetBundle(param)
	-- if self.MaskObj then
	-- 	LuaToCSFunction.PoolDestroy(self.MaskObj)
	-- end
	local param = param or {}
	local ctrlName = self.ctrlName
	local isHidePanel = param.isHidePanel or false
	if isHidePanel then
		CtrlManager.hidePanel(ctrlName)
	else
		CtrlManager.closePanel(ctrlName)
	end
	-- CtrlManager.RemoveCtrl(ctrlName)
	-- self.unloadTimer = Timer.New(function()
	-- 	if param.callBack then
	-- 		param.callBack()
	-- 	end
	-- 	tools.PrintDebug("------------BasePanel移除资源UnloadAssetBundle", self.name)
	-- 	LuaToCSFunction.UnloadAssetBundle(self.gameObject)--这里会检测引用计数销毁assetbundle
	-- 	self.gameObject = nil
	-- 	self.transform = nil
	-- 	self = nil
	-- end, 0.01, 0)
	-- self.unloadTimer:Start()
end

function BaseView:OnDestroy()
	tools.PrintDebug("------------BasePanel移除资源OnDestroy", self.name)
	if self.gameObject then
		if self.BaseSceneBg then
			LuaToCSFunction.PoolDestroy(self.BaseSceneBg)
		end
		if self.BasePanelBg then
			LuaToCSFunction.PoolDestroy(self.BasePanelBg)
		end
		LuaToCSFunction.UnloadAssetBundle(self.gameObject)
		self.gameObject = nil
	end
	self.transform = nil
	self = nil
end

function BaseView:Close()
	if self.model then
		self.model.GoParam = nil
	end
	tools.PrintDebug("----------关闭ＵＩ,预设名字：", self.name)
	if self.gameObject then
		self.gameObject:SetActive(false)
	end
end

function BaseView:Unload()

end

return BaseView