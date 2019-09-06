local BaseView = require("UI.BaseView")
local FightView = class("FightView", BaseView)
local MapItem = require("UI.Fight.MapItem")
local CardItem = require("UI.Fight.CardItem")
local MyCardItem = require("UI.Fight.MyCardItem")

function FightView:ctor()
	self.super.ctor(self, "FightView")
end

function FightView:InitPanel()
	self:CreateBaseView()
	self:InitView()
end

function FightView:InitView()
	self.CardScrollView = tools.FindChild(self.transform, "CardScrollView", tools.UIScrollView)
	self.MapScrollView = tools.FindChild(self.transform, "MapScrollView", tools.UIScrollView)
	self.CardScrollView_UIGrid = tools.FindChild(self.transform, "CardScrollView/UIGrid", tools.UIGrid)
	self.MapScrollView_UIGrid = tools.FindChild(self.transform, "MapScrollView/UIGrid", tools.UIGrid)
	self.StartBtn = tools.FindChildObj(self.transform, "UI/StartBtn")
	self.Result = tools.FindChildObj(self.transform, "Result")
	self.Result_Mask = tools.FindChildObj(self.transform, "Result/Mask")
	self.Result_Label = tools.FindChild(self.transform, "Result/Label")
	self.RturnBtn = tools.FindChildObj(self.transform, "Result/RturnBtn")
	self.RefightBtn = tools.FindChildObj(self.transform, "Result/RefightBtn")
	self.TestBtn = tools.FindChildObj(self.transform, "UI/TestBtn")
	self.TestBtn2 = tools.FindChildObj(self.transform, "UI/TestBtn2")

	self:InitMap()
	

	self:InitEvent()
end

function FightView:InitEvent()
	EventsManager.AddEvent(EventsManager.EventsName.Event_SelectMyCard, self, self.SelectMyCardHandle)
	EventsManager.AddEvent(EventsManager.EventsName.Event_SelectCardItem, self, self.SelectCardItemHandle)
	EventsManager.AddEvent(EventsManager.EventsName.Event_SelectMapItem, self, self.SelectMapItemHandle)
	EventsManager.AddEvent(EventsManager.EventsName.Event_CardDead, self, self.DeadHandle)
	EventsManager.AddEvent(EventsManager.EventsName.Event_CardHurt, self, self.HurtHandle)
	EventsManager.AddEvent(EventsManager.EventsName.Event_CardMove, self, self.CardMoveHandle)
	EventsManager.AddEvent(EventsManager.EventsName.Event_FightResult, self, self.FightResultHandle)
	EventsManager.AddEvent(EventsManager.EventsName.Event_CardAttack, self, self.CardAttackHandle)
	

	self.luaBehaviour:AddClick(self.StartBtn, self, self.StartHandle)
	self.luaBehaviour:AddClick(self.Result_Mask, self, self.ClickHandle)
	self.luaBehaviour:AddClick(self.RturnBtn, self, self.ClickHandle)
	self.luaBehaviour:AddClick(self.RefightBtn, self, self.ClickHandle)
	self.luaBehaviour:AddClick(self.TestBtn, self, self.ClickHandle)
	self.luaBehaviour:AddClick(self.TestBtn2, self, self.ClickHandle)
end

function FightView:GetCardItem(info)
	local list = info.Team == Enum_TeamType.Enemy and self.enemyList or self.myList
	for k,v in pairs(list) do
		if v.info == info then
			return v
		end
	end
end

function FightView:HurtHandle(data)
	self:GetCardItem(data.info):HurtAction(data.hurt)
end

function FightView:CardAttackHandle(data)
	local item = self:GetCardItem(data.info)
	item:AttackAction(self:GetCardItem(data.beattacker))
	-- local pos = item.transform.localPosition
	-- local target = self:GetCardItem(data.beattacker)
	-- TweenRotation.Begin(item.gameObject, 0.1, Quaternion(0, 0, 90, 0))
	-- -- TweenPosition.Begin(item.gameObject, 0.1, target.transform.localPosition)
	-- tools.Invoke(0.1, function()
	-- 	TweenRotation.Begin(item.gameObject, 0.1, Quaternion(0, 0, 0, 0))
	-- 	-- TweenPosition.Begin(item.gameObject, 0.1, Vector3(pos.x, pos.y, 0))
	-- end)
end

function FightView:DeadHandle(info)
	-- local list = info.Team == Enum_TeamType.Enemy and self.enemyList or self.myList
	-- for k,v in pairs(list) do
	-- 	if v.info == info then
	-- 		v:DeadAction()
	-- 		return
	-- 	end
	-- end
end

function FightView:CardMoveHandle(data)
	local item = self:GetCardItem(data.info)
	TweenPosition.Begin(item.gameObject, 0.2, Vector3(data.pos.x, data.pos.y, 0))
	tools.Invoke(0.2, function()
		data.info.CardAiAction:StartAI()
	end)
end

function FightView:StartHandle()
	self.StartBtn:SetActive(false)
	FightScene.StartFight()
end

