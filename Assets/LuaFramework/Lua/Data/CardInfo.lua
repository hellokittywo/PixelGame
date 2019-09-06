local CardInfo = class("CardInfo")
local SkillInfo = require("Data.SkillInfo")
local CardAiAction = require("Scene.Fight.CardAiAction")

function CardInfo:ctor(team, id, cardData)
	self.Team = team
	self.ID = id
	-- if team == Enum_TeamType.Enemy then
	-- 	local info = tables.NpcDic[id]
	-- 	self:InitMonsterProperty(info, cardData)
	-- else
	-- 	local info = tables.CardDic[id]
	-- 	self:InitMyCardProperty(info, cardData)
	-- end
	self.PropertyDic = {}
	local info = tables.CardDic[id]
	local list = tools.Split(info.BaceAttr, "|")
	for i,v in ipairs(list) do
		local p = tools.Split(v, ",")
		self.PropertyDic[tonumber(p[1])] = tonumber(p[2])
	end
	self.Name = info.Name
	self.AttackID = info.AttackId
	self.Pos = {x = 0, y = 0}
	if team == Enum_TeamType.Enemy then
		self:SetPlace(cardData)
	end
	self.HP = self.PropertyDic[3]
	self.MaxHP = self.HP
	self.Attack = self.PropertyDic[1]
	self.Defence = self.PropertyDic[2]
	self.CardAiAction = CardAiAction.new(self)
end

function CardInfo:SetPlace(place)
	self.Place = place
	local pos, x, y = FightScene.GetPos(place)
	self.Pos = pos
	self.PosX = x
	self.PosY = y
end

function CardInfo:InitMonsterProperty(info, cardData)
	self.Attack = info.attack
	self.Defence = info.defense
	self.HP = info.hp
	self.MaxHP = self.HP
	self.Name = info.cardName
	self.Quality = info.cardQuality
	self.AttackID = info.attackSkillID--普攻
	self.BaseSkillID = info.baseSkillID--基础技能
	self.TalentSkillID = info.talentSkillID--天赋技能
	self.MagicWeapon1 = info.Magic_weapon1--法宝1
	self.MagicWeapon2 = info.Magic_weapon2--法宝2
	self.Head = info.fightheadUrl
	self.Icon = info.fightcardUrl
	self.SkillMusic = info.skill_music
	self.DeadMusci = info.dead_music
	self.Place = cardData.Place or 0

	self.Hit = 0
	self.Dodge = 0
	self.Crit = 0
	self.Uprising = 0
	self.CritDamageDeepen = 0
	self.CritDamageReduction = 0
	self.DamageDeepen = 0
	self.DamageReduction = 0
	self.Parry = 0
	self.Penetration = 0
	self.DamageReflex = 0
	self.SuckBlook = 0
end

function CardInfo:InitMyCardProperty(info, cardData)
	self.PredestinedActiveDic = {}
	self.BaseAttack = info.attack
	self.BaseDefence = info.defense
	self.BaseHP = info.hp
	self.HpLevelProperty = info.hpQualifications
	self.AttackLevelProperty = info.attackQualifications
	self.DefenceLevelProperty = info.defenseQualifications
	self.Level = cardData.Level or 1
	self.BreakLevel = CardInfo.BreakLevel or 0
	self.Place = cardData.Place or 0
	self.SkillDic = cardData.SkillDic or {}

	self.Name = info.cardName
	self.Quality = info.cardQuality
	self.AttackID = info.attackSkillID--普攻
	self.BaseSkillID = info.baseSkillID--基础技能
	self.BaseSkillLevel = cardData.BaseSkillLevel or 1
	self.TalentSkillID = info.talentSkillID--天赋技能

	self.MagicWeaponID = cardData.MagicWeapon1 or 0--法宝
	self.Head = info.headUrl
	self.Icon = info.fightcardUrl
	self.Location = info.location
	
	self.SkillMusic = info.skill_music
	self.DeadMusci = info.dead_music

	self.Hit = 0
	self.Dodge = 0
	self.Crit = 0
	self.Uprising = 0
	self.CritDamageDeepen = 0
	self.CritDamageReduction = 0
	self.DamageDeepen = 0
	self.DamageReduction = 0
	self.Parry = 0
	self.Penetration = 0
	self.DamageReflex = 0
	self.SuckBlook = 0

	self.Attack = self.BaseAttack + (self.Level - 1) * self.AttackLevelProperty
	self.Defence = self.BaseDefence + (self.Level - 1) * self.DefenceLevelProperty
	self.HP = self.BaseHP + (self.Level - 1) * self.HpLevelProperty
	self.MaxHP = self.HP

	self.PredestinedList = {info.predestinedID1, info.predestinedID2, info.predestinedID3, info.predestinedID4, info.predestinedID5, 
		info.predestinedID6}

	-- self:UpdateProperty()
end

function CardInfo:CheckCardPredestined()
	self.PredestinedActiveDic = {}
	local dic = CardManager.GetFightCardDic()
	for k,v in pairs(self.PredestinedList) do
		if v ~= 0 then
			local pInfo = tables.CardPredestinedDic[v]
			

			local active = true
			for m,n in pairs(list) do
				if not dic[tonumber(n)] then
					active = false
					break
				end
			end
			if active then
				self:ActiveCardPredestined(pInfo)
			end
		end
	end
	self:UpdateProperty()
end

