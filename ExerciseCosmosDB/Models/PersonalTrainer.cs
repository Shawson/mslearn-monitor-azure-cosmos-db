using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace mslearn_monitor_azure_cosmos_db.Models
{
    public class SpecialistArea
    {
        public int Id { get; set; }
        public bool IsHighlight { get; set; }
    }

    public class Profile
    {
        public string Language { get; set; }
        public string Slug { get; set; }
    }

    public class Identifiers
    {
        public string ExerpId { get; set; }
        public string CalendlyId { get; set; }
    }

    public class ContactDetails
    {
        public string TelephoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
    }

    public class PersonalTrainer
    {
        [JsonProperty(PropertyName = "id")]
        public string PersonalTrainerId { get; set; }
        public string FullName { get; set; }
        public string Locale { get; set; }
        public string Brand { get; set; }
        public string Bio { get; set; }
        public string ProfileImageUrl { get; set; }
        public List<SpecialistArea> SpecialistAreas { get; set; }
        public List<string> Qualifications { get; set; }
        public List<int> AssociatedGyms { get; set; }
        public List<Profile> Profiles { get; set; }
        public Identifiers Identifiers { get; set; }
        public ContactDetails ContactDetails { get; set; }


        static PersonalTrainer[] PersonalTrainers;

        static int NextPersonalTrainer = -1;

        public static void Allocate(int num)
        {
            PersonalTrainers = new PersonalTrainer[num];
            for (int i = 0; i < num; i++)
            {
                PersonalTrainers[i] = PersonalTrainer.Create();
            }
        }

        public static PersonalTrainer Create()
        {
            Bogus.Faker<PersonalTrainer> fakeGenerator = new Bogus.Faker<PersonalTrainer>().Rules(
            (faker, document) =>
            {
                var firstName = faker.Name.FirstName();
                var lastName = faker.Name.LastName();
                var brands = new List<string> { "PGUK", "PGSA", "PFUS" };
                var locales = new List<string> { "en-GB", "ar-SA" };

                document.PersonalTrainerId = faker.Random.Guid().ToString();
                document.FullName = $"{firstName} {lastName}";
                document.Bio = faker.Lorem.Paragraph();
                document.ProfileImageUrl = faker.Internet.Url();
                document.Locale = locales[faker.Random.Int(0, 1)];
                document.Brand = brands[faker.Random.Int(0, 2)];

                document.Identifiers = new Identifiers
                {
                    CalendlyId = faker.Random.AlphaNumeric(10),
                    ExerpId = faker.Random.AlphaNumeric(10)
                };

                document.ContactDetails = new ContactDetails
                {
                    EmailAddress = faker.Internet.Email(firstName, lastName),
                    TelephoneNumber = faker.Phone.PhoneNumber("(#####) ### ###")
                };

                document.Qualifications = Enumerable.Range(1, faker.Random.Int(1, 9))
                      .Select(_ => faker.Lorem.Word())
                      .ToList();

                document.AssociatedGyms = Enumerable.Range(1, faker.Random.Int(1, 3))
                      .Select(_ => faker.Random.Int(1, 3))
                      .ToList();

                document.SpecialistAreas = Enumerable.Range(1, faker.Random.Int(1, 9))
                      .Select(i => new SpecialistArea
                      {
                          Id = i,
                          IsHighlight = faker.Random.Bool()
                      })
                      .ToList();

                document.Profiles = Enumerable.Range(1, faker.Random.Int(1, 3))
                      .Select(_ => new Profile
                      {
                          Language = faker.Random.RandomLocale(),
                          Slug = faker.Lorem.Slug(2)
                      })
                      .ToList();
            });

            return fakeGenerator.Generate();
        }

        public static PersonalTrainer Next()
        {
            int index = Interlocked.Increment(ref NextPersonalTrainer);

            if (index >= PersonalTrainers.Length)
            {
                throw new Exception("PersonalTrainers allocation exceeded");
            }

            return PersonalTrainers[index];
        }
    }
}
