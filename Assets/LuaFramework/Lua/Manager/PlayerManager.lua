PlayerManager = {}
local self = PlayerManager
local PlayerInfo = require("Data.PlayerInfo")

function PlayerManager.Init(registerData)
	self.PlayerInfo = PlayerInfo.new()
	local savedData = self.GetPlayerData("PlayerInfo")
	if savedData and not registerData then
		local idata = self.GetPlayerData("ItemDic1")
		if idata then
			savedData.MaxItemID = idata.MaxItemID
			savedData.ItemDic = idata.ItemDic
			idata = self.GetPlayerData("ItemDic2")
			for k,v in pairs(idata.ItemDic) do
				table.insert(savedData.ItemDic, v)
			end
		end
		savedData.SkillDic = self.GetPlayerData("SkillDic")
		self.PlayerInfo:InitData(savedData)
		self.PlayerInfo.RoleInfo:UpdateProperty()
	else
		self.InitNewPlayer(registerData)
	end
end

function PlayerManager.InitNewPlayer(registerData)
	local data = {}
	if registerData then
		data.Sex = registerData.Sex
		data.HairIndex = registerData.HairIndex
		data.FaceIndex = registerData.FaceIndex
	else
		data.Sex = Enum_SexType.Man
	end
	data.NickName = "Player"
	data.Level = 1
	self.PlayerInfo:InitData(data)
end

function PlayerManager.SaveAllData()
	self.SavePlayerData()
	self.SaveSkillData()
	self.SaveItemData()
end

function PlayerManager.SavePlayerData()
	if not GameStart.SaveData then
		return
	end
	local data = {}
	-- data.Account = GameStart.Account or "test"
	data.Sex = self.PlayerInfo.Sex
	data.HairIndex = self.PlayerInfo.HairIndex
	data.FaceIndex = self.PlayerInfo.FaceIndex
	data.NickName = self.PlayerInfo.NickName
	data.Level = self.PlayerInfo.Level
	data.Coin = self.PlayerInfo.Coin
	data.Exp = self.PlayerInfo.Exp
	data.Diamond = self.PlayerInfo.Diamond
	data.Potential = self.PlayerInfo.Potential
	data.MaxPveTowerLayer = self.PlayerInfo.MaxPveTowerLayer
	data.MaxPveID = self.PlayerInfo.MaxPveID
	local dic = {}
	for k,v in pairs(self.PlayerInfo.CodeDic) do
		dic[k] = {Group = v.Group, Count = v.Count}
	end
	data.CodeDic = dic
	local info = self.PlayerInfo.RoleInfo
	local rInfo = {}
	rInfo.ProfessionID = info.ProfessionID
	rInfo.Anger = info.Anger
	rInfo.RoleID = info.RoleID
	data.RoleInfo = rInfo
	data.PotentialDic = info.PotentialDic
	data.NowTime = os.date("%Y-%m-%d %H:%M:%S")
	data.GiftID = self.PlayerInfo.GiftID or 0
	data.OpenBagIndex = BagManager.OpenBagIndex
	data.AutoSelffQuality = self.PlayerInfo.AutoSelffQuality
	data.AutoRevive = self.PlayerInfo.AutoRevive
	local ss = "return "..table.tostring(data)
	local str = LuaToCSFunction.Encrypt(ss, "14832354452235455178213214568965")
	LuaToCSFunction.PlayerPrefsSave("PlayerInfo", str)
end

function PlayerManager.SaveSkillData()
	if not GameStart.SaveData then
		return
	end
	local skillDic = {}
	for k,v in pairs(self.PlayerInfo.RoleInfo.SkillDic) do
		skillDic[k] = v.Level
	end
	local ss = "return "..table.tostring(skillDic)
	local str = LuaToCSFunction.Encrypt(ss, "14832354452235455178213214568965")
	LuaToCSFunction.PlayerPrefsSave("SkillDic", str)
end

function PlayerManager.SaveItemData()
	if not GameStart.SaveData then
		return
	end
	local data = {}
	local dicList = {[1] = {}, [2] = {}}
	local count = 0
	local index = 1
	for k,v in pairs(BagManager.ItemDic) do
		local info = {Count = v.Count, ItemID = v.ItemID, Level = v.Level, BreakLevel = v.BreakLevel, 
			Exp = v.Exp, ID = v.ID, RoleID = v.RoleID, ExtraEnchantInfo = v.ExtraEnchantInfo}
		info.Enchants = ""
		for i=1,#v.EnchantList do
			local temp = v.EnchantList[i]
			info.Enchants = info.Enchants..temp.Type..","..temp.Level..","..temp.Value.."|"
		end
		if v.StoneEnchantList then
			info.StoneEnchantList = {}
			for i=1,#v.StoneEnchantList do
				local temp = v.StoneEnchantList[i].PInfo
				info.StoneEnchantList[i] = {Type = temp.Type, Value = temp.Value, Level = temp.Level, MinV = temp.MinV, MaxV = temp.MaxV}
			end
		end
		if count > BagManager.BagItemCount * 3 then
			index = 2
		end
		dicList[index][v.ItemID] = info
		count = count + 1
	end
	for k,v in pairs(BagManager.HpItemDic) do
		dicList[1][v.ItemID] = {Count = v.Count, ItemID = v.ItemID, Level = v.Level, BreakLevel = v.BreakLevel, Enchants = "", 
			Exp = v.Exp, ID = v.ID, RoleID = v.RoleID}
	end
	data.MaxItemID = self.PlayerInfo.MaxItemID
	data.ItemDic = dicList[1]
	local ss = "return "..table.tostring(data)--大概最多只能保存1.6W长度的字符串，这里拆分背包保存
	local str = LuaToCSFunction.Encrypt(ss, "14832354452235455178213214568965")
	LuaToCSFunction.PlayerPrefsSave("ItemDic1", str)
	ss = "return "..table.tostring({ItemDic = dicList[2]})
	str = LuaToCSFunction.Encrypt(ss, "14832354452235455178213214568965")
	LuaToCSFunction.PlayerPrefsSave("ItemDic2", str)
end

function PlayerManager.SaveQuestData()
	if not GameStart.SaveData then
		return
	end
	local data = {}
	if QuestManager.TransferQuest then
		local v = QuestManager.TransferQuest
		data.TransferQuest = {ID = v.ID, State = v.State, Progress = v.Progress, Reward = v.Reward, Condition = v.Condition}
	end
	local dic = {}
	for k,v in pairs(QuestManager.QuestDic) do
		dic[v.ID] = {ID = v.ID, State = v.State, Progress = v.Progress, Reward = v.Reward, Condition = v.Condition}
	end
	data.QuestDic = dic
	data.QuestRefreshTime = QuestManager.QuestRefreshTime
	local ss = "return "..table.tostring(data)
	local str = LuaToCSFunction.Encrypt(ss, "14832354452235455178213214568965")
	LuaToCSFunction.PlayerPrefsSave("QuestDic", str)
end

function PlayerManager.GetPlayerData(name)
	local ss = LuaToCSFunction.PlayerPrefsGet(name)
	if ss == "" then
		return
	end
	local str = LuaToCSFunction.Decrypt(ss, "14832354452235455178213214568965")
	local info = loadstring(str)()
	return info
end