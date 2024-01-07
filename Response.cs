namespace FetchData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Response
    {
        public bool IsActive { get; set; }
        public DateTime StoppedCaseDate { get; set; }
        public double BenefitAmount { get; set; }

        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string EID { get; set; }
        public string Relation { get; set; }
        public string DateOfBirth { get; set; }
        public string DewaAcc { get; set; }
        public string age { get; set; }
        public List<HouseHoldMember> houseHoldMembers { get; set; }
        public Response()
        {
            IsActive = true;
            StoppedCaseDate = DateTime.MinValue;
            houseHoldMembers = new List<HouseHoldMember>();
        }
    }
    public class HouseHoldMember
    {
        public string FullName { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string IDNcase { get; set; }
        public string DOBcase { get; set; }
        public HouseHoldMember() { }
    }
}
