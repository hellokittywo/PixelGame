local tools = {}
tools.Debug = true;
tools.ReloadModule = false--每次打开界面都重新加载界面
tools.UILabel = "UILabel"
tools.UIButton = "UIButton"
tools.UISprite = "UISprite"
tools.Camera = "Camera"
tools.UIScrollView = "UIScrollView"
tools.UIWrapContent = "UIWrapContent"
tools.UIPanel = "UIPanel"
tools.UIGrid = "UIGrid"
tools.UISlider = "UISlider"
tools.UIToggle = "UIToggle"
tools.UIInput = "UIInput"
tools.UI2DSprite = "UI2DSprite"
tools.Animator = "Animator"
tools.UITable = "UITable"
tools.UIPlayTween = "UIPlayTween"
tools.TweenColor = "TweenColor"
tools.TweenScale = "TweenScale"
tools.UITexture = "UITexture"
tools.TweenRotation = "TweenRotation"
tools.Collider = "Collider"
tools.BoxCollider = "BoxCollider"
tools.UIWidget = "UIWidget"

tools.TransUrl = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=trnsl.1.1.20170808T073725Z.57add0eb2b1bc0a9.37c28541b6e9d618faf726929215dd4a5633a3da"
tools.BuyFlag = false 	--购买界面弹出的标记
tools.IsIphoneX = LuaToCSFunction.IsIphoneX()

local numberChangeAnims = {}  -- 数字变化的动画
local isAddUpdateBeat = false

function tools.ScreenToWorldPoint(pos)
	return AppConst.UICamera:ScreenToWorldPoint(AppConst.GameCamera:WorldToScreenPoint(Vector3(pos.x, pos.y, pos.z)));
end

function tools.GetComponent(obj, script)
	local sc = obj:GetComponent(script)
	if sc == nil then
		LuaToCSFunction.AddComponent(obj, script)
		sc = obj:GetComponent(script)
	end

	return sc
end

function tools.PrintDebug(...)
	if tools.Debug then
		print(...);
	end
end

function tools.PrintSelfTeamLog(info, ...)
	if tools.Debug then
		if info.Team == Enum_TeamType.Self then
			print(...)
		end
	end
end

function tools.SetAnimation(state, trackIndex, animationName, loop)
	state:ClearTracks()
	state:SetAnimation(trackIndex, animationName, loop)
end

function tools.FindChild(transform, path, type)
	local component = 'UILabel'
	if type ~= nil then
		component = type
	end
	local obj = transform:Find(path):GetComponent(component)

	-- if component == 'UILabel' then
	-- 	tools.SetCurrentFont(obj)
	-- end

	return obj
end

function tools.FindChildObj(transform, path)
	return transform:Find(path).gameObject
end

function tools.Floor(num)
	local num = math.floor(num)
	if num == 0 then
		num = 1
	end
	return num
end

-- local FlyTips = require("UI.Common.FlyTips")
-- local flyTips
-- function tools.FlyTips(str, pos, time)
-- 	time = time or 0.6
-- 	pos = pos or {x = 0, y = 0}
-- 	if not flyTips then
-- 		flyTips = FlyTips.new(Global_UIRoot.transform)
-- 	end
-- 	flyTips:AddTips(str, pos, time)
-- end

-- function tools.FlyTipsList(list, pos, time)
-- 	if not list or #list == 0 then
-- 		return
-- 	end
-- 	if not flyTips then
-- 		flyTips = FlyTips.new(Global_UIRoot.transform)
-- 	end
-- 	if #list == 1 then
-- 		flyTips:AddTips(list[1], pos, time)
-- 		return
-- 	end
-- 	flyTips:FlyTipsList(list, pos, time)
-- end

-- local SkillTips = require("UI.Skill.SkillTips")
-- local skillTips
-- function tools.ShowSkillTips(parent, info)
-- 	if not skillTips then
-- 		skillTips = SkillTips.new(parent, info)
-- 		skillTips:UpdateView(parent, info)
-- 	else
-- 		skillTips.gameObject:SetActive(true)
-- 		skillTips:UpdateView(parent, info)
-- 	end
-- end

-- local AngerTips = require("UI.Common.AngerTips")
-- local angerTips
-- function tools.ShowAngerTips(info)
-- 	if not angerTips then
-- 		angerTips = AngerTips.new(Global_UIRoot.transform, info)
-- 		angerTips:UpdateView(info)
-- 	else
-- 		angerTips:UpdateView(info)
-- 	end
-- end

-- local IconTips = require("UI.Common.IconTips")
-- local iconTips
-- function tools.ShowIconTips(info)
-- 	if not iconTips then
-- 		iconTips = IconTips.new(Global_UIRoot.transform, info)
-- 		iconTips:UpdateView(info)
-- 	else
-- 		iconTips.gameObject:SetActive(true)
-- 		iconTips:UpdateView(info)
-- 	end
-- end

function tools.DestroyTips()
	if flyTips then
		flyTips:Dispose()
		flyTips = nil
	end
	if skillTips then
		skillTips:Dispose()
		skillTips = nil
	end
	if iconTips then
		iconTips:Dispose()
		iconTips = nil
	end
	if angerTips then
		angerTips:Dispose()
		angerTips = nil
	end
end

function tools.HideSkillTips()
	if skillTips then
		skillTips.gameObject:SetActive(false)
	end
end

function tools.GetSumValue(list)
	local value = 0
	for i=1,#list do
		value = value + list[i].Value
	end
	return value
end

function tools.GetRandomRateList(str)
	local list = {}
	local temp = tools.Split(str, "|")
	for i=1,#temp do
		local result = tools.Split(temp[i], ",")
		local pre = list[i - 1] and list[i - 1].Value or 0
		list[tonumber(result[1])] = {Value = tonumber(result[2]) + pre, Type = tonumber(result[1])}
	end
	return list
end

