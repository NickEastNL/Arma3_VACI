private ["_data"];

while {true} do {
    _data = "VACI_Extension" callExtension "get";

	if (_data != "") then
	{
		hint _data;
	};

	sleep 0.066;
};
