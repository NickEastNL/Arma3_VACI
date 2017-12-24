class CfgPatches
{
	class VACI
	{
		units[]={};
		weapons[]={};
		requiredAddons[]={};
		fileName = "VACI.pbo";
		author="Nick v. Oosten (SGCommand)";
		authorUrl="";
		url="";
	};
};
class CfgMods
{
	class VACI
	{
		name = "VoiceAttack Command Interface (VACI)";
		tooltipOwned = "Provided a seamless interface with VoiceAttack";
		overview = "The Mod provides an Extension that allows VoiceAttack to communicate with Arma 3 and have them seamlessly communicate without using keybindings.";
		dir = "@VACI";
	};
};
class CfgFunctions
{
	class VACI
	{
		tag = "VACI";

		class VACI_Functions
		{
			class Initialize
			{
				postInit=1;
				file="\VACI\fn_initialize.sqf";
			};
			class Listener
			{
				file="\VACI\fn_listener.sqf";
			};
			class Execute
			{
				file="\VACI\fn_execute.sqf";
			};
			class Shutdown
			{
				file="\VACI\fn_shutdown.sqf";
			};
		};
	};
};
