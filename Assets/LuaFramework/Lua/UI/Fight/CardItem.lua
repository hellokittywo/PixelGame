local CardItem = class("CardItem")

function CardItem:ctor(parent, info, luaBehaviour)
	self.parent = parent
	self.info = info
	self.luaBehaviour = luaBehaviour
	self:InitView()
end

function CardItem:InitView()
	self.gameObject = LuaToCSFunction.LoadPrefabByName("Prefabs/Fight/CardItem", self.parent)
	self.transform = self.gameObject.transform
	LuaToCSFunction.SetGameObjectLocalPosition(self.gameObject, self.info.Pos.x, self.info.Pos.y, 0)
	self.NameLabel = tools.FindChild(self.transform, "NameLabel")
	self.Icon_Sprite = tools.FindChild(self.transform, "Icon", tools.UISprite)
	self.BoodSlider = tools.FindChild(self.transform, "BoodSlider", tools.UISlider)
	self.NameLabel.text = self.info.Name

	local str = "Card_Bg1"
	if self.info.Team == Enum_TeamType.Enemy then
		str = "Enemy_Bg1"
	end
	tools.SetSpriteName(self.Icon_Sprite, str, {width = 100, height = 100})
	self.luaBehaviour:RemoveClick(self.gameObject)
	self.luaBehaviour:AddClick(self.gameObject, self, self.OnClickHandler)
end

function CardItem:HurtAction(hurt)
	self.BoodSlider.value = self.info.HP / self.info.MaxHP
	HurtEffect.ShowHurt(self, {hurt = hurt})
	-- local scale = 0
	-- if self.info.HP <= 0 then
	-- 	TweenScale.Begin(self.gameObject, 0, Vector3(1, 1, 1))
	-- 	TweenScale.Begin(self.gameObject, 0.1, Vector3(scale, scale, scale))
	-- else
	-- 	scale = 0.9
	-- 	TweenScale.Begin(self.gameObject, 0.1, Vector3(scale, scale, scale))
	-- 	tools.Invoke(0.1, function()
	-- 		TweenScale.Begin(self.gameObject, 0.1, Vector3(1, 1, 1))
	-- 	end)
	-- end
end

function CardItem:AttackAction(target)
	local pos = self.transform.localPosition
	local pos2 = target.transform.localPosition
	-- TweenRotation.Begin(self.gameObject, 0.1, Quaternion(0, 0, 90, 0))
	TweenPosition.Begin(self.gameObject, 0.1, pos2)
	tools.Invoke(0.1, function()
		-- TweenRotation.Begin(self.gameObject, 0.1, Quaternion(0, 0, 0, 0))
		TweenPosition.Begin(self.gameObject, 0.1, pos)
	end)
end

function CardItem:DeadAction()
	self.BoodSlider.value = 0
	local scale = 0
	TweenScale.Begin(self.gameObject, 0.2, Vector3(scale, scale, scale))
	print("---死了呢")
end

function CardItem:OnClickHandler(go)
	EventsManager.DispatchEvent(EventsManager.EventsName.Event_SelectCardItem, {item = self, info = self.info})
end

function CardItem:Dispose()
	self.BoodSlider.value = 1
	LuaToCSFunction.PoolDestroy(self.gameObject)
end

return CardItem