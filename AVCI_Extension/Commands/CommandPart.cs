using System.Collections.Generic;
using System.Speech.Recognition.SrgsGrammar;

namespace AVCI_Extension
{
	abstract class CommandPart
	{
		/// <summary>
		/// List of rules to check against
		/// </summary>
		public List<SrgsRule> RuleList { get; protected set; }

		public Dictionary<string, string> InstructionList { get; protected set; }

		/// <summary>
		/// The root rule for the command
		/// </summary>
		public SrgsRule RootRule { get; protected set; }

		/// <summary>
		/// Gets a new grammar item with any number of alternative terminology and an associated semantic tag
		/// </summary>
		/// <param name="alternates"></param>Array of strings that represent alternative words for the same command
		/// <param name="semantic"></param>Associated semantic tag
		/// <returns></returns>
		protected static SrgsItem GetNewNode(string [] alternates, string semantic)
		{
			return new SrgsItem(new SrgsOneOf(alternates), new SrgsSemanticInterpretationTag("out += \"{" + semantic + "\";"));
		}
	}
}
