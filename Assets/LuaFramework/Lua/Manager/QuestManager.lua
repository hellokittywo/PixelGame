QuestManager = {}
local self = QuestManager
local QuestInfo = require("Data.QuestInfo")

function QuestManager.Init()
	self.QuestDic = {}
	self.CurrentTime = os.date("%Y-%m-%d %H:%M:%S")
	local quests = PlayerManager.GetPlayerData("QuestDic")
	self.PlayerQuest = {}
	if quests then
		self.PlayerQuest.TransferQuest = quests.TransferQuest
		self.PlayerQuest.QuestRefreshTime = quests.QuestRefreshTime
		self.PlayerQuest.QuestDic = {}
		for k,v in pairs(quests.QuestDic) do
			self.PlayerQuest.QuestDic[v.ID] = v
		end
	end
	if self.PlayerQuest.QuestRefreshTime then
		self.QuestRefreshTime = self.PlayerQuest.QuestRefreshTime
		self.InitBountyQuest()
	else
		self.RefereshBountyQuest()
	end
	local time1 = tools.GetTimeList(PlayerManager.PlayerInfo.NowTime)
	local time2 = tools.GetTimeList(self.CurrentTime)
	if time1.year ~= time2.year or time1.month ~= time2.month or time1.day ~= time2.day then
		self.CompelteBountyCount = 0
	else
		self.RefreshCompelteBountyCount()
	end
	self.InitAchievementQuest()
	self.InitTransferQuest()
	PlayerManager.SaveQuestData()
end

function QuestManager.RefreshCompelteBountyCount()
	self.CompelteBountyCount = 0
	for k,v in pairs(self.GetBountyQuestList()) do
		if v.State == Enum_QuestState.Reward then
			self.CompelteBountyCount = self.CompelteBountyCount + 1
		end
	end
end

function QuestManager.InitTransferQuest()
	for k,v in pairs(tables.QuestTypeDic[Enum_QuestType.Transfer]) do
		local info = QuestInfo.new(v.ID, self.PlayerQuest.TransferQuest)
		self.TransferQuest = info
		return
	end
end

function QuestManager.InitAchievementQuest()
	for k,v in pairs(Enum_QuestType) do
		if v >= Enum_QuestType.Achievement_LevelUp then
			for m,n in pairs(tables.QuestTypeDic[v]) do
				local curInfo
				if self.PlayerQuest.QuestDic and self.PlayerQuest.QuestDic[n.ID] then
					curInfo = self.PlayerQuest.QuestDic[n.ID]
				end
				local info = QuestInfo.new(n.ID, curInfo)
				self.QuestDic[n.ID] = info
			end
		end
	end
end

