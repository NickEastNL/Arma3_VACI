using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Text;
using System.Threading.Tasks;

namespace AVCI_Extension
{
	static class CommandList
	{
		private static Subject subjectObject;
		private static Dictionary<string, Command> commandObjects;

		public static SrgsDocument BuildSrgsGrammar(CultureInfo cultureInfo)
		{
			SrgsDocument document = new SrgsDocument();
			document.Culture = cultureInfo;

			// Collect all subject items and add them to the document
			subjectObject = new Subject();
			foreach (SrgsRule rule in subjectObject.RuleList)
			{
				document.Rules.Add(rule);
			}

			SrgsRuleRef subjectRef = new SrgsRuleRef(subjectObject.RootRule);

			commandObjects = new Dictionary<string, Command>();
			SrgsOneOf commandSet = new SrgsOneOf();

			SrgsItem select = new SrgsItem();
			select.Add(new SrgsItem(subjectRef));
			select.Add(new SrgsSemanticInterpretationTag("out.subject=rules.subject;"));
			commandSet.Add(select);

			#region Commands

			#region Move (1)

			// Return to formation (1)
			Command regroup = new Command("REGROUP", new string[] { "regroup", "return to formation", "fall back", "on me" }, "regroup", subjectRef);
			commandObjects.Add("REGROUP", regroup);
			commandSet.Add(regroup.Item);

			// Advance (2)
			Command advance = new Command("ADVANCE", new string[] { "advance", "move up" }, "advance", subjectRef);
			commandObjects.Add("ADVANCE", advance);
			commandSet.Add(advance.Item);

			// Stop (6)
			Command stop = new Command("STOP", new string[] { "stop", "hold position", "halt", "stay here" }, "stop", subjectRef);
			commandObjects.Add("STOP", stop);
			commandSet.Add(stop.Item);

			// Move to (Space)
			Command move = new Command("MOVE", new string[] { "move", "get over there", "move up" }, "move", subjectRef);
			commandObjects.Add("MOVE", move);
			commandSet.Add(move.Item);
			#endregion

			#region Engage (3)

			// Open fire (1)
			Command openFire = new Command("OPENFIRE", new string[] { "open fire", "fire at will", "weapons free", "go loud" }, "open fire", subjectRef);
			commandObjects.Add("OPENFIRE", openFire);
			commandSet.Add(openFire.Item);

			// Hold fire (1)
			Command holdFire = new Command("HOLDFIRE", new string[] { "hold fire", "cease fire", "go quiet" }, "hold fire", subjectRef);
			commandObjects.Add("HOLDFIRE", holdFire);
			commandSet.Add(holdFire.Item);

			// Fire (3)
			Command fire = new Command("FIRE", new string[] { "fire", "take the shot", "send it" }, "fire", subjectRef);
			commandObjects.Add("FIRE", fire);
			commandSet.Add(fire.Item);

			// Engage (4)
			Command engage = new Command("ENGAGE", new string[] { "engage", "move to engage" }, "engage", subjectRef);
			commandObjects.Add("ENGAGE", engage);
			commandSet.Add(engage.Item);

			// Engage at will (5)
			Command engageAtWill = new Command("ENGAGEATWILL", new string[] { "engage at will" }, "engage at will", subjectRef);
			commandObjects.Add("ENGAGEATWILL", engageAtWill);
			commandSet.Add(engageAtWill.Item);

			// Disengage (6)
			Command disengage = new Command("DISENGAGE", new string[] { "disengage" }, "disengage", subjectRef);
			commandObjects.Add("DISENGAGE", disengage);
			commandSet.Add(disengage.Item);

			// Suppressive fire (9)
			Command suppressiveFire = new Command("SUPRESS", new string[] { "supressive fire", "supress" }, "suppressive fire", subjectRef);
			commandObjects.Add("SUPRESS", suppressiveFire);
			commandSet.Add(suppressiveFire.Item);

			#endregion

			#region Combat Mode (7)

			// Stealth (1)
			Command stealth = new Command("STEALTH", new string[] { "stealth", "go quiet", "go silent" }, "stealth", subjectRef);
			commandObjects.Add("STEALTH", stealth);
			commandSet.Add(stealth.Item);

			// Combat (2)
			Command combat = new Command("COMBAT", new string[] { "combat", "danger" }, "combat", subjectRef);
			commandObjects.Add("COMBAT", combat);
			commandSet.Add(combat.Item);

			// Aware (3)
			Command aware = new Command("AWARE", new string[] { "aware", "alert", "stay sharp", "stay frosty", "stay alert" }, "aware", subjectRef);
			commandObjects.Add("AWARE", aware);
			commandSet.Add(aware.Item);

			// Relax (4)
			Command relax = new Command("RELAX", new string[] { "relax", "safe", "at ease" }, "relax", subjectRef);
			commandObjects.Add("RELAX", relax);
			commandSet.Add(relax.Item);

			// Stand up (6)
			Command standUp = new Command("STANDUP", new string[] { "stand up", "get up", "stand" }, "stand up", subjectRef);
			commandObjects.Add("STANDUP", standUp);
			commandSet.Add(standUp.Item);

			// Stay crouched (7)
			Command crouch = new Command("CROUCH", new string[] { "crouch", "get low", "stay low" }, "crouch", subjectRef);
			commandObjects.Add("CROUCH", crouch);
			commandSet.Add(crouch.Item);

			// Go prone (8)
			Command prone = new Command("PRONE", new string[] { "prone", "get down", "go prone", "down", "hit the dirt" }, "prone", subjectRef);
			commandObjects.Add("PRONE", prone);
			commandSet.Add(prone.Item);

			// Copy my stance (9)
			Command copyStance = new Command("COPYSTANCE", new string[] { "copy my stance", "default stance" }, "copy stance", subjectRef);
			commandObjects.Add("COPYSTANCE", copyStance);
			commandSet.Add(copyStance.Item);

			#endregion

			#region Formation (8)

			// Column (1)
			Command column = new Command("COLUMN", new string[] { "formation column", "form column" }, "column", subjectRef);
			commandObjects.Add("COLUMN", column);
			commandSet.Add(column.Item);

			// Staggered column (2)
			Command staggeredColumn = new Command("STAGGEREDCOLUMN", new string[] { "formation staggered column", "form staggered column" }, "staggered column", subjectRef);
			commandObjects.Add("STAGGEREDCOLUMN", staggeredColumn);
			commandSet.Add(staggeredColumn.Item);

			// Wedge (3)
			Command wedge = new Command("WEDGE", new string[] { "formation wedge", "form wedge" }, "wedge", subjectRef);
			commandObjects.Add("WEDGE", wedge);
			commandSet.Add(wedge.Item);

			// Echelon Left (4)
			Command echelonLeft = new Command("ECHELONLEFT", new string[] { "formation echelon left", "form echelon left" }, "echelon left", subjectRef);
			commandObjects.Add("ECHELONLEFT", echelonLeft);
			commandSet.Add(echelonLeft.Item);

			// Echelon Right (5)
			Command echelonRight = new Command("ECHELONRIGHT", new string[] { "formation echelon right", "form echelon right" }, "echelon right", subjectRef);
			commandObjects.Add("ECHELONRIGHT", echelonRight);
			commandSet.Add(echelonRight.Item);

			// Vee (6)
			Command vee = new Command("VEE", new string[] { "formation vee", "form vee" }, "vee", subjectRef);
			commandObjects.Add("VEE", vee);
			commandSet.Add(vee.Item);

			// Line (7)
			Command line = new Command("LINE", new string[] { "formation line", "form line" }, "line", subjectRef);
			commandObjects.Add("LINE", line);
			commandSet.Add(line.Item);

			// File (8)
			Command file = new Command("FILE", new string[] { "formation file", "form file" }, "file", subjectRef);
			commandObjects.Add("FILE", file);
			commandSet.Add(file.Item);

			// Diamond (9)
			Command diamond = new Command("DIAMOND", new string[] { "formation diamond", "form diamond" }, "diamond", subjectRef);
			commandObjects.Add("DIAMOND", diamond);
			commandSet.Add(diamond.Item);

			#endregion

			#endregion

			SrgsRule commands = new SrgsRule("commands");
			commands.Add(commandSet);

			document.Rules.Add(commands);
			document.Root = commands;

			return document;
		}

