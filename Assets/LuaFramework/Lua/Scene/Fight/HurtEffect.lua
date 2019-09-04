local HurtEffect = class("HurtEffect")
local HurtEffectItem = require("UI.Fight.HurtEffectItem")

function HurtEffect.ShowHurt(item, data)
	local effect = HurtEffectItem.new(FightScene.HurtEffectObj, data)
	local pos = item.transform.localPosition
	LuaToCSFunction.SetGameObjectLocalPosition(effect.gameObject, pos.x, pos.y, 0)
	local time = 0.5
	TweenPosition.Begin(effect.gameObject, time, Vector3(pos.x, pos.y + 20, 0))
	tools.Invoke(time, function()
		effect:Dispose()
	end)
end

return HurtEffect