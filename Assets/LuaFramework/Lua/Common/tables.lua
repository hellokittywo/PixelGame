tables = {}
GameLayerName = {
	Effect = 8,
}

Enum_SexType = 
{
	Man = 1,
	Woman = 2
}

Enum_TeamType = 
{
	Self = 1,
	Enemy = 2
}

Enum_BloodType = 
{
	-- None = 0,
	Hurt = 1,
	Treat = 2,
	-- Property = 3,
}


 -- 1 前排全体伤害N   
 -- 2 后排全体伤害N   
 -- 3 前排单体伤害N   
 -- 4 后排单体伤害N   
 -- 5 随机单体伤害N   
 -- 6 随机2目标伤害N   
 -- 7 随机3目标伤害N   
 -- 8 全体伤害N   
 -- 9 直线伤害N   
 -- 10 随机单体（优先中间目标）
 -- 11 目标血量最少者N
 -- 12 友方血量最少者N 
 -- 13 全体友方
 -- 14 友方前排目标（跳过主将）
 -- 15 友方前排目标（不跳过主将）
 -- 16 特殊技：玄武大招
 -- 17 随机二个血量最少者加血
Enum_AttackRange = 
{
	Hurt_Front_All = 1,
	Hurt_Back_All = 2,
	Hurt_Front_One = 3,
	Hurt_Back_One = 4,
	Hurt_Random_One = 5,
	Hurt_Random_Two = 6,
	Hurt_Random_Three = 7,
	Hurt_All = 8,
	Hurt_Line = 9,
	Hurt_Random_OneFirstCenter = 10,
	Hurt__MinBlood = 11,
	Treat_MinBlood = 12,
	Treat_All = 13,
	Treat_Front = 14,
	Treat_FrontTwo = 15,
	Special = 16,
	Treat_RandomTwoMinBlood = 17,
}

Enum_AttackAction = 
{
	NoMove = 0,
	MoveToTarget = 1,
	NoMoveAndFly = 2,
}

Enum_CardAction = 
{
	None = 0,
	Shake = 1,
	Scale = 2,
}
Enum_HitAction = 
{
	None = 0,
	ScaleBackOff = 1,--斜退
	BackOff = 2,--后退
}

tables.LanguageDic = {}
tables.LanguageDic["Bow06"] = "文本"
--local Config = require("Table.Config")
--tables.ConfigDic = {}
--for i=1,#Config do
	--tables.ConfigDic[Config[i].Code] = Config[i]
--end

local Card = require("Table.Hero")
tables.CardDic = {}
for i=1,#Card do
	tables.CardDic[Card[i].Id] = Card[i]
end

local Copy = require("Table.Checkpoint")
tables.CopyDic = {}
for i=1,#Copy do
	tables.CopyDic[Copy[i].Id] = Copy[i]
end

local Skill = require("Table.Skill")
tables.SkillDic = {}
for i=1,#Copy do
	tables.SkillDic[Skill[i].Id] = Skill[i]
end

local Config = require("Table.Config")
tables.ConfigDic = {}
for i=1,#Config do
	tables.ConfigDic[Config[i].Id] = Config[i].Value
end