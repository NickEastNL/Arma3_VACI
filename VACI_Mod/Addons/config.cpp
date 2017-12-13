class CfgPatches
{
	class VACI
	{
		units[]={};
		weapons[]={};
		requiredAddons[]={};
		author="Nick v. Oosten (SGCommand)";
		authorUrl="";
		url="";
	};
};
class CfgFunctions
{
	class VACI
	{
		class VACI_Functions
		{
			class Startup
			{
				postInit=1;
				file="\VACI\init.sqf";
				description="Initializes plugin";
			};
			class Listener
			{
				file="\VACI\listener.sqf";
				description="Listens to VoiceAttack and relays data to Arma";
			};
			class Shutdown
			{
				file="\VACI\exit.sqf";
				description="Shuts down plugin";
			};
		};
	};
};
