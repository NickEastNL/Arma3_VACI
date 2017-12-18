using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition.SrgsGrammar;
using System.Text;
using System.Threading.Tasks;

namespace AVCI_Extension
{
	class Command : CommandPart
	{
		public SrgsItem Item { get; private set; }

		public Command(string semantic, string[] alternates, string instruction)
		{
			GenerateRuleList(semantic, alternates);
			GenerateInstruction(semantic, instruction);
		}

		public Command(string semantic, string[] alternates, string instruction, SrgsRuleRef subjectRef)
		{
			GenerateRuleList(semantic, alternates, subjectRef);
			GenerateInstruction(semantic, instruction);
		}

		private void GenerateRuleList(string semantic, string[] alternates, SrgsRuleRef subjectref = null)
		{
			SrgsOneOf commandAlternates = new SrgsOneOf(alternates);
			SrgsItem command = new SrgsItem();

			if (subjectref != null)
			{
				command.Add(new SrgsItem(subjectref));
				command.Add(new SrgsSemanticInterpretationTag("out.subject=rules.subject;"));
			}

			command.Add(commandAlternates);
			command.Add(new SrgsSemanticInterpretationTag("out.command=\"" + semantic + "\";"));
			/*
			if (directObject != null)
			{
				command.Add(directObject.RuleRef);
				command.Add(new SrgsSemanticInterpretationTag("out.directObject=rules." + directObject.RuleName + ";"));
			}
			*/

			Item = command;

			RuleList = new List<SrgsRule>();
			SrgsRule rule = new SrgsRule(semantic);
			rule.Add(command);

			RuleList.Add(rule);
			RootRule = rule;
		}

		private void GenerateInstruction(string semantic, string instruction)
		{
			InstructionList = new Dictionary<string, string>();
			InstructionList.Add(semantic, instruction);
		}
	}
}
