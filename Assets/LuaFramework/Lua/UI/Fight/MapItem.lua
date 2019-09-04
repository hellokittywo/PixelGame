local MapItem = class("MapItem")

function MapItem:ctor(parent, info, luaBehaviour)
	self.parent = parent
	self.info = info
	self.luaBehaviour = luaBehaviour
	self:InitView()
end

function MapItem:InitView()
	self.gameObject = LuaToCSFunction.LoadPrefabByName("Prefabs/Fight/MapItem", self.parent)
	self.transform = self.gameObject.transform
	LuaToCSFunction.SetGameObjectLocalPosition(self.gameObject, 0, 0, 0)
	self.Icon_Sprite = tools.FindChild(self.transform, "Icon", tools.UISprite)

	self.luaBehaviour:RemoveClick(self.gameObject)
	self.luaBehaviour:AddClick(self.gameObject, self, self.OnClickHandler)
end

function MapItem:OnClickHandler(go)
	EventsManager.DispatchEvent(EventsManager.EventsName.Event_SelectMapItem, {item = self, info = self.info})
end

function MapItem:Dispose()
	LuaToCSFunction.PoolDestroy(self.gameObject)
end

return MapItem