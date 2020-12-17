using System;

namespace asdfghjk
{
    class Program
    {
        static void Main(string[] args)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                uow.GetRepository<User>().Add(new User()
                {
                    Id = 3,
                    Name = "Name",
                    LastName = "LastName"
                });
                uow.SaveChanges();
            }
            Console.WriteLine("Hello World!");
        }
    }
}
