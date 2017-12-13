private ["_data", "_instructions", "_parts", "_type", "_parameter", "_arguments"];

while {true} do {
    _data = "VACI_Extension" callExtension "get";

	if (_data != "") then
	{
		_instructions = _data splitString ";";

		if (!(isNil "_instructions") && (count _instructions > 0)) then {
		    //Loop through the list of Instructions
			{
				hint _x;

			    _parts = _x splitString "|";

				if (!(isNil "_parts") && (count _parts > 1)) then {
					//Get the 'parameter' of the Instruction
					_parameter = _parts select 1;

					//Check if the Instruction is code to be executed directly
					if ((_parts select 0) == "code" && (count _parts == 2)) then {
					    call compile _parameter;
					};

					//Check if the Instruction is a script file to be executed
					if ((_parts select 0) == "script" && (count _parts == 2)) then {
					    execVM _parameter;
					};

					//Check if the Instruction is a script file with arguments to be executed
					if ((_parts select 0) == "script" && (count _parts == 3)) then {
					    _arguments = _parts select 2;

						_array = parseSimpleArray _arguments;

						_array execVM _parameter;

						hint _arguments;
					};
				};
			} forEach _instructions;
		};
	};

	sleep 0.066;
};
