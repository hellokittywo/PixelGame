local MyCardItem = class("MyCardItem")

function MyCardItem:ctor(parent, info, luaBehaviour)
	self.parent = parent
	self.info = info
	self.luaBehaviour = luaBehaviour
	self:InitView()
end

function MyCardItem:InitView()
	self.gameObject = LuaToCSFunction.LoadPrefabByName("Prefabs/Fight/MyCardItem", self.parent)
	self.transform = self.gameObject.transform
	LuaToCSFunction.SetGameObjectLocalPosition(self.gameObject, 0, 0, 0)
	self.NameLabel = tools.FindChild(self.transform, "NameLabel")
	self.NameLabel.text = self.info.Name

	self.selected = false
	self.luaBehaviour:RemoveClick(self.gameObject)
	self.luaBehaviour:AddClick(self.gameObject, self, self.OnClickHandler)
end

function MyCardItem:OnClickHandler(go)
	self.selected = not self.selected
	local scale = self.selected == true and 1.3 or 1
	TweenScale.Begin(self.gameObject, 0.1, Vector3(scale, scale, scale))
	EventsManager.DispatchEvent(EventsManager.EventsName.Event_SelectMyCard, {item = self, info = self.info})
end

function MyCardItem:SetSelected(selected)
	self.selected = selected
	local scale = self.selected == true and 1.3 or 1
	TweenScale.Begin(self.gameObject, 0.1, Vector3(scale, scale, scale))
end

function MyCardItem:Dispose()
	LuaToCSFunction.PoolDestroy(self.gameObject)
end

return MyCardItem