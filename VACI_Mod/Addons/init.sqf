"VACI_Extension" callExtension ""; //Initalizes Extension to start server thread and accept a connection without processing anything

waitUntil {!isNull player};

[] execVM "\VACI\listener.sqf";