function tools.GetDictionaryCount(dic)
	local count = 0
	if dic then
		for k,v in pairs(dic) do
			count = count + 1
		end
	end
	return count
end

function tools.SetCurrentFont(uilabel)
	if not tools.CurrentFont then
		return;
	end
	if uilabel.bitmapFont ~= tools.CurrentFont then
		uilabel.bitmapFont = tools.CurrentFont;
		uilabel.spacingX = 0;
		uilabel.fontStyle = tools.CurrentFontStyle;
	end
end

function tools.SetUIFont_Fzdbsjt(uilabel)
	if not tools.UIFont_Fzdbsjt then
		return;
	end
	if uilabel.bitmapFont ~= tools.UIFont_Fzdbsjt then
		uilabel.bitmapFont = tools.UIFont_Fzdbsjt;
		uilabel.spacingX = 2;
		uilabel.fontStyle = tools.UIFont_FzdbsjtStyle;
	end
end

function tools.SetSpriteRenderer(gameObject, spritePath)
	local render = GameObject.GetComponent(gameObject, "SpriteRenderer")
	render.sprite = LuaToCSFunction.LoadSpriteByName(spritePath)
	return render
end

function tools.SetSpriteName(sprite, name, size)
	sprite.spriteName = name
    if not size then
	    sprite:MakePixelPerfect()
    else
        sprite.height = size.height
        sprite.width = size.width
    end
end

function tools.SetResourceSpriteName(type, id, sprite, size)
	local name = tools.GetResourcesSpriteName(type, id)
	tools.SetSpriteName(sprite, name, size);
end

function tools.GetResourcesSpriteName(type, id)
	local name;
	id = tonumber(id);
	if tonumber(type) == Enum_GlobleType.Diamond then
		name = "Build_Icon_Diamond";
	elseif tonumber(type) == Enum_GlobleType.Resource then
		if id == Enum_UpdateResourceType.Food then
			name = "Build_Buff_Food";
		elseif id == Enum_UpdateResourceType.Wood then
			name = "Build_Buff_Wood";
		elseif id == Enum_UpdateResourceType.Stone then
			name = "Build_Buff_Stone";
		elseif id == Enum_UpdateResourceType.Iron then
			name = "Build_Buff_Iron";
		elseif id == Enum_UpdateResourceType.Diamond then
			name = "Build_Icon_Diamond"
		end
	elseif tonumber(type) == Enum_GlobleType.Population then
		name = "Build_Icon_Population";
	elseif tonumber(type) == Enum_GlobleType.PlayerExp then
		name = "Build_Buff_Exp";
	elseif tonumber(type) == Enum_GlobleType.Goods then
		name = tables.ItemsDic[id].ItemIcon;
	elseif tonumber(type) == Enum_GlobleType.AllianceHonor then
		name = "Build_Buff_Honor"
	end

	return name;
end

--根据联盟关系，得到具体的图片名称和颜色值
function tools.GetRelationshipSpriteColor(relationType)
	local spriteName = nil
	local colorValue = nil
	if relationType == Enum_UnionRelationshipType.Other or relationType == Enum_UnionRelationshipType.Apply then
		spriteName = "gray"
		colorValue = Color.gray
	elseif relationType == Enum_UnionRelationshipType.Alliance then
		spriteName = "blue"
		colorValue = Color.blue
	elseif relationType == Enum_UnionRelationshipType.Member or relationType == Enum_UnionRelationshipType.LordSelf then
		spriteName = "green"
		colorValue = Color.green
	elseif relationType == Enum_UnionRelationshipType.Hostile then
		spriteName = "red"
		colorValue = Color.red
	elseif relationType == Enum_UnionRelationshipType.Friendly then
		spriteName = "yellow"
		colorValue = Color.yellow
	end
	return spriteName, colorValue
end

function tools.ShowGetItemListTips(list)
	local name = ""
	for i=1,#list do
		local v = list[i]
		local itemName = tools.GetItemPropertyByTypeID(v.Type, v.ID)[3]
		-- local itemName = tools.GetResourcesItemName(v.Type, v.ID)
		-- local keyText = ""
		-- if tables.ItemsDic[v.ID] then
		-- 	keyText = tools.GetLanguageByParams(k, {tools.TransferTimeByType(tables.ItemsDic[v.ID].Type,
		-- 		tables.ItemsDic[v.ID].Value)})
		-- else
		-- 	keyText = tables.LanguageDic[k]
		-- end
		name = name..itemName..":"..tools.StringToComma(v.Count).."  "
	end
    tools.ShowTips({Type = Enum_TipsType.Quest,Name = tables.LanguageDic["Bow46"], Content = tools.GetLanguageByParams("Gift04", {name})})
end

function tools.GetResourcesItemName(type, id)
	local name,key;
	local id = tonumber(id);
	local type = tonumber(type)
	if type == Enum_GlobleType.Diamond then
		name = "Item77";
		key = "Recharge14";
	elseif type == Enum_GlobleType.Resource then
		if id == Enum_UpdateResourceType.Food then
			name = "Item_Food";
			key = "Prop27";
		elseif id == Enum_UpdateResourceType.Wood then
			name = "Item_Wood";
			key = "Prop28";
		elseif id == Enum_UpdateResourceType.Stone then
			name = "Item_Stone";
			key = "Prop29";
		elseif id == Enum_UpdateResourceType.Iron then
			name = "Item_Iron";
			key = "Prop30";
		elseif id == Enum_UpdateResourceType.Diamond then
			name = "Item_86"
			key = "Recharge14"
		end
	elseif type == Enum_GlobleType.PlayerExp then
		name = "Item_Exp";
		key = "Bow56";
	elseif type == Enum_GlobleType.Goods then
		name = tables.ItemsDic[id].ItemIcon;
		key = tables.ItemsDic[id].ItemName;
	elseif type == Enum_GlobleType.Population then
		name = "Buff_Population"
		key = "House04"
	elseif type == Enum_GlobleType.Energy then
		name = ""
		key = "CityBuff70"
    elseif type == Enum_GlobleType.AllianceHonor then
        name = "Build_Buff_Honor"
        key = "AllianceBuilding09"
	end

	return name,key;
