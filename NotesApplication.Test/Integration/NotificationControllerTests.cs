using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NotesApplication.Business;
using NotesApplication.Business.CreateNote;
using NotesApplication.Business.UpdateNote;
using NotesApplication.Core;
using NotesApplication.Core.Constants;
using System.Net;

namespace NotesApplication.Test.Integration;

public class NotificationControllerTests : IntegrationTestBase
{
    private const string ControllerBaseUrl = "api/Notification";

    public NotificationControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllNotes_WhenSuccessResponse_ShouldReturnCollectionOfNotes()
    {
        //Arange

        var dbContext = GetNotesDbContext();

        List<Note> notifications = [
       new Note()
       {
           Name = "Name1",
           Description = "Description1"
       }
       ];
        await dbContext.AddRangeAsync(notifications);
        await dbContext.SaveChangesAsync();
        //Act

        var responseMessage = await SendGetRequest(ControllerBaseUrl);

        var response = await ConvertTo<IEnumerable<NotificationResponse>>(responseMessage);
        //Assert

        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Should().NotBeEmpty();
        response.Should().BeEquivalentTo(notifications.Select(x => new NotificationResponse(x.Name, x.Description, x.Id)));
    }

    [Fact]
    public async Task GetNoteByID_WhenSuccessResponse_ShouldReturnNote()
    {
        //Arange

        var dbContext = GetNotesDbContext();

        List<Note> notifications = [
       new Note()
       {
           Name = "Name1",
           Description = "Description1"
       }
       ];

        await dbContext.AddRangeAsync(notifications);
        await dbContext.SaveChangesAsync();

        var id = Guid.Parse(notifications.First().Id.ToString());

        //var id = notifications.Select(x=>x.Id);
        // x => x.Id == request.Id
        //Act

        var responseMessage = await SendGetRequest(ControllerBaseUrl + $"/{id}");

        var response = await ConvertTo<NotificationResponse>(responseMessage);
        //Assert

        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Should().NotBeNull();
        response.Id.Should().Be(id);
        response.Name.Should().Be(notifications.First().Name);
        response.Description.Should().Be(notifications.First().Description);
    }

    [Fact]
    public async Task GetNoteByID_WhenSuccessResponse_ShouldReturnBadRequest()
    {
        //Arange

        Guid id = Guid.Empty;

        //Act

        var responseMessage = await SendGetRequest(ControllerBaseUrl + $"/{id}");

        //Assert
        (await responseMessage.Content.ReadAsStringAsync()).Should().Contain("Пустой Guid".ToUnicode());
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessage.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateNote_WhenSuccessResponse_ShouldReturnOk()
    {
        //Arange
        var name = "Name2";
        var description = "Description3";
        var dbContext = GetNotesDbContext();

        var request = new CreateNoteCommand()
        {
            Name = name,
            Description = description
        };

        //Act

        var responseMessage = await SendPostRequest(ControllerBaseUrl + "/create", request);

        //Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var note = dbContext.Notes.FirstOrDefault();
        note.Should().NotBeNull();
        note.Name.Should().Be(name);
        note.Description.Should().Be(description);
        dbContext.Notes.Count().Should().Be(1);
    }

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { new CreateNoteCommand() { Name = "name", Description = "" },ValidationConst.EmptyDescription.ToUnicode()}, // проверяет пустое описание
            new object[] { new CreateNoteCommand() { Name = "name", Description = new string('a', ValidationConst.MaxNameLength + 1) }, "Херовое описание".ToUnicode() }, // проверяет длину описания
            new object[] { new CreateNoteCommand() {  Name = string.Empty,Description = "description"}, ValidationConst.EmptyName.ToUnicode() }, //проверяет пустое имя
            new object[] { new CreateNoteCommand() { Name = new string('a', ValidationConst.MaxNameLength+1), Description = "description"}, "Херовое имя пользователя".ToUnicode() }, //проверяет длину имени
        };

    [Theory]
    [MemberData(nameof(Data))]
    public async Task CreateNote_WhenValidationFailed_ShouldReturn400(CreateNoteCommand request, string expectedMessage)
    {
        //Arange

        //Act

        var responseMessage = await SendPostRequest(ControllerBaseUrl + "/create", request);

        //Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessage.Should().HaveStatusCode(HttpStatusCode.BadRequest);

        (await responseMessage.Content.ReadAsStringAsync()).Should().Contain(expectedMessage);
    }

    [Fact]
    public async Task UpdateNote_WhenSuccessResponse_ShouldReturnOk()
    {
        //Arange

        var dbContext = GetNotesDbContext();

        var note = new Note("Name1", "Description1");

        await dbContext.AddAsync(note);
        await dbContext.SaveChangesAsync();

        var request = new UpdateNoteRequest()
        {
            Name = "NewName1",
            Description = "NewDescription1"
        };

        //Act

        var responseMessage = await SendPutRequest(ControllerBaseUrl + $"/update/{note.Id}", request);

        //Assert

        var noteFromDb = dbContext.Notes.AsNoTracking().FirstOrDefault();

        responseMessage.Should().HaveStatusCode(HttpStatusCode.OK);
        noteFromDb.Should().NotBeNull();

        noteFromDb.Name.Should().Be(request.Name);
        noteFromDb.Description.Should().Be(request.Description);
    }

