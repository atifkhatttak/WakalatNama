using Business.Helpers.Attributes;
using Data.DomainModels;
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
        [Message("None mean no Leagal status assigned")]
        [Description("None mean no Leagal status assigned")]
        None = -1,
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
    public enum ERejection
    {
        None=0,
        [Description("Other")]
        Other=1,
        [Description("Incomplete Documents")]
        IncompleteDocuments =2,
        [Description("Payment Incomplete")]
        PaymentIncomplete =3
    }
    public enum ETransactionType
    {
        [Description("None")]
        None = 0,
        [Description("Payment Sended")]
        Send=1,
        [Description("Payment Recieved")]
        Recieve = 1
    }
    public enum EPaymentMethod
    {
        [Description("None")]
        None = 0,
        [Description("Cash Payment")]
        Cash = 1
    }
    public enum EPaymentStatus
    {
        [Description("None")]
        None = 0,
        [Description("Payment Completed")]
        Completed = 1,
        [Description("Payment Pending")]
        Pending=2,
        [Description("Payment Failed")]
        Failed=3,
        [Description("Payment Refunded")]
        Refunded=4,
        [Description("Payment Cancelled")]
        Cancelled=5
    }
}