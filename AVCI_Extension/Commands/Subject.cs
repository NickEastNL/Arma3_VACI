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
			//List<SrgsItem> itemList = new List<SrgsItem>();
			//for (int i = 0; i < 12; i++)
			//{
			//	SrgsItem newItem = GetNewNode(new string[] { GetTextFromInt(i) }, GetTextFromInt(i).ToUpper());
			//	itemList.Add(newItem);
			//}

			//SrgsOneOf squadNumbersChoice = new SrgsOneOf(itemList.ToArray());

			SrgsItem one	= GetNewNode(new string[] { "one" }, "ONE");
			SrgsItem two	= GetNewNode(new string[] { "two" }, "TWO");
			SrgsItem three	= GetNewNode(new string[] { "three" }, "THREE");
			SrgsItem four	= GetNewNode(new string[] { "four" }, "FOUR");
			SrgsItem five	= GetNewNode(new string[] { "five" }, "FIVE");
			SrgsItem six	= GetNewNode(new string[] { "six" }, "SIX");
			SrgsItem seven	= GetNewNode(new string[] { "seven" }, "SEVEN");
			SrgsItem eight	= GetNewNode(new string[] { "eight" }, "EIGHT");
			SrgsItem nine	= GetNewNode(new string[] { "nine" }, "NINE");
			SrgsItem ten	= GetNewNode(new string[] { "ten" }, "TEN");
			SrgsItem eleven	= GetNewNode(new string[] { "eleven" }, "ELEVEN");
			SrgsItem twelve	= GetNewNode(new string[] { "twelve" }, "TWELVE");

			SrgsOneOf squadNumbersChoice = new SrgsOneOf(one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve);
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

			// COLLECTING ALL COMMANDS
			SrgsOneOf subjectChoice = new SrgsOneOf();
			subjectChoice.Add(new SrgsItem(new SrgsRuleRef(squadMembers)));
			subjectChoice.Add(new SrgsItem(new SrgsRuleRef(all)));

			SrgsRule subject = new SrgsRule("subject");
			subject.Add(subjectChoice);

			RootRule = subject;
			RuleList.Add(subject);
		}

		private void GenerateInstructionList()
		{
			InstructionList = new Dictionary<string, string>();

			// INDIVIDUAL SELECTION
			InstructionList.Add("ONE", "one");
			InstructionList.Add("TWO", "two");
			InstructionList.Add("THREE", "three");
			InstructionList.Add("FOUR", "four");
			InstructionList.Add("FIVE", "five");
			InstructionList.Add("SIX", "six");
			InstructionList.Add("SEVEN", "seven");
			InstructionList.Add("EIGHT", "eight");
			InstructionList.Add("NINE", "nine");
			InstructionList.Add("TEN", "ten");
			InstructionList.Add("ELEVEN", "eleven");
			InstructionList.Add("TWELVE", "twelve");

			// ALL
			InstructionList.Add("ALL", "all");
		}

		private string GetTextFromInt(int i)
		{
			string number;

			// Choose numerical text based on loop integer
			switch (i)
			{
				case 1:
					number = "one";
					break;
				case 2:
					number = "two";
					break;
				case 3:
					number = "three";
					break;
				case 4:
					number = "four";
					break;
				case 5:
					number = "five";
					break;
				case 6:
					number = "six";
					break;
				case 7:
					number = "seven";
					break;
				case 8:
					number = "eight";
					break;
				case 9:
					number = "nine";
					break;
				case 10:
					number = "ten";
					break;
				case 11:
					number = "eleven";
					break;
				case 12:
					number = "twelve";
					break;
				default:
					number = "zero";
					break;
			}

			return number;
		}
	}
}
