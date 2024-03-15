using FluentAssertions;
using NotesApplication.API.Controllers;
using NotesApplication.Core;
using NotesApplication.Core.CreateNote;
using NotesApplication.Core.GetAllNotes;
using NotesApplication.Core.UpdateNote;
using System;
using System.Net;
using System.Net.Http.Json;

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
        //TODO добавить данные X в базу

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
        // TODO получить записи из контроллера

        var controller = new NotificationController(dbContext);
        var res = await controller.GetAllNotes();

        //Assert
        //TODO сравнить результат с данными Х
        res.Should().NotBeEmpty();
        res.Should().BeEquivalentTo(notifications.Select(x => new NotificationResponse(x.Name, x.Description, x.Id)));
    }

    [Fact]
    public async Task GetAllNotes_WhenSuccess_ShouldReturnEmptyCollectionOfNotes()
    {
        //Arange
        //TODO добавить данные X в базу

        var dbContext = GetNotesDbContext();

        //Act
        // TODO получить записи из контроллера

        var controller = new NotificationController(dbContext);
        var res = await controller.GetAllNotes();

        //Assert
        //TODO сравнить результат с данными Х
        res.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllNotes_WhenSuccessResponse_ShouldReturnCollectionOfNotes()
    {
        //Arange
        //TODO добавить данные X в базу

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
        // TODO получить записи из контроллера

        ///api/Notification/getNotes
        //IEnumerable<NotificationResponse>?
        var responseMessage = await SendGetRequest(ControllerBaseUrl);

        var response = await ConvertTo<IEnumerable<NotificationResponse>>(responseMessage);
        //Assert
        //TODO сравнить результат с данными Х

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
}