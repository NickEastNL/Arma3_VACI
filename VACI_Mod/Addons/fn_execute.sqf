params [["_type", "", [""]], ["_command", "", [0, ""]]];

if (_type == "select") then {
	switch (_command) do {
	    case ("all"): {
			{
		        player groupSelectUnit[_x, true];
		    } forEach units player;
	    };
		case ("disregard"): {
			{
				player groupSelectUnit[_x, false];
			} forEach units player;
		};
		default {
			_number = (parseNumber _command) - 1;
			if (_number <= (count units player)) then {
			    player groupSelectUnit[(units player) select _number, true];
			};
		};
	};
};

if (_type == "do") then {
    switch (_command) do {
		// Move (1)
        case ("regroup"): {
			groupSelectedUnits player doFollow player;
        };
		case ("stop"): {
		    doStop (groupSelectedUnits player);
		};
		case ("move"): {
		    (groupSelectedUnits player) doMove (screenToWorld [0.5,0.5]);
		};

		// Engage (3)
		case ("open fire"): {
		    (group player) setCombatMode "YELLOW";
		};
		case ("hold fire"): {
		    (group player) setCombatMode "GREEN";
		};
		case ("fire"): {
		    //(groupSelectedUnits player) commandFire;
		};
		case ("engage at will"): {
		    (group player) setCombatMode "RED";
		};

		// Formation (8)
		case ("column"): {
		    (group player) setFormation "COLUMN";
		};
		case ("staggered column"): {
		    (group player) setFormation "STAG COLUMN";
		};
		case ("wedge"): {
		    (group player) setFormation "WEDGE";
		};
		case ("echelon left"): {
		    (group player) setFormation "ECH LEFT";
		};
		case ("echelon right"): {
		    (group player) setFormation "ECH RIGHT";
		};
		case ("vee"): {
		    (group player) setFormation "VEE";
		};
		case ("line"): {
		    (group player) setFormation "LINE";
		};
		case ("file"): {
		    (group player) setFormation "FILE";
		};
		case ("diamond"): {
		    (group player) setFormation "DIAMOND";
		};
    };

	{
		player groupSelectUnit[_x, false];
	} forEach units player;
};

true;
