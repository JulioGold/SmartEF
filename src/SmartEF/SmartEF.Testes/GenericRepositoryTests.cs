using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Shouldly;
using SmartEF.Testes.Database.Entities;
using SmartEF.Testes.Fixtures;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace SmartEF.Testes
{
    [Collection(nameof(TestsDbContext))]
    public sealed class GenericRepositoryTests : IAsyncLifetime
    {
        private readonly SqlDbContextFactory _dbContextFactory;
        private TestsDbContext _context;

        public GenericRepositoryTests(SqlDbContextFactory sqlDbContextFactory)
        {
            _dbContextFactory = sqlDbContextFactory;
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_Add_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Úrsula", DateTime.UtcNow, EPersonGender.Other, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            person.Id.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_AddAsync_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Tamara", DateTime.UtcNow, EPersonGender.Other, null);

            await personRepository.AddAsync(person);

            await _context.SaveChangesAsync();

            person.Id.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_GetById_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Suelen", DateTime.UtcNow, EPersonGender.Other, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            var foundPerson = personRepository.GetById(person.Id);

            foundPerson.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_GetByIdAsync_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Regina", DateTime.UtcNow, EPersonGender.Other, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            var foundPerson = await personRepository.GetByIdAsync(person.Id);

            foundPerson.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_GetAllAsync_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Queila", DateTime.UtcNow, EPersonGender.Other, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            var persons = await personRepository.GetAllAsync();

            persons.Count.ShouldBeGreaterThan(0);
            persons.FirstOrDefault(f => f.Id == person.Id).ShouldNotBeNull();
        }

        [Fact]
        public void Dado_RepositorioGenerico_GetAll_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var result = personRepository.GetAll();

            result.Any().ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_Update_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Juliana", DateTime.UtcNow, EPersonGender.Female, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            string newName = "Juliana Silva";

            person.UpdateName(newName);

            personRepository.Update(person);

            await _context.SaveChangesAsync();

            var foundPerson = personRepository.GetById(person.Id);

            foundPerson.Name.ShouldBe(newName);
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_List_Filter_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            Expression<Func<Person, bool>> filter = (person) => person.Name.StartsWith("A") && person.Gender == EPersonGender.Female;
            var result = await personRepository.List(filter, null, null, true).ToListAsync();

            result.Count.ShouldBe(3);
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_List_OrderBy_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            Expression<Func<Person, bool>> filter = (person) => person.Gender == EPersonGender.Female || person.Gender == EPersonGender.Male;
            Func<IQueryable<Person>, IOrderedQueryable<Person>> orderBy = (person) => person.OrderByDescending(o => o.Name);
            var result = await personRepository.List(filter, orderBy, null, true).ToListAsync();

            result.FirstOrDefault().Name.ShouldBe("Julio");
            result.LastOrDefault().Name.ShouldBe("Alana");
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_List_Include_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            Func<IQueryable<Person>, IIncludableQueryable<Person, object>> include = (person) => person.Include(i => i.Contacts);
            var result = await personRepository.List(null, null, include, true).ToListAsync();

            var person1 = result.FirstOrDefault(f => f.Name == "Julio");
            var person2 = result.FirstOrDefault(f => f.Name == "Diana");
            var person3 = result.FirstOrDefault(f => f.Name == "Bruna");

            person1.ShouldNotBeNull();
            person2.ShouldNotBeNull();
            person3.ShouldNotBeNull();

            person1.Contacts.Any().ShouldBeFalse();
            person2.Contacts.Any().ShouldBeTrue();
            person3.Contacts.Any().ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_DeleteByEntity_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Zoe", DateTime.UtcNow, EPersonGender.Other, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            personRepository.Delete(person);
            
            await _context.SaveChangesAsync();

            var foundPerson = personRepository.GetById(person.Id);

            foundPerson.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_DeleteById_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Yasmim", DateTime.UtcNow, EPersonGender.Other, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            personRepository.Delete(person.Id);

            await _context.SaveChangesAsync();

            var foundPerson = personRepository.GetById(person.Id);

            foundPerson.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_DeleteAsyncById_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Xuxa", DateTime.UtcNow, EPersonGender.Other, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            await personRepository.DeleteAsync(person.Id);

            await _context.SaveChangesAsync();

            var foundPerson = personRepository.GetById(person.Id);

            foundPerson.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_DeleteVirtualById_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Walesca", DateTime.UtcNow, EPersonGender.Other, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            personRepository.DeleteVirtual<Person>(person.Id);

            await _context.SaveChangesAsync();

            var foundPerson = personRepository.GetById(person.Id);

            foundPerson.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_RepositorioGenerico_DeleteVirtualAsyncById_RetornarSucesso()
        {
            var personRepository = new PersonRepository(_context);

            var person = Person.Create("Vilma", DateTime.UtcNow, EPersonGender.Other, null);

            personRepository.Add(person);

            await _context.SaveChangesAsync();

            await personRepository.DeleteVirtualAsync<Person>(person.Id);

            await _context.SaveChangesAsync();

            var foundPerson = personRepository.GetById(person.Id);

            foundPerson.ShouldBeNull();
        }

        public async Task InitializeAsync()
        {
            try
            {
                _context = await _dbContextFactory.CriarAsync() as TestsDbContext;

                await CreatePersonAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"MESSAGE: {ex.Message} --- INNEREXCEPTION.MESSAGE: {ex.InnerException?.Message} --- STACKTRACE: {ex?.StackTrace}");
            }
        }

        private async Task CreatePersonAsync()
        {
            var contact1 = Contact.Create("diana@validacao.com.br", EContactType.Email);
            var contact2 = Contact.Create("(51) 999999999", EContactType.Phone);
            var contact3 = Contact.Create("(54) 888888888", EContactType.Phone);
            var contact4 = Contact.Create("bruna@validacao.com.br", EContactType.Email);

            var person1 = Person.Create("Julio", new DateTime(1990, 01, 02), EPersonGender.Male, null);
            var person2 = Person.Create("Cesar", new DateTime(1990, 01, 02), EPersonGender.Male, null);

            var person3 = Person.Create("Alana", DateTime.UtcNow, EPersonGender.Female, null);
            var person4 = Person.Create("Alice", DateTime.UtcNow, EPersonGender.Female, null);
            var person5 = Person.Create("Aline", DateTime.UtcNow, EPersonGender.Female, null);
            var person6 = Person.Create("Bianca", DateTime.UtcNow, EPersonGender.Female, null);
            var person7 = Person.Create("Bruna", DateTime.UtcNow, EPersonGender.Female, [contact4]);
            var person8 = Person.Create("Carla", DateTime.UtcNow, EPersonGender.Female, null);
            var person9 = Person.Create("Diana", DateTime.UtcNow, EPersonGender.Female, [contact1, contact2, contact3]);
            var person10 = Person.Create("Erica", DateTime.UtcNow, EPersonGender.Female, null);
            var person11 = Person.Create("Fabiana", DateTime.UtcNow, EPersonGender.Female, null);
            var person12 = Person.Create("Gabriela", DateTime.UtcNow, EPersonGender.Female, null);

            var pessoas = await _context.Set<Person>().ToListAsync();

            if (!pessoas.Any())
            {
                await _context.Set<Person>().AddAsync(person1);
                await _context.Set<Person>().AddAsync(person2);
                await _context.Set<Person>().AddRangeAsync([person3, person4, person5, person6, person7, person8, person9, person10, person11, person12]);
            }

            await _context.SaveChangesAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }

    public sealed class PersonRepository : GenericRepository<Person>
    {
        public PersonRepository(DbContext context) : base(context)
        {
        }
    }
}
