local this = Component;
local game = GameWrapper;
-- Traits

function init (script, defaults)
    this = Component.__new("angel-ai", 100, script, "entitydead");
	return this;
end

function onAdded()
end

function onRemoved()
end

function handleAction (gameAction)
    actionType = gameAction.Type;
    if (actionType == "entitydead") then
		game.Instance.HandleAction(GameAction.__new("heal", this.Owner.Id, gameAction.SenderId, "325"));
	end
end