local CardAiAction = class("CardAiAction")
local FightAStar = require("Scene.Fight.FightAStar")

function CardAiAction:ctor(card)
	self.info = card
	self.FightAStar = FightAStar.new()
end

function CardAiAction:GetAttackList()
	local info = self.info
	local list = {}
	local dic = (info.Team == Enum_TeamType.Enemy) and FightScene.MyDic or FightScene.EnemyDic
	local temp = {}
	temp[1] = info.Place - FightScene.Column - 1
	temp[2] = info.Place - FightScene.Column
	temp[3] = info.Place - FightScene.Column + 1
	temp[4] = info.Place - 1
	temp[5] = info.Place + 1
	temp[6] = info.Place + FightScene.Column - 1
	temp[7] = info.Place + FightScene.Column
	temp[8] = info.Place + FightScene.Column + 1
	for i,place in pairs(temp) do
		for k,v in pairs(dic) do
			if v.Place == place then
				table.insert(list, v)
			end
		end
	end
	return list
end

function CardAiAction:StartAI()
	TimerManager.RemoveTimerEvent(self, self.Attack)
	local list = self:GetAttackList()
	self.attackList = list
	-- print("---可攻击个数", #list)
	local dis = 10000
	local target
	if #list == 0 then
		local dic = self.info.Team == Enum_TeamType.Enemy and FightScene.MyDic or FightScene.EnemyDic
		for k,v in pairs(dic) do
			local temp = self:GetDistance(v, self.info)
			if temp < dis then
				target = v
				dis = temp
			end
		end
		if not target then
			return
		end
		-- FightScene.RefreshObstruct(target.Place)
		self.FightAStar:GetPathList(self.info, target, function(pathList)
			-- if self.info.Name == "哪吒" then
			-- 	print("----target2222", self.info.Place, target.Place, #pathList)
			-- 	for k,v in pairs(FightScene.ObstructDic) do
			-- 		print(k,v)
			-- 	end
			-- 	print("-------------------------------------")
			-- end
			self.pathList = pathList
			self:MoveAction()
		end)
	else
		self.beattacker = self.attackList[1]
		self.attackIndex = 1
		TimerManager.AddTimerEvent(self, self.Attack)
	end
end

function CardAiAction:MoveAction()
	if #self.attackList == 0 then
		self.timer = Timer.New(function()
			-- print("----路不通1 ", self.info.Name, self.stopMove, self.pathList[1])
			if self.stopMove then
				return
			end
			if self.pathList[1] then
				local pos, place = FightScene.GetPosByXY(self.pathList[1])
				if FightScene.ObstructDic[place] then
					-- print("----路不通2 ", self.info.Name, place)
					self:StartAI()
					return
				end
				-- if self.info.Name == "沙悟净" then
				-- 	print("----移动", self.pathList[1].x, self.pathList[1].y, self.info.Name)
				-- end
				EventsManager.DispatchEvent(EventsManager.EventsName.Event_CardMove, {pos = pos, info = self.info})
				table.remove(self.pathList, 1)
				-- print("------移动", self.info.Name, place)
				-- for k,v in pairs(FightScene.ObstructDic) do
				-- 	print(k)
				-- end
				-- print("-------------------------------------")
				self.info:SetPlace(place)
				FightScene.RefreshObstruct()
				-- FightScene.ObstructDic[place] = 1
				local list = self:GetAttackList()
				for k,v in pairs(list) do
					v.CardAiAction:StopMove()
					-- EventsManager.DispatchEvent(EventsManager.EventsName.Event_StopMove, {info = v})
				end
				self.timer:Stop()
				self.timer = nil
				-- self:StartAI()
			else
				-- print("----走不动", self.info.Name)
				self:StartAI()
			end
		end, 0.5)
		self.timer:Start()
	end
end

function CardAiAction:StopMove()
	self.stopMove = true
	if self.timer then
		self.timer:Stop()
		self.timer = nil
	end
	self:StartAI()
end

function CardAiAction:GetDistance(card1, card2)
	return math.pow((card1.PosY - card2.PosY), 2) + math.pow((card1.PosX - card2.PosX), 2)
end

function CardAiAction:Attack()
	-- if self.attackList[self.attackIndex] then
	-- 	FightScene.Attack(self.info, self.attackList[self.attackIndex])
	-- 	self.attackIndex = self.attackIndex + 1
	-- else
	-- 	self.attackIndex = 1
	-- 	self:Attack()
	-- end
	if self.beattacker and self.beattacker.HP > 0 then
		FightScene.Attack(self.info, self.beattacker)
	else
		self:StartAI()
	end
end

function CardAiAction:Dispose()
	TimerManager.RemoveTimerEvent(self, self.Attack)
end

return CardAiAction