function CardInfo:ActiveCardPredestined(info)
	self.PredestinedActiveDic[info.predestinedID] = 1
	local list = {{Type = info.property1, Value = info.property2}, {Type = info.property3, Value = info.property4}}
	for k,v in pairs(list) do
		if v.Type > 0 then
			if info.Value == 1 then--hp
				self.HP = self.HP * (1 + info.Value / 10000)
				self.MaxHP = self.HP
			elseif info.Value == 2 then--attack
				self.HP = self.HP * (1 + info.Value / 10000)
			elseif info.Value == 3 then--defence
				self.HP = self.HP * (1 + info.Value / 10000)
			end
		end
	end
end

function CardInfo:UpdateProperty()
	local addPowerRate = 0
	local addTalentSkillRate = 0
	---战斗力=（攻+防+0.1*血)*（1+(天赋技能系数+法宝技能系数+暴击率*(1+暴击伤害加深%+暴击伤害减免%)+闪避%+抗暴%+格挡%/2+破挡%/2+吸血%+反伤%+封神主动技能+伤害加深%+伤害减免%)/2）
  	self.Power = math.floor((self:GetAttack() + self:GetDefence() + 0.1 * self:GetMaxHP()) * (1 + (addPowerRate + self:GetCrit() * 
  		0.01 * (1 + self:GetCritDamageDeepen() * 0.01 + self:GetCritDamageReduction() * 0.01 ) + self:GetDodge() * 0.01 + 
  		self:GetUprising() * 0.01 + self:GetParry() * 0.01 / 2 + self:GetPenetration() * 0.01 / 2 + self:GetSuckBlood() * 0.01 + 
  		self:GetDamageReflex() * 0.01  + addTalentSkillRate + self:GetDamageDeepen() * 0.01  + self:GetDamageReduction() * 0.01) / 2))
end

function CardInfo:GetFireSkill()
	local skillID = 0
	local baseid = self.BaseSkillID
	local magicid = 0
	if self.Team == Enum_TeamType.Self then
		local fabao = BagManager.GetEquip(self.Place, 8)
		if fabao then
			magicid = fabao.ID
		end
	else
		magicid = self.MagicWeaponID
	end
	local attackid = self.AttackID
	local baseSkill = tables.SkillDic[baseid]
	local magicSkill = tables.SkillDic[magicid]
	if not baseSkill and not magicSkill then
		skillID = attackid
	elseif baseSkill and not magicSkill then
		local random = math.random(100)
		random = 1
		if random <= baseSkill.trigger_probability then
			skillID = baseid
		else
			skillID = attackid
		end
	elseif magicSkill and not baseSkill then
		local random = math.random(100)
		if random <= magicSkill.trigger_probability then
			skillID = magicid
		else
			skillID = attackid
		end
	elseif magicSkill and baseSkill then
		local rate1 = baseSkill.trigger_probability / 2
		local rate2 = magicSkill.trigger_probability / 2
		local random = math.random(100)
		if random <= rate1 then
			skillID = baseid
		elseif random <= rate1 + rate2 then
			skillID = magicid
		else
			skillID = attackid
		end
	end
	local level = 1
	if self.Team == Enum_TeamType.Self and self.SkillDic[skillID] then
		level = self.SkillDic[skillID]
	end
	local info = SkillInfo.new(skillID, level)
	return info
end

function CardInfo:AttackAction(beattackers)
	EventsManager.DispatchEvent(EventsManager.EventsName.Event_CardAttack, {info = self, beattacker = beattackers[1]})
end

function CardInfo:BeAttack(hurt)
	self.HP = self.HP - hurt
	if self.HP < 0 then
		self.HP = 0
		FightScene.RefreshObstruct()
	end
	-- if self.HP <= 0 then
	-- 	EventsManager.DispatchEvent(EventsManager.EventsName.Event_CardDead, self)
	-- else
		EventsManager.DispatchEvent(EventsManager.EventsName.Event_CardHurt, {info = self, hurt = hurt})
	-- end
end

function CardInfo:GetName()
	return self.Name.. "  +"..self.BreakLevel
end

function CardInfo:GetMaxHP()
	return self.MaxHP
end

function CardInfo:GetAttack()
	return self.Attack
end

function CardInfo:GetDefence()
	return self.Defence
end

--命中
function CardInfo:GetHit()
	return self.Hit
end
--闪避
function CardInfo:GetDodge()
	return self.Dodge
end

--暴击
function CardInfo:GetCrit()
	return self.Crit
end
--抗暴
function CardInfo:GetUprising()
	return self.Uprising
end

--暴击伤害加深
function CardInfo:GetCritDamageDeepen()
	return self.CritDamageDeepen
end
--暴击伤害减免
function CardInfo:GetCritDamageReduction()
	return self.CritDamageReduction
end

--伤害加深
function CardInfo:GetDamageDeepen()
	return self.DamageDeepen
end
--伤害减免
function CardInfo:GetDamageReduction()
	return self.DamageReduction
end

--格挡
function CardInfo:GetParry()
	return self.Parry
end
--破档
function CardInfo:GetPenetration()
	return self.Penetration
end

--伤害反弹
function CardInfo:GetDamageReflex()
	return self.DamageReflex
end

function CardInfo:GetSuckBlood()
	return self.SuckBlook
end

function CardInfo:GetCardIcon()
	return "Texture/Card/"..tools.GetRealUrl(self.Icon)
end

function CardInfo:GetHeadIcon()
	return tools.GetRealUrl(self.Head)
end

return CardInfo