using System.Diagnostics;
using System.Text;
using WebApplication6.Helpers;

namespace WebApplication6
{
    public static class DummyClass
    {
        public static Random Rnd = new Random();
        public static IEnumerable<Person> CreatePeople(int repeats)
        {
            using var activity = Telemetry.MyActivitySource.StartActivity("CeatePeople");
            activity?.SetTag("repeats", repeats);
            List<Person> people = new();
            for (int i = 0; i < repeats; i++)
            {
                var rndInt = (Rnd.Next(0, 100) + 1);
                people.Add(new Person()
                {
                    Id = i,
                    Name = $"Dummy McDummyson The {i + 1}",
                    IsFaulty = rndInt >= 5 && rndInt <= 10
                });
            }

            return people;
        }

        public static int DoImportantWork(IEnumerable<Person> people)
        {
            using var activity = Telemetry.MyActivitySource.StartActivity("DoImportantWork");
            activity?.SetTag("people", people.Count());
            int retVal = 0;
            try
            {
                foreach (var p in people)
                {
                    retVal += p.Name.Length;
                    if (p.IsFaulty)
                    {
                        throw new Exception("Person is bad");
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, "Something bad happened!");
            }
            return retVal;
        }

        public static string DisplayPeoples(IEnumerable<Person> people)
        {
            using var activity = Telemetry.MyActivitySource.StartActivity("DisplayPeoples");
            activity?.SetTag("people", people.Count());
            StringBuilder sb = new();
            foreach (var p in people)
            {
                sb.AppendLine($"{p.Id}: {p.Name} [{(p.IsFaulty ? "F" : "N")}]");
            }
            return sb.ToString();
        }
    }
}
