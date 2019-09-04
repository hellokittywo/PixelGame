BagManager = {}
local self = BagManager
local ItemInfo = require("Data.ItemInfo")

function BagManager.InitData(data)
	self.ItemList = {}
	self.ItemDic = {}
end

function BagManager.GetEquipDicByCard(id)
	local dic = {}
	for k,v in pairs(self.ItemDic) do
		if v.CardID == id then
			dic[v.Place] = v
		end
	end
	return dic
end

function BagManager.GetEquip(place, type)
	for k,v in pairs(self.ItemDic) do
		if v.Type == type and v.Place == place then
			return v
		end
	end
end