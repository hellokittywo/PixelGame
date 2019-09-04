local AudioName = {}
--主场景
AudioName.FightScene = "FightScene"
AudioName.MainScene = "MainScene"
--战斗场景随机音乐
AudioName.FightMusicList = {}
for i=1,17 do
	AudioName.FightMusicList[i] = "RandomAudio"..i
end

AudioName.OnClickAudio = "OnClickAudio"--点击按钮.wav
AudioName.Succeed = "Succeed"--强化附魔成功.wav
AudioName.Defeated = "Defeated"--强化附魔失败.wav
AudioName.DragEnd = "DragEnd"--拖动背包里的物品到任意位置（包括拖到附魔，强化，和背包内自身拖动，灵魂背包全部拾取）.wav
AudioName.OpenBox = "OpenBox"--打开宝箱.wav
AudioName.LevelUp = "LevelUp"--人物升级.wav
AudioName.UsePotions = "UsePotions"--使用药水音效.wav
AudioName.Error = "Error"--错误信息提示（比如没有放入强化材料点强化，附魔，战斗打开背包，点传送石）.wav
AudioName.Popup = "Popup"--弹出提示音效.wav
AudioName.CloseOrOpen = "CloseOrOpen"--关闭界面和打开界面.wav
AudioName.StartFlame = "StartFlame"--开始界面火把燃烧的音效.wav
AudioName.ManDeath = "ManDeath"--男角色死亡.wav
AudioName.WomanDeath = "WomanDeath"--女角色死亡.wav
AudioName.YLFlame = "YLFlame"--炎龙持续燃烧音效.wav
AudioName.PhysicsAttack = "PhysicsAttack"--战士学者普通攻击.wav

--技能音效
AudioName.Parry = "Parry"--触发格挡的声音.wav
AudioName.Stone = "Stone"--大猩猩的石头砸到身上.wav
AudioName.manSkill1OrCrit = "manSkill1OrCrit"--男角色巨力挥砍或暴击.wav
AudioName.WomanSkill1OrCrit = "WomanSkill1OrCrit"--女角色巨力挥砍或暴击.wav
AudioName.Lightning = "Lightning"--电击，法师BOSS怪的电击。.wav
AudioName.LightningSkill = "LightningSkill"--闪电之灵每次击中.wav
AudioName.SkillFlicker = "SkillFlicker"--释放技能闪光的同时.wav
AudioName.BloodEffect = "BloodEffect"--嗜血播放特效时.wav
AudioName.DeathScythe = "DeathScythe"--死神镰刀特效.wav
AudioName.Explode = "Explode"--同归于尽和熔岩巨人特效触发音效.wav
AudioName.Recover = "Recover"--治疗（加血，回天，怪物加血通用）.wav
return AudioName