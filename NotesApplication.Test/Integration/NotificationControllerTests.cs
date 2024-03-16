﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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

        List<Note> notifications = [
       new Note()
        {
            Id = Guid.Parse("35ac679f-8d97-4ada-857c-02b89bb96629"),
            Name = "Name1",
            Description = "Description1"
        }
       ];
        await dbContext.AddRangeAsync(notifications);
        await dbContext.SaveChangesAsync();

        Guid id = Guid.Parse("35ac679f-8d97-4ada-857c-02b89bb96629");
        var newName = "NewName1";
        var newDescription = "NewDescription1";

        var request = new UpdateRequest()
        {
            NewName = newName,
            NewDescription = newDescription
        };

        //Act

        var responseMessage = await SendPutRequest(ControllerBaseUrl + $"/update/{id}", request);

        //Assert

        var note = dbContext.Notes.AsNoTracking().FirstOrDefault();

        responseMessage.Should().HaveStatusCode(HttpStatusCode.OK);
        note.Should().NotBeNull();

        note.Name.Should().Be(newName);
        note.Description.Should().Be(newDescription);

        //note.Name.Should().Be(newName);
        //note.Description.Should().Be(description);
        //dbContext.Notes.Count().Should().Be(1);
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