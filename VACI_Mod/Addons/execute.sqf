params ["_type", "_command"];

if (_type == "select") then {
    switch (_command) do {
        case ("two"): {
            player groupSelectUnit[(units player) select 1, true];
        };
		case ("three"): {
		    player groupSelectUnit[(units player) select 2, true];
		};
		case ("four"): {
		    player groupSelectUnit[(units player) select 3, true];
		};
		case ("five"): {
		    player groupSelectUnit[(units player) select 4, true];
		};
		case ("six"): {
		    player groupSelectUnit[(units player) select 5, true];
		};
		case ("all"): {
		    {
		        player groupSelectUnit[_x, true];
		    } forEach units player;
		};
    };
};

if (_type == "do") then {
    switch (_command) do {
        case ("regroup"): {
			groupSelectedUnits player commandFollow player;
        };
		case ("move"): {
		    (groupSelectedUnits player) commandMove (screenToWorld [0.5,0.5]);
		};
    };

	{
		player groupSelectUnit[_x, false];
	} forEach units player;
};
