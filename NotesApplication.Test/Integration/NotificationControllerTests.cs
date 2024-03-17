using Azure.Core;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NotesApplication.API.Controllers;
using NotesApplication.Core;
using NotesApplication.Core.CreateNote;
using NotesApplication.Core.GetAllNotes;
using NotesApplication.Core.UpdateNote;
using System.Net;

namespace NotesApplication.Test.Integration;

public class NotificationControllerTests : IntegrationTestBase
{
    private const string ControllerBaseUrl = "api/Notification";

    public NotificationControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAllNotes_WhenSuccess_ShouldReturnCollectionOfNotes()
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

        var controller = new NotificationController(dbContext);
        var res = await controller.GetAllNotes();

        //Assert

        res.Should().NotBeEmpty();
        res.Should().BeEquivalentTo(notifications.Select(x => new NotificationResponse(x.Name, x.Description, x.Id)));
    }

    [Fact]
    public async Task GetAllNotes_WhenSuccess_ShouldReturnEmptyCollectionOfNotes()
    {
        //Arange

        var dbContext = GetNotesDbContext();

        //Act

        var controller = new NotificationController(dbContext);
        var res = await controller.GetAllNotes();

        //Assert

        res.Should().BeEmpty();
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

        ///api/Notification/getNotes
        //IEnumerable<NotificationResponse>?
        var responseMessage = await SendGetRequest(ControllerBaseUrl);

        var response = await ConvertTo<IEnumerable<NotificationResponse>>(responseMessage);
        //Assert

        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Should().NotBeEmpty();
        response.Should().BeEquivalentTo(notifications.Select(x => new NotificationResponse(x.Name, x.Description, x.Id)));
    }

    [Fact]
    public async Task CreateNote_WhenSuccessResponse_ShouldReturnOk()
    {
        //Arange
        var name = "Name2";
        var description = "Description3";
        var dbContext = GetNotesDbContext();

        var request = new CreateRequest()
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

    [Theory]
    [MemberData(nameof(Data))]
    public async Task CreateNote_WhenValidationFailed_ShouldReturn400(CreateRequest request, string expectedMessage)
    {
        //Arange

        //Act

        var responseMessage = await SendPostRequest(ControllerBaseUrl + "/create", request);

        //Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessage.Should().HaveStatusCode(HttpStatusCode.BadRequest);

        (await responseMessage.Content.ReadAsStringAsync()).Should().Be(expectedMessage);
        //var message = await ConvertTo<string>(responseMessage);
        //message.Should().NotBeNull().And.Be(expectedMessage);
    }

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { new CreateRequest() { Name = "name", Description = "" }, "Херовое описание" },
            //new object[] { new CreateRequest() { Name = "name", Description = null }, "Херовое описание" },
            //new object[] { new CreateRequest() {  Name = null,Description = "description"}, "Херовое имя пользователя" },
            new object[] { new CreateRequest() {  Name = string.Empty,Description = "description"}, "Херовое имя пользователя" },
        };

    [Fact]
    public async Task UpdateNote_WhenSuccessResponse_ShouldReturnOk()
    {
        //Arange

        var dbContext = GetNotesDbContext();

        var note = new Note("Name1", "Description1");

        await dbContext.AddAsync(note);
        await dbContext.SaveChangesAsync();

        var request = new UpdateRequest()
        {
            NewName = "NewName1",
            NewDescription = "NewDescription1"
        };

        //Act

        var responseMessage = await SendPutRequest(ControllerBaseUrl + $"/update/{note.Id}", request);

        //Assert

        var noteFromDb = dbContext.Notes.AsNoTracking().FirstOrDefault();

        responseMessage.Should().HaveStatusCode(HttpStatusCode.OK);
        noteFromDb.Should().NotBeNull();

        noteFromDb.Name.Should().Be(request.NewName);
        noteFromDb.Description.Should().Be(request.NewDescription);
    }

    [Theory]
    [MemberData(nameof(UpdateData))]
    public async Task UpdateNote_WhenValidationFailed_ShouldReturn400(UpdateRequest request, string expectedMessage)
    {
        //Arange

        var dbContext = GetNotesDbContext();

        var note = new Note("Name1", "Description1");

        await dbContext.AddAsync(note);
        await dbContext.SaveChangesAsync();

        //var request = new UpdateRequest()
        //{
        //    NewName = "NewName1",
        //    NewDescription = "NewDescription1"
        //};

        //Act

        var responseMessage = await SendPutRequest(ControllerBaseUrl + $"/update/{note.Id}", request);

        //Assert

        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessage.Should().HaveStatusCode(HttpStatusCode.BadRequest);

        (await responseMessage.Content.ReadAsStringAsync()).Should().Be(expectedMessage);
    }

    public static IEnumerable<object[]> UpdateData =>
        new List<object[]>
        {
            new object[] { new UpdateRequest()  { NewName= "NewName", NewDescription = "" }, "Херовое описание" },
            new object[] { new UpdateRequest() { NewName = string.Empty, NewDescription = "NewDescription" }, "Херовое имя пользователя" },
        };

    [Fact]
    public async Task UpdateNote_WhenValidationFromDBFailed_ShouldReturn400()
    {
        //Arange

        var dbContext = GetNotesDbContext();

        var note = new Note("Name1", "Description1");

        await dbContext.AddAsync(note);
        await dbContext.SaveChangesAsync();

        var requestSameName = new UpdateRequest()
        {
            NewName = "Name1",
            NewDescription = "NewDescription1"
        };
        var requestSameDescription = new UpdateRequest()
        {
            NewName = "NewName",
            NewDescription = "Description1"
        };

        //Act

        var responseMessageName = await SendPutRequest(ControllerBaseUrl + $"/update/{note.Id}", requestSameName);
        var responseMessageDescription = await SendPutRequest(ControllerBaseUrl + $"/update/{note.Id}", requestSameDescription);

        //Assert
        var noteFromDb = dbContext.Notes.AsNoTracking().FirstOrDefault();

        noteFromDb.Should().NotBeNull();

        responseMessageName.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessageName.Should().HaveStatusCode(HttpStatusCode.BadRequest);

        responseMessageDescription.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessageDescription.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateNote_WhenValidationIdFailed_ShouldReturn400()
    {
        //Arange

        var dbContext = GetNotesDbContext();

        var note = new Note("Name1", "Description1");

        await dbContext.AddAsync(note);
        await dbContext.SaveChangesAsync();

        var notFoundGuid = new Guid();
        var emptyGuid = Guid.Empty;

        var request = new UpdateRequest()
        {
            NewName = "Name123",
            NewDescription = "NewDescription123"
        };

        //Act

        var responseMessage = await SendPutRequest(ControllerBaseUrl + $"/update/{notFoundGuid}", request);
        var responseMessageEmptyGuid = await SendPutRequest(ControllerBaseUrl + $"/update/{emptyGuid}", request);

        //Assert
        var noteFromDb = dbContext.Notes.AsNoTracking().FirstOrDefault();

        noteFromDb.Should().NotBeNull();

        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseMessage.Should().HaveStatusCode(HttpStatusCode.BadRequest);

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