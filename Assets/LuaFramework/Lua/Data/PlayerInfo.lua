local PlayerInfo = class("PlayerInfo")

function PlayerInfo:ctor()
	
end

function PlayerInfo:InitData(data)
	self.Energy = data.Energy or 100
	BagManager.InitData(savedData)
	CardManager.InitData(savedData)
end

function PlayerInfo:AddEnergy(num)
	self.Energy = self.Energy + num
end

return PlayerInfo