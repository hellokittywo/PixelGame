local HurtEffectItem = class("HurtEffectItem")

function HurtEffectItem:ctor(parent, info)
	self.parent = parent
	self.info = info
	self:InitView()
end

function HurtEffectItem:InitView()
	self.gameObject = LuaToCSFunction.LoadPrefabByName("Prefabs/Fight/HurtEffectItem", self.parent)
	self.transform = self.gameObject.transform
	LuaToCSFunction.SetGameObjectLocalPosition(self.gameObject, 0, 0, 0)
	self.EffectLael = tools.FindChild(self.transform, "EffectLael")
	self.HurtLabel = tools.FindChild(self.transform, "HurtLabel")
	self.Icon_Sprite = tools.FindChild(self.transform, "Icon", tools.UISprite)
	self.EffectLael.text = ""
	self.HurtLabel.text = "-"..self.info.hurt
end

function HurtEffectItem:Dispose()
	LuaToCSFunction.PoolDestroy(self.gameObject)
end

return HurtEffectItem