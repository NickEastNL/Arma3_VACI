private ["_data", "_instruction"];

while {true} do {
    _data = "AVCI_Extension" callExtension "get";

	if (_data != "") then {
	    _instruction = _data splitString ";";

		if (!(isNil "_instruction") && (count _instruction > 0)) then {
			_instruction execVM "execute.sqf";
		};
	};

	sleep 0.1;
};
