namespace covidtracking.Entities
{
    public interface IPatient
    {
        public string GetKey();
        public string GetId();
        public string GetFirstName();
        public string GetLastName();
        public string GetPhoneNumber();
    }
}