using System.Collections.Generic;
using System.Speech.Recognition.SrgsGrammar;

namespace AVCI_Extension
{
	class Subject : CommandPart
	{
		public Subject()
		{
			GenerateSubjectList();
			GenerateInstructionList();
		}

		private void GenerateSubjectList()
		{
			RuleList = new List<SrgsRule>();

			// INDIVIDUAL
			List<SrgsItem> numbersList = new List<SrgsItem>();

			for (int num = 1; num < 25; num++)
			{
				numbersList.Add(GetNewNode(new string[] { num.ToString() }, num.ToString().ToUpper()));
			}

			SrgsOneOf squadNumbersChoice = new SrgsOneOf(numbersList.ToArray());
			SrgsItem squadNumbersConcatChoice = new SrgsItem(squadNumbersChoice, new SrgsItem(0, 1, "and"));
			squadNumbersConcatChoice.SetRepeat(1, 12);

			SrgsRule squadNumbers = new SrgsRule("squadNumbers");
			squadNumbers.Add(squadNumbersConcatChoice);
			RuleList.Add(squadNumbers);

			SrgsRule squadMembers = new SrgsRule("squadSelections");
			squadMembers.Add(new SrgsRuleRef(squadNumbers));
			squadMembers.Add(new SrgsSemanticInterpretationTag("out=rules.squadNumbers;"));
			RuleList.Add(squadMembers);

			// ALL
			SrgsItem allItems = GetNewNode(new string[] { "all", "everyone", "team", "squad", "group" }, "ALL");
			SrgsRule all = new SrgsRule("all");
			all.Add(allItems);
			RuleList.Add(all);

			// DESELECT ALL (DISREGARD)
			SrgsItem disregardItems = GetNewNode(new string[] { "disregard" }, "DISREGARD");
			SrgsRule disregard = new SrgsRule("disregard");
			disregard.Add(disregardItems);
			RuleList.Add(disregard);

			// COLLECTING ALL COMMANDS
			SrgsOneOf subjectChoice = new SrgsOneOf();
			subjectChoice.Add(new SrgsItem(new SrgsRuleRef(squadMembers)));
			subjectChoice.Add(new SrgsItem(new SrgsRuleRef(all)));
			subjectChoice.Add(new SrgsItem(new SrgsRuleRef(disregard)));

			SrgsRule subject = new SrgsRule("subject");
			subject.Add(subjectChoice);

			RootRule = subject;
			RuleList.Add(subject);
		}

		private void GenerateInstructionList()
		{
			InstructionList = new Dictionary<string, string>();

			for (int num = 1; num < 25; num++)
			{
				InstructionList.Add(num.ToString().ToUpper(), num.ToString());
			}

			InstructionList.Add("ALL", "all");
			InstructionList.Add("DISREGARD", "disregard");
		}
	}
}
