namespace NotesApplication.Test.Integration;

public class NotificationControllerTests : IntegrationTestBase
{
    public NotificationControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    //TODO создать контекст

    //public NotificationControllerTests()
    //{
    //    // TODO создать базу
    //}

    [Fact]
    public void GetAllNotes_WhenSuccess_ShouldReturnCollectionOfNotes()
    {
        //Arange
        //TODO добавить данные X в базу

        //Act
        // TODO получить записи из контроллера

        //var controller = new NotificationController(null);
        //var res = controller.GetAllNotes();

        //Assert
        //TODO сравнить результат с данными Х
    }
}