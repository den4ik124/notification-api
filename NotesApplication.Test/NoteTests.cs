using FluentAssertions;
using NotesApplication.Core;

namespace NotesApplication.Test;

public class NoteTests
{
    public NoteTests()
    {
    }

    private Note CreateNote()
    {
        return new Note(
            "TODO",
            "TODO");
    }

    [Fact]
    public void Equals_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var note = this.CreateNote();
        Note? other = CreateNote()
;

        // Act
        var result = note.Equals(
            other);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_StateUnderTest_ExpectedBehavior2()
    {
        // Arrange
        var note = this.CreateNote();
        Note? other = new Note(
            "TODO",
            "TODO2");
        ;

        // Act
        var result = note.Equals(
            other);

        // Assert
        result.Should().BeFalse();
    }
}