EventsManager = {}
local Event = require 'events'
local eventsList = {}
EventsManager.EventsName = {
    Event_SelectMyCard = "Event_SelectMyCard",
    Event_SelectCardItem = "Event_SelectCardItem",
    Event_SelectMapItem = "Event_SelectMapItem",
    Event_Attack = "Event_Attack",
    Event_CardHurt = "Event_CardHurt",
    Event_CardDead = "Event_CardDead",
    Event_CardMove = "Event_CardMove",
    Event_FightResult = "Event_FightResult",
    Event_StopMove = "Event_StopMove",
    Event_CardAttack = "Event_CardAttack",
}

function EventsManager.AddEvent(eventName, luatable, callBackFunc)
	Event.AddListener(eventName, callBackFunc, luatable); 
	eventsList[eventName] = 1;
end

function EventsManager.DispatchEvent(eventName, data)
	if not eventsList[eventName] then
		return;
	end
	Event.Brocast(eventName, data); 
end

function EventsManager.RemoveEvent(eventName, luatable, callBackFunc)
	if not eventsList[eventName] then
		return;
	end
	Event.RemoveListener(eventName, callBackFunc, luatable); 
end