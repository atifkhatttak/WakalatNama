using Business.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Enums
{

	public enum ELeagalStatus
    {
        [Message("Case Institution/ Diary")]
        [Description("Case Institution/ Diary")]
        CaseInstitutionDiary = 1,
        [Message("Summon to defendant/s")]
        [Description("Summon to defendant/s")]
        SummonToDefendants = 2,
        [Message("Submission of Written Statement by Defendant/s")]
        [Description("Submission of Written Statement by Defendant/s")]
        SubmissionOfWrittenStatementByDefendants = 3,
        [Message("Framing of issues ")]
        [Description("Framing of issues ")]
        FramingOfIssues = 4,
        [Message("Evidence recorded by Plaintiff/s")]
        [Description("Evidence recorded by Plaintiff/s")]
        EvidenceRecordedByPlaintiffs = 5,
        [Message("Evidence Recorded by Defendant/s")]
        [Description("Evidence Recorded by Defendant/s")]
        EvidenceRecordedByDefendants = 6,
        [Message("Final Arguments")]
        [Description("Final Arguments")]
        FinalArguments = 7,
        [Message("Judgement & Decree reserved or announced")]
        [Description("Judgement & Decree reserved or announced")]
        JudgementAndDecreeReservedOrAnnounced = 8
    }
}
