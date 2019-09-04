FightScene = {}
local self = FightScene
local CardInfo = require("Data.CardInfo")
HurtEffect = require("Scene.Fight.HurtEffect")

function FightScene.Init()
	LuaToCSFunction.PlayBgMusic(AudioName.FightScene, false, true)
	self.UIRoot = GameObject.Find("UI_Root")
	self.GameStart = GameObject.Find("GameStart")
	Global_SceneTransform = self.GameStart.transform
	self.MainCamera = tools.FindChild(self.GameStart.transform, "MainCamera", tools.Camera)
	self.UICamera = tools.FindChildObj(self.UIRoot.transform, "Camera"):GetComponent("Camera")
	self.EffectCamera = tools.FindChildObj(self.UIRoot.transform, "EffectCamera")
	self.HurtEffectObj = tools.FindChildObj(self.UIRoot.transform, "HurtEffectObj").transform
	Global_UIRoot = self.UIRoot
	Global_Scene = "FightScene"
	Global_MainCamera = self.MainCamera
	self.MyDic = {}
	self.InitMap()
	self.InitMonster()

	CtrlManager.openPanel(CtrlManager.CtrlNames.FightCtrl)
end

function FightScene.InitMap()
	-- 13x7的格子
	self.Width = 1280
	self.Height = 720
	self.ItemWidth = 1280 / 13
	self.ItemHeight = 720 / 7
	self.Column = 6
	self.MaxX = 13 * 2
	self.MaxY = 6
end

function FightScene.RefreshObstruct(place)
	self.ObstructDic = {}--障碍物
	for k,v in pairs(self.MyDic) do
		self.ObstructDic[v.Place] = 1
	end
	for k,v in pairs(self.EnemyDic) do
		self.ObstructDic[v.Place] = 1
	end
	-- self.ObstructDic[place] = nil
end

function FightScene.InitMonster()
	self.EnemyDic = {}
	local copy = tables.CopyDic[1]
	local list = tools.Split(copy.Monster, "|")
	for i,v in ipairs(list) do
		local p = tools.Split(v, ",")
		p[1] = 3
		self.EnemyDic[p[1]] = CardInfo.new(Enum_TeamType.Enemy, tonumber(p[2]), tonumber(p[1]))
		return
	end
end

function FightScene.GetPos(place)
	local i = math.ceil(place / self.Column)--x轴下标
	local j = place % (self.Column + 1)
	j = place - (i - 1) * self.Column
	-- local j = math.floor(place / (13 * 7))
	-- local k = place % 13
	
	local pos = {x = -(self.Width / 2 - 50) + (i - 1) * 100, y = self.Height / 2 - 50 - (j - 1) * 100}
	-- tools.PrintDebug("----坐标呢", place, i, j, k, pos.x, pos.y)
	return pos, i, j
end

function FightScene.GetPosByXY(data)
	local i = data.x
	local j = data.y
	local pos = {x = -(self.Width / 2 - 50) + (i - 1) * 100, y = self.Height / 2 - 50 - (j - 1) * 100}
	return pos, (i - 1) * self.Column + j
end

function FightScene.AddMyCard(info)
	self.MyDic[tostring(info.Place)] = info
end

function FightScene.StartFight()
	self.RefreshObstruct()
	self.Round = 1
	local index = 0
	for k,v in pairs(self.MyDic) do
		-- tools.Invoke(0.2 * index, function()
			-- if v.Name=="哪吒" then
				v.CardAiAction:StartAI()
			-- end
		-- end)
		index = index + 1
	end
	for k,v in pairs(self.EnemyDic) do
		-- v.CardAiAction:StartAI()
		-- tools.Invoke(0.2 * index, function()
			v.CardAiAction:StartAI()
		-- end)
		index = index + 1
	end
	-- self.InitCardList()
	-- self.InitTurnInfo()
	
end

function FightScene.CheckResult()
	local count = 0
	for k,v in pairs(self.MyDic) do
		count = count + 1
	end
	if count == 0 then
		return -1
	end
	count = 0
	for k,v in pairs(self.EnemyDic) do
		count = count + 1
	end
	if count == 0 then
		return 1
	end
	return 0
end