end

function tools.GetResourceName(type,id)
	local name = "";
	id = tonumber(id);
	type = tonumber(type)
	if type == Enum_GlobleType.Resource then
		if id == Enum_UpdateResourceType.Food then
			name = tables.LanguageDic["Prop27"];
		elseif id == Enum_UpdateResourceType.Wood then
			name = tables.LanguageDic["Prop28"];
		elseif id == Enum_UpdateResourceType.Stone then
			name = tables.LanguageDic["Prop29"];
		elseif id == Enum_UpdateResourceType.Iron then
			name = tables.LanguageDic["Prop30"];
		end
	elseif type == Enum_GlobleType.PlayerExp then
		name = tables.LanguageDic["Bow56"];
	elseif type == Enum_GlobleType.Goods then
		name = tools.GetLanguageByParams(tables.ItemsDic[id].ItemName, {
			tools.StringToComma(tonumber(tools.TransferTimeByType(tables.ItemsDic[id].Type,tables.ItemsDic[id].Value, tables.ItemsDic[id].TimeUnits)))});
	elseif type == Enum_GlobleType.Diamond then
		name = tables.LanguageDic["Recharge14"]
	elseif type == Enum_GlobleType.AllianceHonor then
		name = tables.LanguageDic["AllianceBuilding09"]
	end
	return name;
end

--获取物品的名字，TimeUnits:显示的时间单位，1是分，2是小时，3是天
function tools.GetItemsName(itemInfo, value)
	local value = value or itemInfo.Value
	if itemInfo.Type == Enum_ItemType.LevelChest then
		value = itemInfo.CastleLevel
	end
	if itemInfo.TimeUnits == 1 then
		value = value / 60
	elseif itemInfo.TimeUnits == 2 then
		value = value / 3600
	elseif itemInfo.TimeUnits == 3 then
		value = value / 86400
	end
	local shopInfo = tables.ShopItemsDic[itemInfo.ID]
	if shopInfo and shopInfo.BuyType == Enum_ShopBuyType.BuyAndUse and itemInfo.Type == Enum_ItemType.Chest then
		local list = tools.Split(tables.ChestDic[tonumber(itemInfo.Value)].NeedGet, ",")
		return tools.GetLanguageByParams(itemInfo.ItemName, {tools.StringToComma(tonumber(list[3]))})
	end
	return tools.GetLanguageByParams(itemInfo.ItemName, {tools.StringToComma(value)})
end

--获取物品的描述，TimeUnits:显示的时间单位，1是分，2是小时，3是天
function tools.GetItemsDesc(itemInfo)
	local value = itemInfo.Value
	if itemInfo.Type == Enum_ItemType.LevelChest then
		value = itemInfo.CastleLevel
	end
	if itemInfo.TimeUnits == 1 then
		value = value / 60
	elseif itemInfo.TimeUnits == 2 then
		value = value / 3600
	elseif itemInfo.TimeUnits == 3 then
		value = value / 86400
	end
	local shopInfo = tables.ShopItemsDic[itemInfo.ID]
	if shopInfo and shopInfo.BuyType == Enum_ShopBuyType.BuyAndUse and tables.ChestDic[tonumber(itemInfo.Value)] then
		local list = tools.Split(tables.ChestDic[tonumber(itemInfo.Value)].NeedGet, ",")
		return tools.GetLanguageByParams(itemInfo.ItemDesc, {tools.StringToComma(tonumber(list[3]))})
	end
	return tools.GetLanguageByParams(itemInfo.ItemDesc, {tools.StringToComma(value)})
end

function tools.GetItemPropertyByTypeID(type, id)
	local type = tonumber(type)
	local id = tonumber(id)
	local quality = "Quality1"
	local spriteName = ""
	local itemName = ""
    local itemDesc = ""
	if type == Enum_GlobleType.Goods then
		local itemsData = tables.ItemsDic[id]
		quality = "Quality"..itemsData.Quality
		spriteName = itemsData.ItemIcon
		itemName = tools.GetItemsName(itemsData)
        itemDesc = tools.GetItemsDesc(itemsData)
	elseif type == Enum_GlobleType.Resource then
		local name = ""
		quality = "Quality2"
		if id == Enum_UpdateResourceType.Food then
			spriteName = "Item_Food"
			name = "Prop27"
		elseif id == Enum_UpdateResourceType.Wood then
			spriteName = "Item_Wood"
			name = "Prop28"
		elseif id == Enum_UpdateResourceType.Stone then
			spriteName = "Item_Stone"
			name = "Prop29"
		elseif id == Enum_UpdateResourceType.Iron then
			spriteName = "Item_Iron"
			name = "Prop30"
		end
		itemName = tables.LanguageDic[name]
	else
		if type == Enum_GlobleType.Diamond then
			quality = "Quality5"
			spriteName = "Item86"
			itemName = tables.LanguageDic["Recharge14"]
            itemDesc = tables.LanguageDic["Describe36"]
		elseif type == Enum_GlobleType.PlayerExp then
			quality = "Quality2"
			spriteName = "Item_Exp"
			itemName = tables.LanguageDic["Bow56"]
		elseif type == Enum_GlobleType.Population then
			quality = "Quality2"
			spriteName = "Item_Population"
			itemName = tables.LanguageDic["House04"]
		elseif type == Enum_GlobleType.MysticalCoin then
			quality = "Quality5"
			spriteName = "Coin"
			itemName = tables.LanguageDic["Merchant16"]
		elseif type == Enum_GlobleType.LotteryMultipleCard then
			quality = "Quality4"
			spriteName = "Item73"
			itemName = ""
		elseif type == Enum_GlobleType.Axe then
			quality = "Quality2"
			spriteName = "Item121"
			itemName = tables.LanguageDic["LuckFlop02"]
		elseif type == Enum_GlobleType.PokeBall then
			quality = "Quality2"
			spriteName = "Item122"
			itemName = tables.LanguageDic["LuckFlop01"]
        elseif type == Enum_GlobleType.AllianceHonor then
			quality = "Quality5"
			spriteName = "Item157"
			itemName = tables.LanguageDic["AllianceBuilding09"]
		elseif type == Enum_GlobleType.Soldier then
			quality = "Quality3"
			local sType = tables.TrainInfoDic[id].KindType
			if sType == Enum_TroopType.Barrack then
				spriteName = "Soldier1"
			elseif sType == Enum_TroopType.Shooting then
				spriteName = "Soldier4"
			elseif sType == Enum_TroopType.Cavalry then
				spriteName = "Soldier3"
			elseif sType == Enum_TroopType.Siege then
				spriteName = "Soldier2"
			elseif sType == Enum_TroopType.Rockfall or 
				sType == Enum_TroopType.Catapult or 
				sType == Enum_TroopType.Barricade then
				spriteName = "Soldier6"
			else
				spriteName = "Soldier5"
			end
			itemName = tables.LanguageDic[tables.TrainInfoDic[id].NameDoc]
		end
	end
	return {quality, spriteName, itemName, itemDesc}
