namespace PassionProject.Migrations
{
    using PassionProject.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PassionProject.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "PassionProject.Models.ApplicationDbContext";
        }

        protected override void Seed(PassionProject.Models.ApplicationDbContext context)
        {
            // --- Clean existing data in the right order ---
            context.Appointments.RemoveRange(context.Appointments);

            var employeesWithServices = context.Employees.Include("Services").ToList();
            foreach (var emp in employeesWithServices)
            {
                emp.Services?.Clear();
            }
            context.SaveChanges();

            context.Employees.RemoveRange(context.Employees);
            context.Services.RemoveRange(context.Services);
            context.SaveChanges();

            // --- Seed Employees with IDs ---
            var employees = new List<Employee>
    {
        new Employee { EmployeeId = 1, Fname = "Ava", Lname = "Martin", DOJ = new DateTime(2020, 3, 15), Bio = "Senior stylist...", EmployeeHasPic = false },
        new Employee { EmployeeId = 2, Fname = "Liam", Lname = "Thompson", DOJ = new DateTime(2019, 7, 22), Bio = "Barber...", EmployeeHasPic = false },
        new Employee { EmployeeId = 3, Fname = "Sophia", Lname = "Hughes", DOJ = new DateTime(2021, 1, 10), Bio = "Coloring expert...", EmployeeHasPic = false },
        new Employee { EmployeeId = 4, Fname = "Noah", Lname = "White", DOJ = new DateTime(2018, 5, 5), Bio = "Wedding hair specialist...", EmployeeHasPic = false },
        new Employee { EmployeeId = 5, Fname = "Mia", Lname = "Green", DOJ = new DateTime(2022, 11, 3), Bio = "Junior stylist...", EmployeeHasPic = false },
        new Employee { EmployeeId = 6, Fname = "Elijah", Lname = "Patel", DOJ = new DateTime(2020, 8, 19), Bio = "Transformation stylist...", EmployeeHasPic = false },
        new Employee { EmployeeId = 7, Fname = "Isabella", Lname = "Scott", DOJ = new DateTime(2023, 2, 27), Bio = "Receptionist...", EmployeeHasPic = false },
        new Employee { EmployeeId = 8, Fname = "James", Lname = "Lewis", DOJ = new DateTime(2017, 9, 13), Bio = "Salon manager...", EmployeeHasPic = false },
        new Employee { EmployeeId = 9, Fname = "Charlotte", Lname = "Adams", DOJ = new DateTime(2021, 6, 8), Bio = "Haircare specialist...", EmployeeHasPic = false },
        new Employee { EmployeeId = 10, Fname = "Benjamin", Lname = "Ross", DOJ = new DateTime(2019, 10, 26), Bio = "Barber and stylist...", EmployeeHasPic = false }
    };
            context.Employees.AddOrUpdate(e => e.EmployeeId, employees.ToArray());

            // --- Seed Services with IDs ---
            var services = new List<Service>
    {
        new Service { ServiceId = 1, Name = "Haircut - Women", Duration = 45, Cost = 45.00m },
        new Service { ServiceId = 2, Name = "Haircut - Men", Duration = 30, Cost = 30.00m },
        new Service { ServiceId = 3, Name = "Hair Coloring - Full", Duration = 90, Cost = 120.00m },
        new Service { ServiceId = 4, Name = "Hair Coloring - Root Touch-up", Duration = 60, Cost = 75.00m },
        new Service { ServiceId = 5, Name = "Highlights", Duration = 90, Cost = 100.00m },
        new Service { ServiceId = 6, Name = "Balayage", Duration = 120, Cost = 150.00m },
        new Service { ServiceId = 7, Name = "Blow Dry & Styling", Duration = 40, Cost = 35.00m },
        new Service { ServiceId = 8, Name = "Hair Wash & Scalp Massage", Duration = 25, Cost = 20.00m },
        new Service { ServiceId = 9, Name = "Keratin Treatment", Duration = 120, Cost = 180.00m },
        new Service { ServiceId = 10, Name = "Beard Trim & Shape", Duration = 20, Cost = 20.00m }
    };
            context.Services.AddOrUpdate(s => s.ServiceId, services.ToArray());

            context.SaveChanges(); // Save all before linking

            // --- Link Services to Employees ---
            var employeeServiceMap = new Dictionary<int, int[]>
    {
        { 1, new[] {1, 3, 6} },
        { 2, new[] {2, 10} },
        { 3, new[] {3, 5, 6} },
        { 4, new[] {1, 4, 7} },
        { 5, new[] {1, 7} },
        { 6, new[] {3, 9} },
        { 7, new[] {8} },
        { 8, new[] {1, 2, 3, 4, 5} },
        { 9, new[] {8, 9} },
        { 10, new[] {2, 10} }
    };

            foreach (var kvp in employeeServiceMap)
            {
                var emp = context.Employees.Include("Services").FirstOrDefault(e => e.EmployeeId == kvp.Key);
                if (emp != null)
                {
                    emp.Services = new List<Service>();
                    foreach (var serviceId in kvp.Value)
                    {
                        var svc = context.Services.Find(serviceId);
                        if (svc != null)
                            emp.Services.Add(svc);
                    }
                }
            }
            context.SaveChanges();

            // --- Add one sample appointment per employee ---
            var appointmentId = 1;
            var tomorrow = DateTime.Today.AddDays(1);

            foreach (var emp in context.Employees.Include("Services").ToList())
            {
                var service = emp.Services.FirstOrDefault();
                if (service != null)
                {
                    context.Appointments.AddOrUpdate(new Appointment
                    {
                        AppointmentId = appointmentId++,
                        CustomerName = $"Customer {emp.Fname}",
                        CustomerEmail = $"{emp.Fname.ToLower()}@example.com",
                        EmployeeId = emp.EmployeeId,
                        ServiceId = service.ServiceId,
                        StartTime = tomorrow.AddHours(9),
                        EndTime = tomorrow.AddHours(9 + (service.Duration / 60.0))
                    });
                }
            }

            context.SaveChanges();

        }
    }
}