function FightScene.CardDead(info)
	local dic = info.Team == Enum_TeamType.Enemy and self.EnemyDic or self.MyDic
	for k,v in pairs(dic) do
		if v.Place == info.Place then
			v.CardAiAction:Dispose()
			dic[k] = nil
			return
		end
	end
end

function FightScene.Attack(attacker, beattacker)
	-- 物理伤害 = 系数1*物理攻击*物理攻击/（系数2*物理攻击+系数3*物理防御)*(1+伤害加成)*(1-伤害减免)
	-- tools.PrintDebug(attacker.Name.." 攻击 "..beattacker.Name.."   ")
	attacker:AttackAction({beattacker})
	local att = attacker.Attack
	local hurt = math.ceil(tables.ConfigDic[1] * att * att / (tables.ConfigDic[2] * att + tables.ConfigDic[3] * attacker.Defence))
	beattacker:BeAttack(hurt)
	if beattacker.HP <= 0 then
		self.CardDead(beattacker)
		local result = self.CheckResult()
		if result ~= 0 then
			for k,v in pairs(self.MyDic) do
				v.CardAiAction:Dispose()
			end
			for k,v in pairs(self.EnemyDic) do
				v.CardAiAction:Dispose()
			end
			EventsManager.DispatchEvent(EventsManager.EventsName.Event_FightResult, result)
			return
		end
		-- if beattacker.Team == Enum_TeamType.Enemy then
		-- 	for k,v in pairs(self.MyDic) do
		-- 		v.CardAiAction:StartAI()
		-- 	end
		-- else
		-- 	for k,v in pairs(self.EnemyDic) do
		-- 		v.CardAiAction:StartAI()
		-- 	end
		-- end
	end
	-- EventsManager.DispatchEvent(EventsManager.EventsName.Event_Attack, {attacker = self.info, beattacker = beattacker})
end


function FightScene.GetDistance(card1, card2)
	return math.pow((card1.PosY - card2.PosY), 2) + math.pow((card1.PosX - card2.PosX), 2)
end

function FightScene.InitTurnInfo()
	tools.PrintDebug("---第"..self.Wave.."波战斗")
	self.Sum1 = #self.Team1
	self.Sum2 = #self.Team2[self.Wave]
	self.FightCount1 = 0
	self.FightCount2 = 0
	self.TeamDead1 = 0
	self.TeamDead2 = 0
	self.SumTurn = 1
	self.TurnOver1 = false
	self.TurnOver2 = false
	self.IsSelfTurn = true
	self.FightLogic()
end

