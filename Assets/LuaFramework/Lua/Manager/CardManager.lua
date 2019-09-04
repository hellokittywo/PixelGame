CardManager = {}
local self = CardManager
local CardInfo = require("Data.CardInfo")

function CardManager.InitData(InitData)
	self.CardDic = {}
	local list = {1, 2, 3, 4, 5, 6}
	for k,v in pairs(list) do
		local info = CardInfo.new(Enum_TeamType.Self, v)
		self.CardDic[info.ID] = info 
	end
	
	-- self.CheckCardPredestined()
	self.SortList()
end

function CardManager.CheckCardPredestined()
	for k,v in pairs(self.CardDic) do
		if v.Place > 0 then
			v:CheckCardPredestined()
		end
	end
end

function CardManager.GetFightCardDic()
	local dic = {}
	for k,v in pairs(self.CardDic) do
		if v.Place > 0 then
			dic[v.ID] = v
		end
	end
	return dic
end

function CardManager.GetNoFightCardList()
	local list = {}
	for k,v in pairs(self.CardDic) do
		if v.Place == 0 then
			list[#list + 1] = v
		end
	end
	table.sort(list, function(a, b)
		if a.Quality == b.Quality then
			if a.Level == b.Level then
				return a.BreakLevel > b.BreakLevel
			else
				return a.Level > b.Level
			end
		else
			return a.Quality > b.Quality
		end
	end)
	return list
end

function CardManager.SortList()
	self.CardList = {}
	for k,v in pairs(self.CardDic) do
		self.CardList[#self.CardList + 1] = v
	end
	table.sort(self.CardList, function(a, b)
		return a.ID < b.ID
	end)
end

function CardManager.ChangeCardPlace(id, place, id2, place2)
	local card1 = self.CardDic[id]
	if card1 then
		card1.Place = place
	end
	card2 = self.CardDic[id2]
	if card2 then
		card2.Place = place2
	end
	self.CheckCardPredestined()
	EventsManager.DispatchEvent(EventsManager.EventsName.Event_ChangeCardPlace)
end