using FluentAssertions;
using Moq;
using TaskManager.Contracts.Result;
using TaskManager.Contracts.Task;
using TaskManager.Process;
using TaskManager.TaskManager;
using Xunit;

namespace TaskManager.Tests.TaskManager
{
    public class PriorityTaskManagerTests
    {
        [Fact]
        public void AddTask_TaskAddedAndStartedAndSaved()
        {
            // arrange
            var processServiceMock = new Mock<IProcessService>();
            var store = new InMemoryTaskStore();

            var sut = new PriorityTaskManager(processServiceMock.Object, store, 20);

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

            var sut = new PriorityTaskManager(processServiceMock.Object, store, 20);

            // act
            var result = sut.Add(new TaskParams(12, priority));

            // assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.AlreadyExists);
        }

        [Fact]
        public void AddTask_ExceedsCapacity_RemovesFirstLowerPriorityTask()
        {
            // arrange
            var processServiceMock = new Mock<IProcessService>();
            var store = new InMemoryTaskStore();

            var sut = new PriorityTaskManager(processServiceMock.Object, store, 3); // <- capacity 3

            // act
            var result = sut.Add(new TaskParams(1, Priority.Medium));
            var result2 = sut.Add(new TaskParams(2, Priority.Low));
            var result3 = sut.Add(new TaskParams(3, Priority.Low));
            var result4 = sut.Add(new TaskParams(4, Priority.Medium));

            // assert
            result.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeTrue();
            result3.IsSuccess.Should().BeTrue();
            result4.IsSuccess.Should().BeTrue();

            store.Count().Should().Be(3);
            store.Contains(1).Should().BeTrue();
            store.Contains(3).Should().BeTrue();
            store.Contains(4).Should().BeTrue();
        }

        [Fact]
        public void AddTask_ExceedsCapacity_AndNoLowerPriorityTask_Failure()
        {
            // arrange
            var processServiceMock = new Mock<IProcessService>();
            var store = new InMemoryTaskStore();

            var sut = new PriorityTaskManager(processServiceMock.Object, store, 2); // <- capacity 2

            // act
            var result = sut.Add(new TaskParams(1, Priority.Medium));
            var result2 = sut.Add(new TaskParams(2, Priority.High));
            var result3 = sut.Add(new TaskParams(3, Priority.Medium));

            // assert
            result.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeTrue();
            result3.IsSuccess.Should().BeFalse();
            result3.ErrorType.Should().Be(ErrorType.CapacityLimit);

            store.Count().Should().Be(2);
            store.Contains(1).Should().BeTrue();
            store.Contains(2).Should().BeTrue();
        }
    }
}
