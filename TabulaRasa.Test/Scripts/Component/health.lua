local this = Component;
local game = GameWrapper;
-- Traits
local maxHP;
local currHP;

function init (script, defaults)
    this = Component.__new("health", 100, script, "takedamage", "heal");
    maxHp = defaults["maxHP"];
    currHp = defaults["currHP"];
	return this;
end

function onAdded()
    this.Owner.Traits["maxHp"] = maxHp;
    this.Owner.Traits["currHp"] = currHp;
end

function onRemoved()
    this.Owner.Traits["maxHp"] = nil;
    this.Owner.Traits["currHp"] = nil;
end

function handleAction (gameAction)
    actionType = gameAction.Type;
    print(actionType)
    if (actionType == "takedamage") then
        this.Owner.Traits["currHp"] = this.Owner.Traits["currHp"] - gameAction.Args[1];
        if (tonumber(this.Owner.Traits["currHp"]) <= 0) then
            print(this.Owner.Id)
            print(this.Owner.Traits["name"])
            game.Instance.HandleAction(GameAction.__new("entitydead", this.Owner.Id, this.Owner.Traits["name"]))
        end
    elseif (actionType == "heal") then
        this.Owner.Traits["currHp"] = this.Owner.Traits["currHp"] + gameAction.Args[1];
        if (this.Owner.Traits["currHp"] > this.Owner.Traits["maxHp"]) then
            this.Owner.Traits["currHp"] = this.Owner.Traits["maxHp"];
        end
    end
end