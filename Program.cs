using Dapper;
using HomeworkDapper.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace HomeworkDapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;";

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            InitDatabase(connection);

            while (true)
            {
                Console.WriteLine(
                "\n----- Dog Shelter Menu -----\n" +
                "1. View all dogs\n" +
                "2. View dogs in shelter (not adopted)\n" +
                "3. View adopted dogs\n" +
                "4. Find dog by name\n" +
                "5. Find dog by ID\n" +
                "6. Find dog by breed\n" +
                "7. Update dog by ID\n" +
                "8. Adopt a dog\n" +
                "9. Bulk insert 20 dogs" +
                "0. Exit\n" +
                "Choose an option: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ReadAllDogs(connection);
                        break;
                    case "2":
                        ReadDogsInShelter(connection);
                        break;
                    case "3":
                        ReadAdoptedDogs(connection);
                        break;
                    case "4":
                        Console.Write("Enter dog name: ");
                        string name = Console.ReadLine();
                        FindDogByName(connection, name);
                        break;
                    case "5":
                        Console.Write("Enter dog ID: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            FindDogById(connection, id);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;
                    case "6":
                        Console.Write("Enter dog breed: ");
                        string breed = Console.ReadLine();
                        FindDogByBreed(connection, breed);
                        break;
                    case "7":
                        Console.Write("Enter dog ID to update: ");
                        if (int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            Console.Write("New name: ");
                            string newName = Console.ReadLine();
                            Console.Write("New age: ");
                            int newAge = int.Parse(Console.ReadLine());
                            Console.Write("New breed: ");
                            string newBreed = Console.ReadLine();
                            Console.Write("Is adopted? (true/false): ");
                            bool isAdopted = bool.Parse(Console.ReadLine());

                            UpdateDogById(connection, updateId, newName, newAge, newBreed, isAdopted);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;
                    case "8":
                        ShowAvailableDogs(connection);
                        AdoptDog(connection);
                        break;
                    case "9":
                        BulkInsertDogs(connection);
                        break;
                    case "0":
                        Console.WriteLine("Exiting program...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private static void InitDatabase(SqlConnection connection)
        {
            connection.Execute(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Dogs' AND xtype='U')
                BEGIN
                    CREATE TABLE Dogs (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(MAX) NOT NULL,
                        Age INT NOT NULL,
                        DogBreed NVARCHAR(MAX) NOT NULL,
                        IsAdopted BIT NOT NULL
                    );
                END
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Adopters' AND xtype='U')
                    BEGIN
                        CREATE TABLE Adopters (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Name NVARCHAR(MAX) NOT NULL,
                            PhoneNumber NVARCHAR(20) NOT NULL
                        );
                    END
            ");
        }
        //Bulk DOGS

        private static void BulkInsertDogs(SqlConnection connection)
        {
            var dogs = new List<Dog>();

            for (int i = 1; i <= 20; i++)
            {
                dogs.Add(new Dog
                {
                    Name = $"Dog_{i}",
                    Age = 1 + i % 10,
                    DogBreed = $"Breed_{i % 5}",
                    IsAdopted = false
                });
            }

            var sql = @"
                INSERT INTO Dogs (Name, Age, DogBreed, IsAdopted)
                VALUES (@Name, @Age, @DogBreed, @IsAdopted);";

            using var transaction = connection.BeginTransaction();
            connection.Execute(sql, dogs, transaction);
            transaction.Commit();

            Console.WriteLine("20 dogs inserted successfully.");
        }




        //Adopt Dog methods

        private static void ShowAvailableDogs(SqlConnection connection)
        {
            var dogs = connection.Query("SELECT * FROM Dogs WHERE IsAdopted = 0;");
            foreach (var dog in dogs)
            {
                Console.WriteLine(dog.ToString());
            }
        }

        private static void AdoptDog(SqlConnection connection)
        {
            Console.Write("Enter Adopter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int adopterId))
            {
                Console.WriteLine("Invalid adopter ID.");
                return;
            }

            Console.Write("Enter Dog ID to adopt: ");
            if (!int.TryParse(Console.ReadLine(), out int dogId))
            {
                Console.WriteLine("Invalid dog ID.");
                return;
            }

            var updated = connection.Execute(@"
        UPDATE Dogs
        SET IsAdopted = 1, AdopterId = @AdopterId
        WHERE Id = @DogId;",
                new { AdopterId = adopterId, DogId = dogId });

            Console.WriteLine(updated > 0 ? "Dog adopted successfully!" : "Adoption failed. Check IDs.");
        }



        //Read All Dogs
        private static void ReadAllDogs(SqlConnection connection)
        {
            var dogs = connection.Query("SELECT * FROM Dogs;");
            foreach (var dog in dogs)
            {
                Console.WriteLine(dog.ToString());
            }
        }

        //Filter
        private static void ReadDogsInShelter(SqlConnection connection)
        {
            var dogs = connection.Query("SELECT * FROM Dogs WHERE IsAdopted = 0;");
            foreach (var dog in dogs)
            {
                Console.WriteLine(dog.ToString());
            }
        }

        private static void ReadAdoptedDogs(SqlConnection connection)
        {
            var dogs = connection.Query("SELECT * FROM Dogs WHERE IsAdopted = 1;");
            foreach (var dog in dogs)
            {
                Console.WriteLine(dog.ToString());
            }
        }

        //Finding with Id, Name, Breed

        private static void FindDogByName(SqlConnection connection, string name)
        {
            var dogs = connection.Query("SELECT * FROM Dogs WHERE Name = @Name;", new { Name = name });
            foreach (var dog in dogs)
            {
                Console.WriteLine(dog.ToString());
            }
        }

        private static void FindDogById(SqlConnection connection, int id)
        {
            var dog = connection.QueryFirstOrDefault("SELECT * FROM Dogs WHERE Id = @Id;", new { Id = id });
            if (dog != null)
            {
                Console.WriteLine(dog.ToString());
            }
            else
            {
                Console.WriteLine("There's no such dog.");
            }
        }

        private static void FindDogByBreed(SqlConnection connection, string breed)
        {
            var dogs = connection.Query("SELECT * FROM Dogs WHERE DogBreed = @Breed;", new { Breed = breed });
            foreach (var dog in dogs)
            {
                Console.WriteLine(dog.ToString());
            }
        }

        //Updating by ID

        private static void UpdateDogById(SqlConnection connection, int id, string newName, int newAge, string newBreed, bool isAdopted)
        {
            var affectedRows = connection.Execute(@"
                UPDATE Dogs
                SET Name = @Name,
                    Age = @Age,
                    DogBreed = @DogBreed,
                    IsAdopted = @IsAdopted
                WHERE Id = @Id;",
                new
                {
                    Name = newName,
                    Age = newAge,
                    DogBreed = newBreed,
                    IsAdopted = isAdopted,
                    Id = id
                });

            Console.WriteLine(affectedRows > 0 ? "Data is updated." : "There's no such dоg.");
        }

        
    }
}
