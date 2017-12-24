"AVCI_Extension" callExtension "init";

waitUntil{!isNull player};

[] call VACI_fnc_Listener;

addMissionEventHandler ["Ended", { [] call VACI_fnc_Shutdown }];
