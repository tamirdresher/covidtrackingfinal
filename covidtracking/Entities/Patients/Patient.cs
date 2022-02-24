using System;
using System.ComponentModel.DataAnnotations;
using covidtracking.Dtos;

namespace covidtracking.Entities{
    public class Patient : IPatient{
            public string govtId { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public DateTime birthDate { get; set; }
            public string phoneNumber { get; set; }
            public string email { get; set; }
            public Address address  { get; set; }
            public int houseResidentsAmount { get; set; }
            public bool isCovidPositive { get; set; }
            public string? infectedByPatientID { get; set; }

        public Patient(CreatePatientDto createPatientDto){
            govtId = createPatientDto.govtId;
            firstName = createPatientDto.firstName;
            lastName = createPatientDto.lastName;
            birthDate = createPatientDto.birthDate;
            phoneNumber = createPatientDto.phoneNumber;
            email = createPatientDto.email;
            address = createPatientDto.address;
            isCovidPositive = createPatientDto.isCovidPositive;
            houseResidentsAmount = createPatientDto.houseResidentsAmount;
            infectedByPatientID = null;
        }

        public Patient(PotentialToPatientDto potentialToPatientDto){
            govtId = potentialToPatientDto.govtId;
            firstName = potentialToPatientDto.firstName;
            lastName = potentialToPatientDto.lastName;
            birthDate = potentialToPatientDto.birthDate;
            phoneNumber = potentialToPatientDto.phoneNumber;
            email = potentialToPatientDto.email;
            address = potentialToPatientDto.address;
            isCovidPositive = potentialToPatientDto.isCovidPositive;
            houseResidentsAmount = potentialToPatientDto.houseResidentsAmount;
            infectedByPatientID = potentialToPatientDto.infectedByPatientID;
        }

        public Patient(string id, string fname, string lname, DateTime bdate, string phone,
                        string email, string _city, string _street, int _house, int _appartment,
                        bool issick, int residents, string infectedby){
            govtId = id;
            firstName = fname;
            lastName = lname;
            birthDate = bdate;
            phoneNumber = phone;
            this.email = email;
            address = new(){
                city = _city,
                street = _street,
                houseNumber = _house,
                appartmentNumber = _appartment
            };
            isCovidPositive = issick;
            houseResidentsAmount = residents;
            infectedByPatientID = infectedby;
         }

        public string GetKey(){
            return firstName+lastName+phoneNumber;
        }

        public string GetFirstName()
        {
            return firstName;
        }

        public string GetLastName()
        {
            return lastName;
        }

        public string GetPhoneNumber()
        {
            return phoneNumber;
        }

        public string GetId()
        {
            return govtId;
        }
    }
}