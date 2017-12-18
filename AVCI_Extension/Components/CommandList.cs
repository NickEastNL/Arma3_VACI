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

			#region Move
			// Move to pointer
			Command move = new Command("MOVE", new string[] { "move", "get over there", "move up" }, "move", subjectRef);
			commandObjects.Add("MOVE", move);
			commandSet.Add(move.Item);

			// Return to formation
			Command regroup = new Command("REGROUP", new string[] { "regroup", "return to formation", "fall back" }, "regroup", subjectRef);
			commandObjects.Add("REGROUP", regroup);
			commandSet.Add(regroup.Item);
			#endregion

			#endregion

			SrgsRule commands = new SrgsRule("commands");
			commands.Add(commandSet);

			document.Rules.Add(commands);
			document.Root = commands;

			return document;
		}

		public static void Execute(SemanticValue semantics)
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
						Logging.Message("ExecuteAsync", subject);

						if (subjectObject.InstructionList.ContainsKey(subject))
						{
							Core.outQueue.Enqueue("select;" + subjectObject.InstructionList[subject]);
						}
					}
				}

				if (command != null && command != "")
				{
					Logging.Message("ExecuteAsync", command);

					Core.outQueue.Enqueue("do;" + commandObjects[command].InstructionList[command]);

					// TODO: Queue target object selection
				}
			}
			catch (Exception ex)
			{
				Logging.Error("ExecuteAsync", "Error: " + ex.Message);
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
	}
}