    public static IEnumerable<object[]> UpdateData =>
        new List<object[]>
        {
            new object[] { new UpdateNoteRequest() { Name = "name", Description = "" },ValidationConst.EmptyDescription.ToUnicode()}, // проверяет пустое описание
            new object[] { new UpdateNoteRequest() { Name = "name", Description = new string('a', ValidationConst.MaxNameLength + 1) }, "Херовое описание".ToUnicode() }, // проверяет длину описания
            new object[] { new UpdateNoteRequest() {  Name = string.Empty,Description = "description"}, ValidationConst.EmptyName.ToUnicode() }, //проверяет пустое имя
            new object[] { new UpdateNoteRequest() { Name = new string('a', ValidationConst.MaxNameLength+1), Description = "description"}, "Херовое имя пользователя".ToUnicode() }, //проверяет длину имени
        };

    [Theory]
    [MemberData(nameof(UpdateData))]
    public async Task UpdateNote_WhenValidationFailed_ShouldReturn400(UpdateNoteRequest request, string expectedMessage)
    {
        //Arange

        var dbContext = GetNotesDbContext();

        var note = new Note("Name1", "Description1");

        await dbContext.AddAsync(note);
        await dbContext.SaveChangesAsync();

        //Act

        var responseMessage = await SendPutRequest(ControllerBaseUrl + $"/update/{note.Id}", request);

        //Assert

        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessage.Should().HaveStatusCode(HttpStatusCode.BadRequest);

        (await responseMessage.Content.ReadAsStringAsync()).Should().Contain(expectedMessage);
    }

    public static IEnumerable<object[]> InvalidUpdateData =>
       new List<object[]>
       {
            new object[] { "Name1", "Description" },
            new object[] { "Name", "Description1" },
       };

    [Theory]
    [MemberData(nameof(InvalidUpdateData))]
    public async Task UpdateNote_WhenValidationFromDBFailed_ShouldReturn400(string name, string description)
    {
        //Arange

        var dbContext = GetNotesDbContext();

        var note = new Note("Name1", "Description1");

        await dbContext.AddAsync(note);
        await dbContext.SaveChangesAsync();

        var requestSame = new UpdateNoteRequest()
        {
            Name = name,
            Description = description
        };

        //Act

        var responseMessage = await SendPutRequest(ControllerBaseUrl + $"/update/{note.Id}", requestSame);

        //Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessage.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateNote_WhenValidationIdFound_ShouldReturn400()
    {
        //Arange

        var dbContext = GetNotesDbContext();

        var note = new Note("Name1", "Description1");

        await dbContext.AddAsync(note);
        await dbContext.SaveChangesAsync();

        var notFoundGuid = new Guid();

        var request = new UpdateNoteCommand()
        {
            NewName = "Name123",
            NewDescription = "NewDescription123"
        };

        //Act

        var responseMessage = await SendPutRequest(ControllerBaseUrl + $"/update/{notFoundGuid}", request);

        //Assert
        var noteFromDb = dbContext.Notes.AsNoTracking().FirstOrDefault();

        noteFromDb.Should().NotBeNull();

        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessage.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateNote_WhenValidationIdEmpty_ShouldReturn400()
    {
        //Arange

        var dbContext = GetNotesDbContext();

        var note = new Note("Name1", "Description1");

        await dbContext.AddAsync(note);
        await dbContext.SaveChangesAsync();

        var emptyGuid = Guid.Empty;

        var request = new UpdateNoteCommand()
        {
            NewName = "Name123",
            NewDescription = "NewDescription123"
        };

        //Act

        var responseMessageEmptyGuid = await SendPutRequest(ControllerBaseUrl + $"/update/{emptyGuid}", request);

        //Assert
        var noteFromDb = dbContext.Notes.AsNoTracking().FirstOrDefault();

        noteFromDb.Should().NotBeNull();

        responseMessageEmptyGuid.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessageEmptyGuid.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteNote_WhenSuccessResponse_ShouldReturnOk()
    {
        //Arange
        var dbContext = GetNotesDbContext();

        List<Note> notifications = [
       new Note()
       {
           Id = Guid.Parse("35ac679f-8d97-4ada-857c-02b89bb96626"),
           Name = "Name1",
           Description = "Description1"
       }
       ];
        await dbContext.AddRangeAsync(notifications);
        await dbContext.SaveChangesAsync();

        Guid id = Guid.Parse("35ac679f-8d97-4ada-857c-02b89bb96626");

        //Act

        var responseMessage = await SendDeleteRequest(ControllerBaseUrl + $"/{id}");

        //Assert
        responseMessage.Should().NotHaveStatusCode(HttpStatusCode.BadRequest);

        dbContext.Notes.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task DeleteNote_WhenSuccessResponse_ShouldReturnBadRequest()
    {
        //Arange

        Guid id = Guid.Empty;

        //Act

        var responseMessage = await SendDeleteRequest(ControllerBaseUrl + $"/{id}");

        //Assert
        (await responseMessage.Content.ReadAsStringAsync()).Should().Contain("Пустой Guid".ToUnicode());
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessage.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteNote_WhenSuccessResponse_ShouldReturnNotFound()
    {
        //Arange
        var dbContext = GetNotesDbContext();

        List<Note> notifications = [
       new Note()
       {
           Id = Guid.Parse("35ac679f-8d97-4ada-857c-02b89bb96626"),
           Name = "Name1",
           Description = "Description1"
       }
       ];
        await dbContext.AddRangeAsync(notifications);
        await dbContext.SaveChangesAsync();

        Guid id = Guid.Parse("35ac679f-8d97-4ada-857c-02b89bb96666");

        //Act

        var responseMessage = await SendDeleteRequest(ControllerBaseUrl + $"/{id}");

        //Assert
        responseMessage.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
}