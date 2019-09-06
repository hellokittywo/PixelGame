local HurtEffect = class("HurtEffect")
local self = HurtEffect
local HurtEffectItem = require("UI.Fight.HurtEffectItem")
local index = 0
local time = 0.5
local list = {}

function HurtEffect.ShowHurt(item, data)
	table.insert(list, {item = item, data = data})
	if not self.timer then
		self.timer = Timer.New(function()
			self:ShowHurtEffect()
		end, time, -1)
		self.timer:Start()
		self:ShowHurtEffect()
	end
end

function HurtEffect:ShowHurtEffect()
	local temp = list[1]
	if not temp then
		return
	end
	local item = temp.item
	local data = temp.data
	local effect = HurtEffectItem.new(FightScene.HurtEffectObj, data)
	local pos = item.transform.localPosition
	LuaToCSFunction.SetGameObjectLocalPosition(effect.gameObject, pos.x, pos.y, 0)
	TweenPosition.Begin(effect.gameObject, time, Vector3(pos.x, pos.y + 20, 0))
	tools.Invoke(time, function()
		effect:Dispose()
	end)
	table.remove(list, 1)
end

return HurtEffect