function FightScene.InitCardList()
	self.Team1 = nil
	self.Team1 = {}
	for k,v in pairs(CardManager.CardList) do
		if v.Place > 0 then
			self.Team1[#self.Team1 + 1] = tools.clone(v)
		end
	end

	self.Team2 = nil
	self.Team2 = {}
	for i=1,self.MapInfo.monster_wave do
		self.Team2[i] = {}
		self.FightRecordList[i] = {}
	end
	local list = tools.Split(self.MapInfo.monster_list, "|")
	for i=1,#list do
		local temp = tools.Split(list[i], ",")
		local place = tonumber(temp[1])
		local index = 1
		if place <= 5 then
			index = 1
		elseif place <= 10 then
			index = 2
		elseif place <= 15 then
			index = 3
		end
		self.Team2[index][#self.Team2[index] + 1] = CardInfo.new(Enum_TeamType.Enemy, tonumber(temp[2]), {Place = place - (index - 1) * 5})
	end
end

function FightScene.CardDeadHandler(_, card)
	if card.Team == Enum_TeamType.Self then
		self.TeamDead1 = self.TeamDead1 + 1
	else
		self.TeamDead2 = self.TeamDead2 + 1
	end
end

function FightScene.NextWave()
	self.Wave = self.Wave + 1
	if self.Wave <= self.MapInfo.monster_wave then
		self.InitTurnInfo()
	end
end

function FightScene.ShowFightView()
	for k,v in pairs(self.Team1) do
		v.HP = v.MaxHP
	end
	for k,v in pairs(self.Team2[self.Wave]) do
		v.HP = v.MaxHP
	end
	local view = CtrlManager.GetCtrl(CtrlManager.CtrlNames.FightCtrl).view
	self.FightView = view
	view:ShowFightView()
end

function FightScene.Drop()
	local drop = tables.DropDic[self.MapInfo.drop_id]
	-- drop.gold_rewards
	-- drop.soul_rewards
	-- drop.drop_number_max
	if drop.item_drop_1 > 0 then

	end
	if drop.item_drop_2 > 0 then

	end
end

function FightScene.FightLogic()
	if self.TeamDead1 >= self.Sum1 then
		tools.PrintDebug("---我方输了")
		self.ShowFightView()
		return
	end
	if self.TeamDead2 >= self.Sum2 then
		PlayerManager.PlayerInfo:AddEnergy(-self.MapInfo.copy_energy)
		tools.PrintDebug("---敌方输了")
		self.Drop()
		self.ShowFightView()
		return
	end
	if self.TurnOver1 and self.TurnOver2 then
		self.TurnOver1 = false
		self.TurnOver2 = false
		self.SumTurn = self.SumTurn + 1
		self.FightCount1 = 0
		self.FightCount2 = 0
	end
	local attacker
	if self.IsSelfTurn then
		if self.TurnOver1 then
			self.IsSelfTurn = false
			self:FightLogic()
			return
		end
		attacker = self.GetAttacker()
		if attacker then
			self.FireSkill(attacker)
		else
			self.IsSelfTurn = false
			self.FightLogic()
		end
	else
		if self.TurnOver2 then
			self.IsSelfTurn = true
			self:FightLogic()
			return
		end
		attacker = self.GetAttacker()
		if attacker then
			self.FireSkill(attacker)
		else
			self.IsSelfTurn = true
			self.FightLogic()
		end
	end
end

function FightScene.GetAttacker()
	local list = self.IsSelfTurn and self.Team1 or self.Team2[self.Wave]
	if self.IsSelfTurn then
		self.FightCount1 = self.FightCount1 + 1
		if self.FightCount1 > self.Sum1 then
			self.TurnOver1 = true
			return
		end
		if list[self.FightCount1].HP <= 0 then
			return self.GetAttacker()
		else
			return list[self.FightCount1]
		end
	else
		self.FightCount2 = self.FightCount2 + 1
		if self.FightCount2 > self.Sum2 then
			self.TurnOver2 = true
			return
		end
		if list[self.FightCount2].HP <= 0 then
			return self.GetAttacker()
		else
			return list[self.FightCount2]
		end
	end
end

function FightScene.FireSkill(attacker)
	local beattackers
	local skillInfo = attacker:GetFireSkill()
	local dic
	local list = {}
	tools.PrintDebug("队伍："..attacker.Team.." "..attacker.Name.."使用技能："..skillInfo.info.skill_id)
	if skillInfo.info.bloodType == Enum_BloodType.Treat then
		dic = attacker.Team == Enum_TeamType.Self and self.Team1 or self.Team2[self.Wave]
		for k,v in pairs(dic) do
			if v.HP > 0 then
				list[#list + 1] = v
			end
		end
		beattackers = self.GetTreatTarget(skillInfo.info.attack_range, list)
	else
		dic = attacker.Team == Enum_TeamType.Enemy and self.Team1 or self.Team2[self.Wave]
		for k,v in pairs(dic) do
			if v.HP > 0 then
				list[#list + 1] = v
			end
		end
		beattackers = self.GetHurtTarget(skillInfo.info.attack_range, list)
	end
	local data = {}
	data.HurtList = {}
	for i=1,skillInfo.info.hurt_count do
		for k,v in pairs(beattackers) do
			tools.PrintDebug("队伍："..attacker.Team.." "..attacker.Name.."使用技能："..skillInfo.info.skill_id.." 攻击了 "..v.Name)
			local hurtData = self.CalculateHurt(attacker, v, skillInfo)
			data.HurtList[#data.HurtList + 1] = hurtData
		end
	end
	data.Attacker = attacker
	data.SkillInfo = skillInfo
	data.Beattacker = beattackers
	self.FightRecordList[self.Wave][#self.FightRecordList[self.Wave] + 1] = data
	self.IsSelfTurn = not self.IsSelfTurn
	self.FightLogic()
end

function FightScene.CalculateHurt(attacker, beattacker, skillInfo)
	local hurtCount = skillInfo.info.hurt_count
	local attackerHit = attacker:GetHit() + 100 - beattacker:GetDodge()--命中
	local attackerCrit = attacker:GetCrit() - beattacker:GetUprising() --暴击
	local attackerCritDeep = attacker:GetCritDamageDeepen() - beattacker:GetCritDamageReduction() --暴击伤害加成
	local commonAttackerDee = attacker:GetDamageDeepen() - beattacker:GetDamageReduction()  --普通伤害加成
	local attackerParry = beattacker:GetParry() - attacker:GetPenetration() --格挡
	local blood = 0
	local fblood = 0---反弹血量
	local xblood = 0 --吸血
	local crit = 0---暴击
	local parry = 0---格挡
	local dodge = 0---闪避
	local curHit = math.random(100)
	local skillCoeff = (skillInfo.Level - 1) * skillInfo.info.skill_incremental_N + skillInfo.info.skill_coefficient_N
	skillCoeff = skillCoeff / 100
	if skillInfo.info.bloodType == Enum_BloodType.Treat then
		blood = (attacker:GetAttack() * skillCoeff) * (1 + commonAttackerDee * 0.01)
		if attackerCrit > curHit then
			blood = blood * 2
			crit = 1
		end
		blood = -blood
	elseif skillInfo.info.bloodType == Enum_BloodType.Hurt then
		if attackerHit >= curHit then
			blood = attacker:GetAttack() - beattacker:GetDefence()
			blood = blood * (1 + commonAttackerDee * 0.01)
			if blood <= 0 then
				blood = 1
			end
			if attackerCrit > curHit then
				blood = blood * (2 + attackerCritDeep * 0.01)
				crit = 1
			end
			blood = math.ceil(blood * skillCoeff)
			local extraSkillType = skillInfo.info.extra_skill_type
			if extra_skill_type == 1 then
				local extraSkillCoeff = (skillInfo.info.extra_skill_coefficient_N + 
					(skillInfo.Level - 1) * skillInfo.info.extra_skill_incremental_N) / 100
				blood = blood + attacker.MaxHP * extraSkillCoeff
			end
			if attackerParry > curHit then
				blood = math.floor(blood / 2)
				parry = 1
			end
		else
			dodge = 1
		end
	end
	local sumBlood = math.ceil(blood / hurtCount)
	local singleHurt = math.ceil(blood / hurtCount)
	local reflex = beattacker:GetDamageReflex()
	if skillInfo.info.bloodType == Enum_BloodType.Hurt then
		if reflex ~= 0 then
			fblood = math.ceil(singleHurt * reflex * 0.01)
			if fblood < 1 then
				fblood = 1
			end
		end
		local suckBlood = attacker:GetSuckBlood() 
		if suckBlood ~= 0 then
			xblood = math.ceil(singleHurt * suckBlood * 0.01)
			if xblood < 1 then
				xblood = 1
			end
		end
	end
	local data = {}
	
	if xblood > 0 or fblood > 0 then
		attacker:BeAttack({Dodge = dodge, Crit = crit, Parry = parry, Suck = xblood, Reflex = fblood, Hurt = 0})
	end
	beattacker:BeAttack({Dodge = dodge, Crit = crit, Parry = parry, Hurt = singleHurt, Suck = 0, Reflex = 0})
	return {Dodge = dodge, Crit = crit, Parry = parry, Hurt = singleHurt, Suck = xblood, Reflex = fblood, Beattacker = beattacker, 
		SkillInfo = skillInfo}
end

function FightScene.GetTreatTarget(range, dic)
	local beattacker
	if range == Enum_AttackRange.Treat_MinBlood then
		beattacker = {}
		local percent = 1
		for k,v in pairs(dic) do
			if v.HP / v.MaxHP < percent then
				beattacker[1] = v
			end
		end
	elseif range == Enum_AttackRange.Treat_All then
		beattacker = dic
	elseif range == Enum_AttackRange.Treat_Front or range == Enum_AttackRange.Treat_FrontTwo then
		local list = self.GetTargetBetween(1, 2, dic)
		if #list == 0 then
			list = self.GetTargetBetween(3, 3, dic)
			if #list == 0 then
				list = self.GetTargetBetween(4, 5, dic)
			end
		end
		beattacker = list
	elseif range == Enum_AttackRange.Treat_RandomTwoMinBlood then
		beattacker = {}
		local list = {}
		for k,v in pairs(dic) do
			list[#list + 1] = v
		end
		table.sort(list, function(a, b)
			return (a.HP / a.MaxHP) < (b.HP / b.MaxHP)
		end)
		for i=1,2 do
			if list[i] then
				beattacker[i] = list[i]
			end
		end
	end
	return beattacker
end

function FightScene.GetTargetBetween(begin, endIndex, dic)
	local list = {}
	for k,v in pairs(dic) do
		if v.Place >= begin and v.Place <= endIndex then
			list[#list + 1] = v
		end
	end
	return list
end

function FightScene.GetTargetLine(list)
	local result = {}
	for k,v in pairs(list) do
		if v.Place == 1 or v.Place == 4 then
			result[#result + 1] = v
		end
	end
	if #result > 0 then
		return result
	end
	for k,v in pairs(list) do
		if v.Place == 3 then
			result[#result + 1] = v
		end
	end
	if #result > 0 then
		return result
	end
	for k,v in pairs(list) do
		if v.Place == 2 or v.Place == 5 then
			result[#result + 1] = v
		end
	end
	return result
end

function FightScene.GetHurtTarget(range, dic)
	local beattacker
	if range == Enum_AttackRange.Hurt_Front_All then
		beattacker = self.GetTargetBetween(1, 2, dic)
		if #beattacker == 0 then
			beattacker = self.GetTargetBetween(3, 3, dic)
			if #beattacker == 0 then
				beattacker = self.GetTargetBetween(4, 5, dic)
			end
		end
	elseif range == Enum_AttackRange.Hurt_Back_All then
		beattacker = self.GetTargetBetween(4, 5, dic)
		if #beattacker == 0 then
			beattacker = self.GetTargetBetween(3, 3, dic)
			if #beattacker == 0 then
				beattacker = self.GetTargetBetween(1, 2, dic)
			end
		end
	elseif range == Enum_AttackRange.Hurt_Front_One then
		for i=1,5 do
			beattacker = self.GetTargetBetween(i, i, dic)
			if #beattacker == 1 then
				break
			end
		end
	elseif range == Enum_AttackRange.Hurt_Back_One then
		for i=5,1,-1 do
			beattacker = self.GetTargetBetween(i, i, dic)
			if #beattacker == 1 then
				break
			end
		end
	elseif range == Enum_AttackRange.Hurt_Random_One or range == Enum_AttackRange.Hurt_Random_Two or 
		range == Enum_AttackRange.Hurt_Random_Three then
		beattacker = {}
		local count = 1
		if range == Enum_AttackRange.Hurt_Random_Two then
			count = 2
		elseif range == Enum_AttackRange.Hurt_Random_Three then
			count = 3
		end
		if count >= #dic then
			beattacker = dic
		else
			local list = {}
			for k,v in pairs(dic) do
				list[#list + 1] = k
			end
			count = #list > count and count or #list
			for i=1,count do
				local random = math.random(#list)
				local index = list[random]
				beattacker[i] = dic[index]
				table.remove(list, random)
			end
		end
	elseif range == Enum_AttackRange.Hurt_All then
		beattacker = dic
	elseif range == Enum_AttackRange.Hurt_Line then
		beattacker = self.GetTargetLine()
	elseif range == Enum_AttackRange.Hurt_Random_OneFirstCenter then
		beattacker = {}
		for k,v in pairs(dic) do
			if v.Place == 3 then
				beattacker[1] = v
			end
		end
		if #beattacker == 0 then
			beattacker[1] = dic[math.random(#dic)]
		end
	elseif range == Enum_AttackRange.Hurt__MinBlood then
		beattacker = {}
		local percent = 1
		for k,v in pairs(dic) do
			if v.HP / v.MaxHP < percent then
				beattacker[1] = v
			end
		end
	end
	return beattacker
end

function FightScene.Destroy()
	CtrlManager.SwithSceneClearCtrl()
end

return FightScene