using FluentAssertions;
using Moq;
using TaskManager.Contracts.Result;
using TaskManager.Contracts.Task;
using TaskManager.Process;
using TaskManager.TaskManager;
using Xunit;

namespace TaskManager.Tests.TaskManager
{
    public class DefaultTaskManagerTests
    {
        [Fact]
        public void AddTask_TaskAddedAndStartedAndSaved()
        {
            // arrange
            var processServiceMock = new Mock<IProcessService>();
            var store = new InMemoryTaskStore();

            var sut = new DefaultTaskManager(processServiceMock.Object, store, 20);

            // act
            var result = sut.Add(new TaskParams(12, Priority.Medium));

            // assert
            result.IsSuccess.Should().BeTrue();
            store.Contains(12).Should().BeTrue();
            processServiceMock.Verify(x => x.Start(It.Is<Priority>(x => x == Priority.Medium)), Times.Once);
        }

        [Theory]
        [InlineData(Priority.High)]
        [InlineData(Priority.Low)]
        public void AddTask_AlreadyExists_Failure(Priority priority)
        {
            // arrange
            var processServiceMock = new Mock<IProcessService>();

            var store = new InMemoryTaskStore();
            store.Add(new TaskInstance(
                new TaskParams(12, Priority.High),
                120,
                processServiceMock.Object,
                store));

            var sut = new DefaultTaskManager(processServiceMock.Object, store, 20);

            // act
            var result = sut.Add(new TaskParams(12, priority));

            // assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.AlreadyExists);
        }

        [Fact]
        public void AddTask_ExceedsCapacity_Failure()
        {
            // arrange
            var processServiceMock = new Mock<IProcessService>();
            var store = new InMemoryTaskStore();

            var sut = new DefaultTaskManager(processServiceMock.Object, store, 1); // <- capacity 1

            // act
            var result = sut.Add(new TaskParams(1, Priority.Medium));
            var result2 = sut.Add(new TaskParams(2, Priority.Medium));

            // assert
            result.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeFalse();
            result2.ErrorType.Should().Be(ErrorType.CapacityLimit);

            store.Count().Should().Be(1);
            store.Contains(1).Should().BeTrue();
        }
    }
}
