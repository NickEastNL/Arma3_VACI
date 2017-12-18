"AVCI_Extension" callExtension "init";

waitUntil{!isNull player};

execVM "listener.sqf";
