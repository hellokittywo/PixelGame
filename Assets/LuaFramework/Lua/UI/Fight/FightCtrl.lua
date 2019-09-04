local BaseCtrl = require("UI.BaseCtrl")
local FightCtrl = class("FightCtrl", BaseCtrl)

function FightCtrl:ctor()
	self.super.ctor(self, "Fight/FightView")
	self.model = require("UI.Fight.FightModel").new()
	self.view = require("UI.Fight.FightView").new()
	self.view.model = self.model
end

function FightCtrl:OnCreate(obj)
	self.inited = true
	self.view:Awake(obj)
end

function FightCtrl:SetData(data)
	self.model:SetData(data)
end

return FightCtrl