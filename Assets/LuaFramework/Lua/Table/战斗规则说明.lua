﻿local 战斗规则说明 = {
	{
		战斗规则说明： = "1.战斗场景是格子的。",
	},
	{
		战斗规则说明： = "2.玩家会有X张卡牌，可以用于摆放于空白的任意位置。",
	},
	{
		战斗规则说明： = "3.敌人的位置暂定都是固定的。但后续的玩法可能会有中途出现的。",
	},
	{
		战斗规则说明： = "4.每个战斗单位按自己的AI行动，目前只做一种，就是打最近的。",
	},
	{
		战斗规则说明： = "5.攻击距离：攻击距离为1代表可以攻击自己周围相邻的8个单位，以此类推",
	},
	{
		战斗规则说明： = "6.攻击速度：以攻击间隔作为一个值，第一次攻击开始触发之后下一次攻击开始的时间间隔。",
	},
	{
		战斗规则说明： = "比如攻击间隔1S，代表每隔1S攻击1次。",
	},
	{
		战斗规则说明： = "7.攻击范围内没有敌人会自动寻路到最近的一个敌人，使其刚好在自己攻击范围内继续进行攻击，注意有地图是有障碍的，可能还不止一个屏，所以需要A*算法。",
	},
	{
		战斗规则说明： = "8.战斗胜利条件，暂时是消灭所有敌人。",
	},
	{
		战斗规则说明： = "9.开始战斗：放置第一个卡牌开始屏幕中央出现战斗按钮，点击战斗按钮战斗开始，中途可以放入余下的卡牌，最多上阵6张。",
	},
	{
		战斗规则说明： = "10.战斗结束：自己的所有卡牌死亡--失败。敌人全部死亡--胜利。",
	},
	{
		战斗规则说明： = "11.胜利结算奖励",
	},
	{
		战斗规则说明： = "12.卡牌放置支持点击放置和拖动放置。",
	},
	{
		战斗规则说明： = "13.当范围内存在多个目标时，先选择最近距离，再在同为最近距离的所有目标里，按正上方方向顺时针顺序选择。",
	},
}
return 战斗规则说明