		#region Send Instructions
		public static void SendInstruction(SemanticValue semantics)
		{
			List<string> subjects = ParseSemanticList("subject", semantics);

			string command = ParseSemantic("command", semantics);

			try
			{
				if (subjects != null)
				{
					// Send selection instruction
					foreach (string subject in subjects)
					{
						Logging.Message("SendInstruction", subject);

						if (subjectObject.InstructionList.ContainsKey(subject))
						{
							Core.outQueue.Enqueue("select;" + subjectObject.InstructionList[subject]);
						}
					}
				}

				if (command != null && command != "")
				{
					Logging.Message("SendInstruction", command);

					Core.outQueue.Enqueue("do;" + commandObjects[command].InstructionList[command]);

					// TODO: Queue target object selection
				}
			}
			catch (Exception ex)
			{
				Logging.Error("SendInstruction", "Error: " + ex.Message);
			}
		}

		private static string ParseSemantic(string key, SemanticValue semantic)
		{
			if (semantic.ContainsKey(key))
			{
				int index = semantic[key].Value.ToString().IndexOf("[object Object]");
				string result = (index < 0)
					? semantic[key].Value.ToString()
					: semantic[key].Value.ToString().Remove(index, "[object Object]".Length);

				string[] resultPieces = result.Split('{', '}', ' ');

				foreach (string piece in resultPieces)
				{
					if (piece != "")
					{
						return piece;
					}
				}
			}

			return null;
		}

		private static List<string> ParseSemanticList(string key, SemanticValue semantic)
		{
			if (semantic.ContainsKey(key))
			{
				List<string> list = new List<string>();

				int index = semantic[key].Value.ToString().IndexOf("[object Object]");
				string result = (index < 0)
					? semantic[key].Value.ToString()
					: semantic[key].Value.ToString().Remove(index, "[object Object]".Length);

				string[] resultPieces = result.Split('{', '}', ' ');

				foreach (string piece in resultPieces)
				{
					if (piece != "")
					{
						list.Add(piece);
					}
				}

				return list;
			}

			return null;
		}
		#endregion
	}
}
