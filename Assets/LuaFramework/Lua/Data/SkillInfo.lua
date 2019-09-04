local SkillInfo = class("SkillInfo")

function SkillInfo:ctor(id, level)
	self.info = tables.SkillDic[id]
	self.Level = level or 1
end

return SkillInfo