function FightView:InitMap()
	self.mapList = {}
	for i=1,FightScene.Page do
		for j=1,13 do
			for k=1,FightScene.Column do
				local item = MapItem.new(self.MapScrollView.transform, #self.mapList + 1, self.luaBehaviour)
				local pos = FightScene.GetPos(#self.mapList + 1)
				-- LuaToCSFunction.SetGameObjectLocalPosition(item.gameObject, (i-1)*1280-590 + (j-1)*100, 310-(k-1)*100, 0)
				LuaToCSFunction.SetGameObjectLocalPosition(item.gameObject, pos.x, pos.y, 0)
				table.insert(self.mapList, item)
			end
		end
	end
end

function FightView:InitCard()
	if self.cardList then
		for k,v in pairs(self.cardList) do
			v:Dispose()
		end
	end
	if self.enemyList then
		for k,v in pairs(self.enemyList) do
			v:Dispose()
		end
	end
	if self.myList then
		for k,v in pairs(self.myList) do
			v:Dispose()
		end
	end
	self.selectIndex = 0
	self.cardList = {}
	for i,v in pairs(CardManager.CardList) do
		v.index = i
		local item = MyCardItem.new(self.CardScrollView_UIGrid.transform, v, self.luaBehaviour, self.SelectHandle)
		table.insert(self.cardList, item)
	end
	self.CardScrollView_UIGrid:Reposition()
	self.enemyList = {}
	for i,v in pairs(FightScene.EnemyDic) do
		local item = CardItem.new(self.MapScrollView.transform, v, self.luaBehaviour)
		table.insert(self.enemyList, item)
	end
	self.myList = {}
end

function FightView:AddCardItem(info, place)
	info:SetPlace(place)
	local item = CardItem.new(self.MapScrollView.transform, info, self.luaBehaviour)
	if info.Team == Enum_TeamType.Enemy then
		table.insert(self.enemyList, item)
	else
		table.insert(self.myList, item)
		self.StartBtn:SetActive(true)
	end
	FightScene.AddMyCard(info)
end

function FightView:RemoveMyCardItem(info)
	local preIndex = info.index
	self.cardList[preIndex]:Dispose()
	for i=info.index,#self.cardList do
		self.cardList[i].info.index = self.cardList[i].info.index - 1
	end
	table.remove(self.cardList, preIndex)
	self.CardScrollView_UIGrid:Reposition()
end

function FightView:SelectMyCardHandle(data)
	self.selectIndex = 0
	if data.item.selected then
		self.selectIndex = data.info.index
		for k,v in pairs(self.cardList) do
			if data.info.index ~= k then
				v:SetSelected(false)
			end
		end
	end
end

function FightView:SelectCardItemHandle(data)
	if self.selectIndex == 0 then
		return
	end

end

function FightView:SelectMapItemHandle(data)
	if self.selectIndex == 0 then
		return
	end
	self:AddCardItem(self.cardList[self.selectIndex].info, data.info)
	self:RemoveMyCardItem(self.cardList[self.selectIndex].info)
	self.selectIndex = 0
end

function FightView:FightResultHandle(result)
	self.Result:SetActive(true)
	if result == 1 then
		self.Result_Label.text = "You Win！"
	else
		self.Result_Label.text = "You Lose！"
	end
end

function FightView:ClickHandle(go)
	if go == self.Result_Mask then
		self.Result:SetActive(false)
	elseif go == self.RturnBtn then
		FightScene.InitMonster()
		self:UpdateView()
	elseif go == self.RefightBtn then
		FightScene.InitMonster()
		self:UpdateView()
	elseif go == self.TestBtn then
		for k,v in pairs(self.cardList) do
			if v.info.ID == 1 then
				self:AddCardItem(v.info, 9)
			elseif v.info.ID == 2 then
				self:AddCardItem(v.info, 14)
			elseif v.info.ID == 3 then
				self:AddCardItem(v.info, 22)
			elseif v.info.ID == 4 then
				self:AddCardItem(v.info, 10)
			elseif v.info.ID == 5 then
				self:AddCardItem(v.info, 4)
			elseif v.info.ID == 6 then
				self:AddCardItem(v.info, 23)
			end
		end
	elseif go == self.TestBtn2 then
		for k,v in pairs(self.cardList) do
			if v.info.ID == 1 then
				self:AddCardItem(v.info, 17)
			elseif v.info.ID == 2 then
				self:AddCardItem(v.info, 18)
			elseif v.info.ID == 3 then
				self:AddCardItem(v.info, 23)
			elseif v.info.ID == 4 then
				self:AddCardItem(v.info, 24)
			elseif v.info.ID == 5 then
				self:AddCardItem(v.info, 29)
			elseif v.info.ID == 6 then
				self:AddCardItem(v.info, 30)
			end
		end
	end
end



function FightView:UpdateView()
	self.Result:SetActive(false)
	self.StartBtn:SetActive(false)
	self:InitCard()
end

function FightView:RemoveEvent()
	EventsManager.RemoveEvent(EventsManager.EventsName.Event_SelectMyCard, self, self.SelectMyCardHandle)
	EventsManager.RemoveEvent(EventsManager.EventsName.Event_SelectCardItem, self, self.SelectCardItemHandle)
	EventsManager.RemoveEvent(EventsManager.EventsName.Event_SelectMapItem, self, self.SelectMapItemHandle)
	EventsManager.RemoveEvent(EventsManager.EventsName.Event_CardDead, self, self.DeadHandle)
	EventsManager.RemoveEvent(EventsManager.EventsName.Event_CardHurt, self, self.HurtHandle)
end

function FightView:OnCloseHandler()
	self:RemoveEvent()
	self:UnloadAssetBundle()
end

function FightView:OnDestroy()
	self:RemoveEvent()
	self.super.OnDestroy(self)
end

return FightView