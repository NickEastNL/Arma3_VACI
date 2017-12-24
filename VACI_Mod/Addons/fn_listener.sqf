private ["_data", "_instruction"];

while {true} do {
    _data = "AVCI_Extension" callExtension "get";

	if (_data != "") then {
	    _instruction = _data splitString ";";

		if (!(isNil "_instruction") && (count _instruction > 1)) then {
			_instruction call VACI_fnc_Execute;
		};
	};

	sleep 0.1;
};