end

function tools.GetToNewDayGapTime()
	local timeList = tools.Split(tools.Split(SocketManager.NowTime, " ")[2], ":")
	local time = 24 * 3600 - (tonumber(timeList[1]) * 3600 + tonumber(timeList[2]) * 60 + tonumber(timeList[3]))
	return time
end

--取一个数的整数部分(四舍五入)
--math.ceil:向上取整
--math.floor:向下取整
function tools.GetIntPart(x)
	local result = math.floor(x)
	if x - result > 0.5 then
		result = math.ceil(x)
	end
	return result;
end

function tools.GetFormatNumber(number)
	local str = tostring(number);
	if #str <= 3 then
		return str;
	end
	local list = {};
	for i=1,#str do
		list[i] = tools.SubStringUTF8(str, i, i);
	end
	local index = 0;
	for i=#list,1,-1 do
		index = index + 1;
		if index == 3 then
			table.insert(list, i, ",");
			index = 0;
		end
	end
	local result = "";
	for i=1,#list do
		if i == 1 and list[i] == "," then
		else
			result = result..list[i];
		end
	end
	return result;
end

function tools.IndexOf(arr, value)
	for i=1,#arr do
		if arr[i] == value then
			return i;
		end
	end
	return -1;
end

function tools.Split(str, sep)
	local str = tostring(str)
	local fields = {}
	str:gsub("[^"..sep.."]+", function(c) fields[#fields+1] = c end)
	return fields;
end

function tools.Replace(str, findStr, replaceStr, num)
	return string.gsub(str, findStr, replaceStr, num)
end

function tools.DisplayToString(count)
	-- local text = nil
	if type(count) ~= "number" then
        return count;
    end
    if count / 10^9 >= 1 then  
        count = math.floor(count / 10^8);
        if count % 10 == 0 then
        	return tostring(count/10).."b";
        end
        return(string.format("%.1f", count/10).."b");
    elseif count / 10^6 >= 1 then  
        count = math.floor(count / 10^5);
        if count % 10 == 0 then
        	return tostring(count/10).."m";
        end
        return(string.format("%.1f", count/10).."m");
	elseif count / 10^3 >= 1 then  
        count = math.floor(count / 10^2);
        if count % 10 == 0 then
        	return tostring(count/10).."k";
        end
        return(string.format("%.1f", count/10).."k");
    else  
        return tostring(count);
    end
end

function tools.StringToComma(count)
	local mCount = 0;
	if count >= 1000000 then
		local str1 = math.floor(count / 1000000);
		local str2 = count % 1000000;
		local str3 = math.floor(str2 / 1000);
		local str4 = str2 % 1000;
		if str3 < 10 then
			str3 = "00"..str3;
		elseif str3 < 100 then
			str3 = "0"..str3;
		end
		if str4 < 10 then
			str4 = "00"..str4;
		elseif str4 < 100 then
			str4 = "0"..str4;
		end
		mCount = str1..","..str3..","..str4;
	elseif count >= 1000 then
		local str1 = math.floor(count / 1000);
		local str2 = count % 1000;
		if str2 < 10 then
			str2 = "00"..str2;
		elseif str2 < 100 then
			str2 = "0"..str2;
		end
		mCount = str1..","..str2;
	else
		mCount = count;
	end
	return mCount;
end

function tools.SortEquipListByQualityDesc(a, b)
	if a.Quality == b.Quality then
		return a.Type < b.Type
	else
		return a.Quality < b.Quality
	end
	return false
end

function tools.SortSoldierListByPower(a, b)
	if a.SoldierID then
		return tables.SoldierInfoDic[a.SoldierID].FightPower > tables.SoldierInfoDic[b.SoldierID].FightPower
	elseif a.ID then
		return tables.SoldierInfoDic[a.ID].FightPower > tables.SoldierInfoDic[b.ID].FightPower
	end
end

function tools.SortList(list)
	for vr=1, #list do
        for var=vr+1, #list do
            if list[vr].DisplayOrder > list[var].DisplayOrder then
            	local temp;
                temp = list[vr];
                list[vr] = list[var];
                list[var] = temp;
            end
        end
   end
end

function tools.RemoveTable(list, start, count)
	local result = {}
	for i=1,#list do
		if i < start or i >= start + count then
			result[#result + 1] = list[i];
		end
	end
	list = nil;
	return result;
end

function tools.RemoveByValue(list, value)
	for i = #list, 1, -1 do  
		if list[i] == value then  
			table.remove(list, i)  
		end  
	end  
--	return list;
end

function tools.TransferBuildingTime(time)
	local day = math.floor(time / 86400)
	local temp = time % 86400
	local hour = math.floor(temp / 3600)
	temp = temp % 3600
	local minute = math.floor(temp / 60)
	local second = temp % 60
	if second < 10 then
		second = "0"..second
	end
	local result = ""
	if day == 0 then
		if hour == 0 then
			if minute == 0 then
				result = "0:"..second
			else
				result = minute..":"..second
			end
		else
			if minute < 10 then
				result = hour..":0"..minute..":"..second
			else
				result = hour..":"..minute..":"..second
			end
		end
	else
		if hour < 10 then
			result = day.."D 0"..hour..":"..minute..":"..second
		else
			result = day.."D "..hour..":"..minute..":"..second
		end
	end
	return result
end

function tools.TransferTimeToDayHMS(time)
	local day = math.floor(time / (3600 * 24));
	local temp = time % (3600 * 24);
	local hour = math.floor(temp / 3600);
	temp = temp % 3600;
	local minute = math.floor(temp / 60);
	local second = temp % 60;
	local result = "";
	if day == 0 then
		if hour == 0 then
			if minute == 0 then
				result = second.."s"
			else
				result = minute.."m "..second.."s";
			end
		else
			result = hour.."h "..minute.."m";
		end
	else
		result = day.."d "..hour.."h";
	end
	return result;
end

function tools.TransferTimeByType(type, value, timeType)
	local value = value
	local time = 0;
	if tonumber(type) == Enum_ItemType.Speed then
		if value > 86400 then
			time = value / 86400;
		elseif value >= 3600 then
			time = value / 3600;
		else
			time = value / 60;
		end
	elseif tonumber(type) == Enum_ItemType.VipTime then
		if value >= 86400 then
			time = value / 86400;
		elseif value > 3600 then
			time = value / 3600;
		else
			time = value / 60;
		end
	elseif timeType and timeType ~= 0 then --道具时间单位1分钟；2小时;3天
        if timeType == 1 then
            time = value / 60
        elseif timeType == 2 then
            time = value / 3600
        elseif timeType == 3 then
            time = value / 86400
        end
    else
		time = value;
	end
	return time;
end

function tools.TransferTimeDayHourMinuteSecond(time)
	local temp = time % 86400;
	local day = math.floor(time / 86400);
	local hour = math.floor(temp / 3600);
	local tep = temp % 3600;
	local minute = math.floor(tep / 60);
	local second = time % 60;
	if day <= 0 then
		day = "";
	else
		day = day.."D ";
	end
	if hour < 10 then
		hour = "0"..hour;
	end
	if minute < 10 then
		minute = "0"..minute;
	end
	if second < 10 then
		second = "0"..second;
	end
	return day..hour..":"..minute..":"..second
end

function tools.TransferTimeHourMinuteSecond(time)
	local temp = time % 3600;
	local hour = math.floor(time / 3600);
	local minute = math.floor(temp / 60);
	local second = time % 60;
	if hour < 10 then
		hour = "0"..hour;
	end
	if minute < 10 then
		minute = "0"..minute;
	end
	if second < 10 then
		second = "0"..second;
	end
	if hour == "00" then
		return minute..":"..second
	else
		return hour..":"..minute..":"..second
	end
end

--获取datetime时间差"2016-7-12 22:10:10"
function tools.GetNowTimeDiffDesc(time)
	local list = tools.GetTimeYMDHMS(time);
	local pretime = {year = list[1], month = list[2], day = list[3], hour = list[4], min = list[5], sec = list[6]};
	local list2 = tools.GetTimeYMDHMS(SocketManager.NowTime)
	local nowtime = {year = list2[1], month = list2[2], day = list2[3], hour = list2[4], min = list2[5], sec = list2[6]};
	local result = tables.LanguageDic["Chat04"];

    pretime = os.time(pretime) -- 转成时间戳
    nowtime = os.time(nowtime)

    local secValue = nowtime - pretime  -- 相差多少秒
    local hourValue = secValue / 3600  -- 相差多少小时
    local h, m = math.modf(hourValue)  -- 整数和小数分开

    if h > 0 then
        if m == 0 or ((m * 60) < 1)  then
	        result = tools.GetLanguageByParams("Chat07", {h});
        else
            local tempM = math.modf(m * 60)
	        result = tools.GetLanguageByParams("Chat05", {h, tempM})
        end
    elseif math.modf(m * 60) > 1 then
        local tempM = math.modf(m * 60)
	    result = tools.GetLanguageByParams("Chat08", {tempM});
    end
	return result;
end

function tools.GetTimeDHMS(time)
	local list = tools.Split(time, " ");
	local day = tonumber(tools.Split(list[1], "-")[3]);
	local temp = tools.Split(list[2], ":");
	local hour = tonumber(temp[1]);
	local min = tonumber(temp[2]);
	local sec = tonumber(temp[3]);
	return {day, hour, min, sec};
end

function tools.GetTimeYMDHMS(time)
	local list = tools.Split(time, " ");
    local temp = tools.Split(list[1], "-")
	local year = tonumber(temp[1]);
	local month = tonumber(temp[2]);
	local day = tonumber(temp[3]);
	local temp = tools.Split(list[2], ":");
	local hour = tonumber(temp[1]);
	local min = tonumber(temp[2]);
	local sec = tonumber(temp[3]);
	return {year, month, day, hour, min, sec};
end

function tools.CompareTimes(time1, time2, desc)
	if time1 == nil or time2 == nil then
		return false;
	end
	if time1 == time2 then
		return false;
	end
	local list = tools.GetTimeYMDHMS(time1);
	local list2 = tools.GetTimeYMDHMS(time2);
	if desc then
		for i=1,#list2 do
			if list2[i] ~= list[i] then
				return list2[i] < list[i];
			end
		end
	else
		for i=1,#list do
			if list[i] ~= list2[i] then
				return list[i] < list2[i];
			end
		end
	end
end

function tools.GetLanguageByParams(key, paras)
	local result = tables.LanguageDic[key];
	result = key
	local index = 1;
	for i=1,#paras do
		if string.find(result,"|"..i.."|") then
			result = string.gsub(result, "|"..i.."|", paras[i]);
		end
	end
	return result;
end

function tools.GetIconSpriteName(buildingType)
	local strName = nil;
	if buildingType == Enum_BuildingType.DragonsField then
		strName = "Arms_Icon_long";
	elseif buildingType == Enum_BuildingType.Barracks then
		strName = "Arms_Icon_bubing";
	elseif buildingType == Enum_BuildingType.ShootingRange then
		strName = "Arms_Icon_gongbing";
	elseif buildingType == Enum_BuildingType.KnightsHall then
		strName = "Arms_Icon_knight";
	elseif buildingType == Enum_BuildingType.CavalrySchool then
		strName = "Arms_Icon_kinght";
	elseif buildingType == Enum_BuildingType.SiegeWorkshop then
		strName = "Arms_Icon_gcq";
	elseif buildingType == Enum_BuildingType.Turret then
		strName = "Arms_Icon_TRap";
	end
	return strName;
end

function tools.CheckMail(email)
	if string.len(email or "") < 6 then return false end  
    local b,e = string.find(email or "", '@')  
    local bstr = ""  
    local estr = ""  

    if b then  
        bstr = string.sub(email, 1, b-1)  
        estr = string.sub(email, e+1, -1)  
    else  
        return false  
    end  
  
    -- check the string before '@'  
    local p1,p2 = string.find(bstr, "[%w_.]+")  

    if (p1 ~= 1) or (p2 ~= string.len(bstr)) then return false end  
  
    -- check the string after '@'  
    if string.find(estr, "^[%.]+") then return false end  
    if string.find(estr, "%.[%.]+") then return false end  
    if string.find(estr, "@") then return false end  
    if string.find(estr, "%s") then return false end --空白符  
    if string.find(estr, "[%.]+$") then return false end  
  
    local _,count = string.gsub(estr, "%.", "")

    if (count < 1 ) or (count > 3) then  
        return false  
    end  
  
    return true
end

function tools.CheckScreenFont(str)
	if str == "" then
		return false;
	end

    local str = string.lower(str)

	for k,v in ipairs(tables.ScreenFontList) do
--		print("-=----------------------    ",string.find(str, v));
		if string.find(str, v) then
--			print("-=----------------------    ",v);
			return false;
		end
	end
	return true;
end

--截取中英混合的UTF8字符串，endIndex可缺省
function tools.SubStringUTF8(str, startIndex, endIndex)
    if startIndex < 0 then
        startIndex = tools.SubStringGetTotalIndex(str) + startIndex + 1;
    end

    if endIndex ~= nil and endIndex < 0 then
        endIndex = tools.SubStringGetTotalIndex(str) + endIndex + 1;
    end

    if endIndex == nil then 
        return string.sub(str, tools.SubStringGetTrueIndex(str, startIndex));
    else
        return string.sub(str, tools.SubStringGetTrueIndex(str, startIndex), tools.SubStringGetTrueIndex(str, endIndex + 1) - 1);
    end
end

--获取中英混合UTF8字符串的真实字符数量
function tools.SubStringGetTotalIndex(str)
    local curIndex = 0;
    local i = 1;
    local lastCount = 1;
    repeat 
        lastCount = tools.SubStringGetByteCount(str, i)
        i = i + lastCount;
        curIndex = curIndex + 1;
    until(lastCount == 0);
    return curIndex - 1;
end

function tools.SubStringGetTrueIndex(str, index)
    local curIndex = 0;
    local i = 1;
    local lastCount = 1;
    repeat 
        lastCount = tools.SubStringGetByteCount(str, i)
        i = i + lastCount;
        curIndex = curIndex + 1;
    until(curIndex >= index);
    return i - lastCount;
end

--返回当前字符实际占用的字符数
function tools.SubStringGetByteCount(str, index)
    local curByte = string.byte(str, index)
    local byteCount = 1;
    if curByte == nil then
        byteCount = 0
    elseif curByte > 0 and curByte <= 127 then
        byteCount = 1
    elseif curByte>=192 and curByte<223 then
        byteCount = 2
    elseif curByte>=224 and curByte<239 then
        byteCount = 3
    elseif curByte>=240 and curByte<=247 then
        byteCount = 4
    end
    return byteCount;
end

function tools.GetTrainsMaxCountByType(buildingType, troopID)
	local trainCount = 0
	if tables.SoldierInfoDic[troopID].SoldierType == Enum_SoldierType.Troop then
		trainCount = BuffManager.GetAllBuffEffect(Enum_AllBuffID.AddSoldierTrainCount)
	elseif tables.SoldierInfoDic[troopID].SoldierType == Enum_SoldierType.Knight then
		trainCount = BuffManager.GetAllBuffEffect(Enum_AllBuffID.AddKnightsTrainCount)
	elseif tables.SoldierInfoDic[troopID].SoldierType == Enum_SoldierType.Guard then
		trainCount = BuffManager.GetAllBuffEffect(Enum_AllBuffID.TrapConstruction)
	end
	local needData = tables.TrainInfoDic[troopID]
	if needData.ItemsID > 0 and needData.ItemsNumber > 0 then
		local count = math.floor(PlayerManager.GetItemCountById(needData.ItemsID) / needData.ItemsNumber)
		if count < trainCount then
			trainCount = count
		end
	elseif needData.Population > 0 then
		local count = math.floor(PlayerManager.PlayerInfo.Population / needData.Population)
		if count < trainCount then
			trainCount = count
		end
	end
	return tools.GetIntPart(trainCount)
end

function tools.CloneGameObject(object)
	local go = newObject(object);
	go.transform.parent = object.transform.parent;
	go.transform.localPosition = object.transform.localPosition;
	go.transform.localScale = object.transform.localScale;
	go.transform.localRotation = object.transform.localRotation;
	return go;
end

--data = {Type = Enum_TipsType.  , Name, Content}
function tools.ShowTips(data)
    if ScenesManager.CurrentScene ~= Enum_ScenesType.MainScene and ScenesManager.CurrentScene ~= Enum_ScenesType.WorldScene then return end
	CtrlManager.setData(CtrlManager.CtrlNames.TipsCtrl,{Type = data.Type,Name = data.Name,Content = data.Content});
	CtrlManager.showPanel(CtrlManager.CtrlNames.TipsCtrl);
end

function tools.ShowInfoView(Type, childType, desc)
	CtrlManager.setData(CtrlManager.CtrlNames.InfoTipsCtrl, {Type = Type, ChildType = childType, Desc = desc});
	CtrlManager.showPanel(CtrlManager.CtrlNames.InfoTipsCtrl);  
end

function tools.ShowAlert(data)
	CtrlManager.setData(CtrlManager.CtrlNames.AlertCtrl, data)
	CtrlManager.showPanel(CtrlManager.CtrlNames.AlertCtrl)
end

function tools.GotoPayView()
	--打开钻石购买界面
	ItemSocketSend.ItemSendList[ItemChildPackageType.RechargeViewInfo]({Flag = false})
	--提示
    tools.ShowTips({Type = 5,Name = tables.LanguageDic["Push24"], Content = tables.LanguageDic["Recharge13"]})
end

function tools.SetUISpriteClickEnable(sprite, enable)
	if enable then
		sprite.color = Color.white;
	else
		sprite.color = Color.black;
	end

	local collider = sprite:GetComponent(tools.Collider)
	if collider then
		collider.enabled = enable
	end
end

function tools.SetUIButonEnable(uibutton, enable)
	uibutton.isEnabled = enable
	uibutton:UpdateColor(true)
end

function tools.GetNowTime(callback)
	tools.WWW("https://cgi.im.qq.com/cgi-bin/cgi_svrtime", callback)
end

function tools.GetTimeList(time)
	local data = {}
	local list = tools.Split(time, " ")
	local temp = tools.Split(list[1], "-")
	data.year = tonumber(temp[1])
	data.month = tonumber(temp[2])
	data.day = tonumber(temp[3])
	temp = tools.Split(list[2], ":")
	data.hour = tonumber(temp[1])
	data.min = tonumber(temp[2])
	data.sec = tonumber(temp[3])
	return data
end

function tools.GetTimerDiffer(time1, time2)
	local time1 = tools.GetTimeList(time1)
	local time2 = tools.GetTimeList(time2)
    time1 = os.time(time1) -- 转成时间戳
    time2 = os.time(time2)
    local secValue = time1 - time2
    return secValue
end

function tools.WWW(url, callback)
	coroutine.start(function()
		local www = UnityEngine.WWW(url)
		coroutine.www(www)
		tools.PrintDebug(url .. "请求的数据是:" .. www.text)
		if callback then
			callback(www.text)
		end
	end)
end

------------------- 数字变化 start ------------------------------
function tools.PlayNumberChangeAnim(text_comp, start_value, end_value)
    local anim_item = {}
    anim_item.text_comp = text_comp
    anim_item.start_value = start_value
    anim_item.end_value = end_value
    anim_item.start_time = Time.unscaledTime

    if math.abs(end_value - start_value) < 30 then
        anim_item.time_length = math.abs(end_value - start_value) / 30
    else
        anim_item.time_length = 1.2
    end

    local is_replace
    for i, v in ipairs(numberChangeAnims) do
        if v.text_comp == anim_item.text_comp then
            numberChangeAnims[i] = anim_item
            is_replace = true
        end
    end
    if not is_replace then
        table.insert(numberChangeAnims, anim_item)
    end
    if not isAddUpdateBeat then
        UpdateBeat:Add(tools.UpdateNumberChangeAnim);
    end
    return anim_item
end

function tools.UpdateNumberChangeAnim()
    if #numberChangeAnims == 0 then
        UpdateBeat:Remove(tools.UpdateNumberChangeAnim);
        isAddUpdateBeat = false
        return
    end
    isAddUpdateBeat = true
    for i = #numberChangeAnims, 1, -1 do
        local anim_item = numberChangeAnims[i]
        
        if Time.unscaledTime >= anim_item.start_time + anim_item.time_length then
            anim_item.text_comp.text = tools.GetFormatNumber(anim_item.end_value)
            table.remove(numberChangeAnims, i)
        else
            local temp_num = anim_item.start_value + math.floor((anim_item.end_value - anim_item.start_value) * (Time.unscaledTime - anim_item.start_time) / anim_item.time_length)
            anim_item.text_comp.text = tools.GetFormatNumber(temp_num)
        end
    end
end
------------------- 数字变化 end ------------------------------
function tools.GetServerIdStr(id)
	local str
    local id_str = tostring(id)
    str = id_str
    if #id_str == 1 then
        str = "K0"..id_str
    else
        str = "K"..id_str
    end
    return str
end

function tools.Translate(string)
    local transferID = tools.GetTransferLanguageType()
    --设置翻译用网址，不需要每次都设置，可以在登陆游戏后设置一次即可
    --Bow.HttpWebResponseUtility.SetTransUrl(tools.TransUrl)
    Bow.HttpWebResponseUtility.TransferLanguage(string, tables.SetLanguageDic[transferID].ForShort)
    --local _, t = string.find(result, "<")
    --if ApplicationPlatform == Enum_ApplicationPlatform.Android then
    --result = string.sub(result, t + 1, -( t - 1))
    --else--if ApplicationPlatform == Enum_ApplicationPlatform.IPhonePlayer then
    --    result = string.sub(result, 6, -6)
    --end

    return result
end

function tools.GetTransferLanguageType()
	local transferID = LuaToCSFunction.GetTransferLanguageType()
	if transferID == 0 then
		LuaToCSFunction.SetTransferLanguageType(Enum_LanguageType.English)
		transferID = Enum_LanguageType.English
	end
	return transferID
end

function tools.GetLocalLanguageType()
	local languageType = LuaToCSFunction.GetLocalLanguageType()
	if languageType == 0 then
        local languageType = Enum_LanguageType.English
        local sysl = tostring(UnityEngine.Application.systemLanguage)
        if sysl == "Chinese" or  sysl == "ChineseSimplified" then
		    languageType = Enum_LanguageType.Chinese
        elseif sysl == "ChineseTraditional" then
		    languageType = Enum_LanguageType.TChinese
        elseif sysl == "Russian" then
		    languageType = Enum_LanguageType.Russian
        elseif sysl == "German" then
		    languageType = Enum_LanguageType.German
        elseif sysl == "French" then
		    languageType = Enum_LanguageType.French
        elseif sysl == "Spanish" then
		    languageType = Enum_LanguageType.Spanish
        else
		    languageType = Enum_LanguageType.English
        end

		LuaToCSFunction.SetLocalLanguageType(languageType)
        tables.ConfigDic["Language"] = languageType
	end

	--if languageType ~= tables.ConfigDic["Language"] then
	--	local Language = require("Table.Language");
	--	tables.LanguageDic = {};
	--	for i=1,#Language do
	--		if tables.ConfigDic["Language"] == Enum_LanguageType.Chinese then--中文
	--			tables.LanguageDic[Language[i].Id] = Language[i].Chinese;
	--		elseif tables.ConfigDic["Language"] == Enum_LanguageType.English then--英文
	--			tables.LanguageDic[Language[i].Id] = Language[i].English;
	--		end
	--	end
	--	languageType = tables.ConfigDic["Language"]
	--end

	return languageType
end

-- itemData = {Type, ID, Count}
-- imageTransform 被按图片的transform(含有uiSprite)
function tools.ShowItemInfoTips(itemData, imageTransform)
    if itemData.Type == Enum_GlobleType.LotteryMultipleCard then return end
    CtrlManager.setData(CtrlManager.CtrlNames.ItemDetailCtrl, {ItemData = itemData, imageTransform = imageTransform})
    CtrlManager.showPanel(CtrlManager.CtrlNames.ItemDetailCtrl)
end

function tools.IsChestItem(id)
	local list = tables.ItemsListDic[Enum_ItemType.Chest]
	for i = 1,#list do
		if list[i].ID == id then
			return true
		end
	end
	return false
end

function tools.PayMoney(info)
	tools.BuyFlag = true
	if Global_Platform == 1 then 		--安卓
		-- LuaToCSFunction.PayMoney("PayMoneySuccess", info.ProId, info.IapId, "")
	elseif Global_Platform == 2 then 	--苹果
		LuaToCSFunction.PayMoney("PayMoneySuccess", info.ProID, info.ID, "")
	end
	-- LuaToCSFunction.TrackAppsFlyerEvent("BuyItems", 0);
end

function tools.AddIphoneXMask()
	-- local IphoneXCamera = tools.FindChildObj(Global_UIRoot.transform, "IphoneXCamera")
	-- if not tools.IsIphoneX then
	-- 	IphoneXCamera:SetActive(false)
	-- 	return
	-- end
	-- local IphoneXMask = LuaToCSFunction.LoadPrefabByName("Prefabs/Common/IphoneXMask", IphoneXCamera.transform)
 --    LuaToCSFunction.SetGameObjectLocalPosition(IphoneXMask, 0, 0, 0)
 --    LuaToCSFunction.SetGameObjectAndChildrenLayer(IphoneXMask, 8)
 --    return IphoneXMask
end

function tools.SetIphoneXAdapt()
	if not tools.IsIphoneX then
		return
	end
	local Camera = tools.FindChild(Global_UIRoot.transform, "Camera", tools.Camera)
	LuaToCSFunction.SetUICameraRect(Camera, 0, 0.04, 1, 0.92)
	Camera.orthographicSize = 1
end

function tools.LoadSceneEffect()
	Global_LoadSceneEffect = LuaToCSFunction.LoadPrefabByName("Prefabs/Effect/LoadSceneEffect", nil)
	Global_LoadSceneEffect.DontDestroyOnLoad(Global_LoadSceneEffect)
    LuaToCSFunction.SetGameObjectLocalPosition(Global_LoadSceneEffect, 0, 0, 0)
    LuaToCSFunction.SetGameObjectLocalScale(Global_LoadSceneEffect, 2, 2, 2)
end

function tools.HideSceneEffect()
	if Global_LoadSceneEffect then
		LuaToCSFunction.PoolDestroy(Global_LoadSceneEffect)
		Global_LoadSceneEffect = nil
	end
end

function tools.GetRealUrl(str)
	return tools.Split(tools.Split(str, "/")[3], ".")[1]
end

function tools.GetQualityColor(quality)
	if quality == 1 then
		return "[ffffff]"
	elseif quality == 2 then
		return "[00ff00]"
	elseif quality == 3 then
		return "[0000ff]"
	elseif quality == 4 then
		return "[800080]"
	elseif quality == 5 then
		return "[FFA500]"
	elseif quality == 6 then
		return "[ff0000]"
	end
end

function tools.Invoke(time, callback)
	local timer = Timer.New(function()
		callback()
	end, time)
	timer:Start()
end

function tools.clone(object)
    local lookup_table = {}
    local function copyObj( object )
        if type( object ) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local new_table = {}
        lookup_table[object] = new_table
        for key, value in pairs( object ) do
            new_table[copyObj( key )] = copyObj( value )
        end
        return setmetatable( new_table, getmetatable( object ) )
    end
    return copyObj( object )
end

return tools