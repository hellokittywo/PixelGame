local Config = {
	{
		Id = 1,
		Value = 1,
		A = "系数1----dmg = 系数1*攻击*攻击/(系数2*攻击+系数3*防御)*(1+伤害加成)*(1-伤害减免)",
	},
	{
		Id = 2,
		Value = 1,
		A = "系数2",
	},
	{
		Id = 3,
		Value = 1,
		A = "系数3",
	},
	{
		Id = 4,
		Value = 2,
		A = "基础暴击倍率",
	},
	{
		Id = 5,
		Value = "0.8,1.2",
		A = "伤害浮动区间",
	},
}
return Config