function QuestManager.GetBountyQuestList()
	local list = {}
	for k,v in pairs(self.QuestDic) do
		if v.Type <= Enum_QuestType.KillBoss then
			list[#list + 1] = v
		end
	end
	return list
end

function QuestManager.InitBountyQuest()
	if self.PlayerQuest.QuestDic then
		for k,v in pairs(self.PlayerQuest.QuestDic) do
			if tables.QuestDic[v.ID].Type <= Enum_QuestType.KillBoss then
				self.QuestDic[v.ID] = QuestInfo.new(v.ID, v)
			end
		end
	else
		self:RefereshBountyQuest()
	end
end

function QuestManager.RefereshBountyQuest()
	local list = QuestManager.GetBountyQuestList()
	for k,v in pairs(list) do
		self.QuestDic[v.ID] = nil
	end
	self.QuestRefreshTime = os.date("%Y-%m-%d %H:%M:%S")
	local firstT
	local firstQ
	for i=1,ConfigManager.RandomQuestNum do
		local random = math.random(100)
		local list = tools.GetRandomRateList(ConfigManager.RandomQuestTypeRate)
		local type
		for j=1,#list do
			if random <= list[j].Value * 100 then
				type = list[j].Type
				break
			end
		end
		local quality
		list = tools.GetRandomRateList(ConfigManager.RandomQuestQualityRate)
		if type == firstT then
			for i=1,#list do
				if list[i].Type == firstQ then
					table.remove(list, i)
					break
				end
			end
			quality = list[math.random(#list)].Type
		else
			for j=1,#list do
				if random <= list[j].Value * 100 then
					quality = list[j].Type
					break
				end
			end
		end
		firstT = type
		firstQ = quality
		local id = tables.QuestTypeDic[type][quality].ID
		self.QuestDic[id] = QuestInfo.new(id)
	end
	PlayerManager.SaveQuestData()
end

function QuestManager.GetAcceptedList(type)
	local list = {}
	for k,v in pairs(self.QuestDic) do
		if v.Type == type and (v.State == Enum_QuestState.Get or v.State == Enum_QuestState.Complete) then
			list[#list + 1] = v
		end
	end
	return list
end

function QuestManager.CheckQuest(type, param)
	local list = {}
	if type == Enum_CheckQuest.PassPve then
		self.CheckPassPveQuest()
	elseif type == Enum_CheckQuest.KillMonster then
		self.CheckKillMonsterQuest(param)
	elseif type == Enum_CheckQuest.GetEquip then
		list = self.GetAcceptedList(Enum_QuestType.GetEquip)
		for i=1,#list do
			local condtion = list[i].Condition
			if param.Layer >= condtion.MinLayer and param.Layer <= condtion.MaxLayer and param.Quality >= condtion.Quality then
				list[i]:SetProgress(list[i].Progress + 1)
				-- if list[i].Progress >= condtion.Value then
				-- 	list[i]:UpdateState(Enum_QuestState.Complete)
				-- end
			end
		end
	elseif type == Enum_CheckQuest.LevelUp then
		list = self.GetAcceptedList(Enum_QuestType.Achievement_LevelUp)
		for i=1,#list do
			list[i]:CheckState()
		end
	elseif type == Enum_CheckQuest.EquipStrength then
		list = self.GetAcceptedList(Enum_QuestType.Achievement_EquipStrength)
		for i=1,#list do
			list[i]:CheckState()
		end
	elseif type == Enum_CheckQuest.GetMercenary then
		list = self.GetAcceptedList(Enum_QuestType.Achievement_GetMercenary)
	end
	EventsManager.DispatchEvent(EventsManager.EventsName.Event_UpdateQuestFightScene)
	PlayerManager.SaveQuestData()
end

function QuestManager.CheckPassPveQuest()
	local list = self.GetAcceptedList(Enum_QuestType.PassPve)
	for i=1,#list do
		list[i]:CheckState()
	end
	list = self.GetAcceptedList(Enum_QuestType.Achievement_PassPve)
	for i=1,#list do
		list[i]:CheckState()
	end
end

function QuestManager.CheckKillMonsterQuest(param)
	local list = {}
	if param.RoleInfo.Type == Enum_EnemyType.Common then
		list = self.GetAcceptedList(Enum_QuestType.KillMonster)
	elseif param.RoleInfo.Type == Enum_EnemyType.Elite then
		list = self.GetAcceptedList(Enum_QuestType.KillEliteMonster)
	elseif param.RoleInfo.Type == Enum_EnemyType.Boss then
		list = self.GetAcceptedList(Enum_QuestType.KillBoss)
	elseif param.RoleInfo.Type == Enum_EnemyType.CavityBoss then
		list = self.GetAcceptedList(Enum_QuestType.KillCavityBoss)
	end
	if param.RoleInfo.MonsterID == self.TransferQuest.Param and self.TransferQuest.State == Enum_QuestState.Get then
		self.TransferQuest:SetProgress(self.TransferQuest.Progress + 1)
	end
	for i=1,#list do
		local condtion = list[i].Condition
		if param.RoleInfo.Type == Enum_EnemyType.Boss then
			if (param.Layer - 0.5) == condtion.Layer then
				list[i]:SetProgress(list[i].Progress + 1)
			end
		else
			if param.Layer >= condtion.MinLayer and param.Layer <= condtion.MaxLayer then
				list[i]:SetProgress(list[i].Progress + 1)
			end
		end
	end
	if param.RoleInfo.Type == Enum_EnemyType.Common or param.RoleInfo.Type == Enum_EnemyType.Elite then
		if param.RoleInfo.Type == Enum_EnemyType.Common then
			list = self.GetAcceptedList(Enum_QuestType.Achievement_KillMonster)
		else
			list = self.GetAcceptedList(Enum_QuestType.Achievement_KillEliteMonster)
		end
		for i=1,#list do
			local condtion = list[i].Condition
			list[i]:SetProgress(list[i].Progress + 1)
		end
	elseif param.RoleInfo.Type == Enum_EnemyType.Boss or param.RoleInfo.Type == Enum_EnemyType.CavityBoss then
		if param.RoleInfo.Type == Enum_EnemyType.Boss then
			list = self.GetAcceptedList(Enum_QuestType.Achievement_KillBoss)
		else
			list = self.GetAcceptedList(Enum_QuestType.Achievement_KillCavityBoss)
		end
		for i=1,#list do
			local condtion = list[i].Condition
			if param.RoleInfo.MonsterID == condtion.Value then
				list[i]:SetProgress(list[i].Progress + 1)
			end
		end
	end
end