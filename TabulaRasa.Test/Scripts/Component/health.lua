local this = Component;
local game = GameWrapper;

-- default traits
local maxHp;
local currHp;

function init (script, defaults)
    this = Component.__new("health", 100, script, "do-takedamage", "do-heal");
    maxHp = defaults["maxHp"];
    currHp = defaults["currHp"];
	return this;
end

function onAdded()
	print(maxHp);
	if (this.Owner.Traits["maxHp"] == nil) then
		this.Owner.Traits["maxHp"] = maxHp;
	end
	if (this.Owner.Traits["currHp"] == nil) then
		this.Owner.Traits["currHp"] = currHp;
	end
end

function onRemoved()
    this.Owner.Traits["maxHp"] = nil;
    this.Owner.Traits["currHp"] = nil;
end

function handleAction (gameAction)
	if (gameAction.Type == "do-takedamage") then
		this.Owner.Traits["currHp"] = this.Owner.Traits["currHp"] - gameAction.Value;
		if (tonumber(this.Owner.Traits["currHp"]) <= 0) then
			this.Owner.Traits["currHp"] = 0;
			print (this.Owner.Name .. " is dead!!!");
			game.Instance.HandleAction(GameAction.__new("entitydead", this.Owner.Id, this.Owner.Traits["name"]))
		end
	elseif (gameAction.Type == "do-heal") then
		print (this.Owner.Name .. " is healed!!!");
		this.Owner.Traits["currHp"] = this.Owner.Traits["currHp"] + gameAction.Value;
		print(this.Owner.Name .. " was healed by " .. gameAction.SenderId);
		if (this.Owner.Traits["currHp"] > this.Owner.Traits["maxHp"]) then
			this.Owner.Traits["currHp"] = this.Owner.Traits["maxHp"];
		end
		print(this.Owner.Name .. " hp: " .. this.Owner.Traits["currHp"] .. "/" .. this.Owner.Traits["maxHp"]);
	end
end