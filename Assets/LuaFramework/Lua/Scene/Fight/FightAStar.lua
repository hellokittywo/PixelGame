local FightAStar = class("FightAStar")

function FightAStar:GetPathList(start, target, callBack)
	-- tools.PrintDebug("---寻路", start.PosX, start.PosY, target.PosX, target.PosY)
	self.openList = {}
	self.tansuoList = {}
	self.pathList = {}
	self.callBack = callBack
	self.startNode = {x = start.PosX, y = start.PosY, g = 0, h = 0, f = 0}
	self.endNode = {x = target.PosX, y = target.PosY, g = 0, h = 0, f = 0}
	FightScene.ObstructDic[target.Place] = nil
	self.startInfo = start
	local node = self:FindPath()
	-- self.callBack({node})
	if node then
  		self:GetPath(node)
  	else
  		self.callBack(self.pathList)
  	end
end

function FightAStar:GetPath(node)
	while node do
		table.insert(self.pathList, 1, node)
		node = node.parent
	end
	table.remove(self.pathList, 1)
	table.remove(self.pathList, #self.pathList)
	self.callBack(self.pathList)
end

function FightAStar:IndexOfXY(list, x, y)
 	for i,v in pairs(list) do
 		if v.x == x and v.y == y then
 			return i
 		end
 	end
 	return false
end

function FightAStar:GetMinNodeInOpenList()
	local index = 1
	local minNode = self.openList[1]
	for i=2,#self.openList do
		if self.openList[i].f < minNode.f then
			minNode = self.openList[i]
			index = i
		end
	end
	return index, minNode
end

function FightAStar:GetArroundNodes(curNode)
	local list = {}
	for i = 1,4 do
		if i == 1 then
			x = curNode.x
	    	y = curNode.y +1
		elseif i == 2 then
	    	x = curNode.x
	    	y = curNode.y - 1
		elseif i == 3 then
	    	x = curNode.x - 1
	    	y = curNode.y
		elseif i == 4 then
	    	x = curNode.x + 1
	    	y = curNode.y
		end
		local id = (x - 1) * FightScene.Column + y
		if x < 1 or x > FightScene.MaxX or y < 1 or y > FightScene.MaxY or FightScene.ObstructDic[id]
			or self:IndexOfXY(self.tansuoList, x, y) then
		else
			local g = math.abs(curNode.x - x) + math.abs(curNode.y - y)
			local px = math.abs(self.endNode.x - x)
			local py = math.abs(self.endNode.y - y)
			local h = px + py
			local openNode = {x = x, y = y, g = g, h = h, px = curNode.x, py = curNode.y, f = g + h}
			list[#list + 1] = openNode
		end
	end
	--1.对方和你的相对位置。如果横着远就先横着，如果竖着远就先竖着。
	--2.如果一样远按对方和你的相对位置，对方相对你偏下，你就横着走，相对你偏上你就竖着接近
	table.sort(list, function(a, b)
		return a.px + a.py > b.px + b.py
	end)
	return list
end

function FightAStar:CalcG(node)
	local g = node.g
	if node.parent then
		g = g + node.parent.g
	end
	return g
end

function FightAStar:GetNode(list, node)
	for i=1,#list do
		if list[i].x == node.x and list[i].y == node.y then
			return list[i]
		end
	end
	return nil
end

-- function FightAStar.FindPath()
-- 	self.openList = self.GetArroundNodes(self.startNode)
-- 	local index, node = self.GetMinNodeInOpenList()
-- 	return node
-- end

function FightAStar:FindPath()
	local temp = 1
	self.openList[#self.openList + 1] = self.startNode
	-- tools.PrintDebug("-------------当前探索起点", self.startNode.x, self.startNode.y, #self.openList);
	while #self.openList > 0 do
		if self.dispose or self.pause then
			return
		end
		-- tools.PrintDebug("--------------查找路径")
		local index, node = self:GetMinNodeInOpenList()
		table.remove(self.openList , index)
		self.tansuoList[#self.tansuoList + 1] = node
		-- self.openList = {}
		local list = self:GetArroundNodes(node)
		if #list == 0 then
			local lastNode = self:GetNode(self.tansuoList, self.endNode)
			return lastNode
		end
		-- if node.parent then
		-- 	tools.PrintDebug(temp.."-------------当前探索结点", self.startInfo.Name, node.x, node.y, node.g, node.h, #list,
		-- 		"parent:", node.parent.x, node.parent.y);
		-- else
		-- 	tools.PrintDebug(temp.."-------------当前探索结点", self.startInfo.Name, node.x, node.y, node.g, node.h, #list,
		-- 	"parent:nil");
		-- end
		for i=1,#list do
			local openNode = list[i]
			print("----节点", openNode.x, openNode.y)
			local hasNodeIndex = self:IndexOfXY(self.openList, openNode.x, openNode.y)
			-- tools.PrintDebug(temp.."-------------结点是否已经包括在openList", hasNodeIndex, openNode.x, openNode.y, openNode.g, openNode.h,
			-- 	"parent：", node.x, node.y);
			if hasNodeIndex then
				local g = self:CalcG(openNode)
				if g < openNode.g then
					openNode.parent = node
					openNode.g = g
					openNode.f = openNode.g + openNode.h
				end
			else
				openNode.parent = node
				-- openNode.g = self:CalcG(openNode)--不能斜着走这里不需要
				openNode.f = openNode.g + openNode.h
				self.openList[#self.openList + 1] = openNode
				-- print("----添加进去", openNode.x, openNode.y, openNode.f)
			end
		end
		temp = temp + 1
		local lastNode = self:GetNode(self.tansuoList, self.endNode)
		if lastNode then
			return lastNode
		end
		if temp > 200 then
			tools.PrintDebug("------------这里死循环了", self.startNode.x, self.startNode.y, self.endNode.x, self.endNode.y);
			-- self:Dispose();
			return
		end
	end
	return
end

return FightAStar